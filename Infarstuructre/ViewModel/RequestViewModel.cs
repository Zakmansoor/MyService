using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domin.Entity;
using System.ComponentModel.DataAnnotations;
namespace Infarstuructre.ViewModel
{


    public class RequestViewModel
    {

        public int RequestId { get; set; }


        [Required]
        [Display(Name = "Service")]
        public int ServiceId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Order Date")]
        public DateTime OrderDate { get; set; }

        [Required]
        public bool Status { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public string Comment { get; set; }

        // Collections to populate dropdowns in the view
        public IEnumerable<Service> Services { get; set; }
    }
}


