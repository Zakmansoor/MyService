using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domin.Entity
{
    public  class Notification
    {
        [Key]
        public int NotificationId { get; set; }
        public string UserId { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public DateTime CreatedAt { get; set; } 

        public int requestId { get; set; }

        [ForeignKey("requestId")]

        public Request Request { get; set; }
        
    }
}
