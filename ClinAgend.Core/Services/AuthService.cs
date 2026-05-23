using ClinAgend.Data.Models;
using ClinAgend.Models.DTOs;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ClinAgend.Data;
using ClinAgend.Models.Enums;
using ClinAgend.Core.Interfaces;
using System.Web;
using ClinAgend.Models.Settings;
using Microsoft.Extensions.Options;

namespace ClinAgend.Core.Services
{
    public class AuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;
        private readonly IDoctorService _doctorService;
        private readonly IEmailService _emailService;
        private readonly AppSettings _appSettings;
        private readonly IEmailTemplateService _templateService;

        public AuthService(
           UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager,
           ApplicationDbContext context,
           IDoctorService doctorService,
           IEmailService emailService,
           IOptions<AppSettings> appSettings,
           IEmailTemplateService templateService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _doctorService = doctorService;
            _emailService = emailService;
            _appSettings = appSettings.Value;
            _templateService = templateService;
        }

        public async Task<OperationResult> LoginAsync(string access, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return new OperationResult(false, "Email e senha são obrigatórios.");

            email = _userManager.NormalizeEmail(email.Trim());
            access = access?.Trim().ToUpperInvariant();

            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return new OperationResult(false, "Email ou senha inválidos.");

            var signInResult = await _signInManager.CheckPasswordSignInAsync(
                user,
                password,
                lockoutOnFailure: true
            );

            if (!signInResult.Succeeded)
                return new OperationResult(false, "Email ou senha inválidos.");

            if (user.Type != UserType.SuperAdmin)
            {
                if (string.IsNullOrWhiteSpace(access))
                    return new OperationResult(false, "Email ou senha inválidos.");

                var clinic = await _context.Clinics
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Acess.ToUpper() == access);

                if (clinic == null || user.ClinicId != clinic.ClinicId)
                    return new OperationResult(false, "Email ou senha inválidos.");
            }

            await _signInManager.SignInAsync(user, isPersistent: false);
            return new OperationResult(true, string.Empty);
        }


        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<OperationResult> RegisterUserAsync(
             string fullName,
             string email,
             string password,
             UserType userType,
             int clinicId,
             int? doctorId = null)
        {
            if (string.IsNullOrWhiteSpace(fullName) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                return new OperationResult(false, "Dados do usuário inválidos.");
            }

            if (userType == UserType.Doctor && doctorId == null)
            {
                return new OperationResult(false, "Usuário do tipo médico precisa estar vinculado a um médico.");
            }

            email = email.Trim();
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    Name = fullName,
                    ClinicId = clinicId,
                    Type = userType
                };

                var result = await _userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    if (result.Errors.Any(e => e.Code == "DuplicateEmail"))
                    {
                        return new OperationResult(false, "Já existe um usuário cadastrado com este e-mail.");
                    }

                    return new OperationResult(false, "Não foi possível cadastrar o usuário.");
                }

                if (userType == UserType.Doctor)
                {
                    await _doctorService.LinkUserAsync(doctorId!.Value, user.Id);
                }

                await transaction.CommitAsync();
                return new OperationResult(true, string.Empty);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<OperationResult> ChangePasswordAsync(
            string userId,
            string currentPassword,
            string newPassword)
        {
            if (string.IsNullOrWhiteSpace(userId) ||
                string.IsNullOrWhiteSpace(currentPassword) ||
                string.IsNullOrWhiteSpace(newPassword))
            {
                return new OperationResult(false, "Dados inválidos.");
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new OperationResult(false, "Usuário não encontrado.");

            var result = await _userManager.ChangePasswordAsync(
                user,
                currentPassword,
                newPassword);

            if (!result.Succeeded)
            {
                return new OperationResult(false, "Não foi possível alterar a senha.");
            }

            return new OperationResult(true, string.Empty);
        }


        public async Task ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return;

            var token = await _userManager
                .GeneratePasswordResetTokenAsync(user);

            var resetLink =
                $"{_appSettings.BaseUrl}/redefinir-senha" +
                $"?email={Uri.EscapeDataString(email)}" +
                $"&token={Uri.EscapeDataString(token)}";

            var body = await _templateService
                .GetTemplateAsync("ForgotPassword.html");

            body = body.Replace(
                "{{RESET_LINK}}",
                resetLink);

            body = body.Replace(
                "{{CURRENT_YEAR}}",
                DateTime.Now.Year.ToString());

            await _emailService.SendAsync(
                email,
                "Redefinição de senha",
                body);
        }

        public async Task<OperationResult> ResetPasswordAsync(
              string email,
              string token,
              string newPassword)
        {
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(token) ||
                string.IsNullOrWhiteSpace(newPassword))
            {
                return new OperationResult(
                    false,
                    "Dados inválidos.");
            }

            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return new OperationResult(
                    false,
                    "Usuário não encontrado.");
            }

            var result = await _userManager.ResetPasswordAsync(
                user,
                token,
                newPassword);

            if (!result.Succeeded)
            {
                var invalidToken = result.Errors.Any(e => e.Code == "InvalidToken");

                if (invalidToken)
                {
                    return new OperationResult(
                        false,
                        "O link de redefinição é inválido ou expirou.");
                }

                return new OperationResult(
                    false,
                    result.Errors.First().Description);
            }

            return new OperationResult(
                true,
                "Senha alterada com sucesso!");
        }
    }
}
