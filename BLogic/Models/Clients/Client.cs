using BLogic.Models.Contracts;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLogic.Models.Clients
{
    public class Client
    {
        public int ClientId { get; set; }
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
        //1:N (klient:smlouvy)
        public ICollection<Contract> Contracts { get; set; }
    }
}
