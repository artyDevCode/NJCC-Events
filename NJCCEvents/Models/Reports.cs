using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace NJCCEvents.Models
{
    public class Reports
    {
        public int Id { get; set; }

        [Display(Name = "Start Date")]
        public DateTime StartDateTime { get; set; }

        [Display(Name = "End Date")]
        public DateTime EndDateTime { get; set; }
       
        [Display(Name = "Event Name")]
        public string EventName { get; set; }
        
        [Display(Name = "Event Type")]
        public string EventType { get; set; }

        public string Organisation { get; set; }       
        public string Location { get; set; }
       
        [Display(Name = "Number of people")]
        public int NumOfPeople { get; set; }
       
        [Display(Name = "NJC Staff Contact")]
        public string StaffContact { get; set; }

        public string Aim { get; set; }

        [Display(Name = "Target Audience")]
        public string TargetAudience { get; set; }
      
        [Display(Name = "Organisation Contact")]
        public string OrganisationContact { get; set; }
            
    }
}