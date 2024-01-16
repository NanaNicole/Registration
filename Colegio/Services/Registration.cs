using Colegio.Models;
using Colegio.Services.Interfaces;
using Confluent.Kafka;
using Newtonsoft.Json;
using Student.Producer;

namespace Colegio.Services
{
    public class Registration : IRegistration
    {

        private readonly ILogger<Registration> _logger;
        private readonly IColegioContext _dbContext;
        private readonly IProducer _producer;

        public Registration(ILogger<Registration> logger, IColegioContext dbContext, IProducer producer)
        {
            _logger = logger;
            _dbContext = dbContext;
            _producer = producer;
        }

        public List<RegistrationDto> GetRegistrations()
        {
            List<RegistrationDto> result = null;
            try
            {
                result = _dbContext.Registrations.ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError($"error obteniendo data del enrolamiento {ex.Message}");
            }
            return result;
        }

        public RegistrationDto GetRegistrationByEstudentIdentification(int studentIdentification)
        {
            RegistrationDto result = null;
            try
            {
                result = _dbContext.Registrations.Where(x => x.StudentIdentification == studentIdentification)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError($"error obteniendo data del enrolamiento {ex.Message.ToString()}");
            }
            return result;
        }

        public RegistrationDto GetRegistrationByInstitution(string institution)
        {
            RegistrationDto result = null;
            try
            {
                result = _dbContext.Registrations.Where(x => x.Institution == institution)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError($"error obteniendo data del enrolamiento {ex.Message.ToString()}");
            }
            return result;
        }
        public RegistrationDto GetRegistrationByCity(string city)
        {
            RegistrationDto result = null;
            try
            {
                result = _dbContext.Registrations.Where(x => x.City == city)
                    .FirstOrDefault();
            }
            catch (Exception ex)
            {
                _logger.LogError($"error obteniendo data del enrolamiento {ex.Message.ToString()}");
            }
            return result;
        }

        public bool CreateRegistration(RegistrationDto registration)
        {
            bool result = false;
            try
            {
                registration.Id = Guid.NewGuid();
                _dbContext.Registrations.Add(registration);
                _dbContext.SaveChanges();
                result = true;
                _producer.ProduceMessage(JsonConvert.SerializeObject(registration));
            }
            catch (Exception ex)
            {
                _logger.LogError($"error creando data del enrolamiento {ex.Message.ToString()}");
            }
            return result;
        }

        public bool DeleteRegistration(Guid id)
        {
            bool result = false;
            try
            {
                RegistrationDto registration = FindRegistration(id);
                if (registration != null)
                {
                    _dbContext.Registrations.Remove(registration);
                    _dbContext.SaveChanges();
                    result = true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"error obteniendo data del enrolamiento {ex.Message.ToString()}");
            }
            return result;
        }

        public RegistrationDto FindRegistration(Guid id)
        {
            return _dbContext.Registrations.Find(id);
        }
    }
}
