namespace NJCCEvents.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using NJCCEvents.Models;

    internal sealed class Configuration : DbMigrationsConfiguration<NJCCEvents.Models.njccEventsDB>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(NJCCEvents.Models.njccEventsDB context)
        {
            context.tblAim.AddOrUpdate(a => a.AimType,
           new Aim
           {
               AimType = "Community engagement"
           },
            new Aim
            {
                AimType = "Cultural awareness"
            },
            new Aim
            {
                AimType = "Education"
            },
            new Aim
            {
                AimType = "Networking"
            },
            new Aim
            {
                AimType = "Professional development"
            },
            new Aim
            {
                AimType = "Promotion"
            },
            new Aim
            {
                AimType = "Staff communication"
            });

             context.tblEventTypes.AddOrUpdate(a => a.Event,
           new EventTypes
           {
               Event = "NJCC"
           },
            new EventTypes
            {
                Event = "Community"
            });

             context.tblEventDocument.AddOrUpdate(a => a.EventName,
              new EventDocument
              {
                   // Text = "Event 1",
                    StartDateTime = Convert.ToDateTime("15:00"),
                    EndDateTime = Convert.ToDateTime("15:10"),
                    //Resource = "A",
                 
                    EventName = "Test Event Name1",
                    EventType = "NJCC",
                    Organisation = "",
                    Location = "Community Meeting Room",
                    NumOfPeople = 10,
                    StaffContact = "Arturo Plottier",
                    Aim = "Education",
                    TargetAudience = "Law students",
                    OrganisationContact = "test OrgContact1",
                    DocumentCreatedBy = "Ricardo Marshall",
                    DocumentDateCreated = DateTime.Now,
                    DocumentModifiedBy = "Ricardo Marshall",
                    DocumentDateModified = DateTime.Now,
                    DocumentDeleted = false,
                    Comments = "Test comments test",
                    Feedback = "Feedback test"
                   // Attachments = ""                
              },
                new EventDocument
                {
                   // Text = "Event 2",
                    StartDateTime = Convert.ToDateTime("11:00"),
                    EndDateTime = Convert.ToDateTime("13:30"),
                    //Resource = "B",

                    EventName = "Test Event Name2",
                    EventType = "Community",
                    Organisation = "organization test 2",
                    Location = "Collingwood estate",
                    NumOfPeople = 8,
                    StaffContact = "Arturo Plottier",
                    Aim = "Promotion",
                    TargetAudience = "anyone test",
                    OrganisationContact = "test OrgContact2",
                    DocumentCreatedBy = "Arturo Plottier",
                    DocumentDateCreated = DateTime.Now,
                    DocumentModifiedBy = "Arturo Plottier",
                    DocumentDateModified = DateTime.Now,
                    DocumentDeleted = false,
                    Comments = "Test comments test2",
                    Feedback = "Feedback test2"
                   // Attachments = ""
                },
                  new EventDocument
                  {
                     // Text = "Event 3",
                      StartDateTime = Convert.ToDateTime("09:00"),
                      EndDateTime = Convert.ToDateTime("10:00"),
                      //Resource = "c",

                      EventName = "Test Event Name3",
                      EventType = "NJCC",
                      Organisation = "",
                      Location = "Community Meeting Room",
                      NumOfPeople = 3,
                      StaffContact = "Arturo Plottier",
                      Aim = "Networking",
                      TargetAudience = "Law students test3",
                      OrganisationContact = "test OrgContact3",
                      DocumentCreatedBy = "Arturo Plottier",
                      DocumentDateCreated = DateTime.Now,
                      DocumentModifiedBy = "Arturo Plottier",
                      DocumentDateModified = DateTime.Now,
                      DocumentDeleted = false,
                      Comments = "Test comments test3",
                      Feedback = "Feedback test3"
                      //Attachments = ""
                  });
        }
    }
}
