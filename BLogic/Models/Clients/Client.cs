using BLogic.Models.Contracts;
using System.Collections.Generic;

namespace BLogic.Models.Clients
{
    public class Client
    {
        public int ClientId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string BirthNumber { get; set; }
        public int Age { get; set; }
        //1:N (klient:smlouvy)
        public ICollection<Contract> Contracts { get; set; }
    }
}
