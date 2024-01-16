using Colegio.Models;

namespace Colegio.Services.Interfaces
{
    public interface IRegistration
    {
        public List<RegistrationDto> GetRegistrations();
        public RegistrationDto GetRegistrationByEstudentIdentification(int studentIdentification);
        public RegistrationDto GetRegistrationByInstitution(string institution);
        public RegistrationDto GetRegistrationByCity(string city);
        public bool CreateRegistration(RegistrationDto estudent);
        public bool DeleteRegistration(Guid id);

        public RegistrationDto FindRegistration(Guid id);
    }
}
