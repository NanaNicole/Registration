using Colegio.Models;
using Microsoft.EntityFrameworkCore;

namespace Colegio
{
    public interface IColegioContext
    {
        DbSet<RegistrationDto> Registrations { get; set; }

        void SaveChanges();
    }
}