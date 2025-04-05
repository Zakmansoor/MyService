using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.Entity
{
    public class Service
    {
        [Key]

        public int ServiceID { get; set; }
        [Required(ErrorMessage = "Name Servic is required.")]

        public string Name { get; set; }
        [Required(ErrorMessage = "Describe is required.")]

        public string Description { get; set; }
        [Required(ErrorMessage = "Date is required.")]

        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public bool IsActive { get; set; }
        public virtual ICollection<Request> Requests { get; set; }

    }
}
