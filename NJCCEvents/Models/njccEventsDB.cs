using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace NJCCEvents.Models
{
    public class njccEventsDB :DbContext
    {
        public njccEventsDB()
            : base("name=DefaultConnection")
        {

        }
        public DbSet<EventDocument> tblEventDocument { get; set; }
        public DbSet<Aim> tblAim { get; set; }
        public DbSet<EventTypes> tblEventTypes { get; set; }
        public DbSet<CalendarData> tblCalendarData { get; set; }
        public DbSet<Access> tblAccess { get; set; }
    }
}