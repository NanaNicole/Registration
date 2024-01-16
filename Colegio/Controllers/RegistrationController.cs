using Colegio.Models;
using Colegio.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Colegio.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RegistrationController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

        private readonly ILogger<RegistrationController> _logger;
        private ColegioContext _dbcontext;
        private readonly IRegistration _registration;

        public RegistrationController(ILogger<RegistrationController> logger, ColegioContext dbcontext, IRegistration registration)
        {
            _logger = logger;
            _dbcontext = dbcontext;
            _registration = registration;
        }

        [HttpGet]
        [Route("GetRegistrations")]
        public IActionResult GetRegistrations()
        {

            return Ok(_registration.GetRegistrations());
        }

        [HttpGet]
        [Route("GetRegistrationByEstudentIdentification/{studentIdentification}")]
        public IActionResult GetRegistrationByEstudentIdentification(int studentIdentification)
        {
            return Ok(_registration.GetRegistrationByEstudentIdentification(studentIdentification));
        }

        [HttpGet]
        [Route("GetRegistrationByInstitution/{institution}")]
        public IActionResult GetRegistrationByInstitution(string institution)
        {
            return Ok(_registration.GetRegistrationByInstitution(institution));
        }

        [HttpGet]
        [Route("GetRegistrationByCity/{studentIdentification}")]
        public IActionResult GetRegistrationByCity(string city)
        {
            return Ok(_registration.GetRegistrationByCity(city));
        }

        [HttpPost]
        [Route("CreateRegistration")]
        public IActionResult CreateRegistration(RegistrationDto registration)
        {
            return Ok(_registration.CreateRegistration(registration));
        }

        [HttpDelete]
        [Route("DeleteRegistration/{id}")]
        public IActionResult DeleteRegistration(Guid id)
        {
            try
            {
                if (_registration.DeleteRegistration(id))
                {
                    return Ok();
                }
                else
                {
                    return BadRequest("No se pudo eliminar correctamente el enrolamiento");
                }
            }
            catch (Exception ex)
            {
                return BadRequest($"Error eliminando enrolamientos {ex.Message.ToString()}");
            }
        }


    }
}