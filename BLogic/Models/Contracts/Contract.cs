using BLogic.Models.Clients;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLogic.Models.Contracts
{
    public class Contract
    {
        public int ContractId { get; set; }
        [Required(ErrorMessage = "Evidenční číslo musí být vyplněno.")]
        [Display(Name = "Evidenční číslo")]
        public string EvidenceNumber { get; set; }
        [Required(ErrorMessage = "Datum uzavření musí být vyplněno.")]
        [Display(Name = "Datum uzavření")]
        [DataType(DataType.Date)]
        public DateTime ClosureDate { get; set; }
        [Required(ErrorMessage = "Datum platnosti musí být vyplněno.")]
        [Display(Name = "Datum platnosti")]
        [DataType(DataType.Date)]
        public DateTime ValidityDate { get; set; }

        //1:N (klient:smlouvy)
        public Client Client { get; set; }
        //M:N (poradce:smlouva)
        public ICollection<AdvisorContract> AdvisorContracts { get; set; }
    }
}
