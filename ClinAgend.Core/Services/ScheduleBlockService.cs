using ClinAgend.Core.Exceptions;
using ClinAgend.Core.Interfaces;
using ClinAgend.Data.Interfaces;
using ClinAgend.Models.DTOs;
using ClinAgend.Models.Models;

namespace ClinAgend.Core.Services
{
    public class ScheduleBlockService : IScheduleBlockService
    {
        private readonly IGenericRepository<ScheduleBlock> _scheduleBlockRepository;
        private readonly IScheduleValidatorService _scheduleValidatorService;

        public ScheduleBlockService(IGenericRepository<ScheduleBlock> scheduleBlockRepository, IScheduleValidatorService scheduleValidatorService)
        {
            _scheduleBlockRepository = scheduleBlockRepository;
            _scheduleValidatorService = scheduleValidatorService;
        }


        public async Task<OperationResult> SaveScheduleBlockAsync(ScheduleBlock scheduleBlock)
        {
            try
            {
                if (scheduleBlock == null)
                    return new OperationResult(false, "Dados do bloqueio inválidos.");

                if (scheduleBlock.StartTime == default || scheduleBlock.EndTime == default)
                    return new OperationResult(false, "Data e hora do bloqueio são obrigatórias.");

                if (scheduleBlock.EndTime <= scheduleBlock.StartTime)
                    return new OperationResult(false, "A data final deve ser maior que a data inicial.");

                if (scheduleBlock.DoctorId <= 0)
                    return new OperationResult(false, "Médico inválido.");

                if (scheduleBlock.ClinicId <= 0)
                    return new OperationResult(false, "Clínica inválida.");

                await _scheduleValidatorService.ValidateScheduleBlockTimeAsync(scheduleBlock);

                if (scheduleBlock.ScheduleBlockId == 0)
                {
                    await _scheduleBlockRepository.AddAsync(scheduleBlock);
                }
                else
                {
                    var existing = await _scheduleBlockRepository.GetByAsync(p => p.ScheduleBlockId == scheduleBlock.ScheduleBlockId);
                    if (existing == null)
                        return new OperationResult(false, "Bloqueio não encontrado");

                    existing.Description = scheduleBlock.Description;
                    existing.StartTime = scheduleBlock.StartTime;
                    existing.EndTime = scheduleBlock.EndTime;

                    await _scheduleBlockRepository.UpdateAsync(existing);
                }

                return new OperationResult(true, string.Empty);
            }
            catch (DomainValidationException ex)
            {
                return new OperationResult(false, ex.Message);
            }
            catch (Exception ex)
            {
                return new OperationResult(
                 false,
                 "Ocorreu um erro inesperado ao salvar o bloqueio. Tente novamente mais tarde.");
            }
        }

        public async Task<ScheduleBlock> GetScheduleBlockById(int scheduleBlockId)
        {
            return await _scheduleBlockRepository.GetByAsNoTrackingAsync(
                s => s.ScheduleBlockId == scheduleBlockId
                );
        }

        public async Task<List<ScheduleBlock>> GetDoctorScheduleBlocksByDateRangeAsync(int doctorId, DateTime start, DateTime end)
        {
            if (end < start)
                throw new ArgumentException("A data final não pode ser menor que a data inicial.");

            return await _scheduleBlockRepository.GetManyAsNoTrackingByAsync(
                s =>
                s.DoctorId == doctorId &&
                s.StartTime >= start &&
                s.EndTime <= end
           );
        }

        public async Task<OperationResult> DeleteScheduleBlockAsync(int scheduleBlockId)
        {
            try
            {
                var scheduleBlock = await _scheduleBlockRepository.GetByAsync(p => p.ScheduleBlockId == scheduleBlockId);
                if (scheduleBlock == null)
                    return new OperationResult(false, "Bloqueio não encontrado");

                await _scheduleBlockRepository.DeleteAsync(scheduleBlock);
                return new OperationResult(true, string.Empty);
            }
            catch (Exception ex)
            {
                return new OperationResult(
                   false,
                   "Ocorreu um erro inesperado ao excluir o bloqueio. Tente novamente mais tarde.");
            }
        }


    }
}
