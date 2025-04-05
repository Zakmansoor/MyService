
        using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domin.Entity
    {
        public class historypaid
        {
            [Key]
            public int Id { get; set; }
            public double amount { get; set; }
            public DateTime DateTime { get; set; }
            public int RequestId { get; set; }
       public bool makepaid { get; set; }
            public string NameServece { get; set; }
            [ForeignKey("RequestId")]
            
            public Request Request { get; set; }
        }
    }

