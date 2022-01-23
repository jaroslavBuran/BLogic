using System.ComponentModel.DataAnnotations;

namespace BLogic.Models
{
    public class ChangeEmailViewModel
    {
        [Display(Name = "Stávající email")]
        public string OldEmail { get; set; }
        [Required(ErrorMessage = "Nový email musí být vyplněn!")]
        [Display(Name = "Nový email")]
        [DataType(DataType.EmailAddress, ErrorMessage = "Email musí být ve formátu: jmeno@domena.cz")]
        public string NewEmail { get; set; }
    }
}
