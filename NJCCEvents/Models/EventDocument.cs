using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NJCCEvents.Models
{
    public class EventDocument
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name="Start Date")]
        public DateTime StartDateTime { get; set; }

        [Required]
        [Display(Name="End Date")]
        public DateTime EndDateTime { get; set; }
       
        [Required]
        [Display(Name="Event Name")]
        public string EventName { get; set; }

        [Required]
        [Display(Name="Event Type")]
        public string EventType { get; set; }

        public string Organisation { get; set; }

        [Required]
        public string Location { get; set; }

        [Required]
        [Range(1,10000)]
        [Display(Name="Number of people")]
        public int NumOfPeople { get; set; }

        [Required]
        [Display(Name="NJC Staff Contact")]
        public string StaffContact { get; set; }

        public string Aim { get; set; }

        [Display(Name="Target Audience")]
        public string TargetAudience { get; set; }

        [Required]
        [Display(Name = "Organisation Contact")]
        public string OrganisationContact { get; set; }

        public string Comments { get; set; }
        public string Feedback { get; set; }

        [AllowHtml]
        public string Attachments { get; set; }

        public bool DocumentDeleted { get; set; }

        [Display(Name = "Date Modified")]
        public DateTime DocumentDateModified { get; set; }

        [Display(Name = "Modified By")]
        public string DocumentModifiedBy { get; set; }


        [Display(Name = "Date Created")]
        public DateTime DocumentDateCreated { get; set; }

        [Display(Name = "Created By")]
        public string DocumentCreatedBy { get; set; }     
       // public virtual IEnumerable<AttachmentContent> Attachments { get; set; }
      //  public virtual IEnumerable<HttpPostedFileBase> Attachments { get; set; }
    }
}