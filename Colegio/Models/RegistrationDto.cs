using System.ComponentModel.DataAnnotations.Schema;

namespace Colegio.Models
{
    public class RegistrationDto
    {
        //[Key]
        public Guid Id { get; set; }
        public int StudentIdentification { get; set; }
        public string Institution { get; set; }
        public string City { get; set; }

        public Guid GradeId { get; set; }
        public Guid StudentId { get; set; }
    }
}
