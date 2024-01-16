using Colegio.Models;
using Microsoft.EntityFrameworkCore;

namespace Colegio
{
    public class ColegioContext : DbContext, IColegioContext
    {
        public DbSet<RegistrationDto> Registrations { get; set; }

        public ColegioContext(DbContextOptions<ColegioContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<RegistrationDto>(registration =>
            {
                registration.ToTable("Registration");
                registration.HasKey(x => x.Id);
                registration.Property(x => x.StudentIdentification).IsRequired().HasMaxLength(50);
                registration.Property(x => x.Institution).IsRequired();
                registration.Property(x => x.City).IsRequired();
                registration.Property(x => x.GradeId).IsRequired();
                registration.Property(x => x.StudentId).IsRequired();
            });
        }

        void IColegioContext.SaveChanges()
        {
            base.SaveChanges();
        }
    }
}
