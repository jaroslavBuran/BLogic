using BLogic.Models.Clients;
using System;
using System.Collections.Generic;

namespace BLogic.Models.Contracts
{
    public class Contract
    {
        public int ContractId { get; set; }
        public string EvidenceNumber { get; set; }
        public DateTime ClosureDate { get; set; }
        public DateTime ValidityDate { get; set; }

        //1:N (klient:smlouvy)
        public Client Client { get; set; }
        //M:N (poradce:smlouva)
        public ICollection<AdvisorContract> AdvisorContracts { get; set; }
    }
}
