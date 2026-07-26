using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TenxOCC.Data.Entities
{
    [Table("Users")]
    public class Users : BaseEntity, IValidatableObject
    {
        [Key]
        public int UserId { get; set; }

        [StringLength(100)]
        public string UserName { get; set; }

        [StringLength(100)]
        public string Password { get; set; }

        [Required]
        [StringLength(150)]
        public string Email { get; set; }

        [StringLength(50)]
        public string Phone { get; set; }

       

        //Vamshi Sai
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            // Email is mandatory
            if (string.IsNullOrWhiteSpace(Email))
            {
                results.Add(new ValidationResult("Email cannot be empty", new[] { nameof(Email) }));
            }

            // Validate password ONLY if it is being changed
            if (!string.IsNullOrWhiteSpace(Password))
            {
                if (Password.Length < 6)
                {
                    results.Add(new ValidationResult(
                        "Password must be at least 6 characters",
                        new[] { nameof(Password) }
                    ));
                }
            }

           

            return results;
        }
  }

}
