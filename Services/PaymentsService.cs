using System.Net;
using System.Text.RegularExpressions;
using APIx.RequestDTOs;
using APIx.Exceptions;
using APIx.Models;
using APIx.Repositories;
using APIx.ResponseDTOs;

namespace APIx.Services;

public partial class PaymentsService(UsersRepository usersRepository,
    MessageService messageService, AccountsRepository accountsRepository,
    KeysRepository keysRepository, PaymentsRepository paymentsRepository)
{
    private readonly UsersRepository _usersRepository = usersRepository;
    private readonly MessageService _messageService = messageService;
    private readonly AccountsRepository _accountsRepository = accountsRepository;
    private readonly KeysRepository _keysRepository = keysRepository;
    private readonly PaymentsRepository _paymentsRepository = paymentsRepository;
    public async Task<ResPostPaymentsDTO> PostPayment(ReqPostPaymentsDTO postPaymentsDTO, int paymentProviderId)
    {
        // transação não pode se repetir nos últimos 30 segundos (chave de idempotencia baseada no valor, pixKey e account)
        string userCpf = postPaymentsDTO.GetOriginUserCpf();
        User user = await ValidateUser(userCpf);
        PixKey pixKey = postPaymentsDTO.GetDestinyPixKey();
        PixKey pixKeyDB = await ValidateKeyToRetrieval(pixKey.Type, pixKey.Value);
        PaymentProviderAccount paymentProviderAccountOrigin = postPaymentsDTO.GetOriginPaymentProviderAccount();
        if (paymentProviderAccountOrigin.Number == pixKeyDB.PaymentProviderAccount.Number &&
            paymentProviderAccountOrigin.Agency == pixKeyDB.PaymentProviderAccount.Agency)
        {
            throw new AppException(HttpStatusCode.BadRequest, "Origin and destiny account cannot be the same");
        }

        PaymentProviderAccount accountDB = await RetrieveOrCreateAccount(paymentProviderAccountOrigin, paymentProviderId, user.Id);
        Payment payment = postPaymentsDTO.GetPayment();
        payment.PixKeyId = pixKeyDB.Id;
        payment.PaymentProviderAccountId = accountDB.Id;
        Payment paymentDB = await _paymentsRepository.CreatePayment(payment);
        _messageService.SendMessage(paymentDB.Id);
        
        return new ResPostPaymentsDTO(paymentDB);;
    }

    public async Task<User> ValidateUser(string userCpf)
    {
        return await _usersRepository.RetrieveUserByCpf(userCpf) ??
            throw new AppException(HttpStatusCode.NotFound, "User not found");
    }

    public async Task<PaymentProviderAccount> RetrieveOrCreateAccount(PaymentProviderAccount paymentProviderAccount,
        int paymentProviderId, int userId)
    {

        PaymentProviderAccount? paymentProviderAccountFromDb = await _accountsRepository
            .RetrieveAccount(paymentProviderAccount.Number, paymentProviderAccount.Agency);

        if (paymentProviderAccountFromDb == null)
        {
            paymentProviderAccount.PaymentProviderId = paymentProviderId;
            paymentProviderAccount.UserId = userId;

            return await _accountsRepository.CreateAccount(paymentProviderAccount);
        }

        if (paymentProviderAccountFromDb.PaymentProviderId != paymentProviderId)
        {
            throw new AppException(HttpStatusCode.BadRequest, "Account already exists with another payment provider");
        }
        else if (paymentProviderAccountFromDb.UserId != userId)
        {
            throw new AppException(HttpStatusCode.BadRequest, "Account already exists with another user");
        }

        return paymentProviderAccountFromDb;
    }

    [GeneratedRegex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")]
    private static partial Regex EmailRegex();

    [GeneratedRegex(@"^(\+)([0-9]{13})$")]
    private static partial Regex PhoneRegex();

    [GeneratedRegex(@"^([A-Za-z0-9]{8})-([A-Za-z0-9]{4})-([A-Za-z0-9]{4})-([A-Za-z0-9]{4})-([A-Za-z0-9]{12})$")]
    private static partial Regex RandomKeyRegex();

    [GeneratedRegex(@"^[0-9]{11}$")]
    private static partial Regex CpfRegex();

   public async Task<PixKey> ValidateKeyToRetrieval(string type, string value)
    {
        if (type != "CPF" && type != "Email" && type != "Phone" && type != "Random")
        {
            throw new AppException(HttpStatusCode.UnprocessableContent, "Invalid type");
        }
        else if (type == "CPF" && !CpfRegex().IsMatch(value))
        {
            throw new AppException(HttpStatusCode.UnprocessableContent, "Invalid CPF");
        }
        else if (type == "Email" && !EmailRegex().IsMatch(value))
        {
            throw new AppException(HttpStatusCode.UnprocessableContent, "Invalid email");
        }
        else if (type == "Phone" && !PhoneRegex().IsMatch(value))
        {
            throw new AppException(HttpStatusCode.UnprocessableContent, "Invalid phone");
        }
        else if (type == "Random" && !RandomKeyRegex().IsMatch(value))
        {
            throw new AppException(HttpStatusCode.UnprocessableContent, "Invalid random key");
        }

        return await _keysRepository.RetrieveKeyByTypeAndValue(type, value) ??
            throw new AppException(HttpStatusCode.NotFound, "Key not found");
    }    
}