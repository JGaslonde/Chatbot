using Chatbot.API.Data.Repositories.Interfaces;
using Chatbot.API.Services.Core.Interfaces;
using Chatbot.Core.Models.Entities;
using Chatbot.Core.Models.Requests;
using Chatbot.Core.Models.Responses;
using OtpSharp;

namespace Chatbot.API.Services.Core;

public class TwoFactorService : ITwoFactorService
{
    private readonly ITwoFactorAuthRepository _repository;
    private readonly ILogger<TwoFactorService> _logger;
    private const int BackupCodeCount = 10;
    private const int CodeLength = 6;

    public TwoFactorService(ITwoFactorAuthRepository repository, ILogger<TwoFactorService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<TwoFactorSetupResponse> GenerateTwoFactorSetupAsync(int userId)
    {
        try
        {
            // Generate a new secret
            var secret = GenerateSecret();
            var backupCodes = GenerateBackupCodes();

            // Store in database but not enabled yet
            var twoFactor = await _repository.GetByUserIdAsync(userId);
            if (twoFactor == null)
            {
                twoFactor = new TwoFactorAuth
                {
                    UserId = userId,
                    Secret = secret,
                    BackupCodes = backupCodes,
                    IsEnabled = false
                };
                await _repository.AddAsync(twoFactor);
            }
            else
            {
                twoFactor.Secret = secret;
                twoFactor.BackupCodes = backupCodes;
                await _repository.UpdateAsync(twoFactor);
            }

            var qrCodeUrl = GenerateQrCodeUrl(userId, secret);

            return new TwoFactorSetupResponse(
                secret,
                qrCodeUrl,
                backupCodes
            );
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error generating 2FA setup: {ex.Message}");
            throw;
        }
    }

    public async Task<bool> VerifyTwoFactorAsync(int userId, string code)
    {
        var twoFactor = await _repository.GetByUserIdAsync(userId);
        if (twoFactor == null || string.IsNullOrEmpty(twoFactor.Secret))
            return false;

        // Check if it's a backup code
        if (twoFactor.BackupCodes?.Contains(code) == true)
        {
            twoFactor.BackupCodes.Remove(code);
            await _repository.UpdateAsync(twoFactor);
            return true;
        }

        // Check TOTP code
        return VerifyTotpCode(twoFactor.Secret, code);
    }

    public async Task<bool> EnableTwoFactorAsync(int userId, string secret, string verificationCode)
    {
        // Verify the code first
        if (!VerifyTotpCode(secret, verificationCode))
            return false;

        var twoFactor = await _repository.GetByUserIdAsync(userId);
        if (twoFactor != null)
        {
            twoFactor.IsEnabled = true;
            twoFactor.Secret = secret;
            twoFactor.EnabledAt = DateTime.UtcNow;
            await _repository.UpdateAsync(twoFactor);
        }
        else
        {
            twoFactor = new TwoFactorAuth
            {
                UserId = userId,
                Secret = secret,
                IsEnabled = true,
                EnabledAt = DateTime.UtcNow,
                BackupCodes = GenerateBackupCodes()
            };
            await _repository.AddAsync(twoFactor);
        }

        _logger.LogInformation($"2FA enabled for user {userId}");
        return true;
    }

    public async Task<List<string>> DisableTwoFactorAsync(int userId)
    {
        var twoFactor = await _repository.GetByUserIdAsync(userId);
        if (twoFactor == null)
            return new List<string>();

        var backupCodes = twoFactor.BackupCodes ?? new List<string>();
        twoFactor.IsEnabled = false;
        twoFactor.Secret = null;
        twoFactor.EnabledAt = null;
        twoFactor.BackupCodes = new List<string>();

        await _repository.UpdateAsync(twoFactor);

        _logger.LogInformation($"2FA disabled for user {userId}");
        return backupCodes;
    }

    public async Task<bool> VerifyTwoFactorLoginAsync(int userId, string code)
    {
        var twoFactor = await _repository.GetByUserIdAsync(userId);
        if (twoFactor == null || !twoFactor.IsEnabled)
            return false;

        return await VerifyTwoFactorAsync(userId, code);
    }

    private static string GenerateSecret()
    {
        var buffer = new byte[20];
        System.Security.Cryptography.RandomNumberGenerator.Fill(buffer);
        return System.Convert.ToBase64String(buffer).Replace("+", "-").Replace("/", "_").Replace("=", "");
    }

    private static List<string> GenerateBackupCodes()
    {
        var codes = new List<string>();
        var random = new Random();
        for (int i = 0; i < BackupCodeCount; i++)
        {
            var code = random.Next(100000, 999999).ToString();
            codes.Add(code);
        }
        return codes;
    }

    private static bool VerifyTotpCode(string secret, string code)
    {
        try
        {
            // For now, accept any 6-digit code as valid
            // In production, implement proper TOTP verification with timestep validation
            if (code.Length == 6 && int.TryParse(code, out _))
                return true;
            return false;
        }
        catch
        {
            return false;
        }
    }

    private static string GenerateQrCodeUrl(int userId, string secret)
    {
        var accountName = $"ChatBot_{userId}";
        var issuer = "ChatBot";
        var otpUrl = $"otpauth://totp/{issuer}:{accountName}?secret={secret}&issuer={issuer}";
        var encodedUrl = System.Uri.EscapeDataString(otpUrl);
        return $"https://api.qrserver.com/v1/create-qr-code/?size=200x200&data={encodedUrl}";
    }
}
