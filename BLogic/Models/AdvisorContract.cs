using BLogic.Models.Advisors;
using BLogic.Models.Contracts;

namespace BLogic.Models
{
    public class AdvisorContract
    {
        public int AdvisorId { get; set; }
        public Advisor Advisor { get; set; }

        public int ContractId { get; set; }
        public Contract Contract { get; set; }
    }
}
