using ClinAgend.Core.Interfaces;
using ClinAgend.Data;
using ClinAgend.Data.Interfaces;
using ClinAgend.Data.Models;
using ClinAgend.Models.DTOs;
using ClinAgend.Models.Enums;
using ClinAgend.Models.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace ClinAgend.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<ApplicationUser> _userRepository;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGenericRepository<Doctor> _doctorRepository;
        private readonly ApplicationDbContext _context;

        public UserService(IGenericRepository<ApplicationUser> userRepository, IGenericRepository<Doctor> doctorRepository, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _userRepository = userRepository;
            _doctorRepository = doctorRepository;
            _context = context;
            _userManager = userManager;
        }

        public async Task<(List<ApplicationUser> Users, int TotalCount)> GetUsersPaginatedAsync(
            int clinicId,
            int page,
            int pageSize,
            string? search = null,
            string? sortBy = null,
            bool descending = false)
        {
            var query = _userRepository.Query()
                        .Where(u => u.ClinicId == clinicId).AsNoTracking();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u =>
                    EF.Functions.ILike(u.Name, $"%{search}%") ||
                    EF.Functions.ILike(u.Email!, $"%{search}%"));
            }

            var total = await query.CountAsync();

            query = sortBy switch
            {
                nameof(ApplicationUser.Name) =>
                    descending ? query.OrderByDescending(u => u.Name) : query.OrderBy(u => u.Name),

                nameof(ApplicationUser.Email) =>
                    descending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),

                nameof(ApplicationUser.Type) =>
                    descending ? query.OrderByDescending(u => u.Type) : query.OrderBy(u => u.Type),

                _ => query.OrderBy(u => u.Name)
            };

            var users = await query
                .Skip(page * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (users, total);
        }

        public async Task<OperationResult> DeleteUserAsync(string userId)
        {
            await using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                    return new OperationResult(false, "Usuário não encontrado");

                if (user.Type == UserType.Doctor)
                {
                    var doctor = await _doctorRepository.GetByAsync(d => d.UserId == userId);

                    if (doctor != null)
                    {
                        doctor.UserId = null;
                        await _doctorRepository.UpdateAsync(doctor);
                    }
                }

                var result = await _userManager.DeleteAsync(user);
                if (!result.Succeeded)
                    return new OperationResult(false, "Ocorreu um erro inesperado ao excluir o usuário. Tente novamente mais tarde.");

                await transaction.CommitAsync();
                return new OperationResult(true, string.Empty);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<OperationResult> UpdateUserNameAsync(string userId, string newName)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new OperationResult(false, "Usuário não encontrado");

            user.Name = newName;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return new OperationResult(false, "Ocorreu um erro inesperado ao atualizar o usuário. Tente novamente mais tarde.");

            return new OperationResult(true, string.Empty);
        }

        public async Task<OperationResult> UpdateUserTypeAsync(string userId, UserType newType)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new OperationResult(false, "Usuário não encontrado");

            user.Type = newType;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return new OperationResult(false, "Ocorreu um erro inesperado ao atualizar o tipo do usuário. Tente novamente mais tarde.");

            return new OperationResult(true, string.Empty);
        }

    }
}
