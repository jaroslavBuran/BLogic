using BLogic.Models.Advisors;
using BLogic.Models.Clients;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BLogic.Models.Contracts
{
    public class EditContractViewModel
    {
        public Contract Contract { get; set; }

        public Client Client { get; set; }

        public Advisor Advisor { get; set; }
        public IEnumerable<Advisor> Advisors { get; set; }
        public IEnumerable<SelectListItem> Statuses { set; get; }
    }
}
