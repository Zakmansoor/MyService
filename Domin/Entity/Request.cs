using Domin.Entity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domin.Entity
{
    public class Request
    {
        [Key]
        public int RequestId { get; set; }

        public string UserId { get; set; }

        public int ServiceId { get; set; }

        public DateTime OrderDate { get; set; }

        public bool Status { get; set; }

        // Comment is optional, so it is left without [Required]
        public string Comment { get; set; }
       
        public Paid Paid { get; set; }
        [ForeignKey("ServiceId")]
        public virtual Service Service { get; set; }
        public List<Notification>  Notification { get; set; }
    }
}
