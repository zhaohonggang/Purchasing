using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Purchasing.Core.Domain;

namespace Purchasing.Web.Models
{
    public class WorkgroupPeoplePostModel
    {
        [Required(ErrorMessage = "Must add at least one user")]
        public List<string> Users { get; set; }
        [Required]
        public Role Role { get; set; }
    }
}