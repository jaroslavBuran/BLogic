using System.ComponentModel.DataAnnotations;

namespace BLogic.Models
{
    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "Stávající heslo musí být vyplněno!")]
        [DataType(DataType.Password)]
        [Display(Name = "Současné heslo")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "Nové heslo musí být vyplněno!")]
        [DataType(DataType.Password)]
        [StringLength(100, ErrorMessage = "{0} musí mít délku alespoň {2} a nejvíc {1} znaků.", MinimumLength = 6)]
        [RegularExpression("^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z])(?=.*[^a-zA-Z0-9]).{5,}$", ErrorMessage = "Heslo musí obsahovat alespoň jeden speciální znak, alespoň jedno číslo a alespoň jedno velké a jedno malé písmeno")]
        [Display(Name = "Nové heslo")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Potvrzení hesla musí být vyplněno!")]
        [DataType(DataType.Password)]
        [Display(Name = "Potvrzení nového hesla")]
        [Compare("NewPassword", ErrorMessage = "Zadaná hesla se musí shodovat!")]
        public string ConfirmPassword { get; set; }
    }
}
