using RaceDay.API.DTOs;
using RaceDay.API.DTOs.Auth;
using System.Threading.Tasks;

namespace RaceDay.API.Services;

public interface IAuthService
{
    Task<AuthResponseDto?> RegisterAsync(RegisterRequestDto dto);
    Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto);
    Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto);
}