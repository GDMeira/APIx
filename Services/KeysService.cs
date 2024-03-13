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
        PixKey[] userPixKeys = await ValidateKeyToCreation(pixKey, user, paymentProviderId);
        PaymentProviderAccount receivedAccount = postKeysDTO.GetPaymentProviderAccount();
        PaymentProviderAccount? paymentProviderAccountDB = await RetrieveAccount(receivedAccount,
            paymentProviderId, user.Id, userPixKeys);
        PixKey newPixKey;

        if (paymentProviderAccountDB != null)
        {
            pixKey.PaymentProviderAccountId = paymentProviderAccountDB.Id;
            newPixKey = await CreateKey(pixKey);
        }
        else
        {
            receivedAccount.UserId = user.Id;
            receivedAccount.PaymentProviderId = paymentProviderId;
            newPixKey = await CreateKey(pixKey, receivedAccount);
        }

        var response = new ResPostKeysDTO(newPixKey, user, newPixKey.PaymentProviderAccount);

        return response;
    }

    public async Task<User> ValidateUser(string userCpf)
    {
        return await _usersRepository.RetrieveUserByCpf(userCpf) ??
            throw new AppException(HttpStatusCode.NotFound, "User not found");
    }

    public async Task<PixKey[]> ValidateKeyToCreation(PixKey pixKey, User user, int paymentProviderId)
    {
        if (pixKey.Type == "CPF" && pixKey.Value != user.Cpf)
        {
            throw new AppException(HttpStatusCode.BadRequest, "Invalid CPF");
        }
        else if (pixKey.Type == "Email" && !EmailRegex().IsMatch(pixKey.Value))
        {
            throw new AppException(HttpStatusCode.BadRequest, "Invalid email");
        }
        else if (pixKey.Type == "Phone" && !PhoneRegex().IsMatch(pixKey.Value))
        {
            throw new AppException(HttpStatusCode.BadRequest, "Invalid phone");
        }

        PixKey[] pixKeys = await _keysRepository.RetrieveKeysByUserId(user.Id);

        if (pixKeys.Length >= 20)
        {
            throw new AppException(HttpStatusCode.BadRequest, "User already has 20 keys");
        }

        PixKey[] pixKeysFromSameProvider = pixKeys.Where(p =>
            p.PaymentProviderAccount.PaymentProviderId == paymentProviderId).ToArray();

        if (pixKeysFromSameProvider.Length >= 5)
        {
            throw new AppException(HttpStatusCode.BadRequest, "User already has 5 keys with this payment provider");
        }

        if (pixKeysFromSameProvider.Where(p => p.Value == pixKey.Value).ToArray().Length > 0)
        {
            throw new AppException(HttpStatusCode.Conflict, "User already has this key");
        }

        return pixKeys;
    }

    public async Task<PaymentProviderAccount?> RetrieveAccount(PaymentProviderAccount paymentProviderAccount,
        int paymentProviderId, int userId, PixKey[] userPixKeys)
    {
        PixKey? pixFromSameAccount = userPixKeys.FirstOrDefault(p =>
            p.PaymentProviderAccount.Number == paymentProviderAccount.Number &&
            p.PaymentProviderAccount.Agency == paymentProviderAccount.Agency);

        PaymentProviderAccount? paymentProviderAccountFromDb =
            pixFromSameAccount?.PaymentProviderAccount ?? await _accountsRepository
            .RetrieveAccount(paymentProviderAccount.Number, paymentProviderAccount.Agency);

        if (paymentProviderAccountFromDb == null)
        {
            return paymentProviderAccountFromDb;
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

    public async Task<PixKey> CreateKey(PixKey pixKey)
    {
        if (pixKey.Type == "Random")
        {
            try
            {
                pixKey.Value = RandomKeyGenerator.GenerateRandomKey();
                Console.WriteLine(pixKey.Value);
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
                pixKey.Value = RandomKeyGenerator.GenerateRandomKey();
                return await _keysRepository.CreateKey(pixKey, account);
            }
            catch (Exception)
            {
                return await CreateKey(pixKey, account);
            }
        }

        try
        {
            return await _keysRepository.CreateKey(pixKey, account);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new AppException(HttpStatusCode.Conflict, $"This key {pixKey.Value} already exists");
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

    public async Task<ResGetKeysDTO> GetKeys(string type, string value)
    {
        ValidateKeyToRetrieval(type, value);
        PixKey pixKey = await _keysRepository.RetrieveKeyByTypeAndValue(type, value) ??
            throw new AppException(HttpStatusCode.NotFound, "Key not found");

        return new ResGetKeysDTO(pixKey, pixKey.PaymentProviderAccount.User,
                    pixKey.PaymentProviderAccount, pixKey.PaymentProviderAccount.PaymentProvider);
    }

    public void ValidateKeyToRetrieval(string type, string value)
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
    }
}