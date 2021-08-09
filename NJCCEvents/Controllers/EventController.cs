using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NJCCEvents.Models;
using System.DirectoryServices;
using System.IO;


namespace NJCCEvents.Controllers
{
    public class EventController : Controller
    {
        private njccEventsDB db = new njccEventsDB();

        [SharePointContextFilter]
        // GET: /Event/
        public async Task<ActionResult> Index()
        {
            // Check access levels and pass to view
            int index = User.Identity.Name.IndexOf("\\");
            string user = User.Identity.Name.Substring(index + 1);
            List<Access> AccessGroupsModel = db.tblAccess
                             .Where(r => r.UserId == user)
                             .ToList();

            Access currentUser = (from u in db.tblAccess
                                  where u.UserId == user
                                  select u).FirstOrDefault();

            if (currentUser != null)
                ViewData["UserName"] = currentUser.UserName;
            else
            { ViewData["UserName"] = User.Identity.Name; }
            ViewData["InOwnerRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("owner")).Count() > 0 ? "true" : "false";
            ViewData["InUsersRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("user")).Count() > 0 ? "true" : "false";

            if (ViewData["InUsersRole"] != "true" && ViewData["InOwnerRole"] != "true")
            {
                return RedirectToAction("Unauthorised", "Event", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
            }
            
            return View(await db.tblEventDocument.ToListAsync());
          
        }


        [SharePointContextFilter]
        public ActionResult Unauthorised(string alertMessage)
        {
            // Check access levels and pass to view
            int index = User.Identity.Name.IndexOf("\\");
            string user = User.Identity.Name.Substring(index + 1);

            Access currentUser = (from u in db.tblAccess
                                  where u.UserId == user
                                  select u).FirstOrDefault();

            if (currentUser != null)
                ViewData["UserName"] = currentUser.UserName;
            else
            { ViewData["UserName"] = User.Identity.Name; }

            TempData["alertMessage"] = alertMessage;
            return View("Unauthorised");
        }

        [SharePointContextFilter]
        public ActionResult Alert(string alertMessage)
        {
            // Check access levels and pass to view
            int index = User.Identity.Name.IndexOf("\\");
            string user = User.Identity.Name.Substring(index + 1);
            List<Access> AccessGroupsModel = db.tblAccess
                             .Where(r => r.UserId == user)
                             .ToList();

            Access currentUser = (from u in db.tblAccess
                                  where u.UserId == user
                                  select u).FirstOrDefault();

            if (currentUser != null)
                ViewData["UserName"] = currentUser.UserName;
            else
            { ViewData["UserName"] = User.Identity.Name; }
            ViewData["InOwnerRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("owner")).Count() > 0 ? "true" : "false";
            ViewData["InUsersRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("user")).Count() > 0 ? "true" : "false";
            TempData["alertMessage"] = alertMessage;
            return View();
        }
        [SharePointContextFilter]
        // GET: /Event/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventDocument eventdocument = await db.tblEventDocument.FindAsync(id);
            if (eventdocument == null)
            {
                return HttpNotFound();
            }
            return View(eventdocument);
        }

        private List<string> getLocation()
        {
            var model = db.tblEventDocument          
           .GroupBy(a => a.Location)
           .Select(r => r.FirstOrDefault().Location)
           .ToList();

            return model;
        }

        private List<string> getEventTypes()
        {
            List<string> model = db.tblEventTypes
                .Select(r => r.Event).ToList();
            return model;
        }

        private List<string> getAim()
        {
            List<string> model = db.tblAim
                .Select(r => r.AimType).ToList();
            return model;
        }

      
        [SharePointContextFilter]
        // GET: /Event/Create
        public ActionResult Create(string Start, string End) //(DateTime Start, DateTime End)
        {
            //pass the date/time
            EventDocument model = new EventDocument();
            model.StartDateTime = Convert.ToDateTime(Start);
            model.EndDateTime = Convert.ToDateTime(End);
            var EModel = getEventTypes();
            SelectList EMNames = new SelectList(EModel);
            ViewData["ENames"] = EMNames;
            var AModel = getAim();
            SelectList AMNames = new SelectList(AModel);
            ViewData["ANames"] = AMNames;
            var LModel = getLocation();
            SelectList LMNames = new SelectList(LModel);
            ViewData["LNames"] = LMNames;

            
  
            return View(model);
        }

       [SharePointContextFilter]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "Id,Text,StartDateTime,EndDateTime,EventName,EventType,Organisation,Location,NumOfPeople,StaffContact,Aim,TargetAudience, OrganisationContact,Comments,Feedback,Attachments, DocumentDateCreated,DocumentDateModified, DocumentModifiedBy, DocumentCreatedBy")] EventDocument eventdocument)  //Resource,Color,Allday,
        {
            if (eventdocument.StartDateTime <= eventdocument.EndDateTime) //make sure that the start date is less than the end date
                if (ModelState.IsValid)
                {             
                    eventdocument.DocumentDateCreated = DateTime.Now;
                    eventdocument.DocumentDateModified = DateTime.Now;
                    eventdocument.DocumentCreatedBy = User.Identity.Name;
                    eventdocument.DocumentModifiedBy = User.Identity.Name;
                    eventdocument.DocumentDeleted = false;

                    if (eventdocument.EventType == "NJCC")  //set the organisation to N/A if event type NJCC
                        eventdocument.Organisation = "N/A";

                    db.tblEventDocument.Add(eventdocument);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", "Home", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
                }
            var EModel = getEventTypes();
            SelectList EMNames = new SelectList(EModel);
            ViewData["ENames"] = EMNames;
            var AModel = getAim();
            SelectList AMNames = new SelectList(AModel);
            ViewData["ANames"] = AMNames;
            var LModel = getLocation();
            SelectList LMNames = new SelectList(LModel);
            ViewData["LNames"] = LMNames;
            return View(eventdocument);
        }



        //public static DirectoryEntry GetDirectoryEntry()
        //{
        //    DirectoryEntry de = new DirectoryEntry();
        //    de.Path = "LDAP://dev.justice.vic.gov.au/CN=Users,DC=dev,DC=justice,DC=vic,DC=gov,DC=au";  //"LDAP://OU=Domain,DC=YourDomain,DC=com";
        //    de.AuthenticationType = AuthenticationTypes.Secure;

        //    return de;
        //}


        //public List<string> UserExists()
        //{
        //   // string username = "Arturo Plottier";
        //    DirectoryEntry de = AccessController.GetDirectoryEntry();
        //    DirectorySearcher deSearch = new DirectorySearcher();
        //    List<string> groupMembers = new List<string>();

        //    de.Password = "Password1";
        //    de.Username = "adm-art";

        //    deSearch.SearchRoot = de;
        //    deSearch.Filter = "(&(objectClass=user) (cn=" + "*" + "))";

        //   // SearchResultCollection results = deSearch.FindAll();

        //    foreach (SearchResult sr in deSearch.FindAll())
        //    {
        //        foreach (string str in sr.Properties["name"])
        //        {
                    
        //           // string str2 = str.Substring(str.IndexOf("=") + 1, str.IndexOf(",") - str.IndexOf("=") - 1);
        //            groupMembers.Add(str);
        //            //Response.Write(str2 + " 34.");
        //        }
        //    }
        //    return groupMembers; // results;
        //} 

    
      //  [Authorize(Roles="Admin, NJCC")]
        // GET: /Event/Edit/5
        [SharePointContextFilter]
        public async Task<ActionResult> Edit(int? id)
        {
            // Check access levels and pass to view
            int index = User.Identity.Name.IndexOf("\\");
            string user = User.Identity.Name.Substring(index + 1);
            List<Access> AccessGroupsModel = db.tblAccess
                             .Where(r => r.UserId == user)
                             .ToList();

            Access currentUser = (from u in db.tblAccess
                                  where u.UserId == user
                                  select u).FirstOrDefault();

            if (currentUser != null)
                ViewData["UserName"] = currentUser.UserName;
            else
            { ViewData["UserName"] = User.Identity.Name; }
            ViewData["InOwnerRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("owner")).Count() > 0 ? "true" : "false";
            ViewData["InUsersRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("user")).Count() > 0 ? "true" : "false";

            if (ViewData["InUsersRole"] != "true" && ViewData["InOwnerRole"] != "true")
            {
                return RedirectToAction("Unauthorised", "Event", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
            }
            

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventDocument eventdocument = await db.tblEventDocument.FindAsync(id);
            if (eventdocument == null)
            {
                return HttpNotFound();
            }



            //AccessController ac = new AccessController();
            //var ADModel = ac.getUsers(null);
            //SelectList ADNames = new SelectList(ADModel);
            //ViewData["ADNames"] = ADNames;

            var EModel = getEventTypes();
            SelectList EMNames = new SelectList(EModel);
            ViewData["ENames"] = EMNames;
            var AModel = getAim();
            SelectList AMNames = new SelectList(AModel);
            ViewData["ANames"] = AMNames;
            var LModel = getLocation();
            SelectList LMNames = new SelectList(LModel);
            ViewData["LNames"] = LMNames;

            return View(eventdocument);
        }


        // POST: /Event/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SharePointContextFilter]
        public async Task<ActionResult> Edit([Bind(Include = "Id,Text,StartDateTime,EndDateTime,EventName,EventType, Organisation,Location,NumOfPeople,StaffContact,Aim,TargetAudience, OrganisationContact,Comments,Feedback,Attachments,         DocumentDateCreated,DocumentDateModified, DocumentModifiedBy, DocumentCreatedBy")] EventDocument  eventdocument) //,Resource,Color,Allday
        {

            if (eventdocument.StartDateTime <= eventdocument.EndDateTime) //make sure that the start date is less than the end date
                if (ModelState.IsValid)
                {

                    //if (fileName.Count() > 0)
                    //{
                    //    var fileName = eventdocument.Attachments.Select(r => r.FileName).ToList(); // Path.GetFileName(eventdocument.Attachments)
                    //}
                    eventdocument.DocumentDateModified = DateTime.Now;
                    eventdocument.DocumentModifiedBy = User.Identity.Name;
                    db.Entry(eventdocument).State = EntityState.Modified;
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", "Home", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
                }

            var EModel = getEventTypes();
            SelectList EMNames = new SelectList(EModel);
            ViewData["ENames"] = EMNames;
            var AModel = getAim();
            SelectList AMNames = new SelectList(AModel);
            ViewData["ANames"] = AMNames;
            var LModel = getLocation();
            SelectList LMNames = new SelectList(LModel);
            ViewData["LNames"] = LMNames;
            return View(eventdocument);

        }

        // GET: /Event/Delete/5
        [SharePointContextFilter]
        public async Task<ActionResult> Delete(int? id)
        {
            // Check access levels and pass to view
            int index = User.Identity.Name.IndexOf("\\");
            string user = User.Identity.Name.Substring(index + 1);
            List<Access> AccessGroupsModel = db.tblAccess
                             .Where(r => r.UserId == user)
                             .ToList();

            Access currentUser = (from u in db.tblAccess
                                  where u.UserId == user
                                  select u).FirstOrDefault();

            if (currentUser != null)
                ViewData["UserName"] = currentUser.UserName;
            else
            { ViewData["UserName"] = User.Identity.Name; }
            ViewData["InOwnerRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("owner")).Count() > 0 ? "true" : "false";
            ViewData["InUsersRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("user")).Count() > 0 ? "true" : "false";
            
             if (ViewData["InOwnerRole"] != "true")
            {
                return RedirectToAction("Alert", "Event", new { alertMessage = "Unable to delete event. Please contact system owner." ,  SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
            }

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EventDocument eventdocument = await db.tblEventDocument.FindAsync(id);
            
            if (eventdocument == null)
            {
                return HttpNotFound();
            }
            return View(eventdocument);
        }

        // POST: /Event/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [SharePointContextFilter]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            // Check access levels and pass to view
            int index = User.Identity.Name.IndexOf("\\");
            string user = User.Identity.Name.Substring(index + 1);
            List<Access> AccessGroupsModel = db.tblAccess
                             .Where(r => r.UserId == user)
                             .ToList();

            Access currentUser = (from u in db.tblAccess
                                  where u.UserId == user
                                  select u).FirstOrDefault();

            if (currentUser != null)
                ViewData["UserName"] = currentUser.UserName;
            else
            { ViewData["UserName"] = User.Identity.Name; }
            ViewData["InOwnerRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("owner")).Count() > 0 ? "true" : "false";
            ViewData["InUsersRole"] = AccessGroupsModel.Where(r => r.AccessGroup.ToLower().Contains("user")).Count() > 0 ? "true" : "false";

            if (ViewData["InOwnerRole"] != "true")
            {
                return RedirectToAction("Alert", "Event", new { alertMessage = "Unable to delete event. Please contact system owner." , SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
            }

            EventDocument eventdocument = await db.tblEventDocument.FindAsync(id);
            eventdocument.DocumentDeleted = true;
            eventdocument.DocumentDateModified = DateTime.Now;
            eventdocument.DocumentModifiedBy = User.Identity.Name;
            await db.SaveChangesAsync();
            return RedirectToAction("Index", "Home", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
