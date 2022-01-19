using System.Collections.Generic;

namespace BLogic.Models.Advisors
{
    public class Advisor
    {
        public int AdvisorId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string BirthNumber { get; set; }
        public int Age { get; set; }

        //M:N (poradce:smlouva)
        public ICollection<AdvisorContract> AdvisorContracts { get; set; }
    }
}
