using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using DayPilot.Web.Mvc;
using DayPilot.Web.Mvc.Data;
using DayPilot.Web.Mvc.Enums;
using DayPilot.Web.Mvc.Events.Calendar;
using DayPilot.Web.Mvc.Events.Common;
using DayPilot.Web.Mvc.Events.Navigator;
using DayPilot.Web.Mvc.Json;
using BeforeCellRenderArgs = DayPilot.Web.Mvc.Events.Calendar.BeforeCellRenderArgs;
using TimeRangeSelectedArgs = DayPilot.Web.Mvc.Events.Calendar.TimeRangeSelectedArgs;
using NJCCEvents.Models;


using System.IO;
using System.Web.UI.WebControls;
using System.Web.UI;
using Microsoft.Reporting.WebForms;
using ReportViewerForMvc;

namespace NJCCEvents.Controllers
{
    public class HomeController : Controller
    {
        public static string sphostUrl;
        // njccEventsDB _db = new njccEventsDB();
        public static njccEventsDB _db;

        public HomeController()
        {
            _db = new njccEventsDB();

        }

        public HomeController(njccEventsDB db)
        {
            _db = db;
        }

        [SharePointContextFilter]
        public ActionResult Index()
        {
            sphostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri;
            // Check access levels and pass to view
            int index = User.Identity.Name.IndexOf("\\");
            string user = User.Identity.Name.Substring(index + 1);
            List<Access> AccessGroupsModel = _db.tblAccess
                             .Where(r => r.UserId == user)
                             .ToList();

            Access currentUser = (from u in _db.tblAccess
                                  where u.UserId == user
                                  select u).FirstOrDefault();

            if (currentUser != null)
                ViewData["UserName"] = currentUser.UserName;
            else
            { ViewData["UserName"] = User.Identity.Name; }
            ViewData["InOwnerRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("owner")).Count() > 0 ? "true" : "false";
            ViewData["InUsersRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("user")).Count() > 0 ? "true" : "false";

            if ((ViewData["InUsersRole"] != "true") && ViewData["InOwnerRole"] != "true")
            {
                return RedirectToAction("Unauthorised", "Event", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
            }

            return View();
        }
        [SharePointContextFilter]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }
        [SharePointContextFilter]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
        [SharePointContextFilter]
        public ActionResult ExportData()
        {
            /*  To export a model directly to excel
                       GridView model = new GridView();                    
                       model.DataSource = _db.tblEventDocument.ToList();
                       model.DataBind();

                       Response.ClearContent();
                       Response.Buffer = true;
                       Response.AddHeader("content-disposition", "attachment; filename=Export.xls");
                       Response.ContentType = "application/ms-excel";
                       Response.Charset = "";
                       StringWriter sw = new StringWriter();
                       HtmlTextWriter htw = new HtmlTextWriter(sw);
                       model.RenderControl(htw);
                       Response.Output.Write(sw.ToString());
                       Response.Flush();
                       Response.End();

                  */

            // Check access levels and pass to view
            int index = User.Identity.Name.IndexOf("\\");
            string user = User.Identity.Name.Substring(index + 1);
            List<Access> AccessGroupsModel = _db.tblAccess
                             .Where(r => r.UserId == user)
                             .ToList();

            Access currentUser = (from u in _db.tblAccess
                                  where u.UserId == user
                                  select u).FirstOrDefault();

            if (currentUser != null)
                ViewData["UserName"] = currentUser.UserName;
            else
            { ViewData["UserName"] = User.Identity.Name; }
            ViewData["InOwnerRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("owner")).Count() > 0 ? "true" : "false";
            ViewData["InUsersRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("user")).Count() > 0 ? "true" : "false";

            if ((ViewData["InUsersRole"] != "true") && ViewData["InOwnerRole"] != "true")
            {
                return RedirectToAction("Unauthorised", "Event", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
            }

            SetLocalReport();

            return View();
        }

        private void SetLocalReport()
        {
            ReportViewer reportViewer = new ReportViewer();
            reportViewer.ProcessingMode = ProcessingMode.Local;
            reportViewer.SizeToReportContent = true;
            reportViewer.Width = Unit.Percentage(100);
            reportViewer.Height = Unit.Percentage(100);

            var data = _db.tblEventDocument
                .ToList()
                .Where(r => r.DocumentDeleted == false)
                .Select(r => new Reports
                {
                    Id = r.Id,
                    StartDateTime = r.StartDateTime,
                    EndDateTime = r.EndDateTime,
                    EventName = r.EventName,
                    EventType = r.EventType,
                    Organisation = r.Organisation,
                    Location = r.Location,
                    NumOfPeople = r.NumOfPeople,
                    StaffContact = r.StaffContact,
                    Aim = r.Aim,
                    TargetAudience = r.TargetAudience,
                    OrganisationContact = r.OrganisationContact
                });
            reportViewer.LocalReport.ReportPath = Request.MapPath(Request.ApplicationPath) + @"\DataReports\DataReports.rdlc";
            reportViewer.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", data));
           //reportViewer.LocalReport.SetParameters(GetParametersLocal());

            ViewBag.ReportViewer = reportViewer;
        }

       
        //private ReportParameter[] GetParametersLocal()
        //{
        //    ReportParameter p1 = new ReportParameter("ReportTitle", "Local Report Example");
        //    return new ReportParameter[] { p1 };
        //}





        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _db.Dispose();
            }
            base.Dispose(disposing);
        }

        public ActionResult Backend()
        {
           // return RedirectToAction("Create","Event", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri});
            return new Dpc().CallBack(this);
        }

        public ActionResult NavigatorBackend()
        {
            return new Dpn().CallBack(this);
        }

     
        public class Dpn : DayPilotNavigator
        {
            protected override void OnVisibleRangeChanged(VisibleRangeChangedArgs visibleRangeChangedArgs)
            {

                DataStartField = "start";
                DataEndField = "end";
                DataIdField = "id";

            }
        }

      
        public class Dpc : DayPilotCalendar
        {

            // njccEventsDB _db = new njccEventsDB();
            //njccEventsDB _db;
            //public Dpc()
            //{

            //}

            // public Dpc(HomeController hc)
            // {
            //     _db = hc._db;
            // }

            //create a new event
            protected override void OnTimeRangeSelected(TimeRangeSelectedArgs e)
            {           
               // Redirect("~/Event/Create?Start=" + e.Start.ToString() + "&End=" + e.End.ToString());             
                Redirect("~/Event/Create?Start=" + e.Start.ToString() + "&End=" + e.End.ToString() + "&SPHostUrl=" + sphostUrl); //  http%3A%2F%2Fdev-shp2013%2F");
            }

            protected override void OnEventMove(DayPilot.Web.Mvc.Events.Calendar.EventMoveArgs e)
            {

                if (e.Id != null)
                {
                    var dr = _db.tblEventDocument.First(r => r.Id.ToString() == e.Id); //.Where(r => r.Id.ToString() == e.Id).Single();
                    if (dr != null)
                    {

                        dr.StartDateTime = e.NewStart;
                        dr.EndDateTime = e.NewEnd;
                        _db.Entry(dr).State = EntityState.Modified;
                        _db.SaveChanges();

                    }
                }

                UpdateWithMessage("Event moved.");

            }

            protected override void OnEventClick(EventClickArgs e)
            {
                UpdateWithMessage("Event clicked: " + e.Text);
                Redirect("~/Event/Details?Id=" + e.Id + "&SPHostUrl=" + sphostUrl); //http%3A%2F%2Fdev-shp2013%2F");
                //Redirect("http://www.daypilot.org/");
            }

            protected override void OnEventDelete(EventDeleteArgs e)
            {
                UpdateWithMessage("Event delete clicked: " + e.Text);
                Redirect("~/Event/Delete?Id=" + e.Id + "&SPHostUrl=" + sphostUrl); //http%3A%2F%2Fdev-shp2013%2F");

            }

            protected override void OnEventResize(DayPilot.Web.Mvc.Events.Calendar.EventResizeArgs e)
            {
                // new EventManager(Controller).EventMove(e.Id, e.NewStart, e.NewEnd);
                UpdateWithMessage("Event resize.");
                var dr = _db.tblEventDocument.First(r => r.Id.ToString() == e.Id);
                dr.StartDateTime = e.NewStart;
                dr.EndDateTime = e.NewEnd;

                _db.Entry(dr).State = EntityState.Modified;
                _db.SaveChanges();

            }


            protected override void OnEventMenuClick(EventMenuClickArgs e)
            {
                switch (e.Command)
                {
                    case "Delete":
                        UpdateWithMessage("Event deleted.");
                        Redirect("~/Event/Delete?Id=" + e.Id + "&SPHostUrl=" + sphostUrl); //http%3A%2F%2Fdev-shp2013%2F");

                        break;
                    case "Edit":
                        UpdateWithMessage("Event Opened.");
                        Redirect("~/Event/Edit?Id=" + e.Id + "&SPHostUrl=" + sphostUrl); //http%3A%2F%2Fdev-shp2013%2F");

                        break;
                }
            }

            protected override void OnCommand(CommandArgs e)
            {
                switch (e.Command)
                {
                    case "navigate": //Navigation date panel
                        StartDate = (DateTime)e.Data["start"];
                        Update(CallBackUpdateType.Full);
                        break;
                }
            }

            protected override void OnInit(InitArgs initArgs)
            {

                DataIdField = "id";
                DataTextField = "text";
                DataStartField = "eventstart";
                DataEndField = "eventend";

                Update(CallBackUpdateType.Full);  //Update();


                UpdateWithMessage("Welcome!", CallBackUpdateType.Full);

            }


            protected override void OnFinish()
            {
                // only load the data if an update was requested by an Update() call
                if (UpdateType == CallBackUpdateType.None)
                {
                    return;
                }

                Events = _db.tblEventDocument
                    .Where(r => r.DocumentDeleted == false)
                    .ToList()
                    .Select(r => new CalendarData
                    {
                        Id = r.Id,
                        Text = r.EventName,
                        Start = r.StartDateTime,
                        End = r.EndDateTime
                        //Resource = r.Resource,
                        //Allday = r.Allday,
                        //Color = r.Color

                    });


                DataStartField = "start";
                DataEndField = "end";
                DataTextField = "text";
                DataIdField = "id";
                DataResourceField = "resource";

                DataAllDayField = "allday";


            }

        }
    }
}