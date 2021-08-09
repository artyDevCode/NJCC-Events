using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NJCCEvents.Models
{
    public class EventTypes
    {
        [Key]
        public int Id { get; set; }
        public string Event { get; set; }
    }
}