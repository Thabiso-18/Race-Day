using Microsoft.EntityFrameworkCore;
using RaceDay.API.Data;
using RaceDay.API.DTOs;
using RaceDay.API.DTOs.Auth;
using RaceDay.API.Models;
using System;
using System.Threading.Tasks;

namespace RaceDay.API.Services;

public class AuthService : IAuthService
{
    private readonly RaceDayDbContext _db;
    private readonly ITokenService _tokenService;

    public AuthService(RaceDayDbContext db, ITokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    public async Task<AuthResponseDto?> RegisterAsync(RegisterRequestDto dto)
    {
        if (dto.Role != "Organiser" && dto.Role != "Participant") return null;
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email)) return null;

        var user = new User
        {
            Name = dto.Name,
            Surname = dto.Surname,
            Email = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        if (dto.Role == "Participant")
        {
            _db.Participants.Add(new Participant
            {
                UserId = user.UserId,
                DateOfBirth = dto.DateOfBirth ?? DateTime.UtcNow.AddYears(-20),
                IdentityNumber = dto.IdentityNumber ?? Guid.NewGuid().ToString("N")[..13],
                EmergencyContact = dto.EmergencyContact,
                LicenseNumber = dto.LicenseNumber
            });
        }
        else
        {
            _db.Organisers.Add(new Organiser
            {
                UserId = user.UserId,
                OrganisationName = dto.OrganisationName ?? "Independent",
                ContactPhone = dto.ContactPhone
            });
        }

        await _db.SaveChangesAsync();

        return new AuthResponseDto
        {
            Token = _tokenService.GenerateToken(user),
            User = new UserDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Role = user.Role
            }
        };
    }

    public async Task<AuthResponseDto?> LoginAsync(LoginRequestDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
        if (user == null || !BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash)) return null;

        return new AuthResponseDto
        {
            Token = _tokenService.GenerateToken(user),
            User = new UserDto
            {
                UserId = user.UserId,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Role = user.Role
            }
        };
    }

    public async Task<bool> ChangePasswordAsync(int userId, ChangePasswordDto dto)
    {
        var user = await _db.Users.FindAsync(userId);
        if (user == null) return false;
        if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash)) return false;

        user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
        await _db.SaveChangesAsync();
        return true;
    }
}