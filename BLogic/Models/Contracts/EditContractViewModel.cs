using BLogic.Models.Advisors;
using BLogic.Models.Clients;

namespace BLogic.Models.Contracts
{
    public class EditContractViewModel
    {
        public Contract Contract { get; set; }

        public Client Client { get; set; }

        public Advisor Advisor { get; set; }
    }
}
