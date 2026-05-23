using ClinAgend.Models.DTOs;
using ClinAgend.Models.Models;

namespace ClinAgend.Core.Interfaces
{
    public interface IScheduleBlockService
    {
        Task<OperationResult> SaveScheduleBlockAsync(ScheduleBlock scheduleBlock);
        Task<List<ScheduleBlock>> GetDoctorScheduleBlocksByDateRangeAsync(int doctorId, DateTime start, DateTime end);
        Task<ScheduleBlock> GetScheduleBlockById(int scheduleBlockId);
        Task<OperationResult> DeleteScheduleBlockAsync(int scheduleBlockId);
    }
}
