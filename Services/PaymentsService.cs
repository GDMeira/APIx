using System.Net;
using System.Text.RegularExpressions;
using APIx.RequestDTOs;
using APIx.Exceptions;
using APIx.Models;
using APIx.Repositories;
using APIx.ResponseDTOs;
using APIx.Helpers.RabbitMQ;

namespace APIx.Services;

public partial class PaymentsService(UsersRepository usersRepository,
    IRabbitManager messagePublisher, AccountsRepository accountsRepository,
    KeysRepository keysRepository, PaymentsRepository paymentsRepository)
{
    private readonly UsersRepository _usersRepository = usersRepository;
    private readonly IRabbitManager _messagePublisher = messagePublisher;
    private readonly AccountsRepository _accountsRepository = accountsRepository;
    private readonly KeysRepository _keysRepository = keysRepository;
    private readonly PaymentsRepository _paymentsRepository = paymentsRepository;
    public async Task<ResPostPaymentsDTO> PostPayment(ReqPostPaymentsDTO postPaymentsDTO, int paymentProviderId)
    {
        string userCpf = postPaymentsDTO.GetOriginUserCpf();
        User user = await ValidateUser(userCpf);
        PixKey pixKey = postPaymentsDTO.GetDestinyPixKey();
        PixKey pixKeyDB = await ValidateKeyToRetrieval(pixKey.Type, pixKey.Value);
        PaymentProviderAccount originAccount = postPaymentsDTO.GetOriginPaymentProviderAccount();
        if (originAccount.Number == pixKeyDB.PaymentProviderAccount.Number &&
            originAccount.Agency == pixKeyDB.PaymentProviderAccount.Agency)
        {
            throw new ForbiddenOperationException("Origin and destiny account cannot be the same");
        }

        originAccount.UserId = user.Id;
        originAccount.PaymentProviderId = paymentProviderId;
        PaymentProviderAccount accountDB = await RetrieveOrCreateAccount(originAccount);
        originAccount.User = user;
        Payment payment = postPaymentsDTO.GetPayment();
        payment.PixKeyId = pixKeyDB.Id;
        payment.PaymentProviderAccountId = accountDB.Id;
        await CheckIdempotence(payment);
        Payment paymentDB = await _paymentsRepository.CreatePayment(payment, accountDB, pixKeyDB);
        string queue = "payments";
        bool headers = true;
        _messagePublisher.Publish(paymentDB.Id, queue, headers);

        return new ResPostPaymentsDTO(paymentDB);
    }

    public async Task<User> ValidateUser(string userCpf)
    {
        return await _usersRepository.RetrieveUserByCpf(userCpf) ??
            throw new NotFoundException("User not found");
    }

    public async Task<PaymentProviderAccount> RetrieveOrCreateAccount(PaymentProviderAccount account)
    {

        PaymentProviderAccount? paymentProviderAccountFromDb = await _accountsRepository
            .RetrieveAccount(account);

        if (paymentProviderAccountFromDb == null)
        {
            return await _accountsRepository.CreateAccount(account);
        }

        if (paymentProviderAccountFromDb.PaymentProviderId != account.PaymentProviderId)
        {
            throw new ConflictOnCreationException("Account already exists with another payment provider");
        }
        else if (paymentProviderAccountFromDb.UserId != account.UserId)
        {
            throw new ConflictOnCreationException("Account already exists with another user");
        }

        return paymentProviderAccountFromDb;
    }

    public async Task<PixKey> ValidateKeyToRetrieval(string type, string value)
    {
        if (type != "CPF" && type != "Email" && type != "Phone" && type != "Random")
        {
            throw new UnprocessableEntryException("Invalid type");
        }
        else if (type == "CPF" && !CpfRegex().IsMatch(value))
        {
            throw new UnprocessableEntryException("Invalid CPF");
        }
        else if (type == "Email" && !EmailRegex().IsMatch(value))
        {
            throw new UnprocessableEntryException("Invalid email");
        }
        else if (type == "Phone" && !PhoneRegex().IsMatch(value))
        {
            throw new UnprocessableEntryException("Invalid phone");
        }
        else if (type == "Random" && !RandomKeyRegex().IsMatch(value))
        {
            throw new UnprocessableEntryException("Invalid random key");
        }

        return await _keysRepository.RetrieveKeyByValue(value) ??
            throw new NotFoundException("Key not found");
    }

    public async Task CheckIdempotence(Payment payment)
    {
        Payment? paymentFromDb = await _paymentsRepository.RetrievePaymentByValueAndPixKeyAndAccount(payment);

        if (paymentFromDb != null)
        {
            throw new ForbiddenOperationException("Payment already exists");
        }
    }

    [GeneratedRegex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")]
    private static partial Regex EmailRegex();

    [GeneratedRegex(@"^(\+)([0-9]{13})$")]
    private static partial Regex PhoneRegex();

    [GeneratedRegex(@"^([A-Za-z0-9]{8})-([A-Za-z0-9]{4})-([A-Za-z0-9]{4})-([A-Za-z0-9]{4})-([A-Za-z0-9]{12})$")]
    private static partial Regex RandomKeyRegex();

    [GeneratedRegex(@"^[0-9]{11}$")]
    private static partial Regex CpfRegex();
}