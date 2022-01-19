using BLogic.Models.Clients;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BLogic.Models.Contracts
{
    public class Contract
    {
        public int ContractId { get; set; }
        public string EvidenceNumber { get; set; }
        [DataType(DataType.Date)]
        public DateTime ClosureDate { get; set; }
        [DataType(DataType.Date)]
        public DateTime ValidityDate { get; set; }

        //1:N (klient:smlouvy)
        public Client Client { get; set; }
        //M:N (poradce:smlouva)
        public ICollection<AdvisorContract> AdvisorContracts { get; set; }
    }
}
