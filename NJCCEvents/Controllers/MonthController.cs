using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using DayPilot.Web.Mvc;
using DayPilot.Web.Mvc.Data;
using DayPilot.Web.Mvc.Enums;
using DayPilot.Web.Mvc.Events;
using DayPilot.Web.Mvc.Events.Common;
using DayPilot.Web.Mvc.Events.Month;
using NJCCEvents.Models;
using System.Data.Entity;


namespace NJCCEvents.Controllers
{
    public class MonthController : Controller
    {

        public static njccEventsDB _db;
        //
        // GET: /Month/
        [SharePointContextFilter]
        public ActionResult Index()
        {
            // Check access levels and pass to view
            //int index = User.Identity.Name.IndexOf("\\");
            //string user = User.Identity.Name.Substring(index + 1);
            //List<Access> AccessGroupsModel = _db.tblAccess
            //                 .Where(r => r.UserId == user)
            //                 .ToList();

            //var currentUser = (from u in db.tblAccess
            //                   where u.UserId == user
            //                   select u).FirstOrDefault();

            //ViewData["InOwnerRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("owner")).Count() > 0 ? "true" : "false";
            //ViewData["InUsersRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("user")).Count() > 0 ? "true" : "false";
            //ViewData["UserName"] = currentUser.UserName;

            //if ((ViewData["InUsersRole"] != "true") && ViewData["InOwnerRole"] != "true")
            //{
            //    return RedirectToAction("Unauthorised", "Event");
            //}

            return View();
        }

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        _db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}

        public ActionResult Backend()
        {
            return new Dpm().CallBack(this);
        }

        public class Dpm : DayPilotMonth
        {
            njccEventsDB _db = new njccEventsDB();
            //protected override void OnBeforeEventRender(BeforeEventRenderArgs e)
            //{
            //    if (Id == "dpm_areas")
            //    {
            //        e.Areas.Add(new Area().Width(17).Bottom(9).Right(2).Top(3).CssClass("event_action_delete").JavaScript("dpm_areas.commandCallBack('delete', {id:e.value() });"));
            //        e.Areas.Add(new Area().Width(17).Bottom(9).Right(19).Top(3).CssClass("event_action_menu").ContextMenu("menu"));
            //    }

            //    if (e.Recurrent)
            //    {
            //        e.InnerHtml += " (R)";
            //    }

            //}

            //protected override void OnBeforeCellRender(BeforeCellRenderArgs e)
            //{
            //    if (Id == "dpm_today" && e.Start == DateTime.Today)
            //    {
            //        e.HeaderBackColor = "#ff6666";
            //        e.BackgroundColor = "#ffaaaa";
            //    }

            //}

            //protected override void OnEventBubble(EventBubbleArgs e)
            //{
            //    e.BubbleHtml = "Showing event details for: " + e.Id;
            //}

            protected override void OnTimeRangeSelected(TimeRangeSelectedArgs e)
            {
                //new EventManager(Controller).EventCreate(e.Start, e.End, "Default name", "A");
                //Update();
                Redirect("Event/Create?Start=" + e.Start.ToString() + "&End=" + e.End.ToString() + "&SPHostUrl=" + HomeController.sphostUrl); //http%3A%2F%2Fdev-shp2013%2F");
            }

            protected override void OnEventMove(EventMoveArgs e)
            {
                //new EventManager(Controller).EventMove(e.Id, e.NewStart, e.NewEnd);
                //if (Id == "dpm_position")
                //{
                //    UpdateWithMessage("Moved to position: " + e.Position);
                //}
                //else
                //{
                //    UpdateWithMessage("Event moved.");
                //}
                if (e.Id != null)
                {
                    var dr = _db.tblEventDocument.First(r => r.Id.ToString() == e.Id + "&SPHostUrl=" + HomeController.sphostUrl); //http%3A%2F%2Fdev-shp2013%2F"); //.Where(r => r.Id.ToString() == e.Id).Single();
                    if (dr != null)
                    {
                        //dr["start"] = start;
                        //dr["end"] = end;
                        dr.StartDateTime = e.NewStart;
                        dr.EndDateTime = e.NewEnd;
                        _db.Entry(dr).State = EntityState.Modified;
                        _db.SaveChanges();

                        // dr.AcceptChanges();
                    }
                }
            }

          
            protected override void OnEventClick(EventClickArgs e)
            {
                UpdateWithMessage("Event clicked: " + e.Text);
                Redirect("Event/Details?Id=" + e.Id + "&SPHostUrl=" + HomeController.sphostUrl); //http%3A%2F%2Fdev-shp2013%2F");
            }

            protected override void OnEventResize(EventResizeArgs e)
            {
                //new EventManager(Controller).EventMove(e.Id, e.NewStart, e.NewEnd);
                //Update();
                UpdateWithMessage("Event resize.");
                var dr = _db.tblEventDocument.First(r => r.Id.ToString() == e.Id);
                dr.StartDateTime = e.NewStart;
                dr.EndDateTime = e.NewEnd;

                _db.Entry(dr).State = EntityState.Modified;
                _db.SaveChanges();
            }

            protected override void OnEventMenuClick(EventMenuClickArgs e)
            {
                //switch (e.Command)
                //{
                //    case "Delete":
                //        UpdateWithMessage("Event clicked: " + e.Text);
                //        new EventManager(Controller).EventDelete(e.Id);
                //        Update();
                //        break;
                //}
                switch (e.Command)
                {
                    case "Delete":
                        UpdateWithMessage("Event deleted.");
                        Redirect("Event/Delete?Id=" + e.Id + "&SPHostUrl=" + HomeController.sphostUrl); //http%3A%2F%2Fdev-shp2013%2F");
                        // new EventManager(Controller).EventDelete(e.Id);
                        // Update();
                        break;
                    case "Edit":
                        UpdateWithMessage("Event Opened.");
                        Redirect("Event/Edit?Id=" + e.Id + "&SPHostUrl=" + HomeController.sphostUrl); //http%3A%2F%2Fdev-shp2013%2F");
                        // new EventManager(Controller).EventDelete(e.Id);
                        // Update();
                        break;
                }
            }

            protected override void OnCommand(CommandArgs e)
            {
                switch (e.Command)
                {
                    case "navigate":
                        DateTime start = (DateTime)e.Data["start"];
                        StartDate = start;
                        Update(CallBackUpdateType.Full);
                        break;
                    //case "selected":
                    //    if (SelectedEvents.Count > 0)
                    //    {
                    //        EventInfo ei = SelectedEvents[0];
                    //        SelectedEvents.RemoveAt(0);
                    //        UpdateWithMessage("Event removed from selection: " + ei.Text);
                    //    }

                    //    break;
                    //case "weekend":
                    //    ShowWeekend = (string)e.Data["status"] == "yes";
                    //    Update(CallBackUpdateType.Full);
                    //    break;
                    //case "refresh":
                    //    UpdateWithMessage("Refreshed", CallBackUpdateType.Full);
                    //    break;
                    //case "previous":
                    //    StartDate = StartDate.AddMonths(-1);
                    //    Update(CallBackUpdateType.Full);
                    //    break;
                    //case "today":
                    //    StartDate = DateTime.Today;
                    //    Update(CallBackUpdateType.Full);
                    //    break;
                    //case "next":
                    //    StartDate = StartDate.AddMonths(1);
                    //    Update(CallBackUpdateType.Full);
                    //    break;
                    //case "delete":
                    //    string id = (string)e.Data["id"];
                    //    new EventManager(Controller).EventDelete(id);
                    //    Update(CallBackUpdateType.EventsOnly);
                    //    break;
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

            //protected override void OnBeforeHeaderRender(BeforeHeaderRenderArgs e)
            //{
            //}

            protected override void OnFinish()
            {
                // only load the data if an update was requested by an Update() call
                if (UpdateType == CallBackUpdateType.None)
                {
                    return;
                }

                // this select is a really bad example, no where clause
                //if (Id == "dpm_recurring")
                //{
                //    Events = new EventManager(Controller, "recurring").Data.AsEnumerable();
                //    DataRecurrenceField = "recurrence";
                //}
                //else
                //{
                //    Events = new EventManager(Controller).Data.AsEnumerable();
                //}
                Events = _db.tblEventDocument
                  .ToList()
                  .Where(r => r.DocumentDeleted == false)
                  .Select(r => new CalendarData
                  {
                      Id = r.Id,
                      Start = r.StartDateTime,
                      Text = r.EventName,
                      End = r.EndDateTime
                      //Resource = r.Resource,
                      //Allday = r.Allday,
                      //Color = r.Color
                    //  Text = r.Text
                  });

                //no need to override the default field names
                DataStartField = "start";
                DataEndField = "end";
                DataTextField = "text";
                DataIdField = "id";
            }

        }
	}
}