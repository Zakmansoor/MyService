    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    namespace Domin.Entity
    {

        public class Mapping
        {
            public int Id { get; set; }
            public double Latitude { get; set; }
            public double Longitude { get; set; }
            public int RequestId { get; set; } // Foreign key to Request
            [ForeignKey("RequestId")]
            public virtual Request Request { get; set; }
        }

    }