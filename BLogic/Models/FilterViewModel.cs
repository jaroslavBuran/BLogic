using BLogic.Models.Advisors;
using BLogic.Models.Clients;
using BLogic.Models.Contracts;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace BLogic.Models
{
    public class FilterViewModel
    {
        public IEnumerable<Contract> DataContract { set; get; }
        public IEnumerable<Client> DataClient { set; get; }
        public IEnumerable<Advisor> DataAdvisor { set; get; }
        public IEnumerable<SelectListItem> Statuses { set; get; }
        public string SelectedStatus { set; get; }
    }
}
