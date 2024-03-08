using System.Net;
using System.Text.RegularExpressions;
using APIx.RequestDTOs;
using APIx.ResponseDTOs;
using APIx.Exceptions;
using APIx.Helpers;
using APIx.Models;
using APIx.Repositories;

namespace APIx.Services;

public partial class KeysService(AuthRepository authRepository, UsersRepository usersRepository,
    KeysRepository keysRepository, AccountsRepository accountsRepository)
{
    private readonly AuthRepository _authRepository = authRepository;
    private readonly UsersRepository _usersRepository = usersRepository;
    private readonly KeysRepository _keysRepository = keysRepository;
    private readonly AccountsRepository _accountsRepository = accountsRepository;
    public async Task<ResPostKeysDTO> PostKeys(ReqPostKeysDTO postKeysDTO, string? authorization)
    {
        PaymentProvider paymentProvider = await PaymentProviderTokenValidate(authorization);
        string userCpf = postKeysDTO.GetUserCpf();
        User user = await ValidateUser(userCpf);
        PixKey pixKey = postKeysDTO.GetPixKey();
        await ValidateKeyToCreation(pixKey, user, paymentProvider.Id);
        PaymentProviderAccount paymentProviderAccount = await RetrieveOrCreateAccount(postKeysDTO.GetPaymentProviderAccount(),
            paymentProvider.Id, user.Id);
        pixKey.PaymentProviderAccountId = paymentProviderAccount.Id;
        PixKey newPixKey = await CreateKey(pixKey);
        var response = new ResPostKeysDTO(newPixKey, user, paymentProviderAccount);

        return response;
    }

    public async Task<PaymentProvider> PaymentProviderTokenValidate(string? authorization)
    {
        if (authorization == null)
        {
            throw new AppException(HttpStatusCode.Unauthorized, "Token not found");
        }

        string token = authorization.Split(" ")[1]; //Bearer token

        if (string.IsNullOrEmpty(token))
        {
            throw new AppException(HttpStatusCode.Unauthorized, "Token not found");
        }

        PaymentProvider paymentProvider = await _authRepository.RetrievePaymentProviderByToken(token) ??
            throw new AppException(HttpStatusCode.Unauthorized, "Invalid token");

        return paymentProvider;
    }

    public async Task<User> ValidateUser(string userCpf)
    {
        System.Console.WriteLine(userCpf);
        return await _usersRepository.RetrieveUserByCpf(userCpf) ??
            throw new AppException(HttpStatusCode.NotFound, "User not found");
    }

    public async Task ValidateKeyToCreation(PixKey pixKey, User user, int paymentProviderId)
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
            paymentProviderAccountFromDb = await _accountsRepository
                                                    .CreateAccount(paymentProviderAccount);
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
    [GeneratedRegex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$")]
    private static partial Regex EmailRegex();

    [GeneratedRegex(@"^(\+)([0-9]{13})$")]
    private static partial Regex PhoneRegex();

    [GeneratedRegex(@"^([A-Za-z0-9]{8})-([A-Za-z0-9]{4})-([A-Za-z0-9]{4})-([A-Za-z0-9]{4})-([A-Za-z0-9]{12})$")]
    private static partial Regex RandomKeyRegex();

    [GeneratedRegex(@"^[0-9]{11}$")]
    private static partial Regex CpfRegex();

    public async Task<ResGetKeysDTO> GetKeys(string type, string value, string? authorization)
    {
        PaymentProvider paymentProvider = await PaymentProviderTokenValidate(authorization);
        ValidateKeyToRetrieval(type, value);
        PixKey pixKey = await _keysRepository.RetrieveKeyByTypeAndValue(type, value) ??
            throw new AppException(HttpStatusCode.NotFound, "Key not found");

        return new ResGetKeysDTO(pixKey, pixKey.PaymentProviderAccount.User,
                    pixKey.PaymentProviderAccount, paymentProvider);
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