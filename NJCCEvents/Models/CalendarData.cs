using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NJCCEvents.Models
{
    public class CalendarData
    {
         //dr["id"] = 1;
         //   dr["start"] = Convert.ToDateTime("15:00");
         //   dr["end"] = Convert.ToDateTime("15:00");
         //   dr["text"] = "Event 1";
         //   dr["resource"] = "A";
        [Key]
        public int Id { get; set; } 
        public DateTime Start { get; set; }
        public DateTime End { get; set; }
        public string Resource { get; set; }
        public string Color { get; set; }
        public bool Allday { get; set; }
        public string Text { get; set; }
    }
}