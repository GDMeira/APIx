using APIx.Repositories;

namespace APIx.Services;

public class AuthService(AuthRepository authRepository)
{
    private readonly AuthRepository _authRepository = authRepository;
    
}