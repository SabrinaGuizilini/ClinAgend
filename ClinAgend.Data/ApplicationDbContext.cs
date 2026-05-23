using ClinAgend.Data.Models;
using ClinAgend.Models.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ClinAgend.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Clinic> Clinics { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }
        public DbSet<ScheduleBlock> ScheduleBlocks { get; set; }
        public DbSet<ClinicSettings> ClinicSettings { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Clinic>()
                .HasOne(c => c.Settings)
                .WithOne(s => s.Clinic)
                .HasForeignKey<ClinicSettings>(s => s.ClinicId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ApplicationUser>()
               .HasOne(u => u.Clinic)
               .WithMany()
               .HasForeignKey(u => u.ClinicId)
               .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany()
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany()
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .HasOne(a => a.Clinic)
                .WithMany()
                .HasForeignKey(a => a.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ScheduleBlock>()
                .HasOne(a => a.Doctor)
                .WithMany()
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<ScheduleBlock>()
                .HasOne(a => a.Clinic)
                .WithMany()
                .HasForeignKey(a => a.ClinicId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Appointment>()
                .Property(a => a.StartTime)
                .HasColumnType("timestamp without time zone");

            builder.Entity<Appointment>()
                .Property(a => a.EndTime)
                .HasColumnType("timestamp without time zone");

            builder.Entity<Patient>()
                 .Property(p => p.BirthDate)
                 .HasColumnType("date");

            builder.Entity<ScheduleBlock>()
                .Property(s => s.StartTime)
                .HasColumnType("timestamp without time zone");

            builder.Entity<ScheduleBlock>()
                .Property(s => s.EndTime)
                .HasColumnType("timestamp without time zone");

            builder.Entity<Doctor>()
                .Property(d => d.StartTime)
                .HasColumnType("time without time zone");

            builder.Entity<Doctor>()
                .Property(d => d.EndTime)
                .HasColumnType("time without time zone");
        }
    }
}
