using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Diplomain_Sargsyan_Gevorg.Models
{
    public class Context : DbContext
    {

        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<User> Users { get; set; }
        public Context(DbContextOptions<Context> options)
    : base(options)
        {


        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<PateintsDoctor>().HasKey(t => new { t.DoctorId, t.PatientId });

            //modelBuilder.Entity<PateintsDoctor>()
            //    .HasOne(pt => pt.Patient)
            //    .WithMany(p => p.PateintsDoctor)
            //    .HasForeignKey(p => p.PatientId);

            //modelBuilder.Entity<PateintsDoctor>()
            //    .HasOne(d => d.Doctor)
            //    .WithMany(d => d.PatientsDoctor)
            //    .HasForeignKey(d => d.DoctorId);

        }


    }
    public class Message
    {
        public int Id { get; set; }
        public int IdFrom { get; set; }
        public int IdTo { get; set; }
        public string Text { get; set; }

        public string IsDoctor{ get; set; }
        public User User { get; set; }
        public DateTime Timestamp { get; set; }
        public Doctor Doctor { get; set; }
        public Patient Patient { get; set; }


    }


    public class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public bool IsDoctor { get; set; }



    }
    public class Doctor
    {
        [ForeignKey("User")]
        public int UserId { get; set; }

        public User User { get; set; }
        public int Id { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string MiddleName{ get; set; }
        public string Email{ get; set; }

        public string PhoneNumber { get; set; }
        public int Age { get; set; }

        public byte[] Avatar { get; set; }
        public string Speciality { get; set; }
        public List<Patient> Patients { get; set; } = new List<Patient>();
        public List<Message> Messages { get; set; } = new List<Message>();

    }

    public class Patient 
    {
        [ForeignKey("User")]
        public int UserId { get; set; }

        public User User { get; set; }

        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }

        public string PhoneNumber { get; set; }
        public int Age { get; set; }
        public byte[] Avatar { get; set; }
        public string Speciality { get; set; }
        public List<Doctor> Doctors { get; set; } = new List<Doctor>();
        public List<Message> Messages { get; set; } = new List<Message>();

    }

}
