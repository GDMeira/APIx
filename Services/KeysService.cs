using System.Net;
using System.Text.RegularExpressions;
using APIx.RequestDTOs;
using APIx.ResponseDTOs;
using APIx.Exceptions;
using APIx.Helpers;
using APIx.Models;
using APIx.Repositories;

namespace APIx.Services;

public partial class KeysService(UsersRepository usersRepository,
    KeysRepository keysRepository, AccountsRepository accountsRepository)
{
    private readonly UsersRepository _usersRepository = usersRepository;
    private readonly KeysRepository _keysRepository = keysRepository;
    private readonly AccountsRepository _accountsRepository = accountsRepository;
    public async Task<ResPostKeysDTO> PostKeys(ReqPostKeysDTO postKeysDTO, int paymentProviderId)
    {
        string userCpf = postKeysDTO.GetUserCpf();
        User user = await ValidateUser(userCpf);
        PixKey pixKey = postKeysDTO.GetPixKey();
        ValidateInputKey(pixKey, user);
        PaymentProviderAccount receivedAccount = postKeysDTO.GetPaymentProviderAccount();
        receivedAccount.PaymentProviderId = paymentProviderId;
        receivedAccount.UserId = user.Id;
        PaymentProviderAccount? paymentProviderAccountDB = await RetrieveAccount(receivedAccount);

        ResPostKeysDTO response = await CreateKeyTransaction(pixKey, user, paymentProviderAccountDB, receivedAccount);

        return response;
    }

    public async Task<User> ValidateUser(string userCpf)
    {
        return await _usersRepository.RetrieveUserByCpf(userCpf) ??
            throw new NotFoundException("User not found");
    }

    public void ValidateInputKey(PixKey pixKey, User user)
    {
        if (pixKey.Type == "CPF" && pixKey.Value != user.Cpf)
        {
            throw new UnprocessableEntryException("Invalid CPF");
        }
        else if (pixKey.Type == "Email" && !EmailRegex().IsMatch(pixKey.Value))
        {
            throw new UnprocessableEntryException("Invalid email");
        }
        else if (pixKey.Type == "Phone" && !PhoneRegex().IsMatch(pixKey.Value))
        {
            throw new UnprocessableEntryException("Invalid phone");
        }
    }

    public async Task ValidateKeyToCreation(PixKey pixKey, User user, int paymentProviderId)
    {
        if (pixKey.Type != "Random")
        {
            PixKey? pixKeyDB = await _keysRepository.RetrieveKeyByValue(pixKey.Value, getSetCache: false);
            if (pixKeyDB != null)
                throw new ForbiddenOperationException("Key already exists");
        }

        int totalPixKeys = await _keysRepository.CountKeysByUserId(user.Id);

        if (totalPixKeys >= 20)
            throw new ForbiddenOperationException("User already has 20 keys");

        int totalPixKeysWithThisProvider = await _keysRepository
            .CountKeysByUserIdAndProviderId(user.Id, paymentProviderId);

        if (totalPixKeysWithThisProvider >= 5)
            throw new ForbiddenOperationException("User already has 5 keys with this payment provider");
    }

    public async Task<PaymentProviderAccount?> RetrieveAccount(PaymentProviderAccount account)
    {
        PaymentProviderAccount? paymentProviderAccountFromDb = await _accountsRepository
            .RetrieveAccount(account);

        if (paymentProviderAccountFromDb == null)
        {
            return paymentProviderAccountFromDb;
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

    public async Task<PixKey> CreateKey(PixKey pixKey)
    {
        if (pixKey.Type == "Random")
        {
            try
            {
                pixKey.Value = Guid.NewGuid().ToString();
                return await _keysRepository.CreateKey(pixKey);
            }
            catch (Exception)
            {
                return await CreateKey(pixKey);
            }
        }

        try
        {
            return await _keysRepository.CreateKey(pixKey);
        }
        catch (Exception)
        {
            throw new AppException(HttpStatusCode.Conflict, $"This key {pixKey.Value} already exists");
        }
    }

    public async Task<PixKey> CreateKey(PixKey pixKey, PaymentProviderAccount account)
    {
        if (pixKey.Type == "Random")
        {
            try
            {
                pixKey.Value = Guid.NewGuid().ToString();
                return await _keysRepository.CreateKey(pixKey, account);
            }
            catch (Exception)
            {
                return await CreateKey(pixKey, account);
            }
        }

        return await _keysRepository.CreateKey(pixKey, account);
    }

    public async Task<ResPostKeysDTO> CreateKeyTransaction(PixKey pixKey, User user,
    PaymentProviderAccount? paymentProviderAccountDB, PaymentProviderAccount receivedAccount)
    {
        using var transaction = _keysRepository.BeginTransaction();
        try
        {
            await ValidateKeyToCreation(pixKey, user, receivedAccount.PaymentProviderId);
            PixKey newPixKey;

            if (paymentProviderAccountDB != null)
            {
                pixKey.PaymentProviderAccountId = paymentProviderAccountDB.Id;
                newPixKey = await CreateKey(pixKey);
                newPixKey.PaymentProviderAccount = paymentProviderAccountDB;
            }
            else
            {
                newPixKey = await CreateKey(pixKey, receivedAccount);
            }

            transaction.Commit();

            return new ResPostKeysDTO(newPixKey, user);
        }
        catch (Exception)
        {
            transaction.Rollback();
            throw;
        }
    }
    public async Task<ResGetKeysDTO> GetKeys(string type, string value)
    {
        ValidateKeyToRetrieval(type, value);
        PixKey pixKey = await _keysRepository.RetrieveKeyByValue(value) ??
            throw new NotFoundException("Key not found");

        return new ResGetKeysDTO(pixKey);
    }

    public void ValidateKeyToRetrieval(string type, string value)
    {
        if (type != "CPF" && type != "Email" && type != "Phone" && type != "Random")
        {
            throw new UnprocessableRouteException("Invalid type");
        }
        else if (type == "CPF" && !CpfRegex().IsMatch(value))
        {
            throw new UnprocessableRouteException("Invalid CPF");
        }
        else if (type == "Email" && !EmailRegex().IsMatch(value))
        {
            throw new UnprocessableRouteException("Invalid email");
        }
        else if (type == "Phone" && !PhoneRegex().IsMatch(value))
        {
            throw new UnprocessableRouteException("Invalid phone");
        }
        else if (type == "Random" && !RandomKeyRegex().IsMatch(value))
        {
            throw new UnprocessableRouteException("Invalid random key");
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