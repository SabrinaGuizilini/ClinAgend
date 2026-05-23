using ClinAgend.Core.Exceptions;
using ClinAgend.Core.Interfaces;
using ClinAgend.Data.Interfaces;
using ClinAgend.Models.Models;
using Microsoft.EntityFrameworkCore;

namespace ClinAgend.Core.Services
{
    public class ScheduleValidatorService : IScheduleValidatorService
    {
        private readonly IGenericRepository<Appointment> _appointmentRepository;
        private readonly IGenericRepository<ScheduleBlock> _scheduleBlockRepository;
        private readonly IGenericRepository<Doctor> _doctorRepository;

        public ScheduleValidatorService(
            IGenericRepository<Appointment> appointmentRepository,
            IGenericRepository<ScheduleBlock> scheduleBlockRepository,
            IGenericRepository<Doctor> doctorRepository)
        {
            _appointmentRepository = appointmentRepository;
            _scheduleBlockRepository = scheduleBlockRepository;
            _doctorRepository = doctorRepository;
        }

        public async Task ValidateAppointmentTimeAsync(Appointment appointment)
        {
            // 1 - Validação básica de início e fim
            if (appointment.EndTime <= appointment.StartTime)
                throw new DomainValidationException("O horário de término deve ser posterior ao horário de início.");

            if (appointment.StartTime < DateTime.Now && appointment.AppointmentId == 0)
                throw new DomainValidationException("Não é possível agendar atendimentos para datas passadas.");

            // 2 - Busca os dados do médico
            var doctor = await _doctorRepository.GetByAsNoTrackingAsync(d => d.DoctorId == appointment.DoctorId);
            if (doctor == null)
                throw new DomainValidationException("Médico não encontrado.");

            // 3 - Verifica se o dia da consulta é um dia de trabalho
            var appointmentDay = GetPortugueseDayOfWeek(appointment.StartTime.DayOfWeek);
            var workingDays = doctor.WorkingDays.Split(',')
                .Select(d => d.Trim())
                .ToList();

            if (!workingDays.Contains(appointmentDay))
                throw new DomainValidationException($"O médico não atende à(o)s {appointmentDay}s.");

            // 4 - Verifica se está dentro do horário de expediente
            var startTime = appointment.StartTime.TimeOfDay;
            var endTime = appointment.EndTime.TimeOfDay;

            if (startTime < doctor.StartTime || endTime > doctor.EndTime)
                throw new DomainValidationException("O horário está fora do expediente do médico.");

            // 5 - Verifica se há bloqueios de horário
            bool hasBlock = await HasBlockConflictAsync(
                appointment.DoctorId,
                appointment.StartTime,
                appointment.EndTime
            );

            if (hasBlock)
                throw new DomainValidationException("O horário selecionado está bloqueado para o médico.");

            // 6 - Verifica se há conflito com outras consultas
            var sameDayAppointments = await _appointmentRepository.Query()
                .Where(a => a.DoctorId == appointment.DoctorId &&
                            a.StartTime.Date == appointment.StartTime.Date &&
                            a.AppointmentId != appointment.AppointmentId &&
                            a.AppointmentStatus != Models.Enums.AppointmentStatus.Cancelado)
                .ToListAsync();

            foreach (var existing in sameDayAppointments)
            {
                bool overlap = appointment.StartTime < existing.EndTime &&
                               appointment.EndTime > existing.StartTime;

                if (overlap)
                {
                    throw new DomainValidationException($"O horário conflita com outra consulta agendada entre {existing.StartTime:HH:mm} e {existing.EndTime:HH:mm}.");
                }
            }

            // 7 - Verifica intervalo mínimo entre consultas
            if (doctor.BreakBetweenAppointments.HasValue && doctor.BreakBetweenAppointments.Value > 0)
            {
                int breakMinutes = doctor.BreakBetweenAppointments.Value;

                foreach (var existing in sameDayAppointments)
                {
                    var minStart = existing.EndTime;
                    var maxEnd = existing.EndTime.AddMinutes(breakMinutes);

                    bool overlapWithBreak = appointment.StartTime < maxEnd && appointment.EndTime > minStart;

                    if (overlapWithBreak)
                        throw new DomainValidationException($"É necessário um intervalo mínimo de {breakMinutes} minutos entre as consultas.");
                }
            }

            // 8 - Verifica o limite diário de consultas
            if (doctor.MaxAppointmentsPerDay.HasValue)
            {
                int sameDayCount = sameDayAppointments.Count;

                if (sameDayCount >= doctor.MaxAppointmentsPerDay.Value)
                    throw new DomainValidationException("O limite diário de consultas para este médico foi atingido.");
            }
        }

        public async Task ValidateScheduleBlockTimeAsync(ScheduleBlock scheduleBlock)
        {
            // 1️ - Validação básica de consistência
            if (scheduleBlock.StartTime >= scheduleBlock.EndTime)
                throw new DomainValidationException("A data e hora inicial devem ser anteriores à data e hora final.");

            if (scheduleBlock.StartTime < DateTime.Now && scheduleBlock.ScheduleBlockId == 0)
                throw new DomainValidationException("Não é possível agendar bloqueios para datas passadas.");

            // 2️ - Verifica se o médico existe e obtém suas informações
            var doctor = await _doctorRepository.GetByAsNoTrackingAsync(d => d.DoctorId == scheduleBlock.DoctorId);
            if (doctor == null)
                throw new DomainValidationException("Médico não encontrado.");

            // 3️ - Valida se o bloqueio ocorre em dias de trabalho do médico
            var StartBlockDay = GetPortugueseDayOfWeek(scheduleBlock.StartTime.DayOfWeek);
            var EndBlockDay = GetPortugueseDayOfWeek(scheduleBlock.EndTime.DayOfWeek);
            var workingDays = doctor.WorkingDays.Split(',')
                .Select(d => d.Trim())
                .ToList();

            if (!workingDays.Contains(StartBlockDay) || !workingDays.Contains(EndBlockDay))
                throw new DomainValidationException("O bloqueio deve estar dentro dos dias de trabalho do médico.");

            // 4️ -  Verifica se o horário está dentro do expediente do médico
            var doctorStart = doctor.StartTime;
            var doctorEnd = doctor.EndTime;

            var blockStart = scheduleBlock.StartTime.TimeOfDay;
            var blockEnd = scheduleBlock.EndTime.TimeOfDay;

            if (blockStart < doctorStart || blockEnd > doctorEnd)
                throw new DomainValidationException($"O bloqueio deve estar dentro do horário de expediente do médico ({doctorStart:hh\\:mm} - {doctorEnd:hh\\:mm}).");

            // 5️ - Verifica se há outro bloqueio que conflita (ignorando o próprio se for edição)
            bool hasBlockConflict = await HasBlockConflictAsync(
                scheduleBlock.DoctorId,
                scheduleBlock.StartTime,
                scheduleBlock.EndTime,
                scheduleBlock.ScheduleBlockId == 0 ? null : scheduleBlock.ScheduleBlockId
            );

            if (hasBlockConflict)
                throw new DomainValidationException("Já existe um bloqueio de horário que conflita com o intervalo informado.");

            // 6️ - Verifica se há consultas que conflitam com o intervalo
            var sameDayAppointments = await _appointmentRepository.Query()
                  .Where(a => a.DoctorId == scheduleBlock.DoctorId &&
                              a.StartTime.Date == scheduleBlock.StartTime.Date &&
                              a.AppointmentStatus != Models.Enums.AppointmentStatus.Cancelado)
                  .ToListAsync();

            foreach (var existing in sameDayAppointments)
            {
                bool overlap = scheduleBlock.StartTime < existing.EndTime &&
                               scheduleBlock.EndTime > existing.StartTime;

                if (overlap)
                {
                    throw new DomainValidationException($"O horário conflita com uma consulta agendada entre {existing.StartTime:HH:mm} e {existing.EndTime:HH:mm}.");
                }
            }

            // 7 - Verifica intervalo mínimo entre consultas
            if (doctor.BreakBetweenAppointments.HasValue && doctor.BreakBetweenAppointments.Value > 0)
            {
                int breakMinutes = doctor.BreakBetweenAppointments.Value;

                foreach (var existing in sameDayAppointments)
                {
                    var minStart = existing.EndTime;
                    var maxEnd = existing.EndTime.AddMinutes(breakMinutes);

                    bool overlapWithBreak = scheduleBlock.StartTime < maxEnd && scheduleBlock.EndTime > minStart;

                    if (overlapWithBreak)
                        throw new DomainValidationException($"O horário conflita com o intervalo de {breakMinutes} minutos de uma consulta.");
                }
            }
        }

        private async Task<bool> HasBlockConflictAsync(int doctorId, DateTime start, DateTime end, int? excludeBlockId = null)
        {
            var query = _scheduleBlockRepository.Query()
                .Where(b =>
                    b.DoctorId == doctorId &&
                    b.StartTime < end &&
                    b.EndTime > start
                );

            // Ignora o próprio bloqueio, se for atualização
            if (excludeBlockId.HasValue)
                query = query.Where(b => b.ScheduleBlockId != excludeBlockId.Value);

            return await query.AnyAsync();
        }


        private static string GetPortugueseDayOfWeek(DayOfWeek day)
        {
            return day switch
            {
                DayOfWeek.Monday => "Segunda",
                DayOfWeek.Tuesday => "Terça",
                DayOfWeek.Wednesday => "Quarta",
                DayOfWeek.Thursday => "Quinta",
                DayOfWeek.Friday => "Sexta",
                DayOfWeek.Saturday => "Sábado",
                DayOfWeek.Sunday => "Domingo",
                _ => throw new ArgumentOutOfRangeException(nameof(day), day, null)
            };
        }
        
    }
}
