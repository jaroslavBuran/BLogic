using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLogic.Models.Advisors
{
    public class Advisor
    {
        public int AdvisorId { get; set; }
        [Required(ErrorMessage = "Jméno musí být vyplněno.")]
        [Display(Name = "Jméno")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Příjmení musí být vyplněno.")]
        [Display(Name = "Příjmení")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Email musí být vyplněn.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Telefon musí být vyplněn.")]
        [Display(Name = "Telefon")]
        public string Phone { get; set; }
        [Required(ErrorMessage = "Rodné číslo musí být vyplněno.")]
        [Display(Name = "Rodné číslo")]
        public string BirthNumber { get; set; }
        [Required(ErrorMessage = "Věk musí být vyplněn.")]
        [Display(Name = "Věk")]
        public int Age { get; set; }

        //M:N (poradce:smlouva)
        public ICollection<AdvisorContract> AdvisorContracts { get; set; }
    }
}
