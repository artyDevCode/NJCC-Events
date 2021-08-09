using NJCCEvents.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace NJCCEvents.Controllers
{
    public class AccessController : Controller
    {
        private njccEventsDB db = new njccEventsDB();
        //
        // GET: /Access/
        [SharePointContextFilter]
        public ActionResult Index()
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

            if ((ViewData["InUsersRole"] != "true") && ViewData["InOwnerRole"] != "true")
            {
                return RedirectToAction("Unauthorised", "Event", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
            }

            var model = db.tblAccess
            .ToList();

            var modelsorted = model.OrderBy(r => r.AccessGroup).ThenBy(e => e.UserName).ToList();

            return View(modelsorted);
        }

        [SharePointContextFilter]
        // GET: /Access/Create
        public ActionResult Create()
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

            if ((ViewData["InUsersRole"] != "true") && ViewData["InOwnerRole"] != "true")
            {
                return RedirectToAction("Unauthorised", "Event", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
            }

            string[] AccessGroupsModel1 = ConfigurationManager.AppSettings.AllKeys
                                         .Where(key => key.Contains("Group"))
                                         .Select(key => ConfigurationManager.AppSettings[key])
                                         .ToArray();

            SelectList AGNames = new SelectList(AccessGroupsModel1);
            ViewData["AGNames"] = AGNames;

            Access model = new Access();

            return View(model);
        }

        // POST: /Access/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [SharePointContextFilter]
        public async Task<ActionResult> Create([Bind(Include = "Id,AccessGroup,UserId,UserName")] Access access)
        {

            if (ModelState.IsValid)
            {

                var accessCount = db.tblAccess
                    .Where(r => r.UserName == access.UserName && r.AccessGroup == access.AccessGroup)
                .ToList();

                int totalRowsCount = accessCount.Count();

                if (totalRowsCount == 0)
                { 
                    access.UserId = GetUserId(access.UserName);
                    db.tblAccess.Add(access);
                    await db.SaveChangesAsync();
                    return RedirectToAction("Index", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
                }
                else
                {
                    return RedirectToAction("Alert", "Event", new { alertMessage = "User access for " + access.UserName + " as " + access.AccessGroup + " is already defined." , SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
                }

            }

            string[] AccessGroupsModel = ConfigurationManager.AppSettings.AllKeys
                                         .Where(key => key.Contains("Group"))
                                         .Select(key => ConfigurationManager.AppSettings[key])
                                         .OrderBy(key => ConfigurationManager.AppSettings[key])
                                         .ToArray();
            SelectList AGNames = new SelectList(AccessGroupsModel);
            ViewData["AGNames"] = AGNames;

            return View(access);
        }
        [SharePointContextFilter]
        // GET: /Access/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Alert", "Event", new { alertMessage = "Access control not found, contact administrator." ,  SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
            }
            Access access = db.tblAccess.Find(id);
            if (access == null)
            {
                return RedirectToAction("Alert", "Event", new { alertMessage = "Access control not found, contact administrator." ,  SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
            }
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

            if ((ViewData["InUsersRole"] != "true") && ViewData["InOwnerRole"] != "true")
            {
                return RedirectToAction("Unauthorised", "Event", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
            }
            return View(access);
        }

        // POST: /ALRS/Delete/5
        [SharePointContextFilter]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Access access = db.tblAccess.Find(id);
            db.tblAccess.Remove(access);
            db.SaveChanges();
            return RedirectToAction("Index", new { SPHostUrl = SharePointContext.GetSPHostUrl(HttpContext.Request).AbsoluteUri });
        }

        public string GetUserId(string user)
        {
            string uid = "";

            PrincipalContext ctx = new PrincipalContext(ContextType.Domain, ConfigurationManager.AppSettings["ADDomain"], ConfigurationManager.AppSettings["ADConnectionUserName"], ConfigurationManager.AppSettings["ADConnectionPassword"]);
            UserPrincipal u = UserPrincipal.FindByIdentity(ctx, user);
            if (u != null)
            {
                uid = u.SamAccountName;
            }
            return uid;
        }

        public static DirectoryEntry GetDirectoryEntry()
        {
            DirectoryEntry de = new DirectoryEntry();
            de.Path = ConfigurationManager.AppSettings["ADConnection"]; 
            de.AuthenticationType = AuthenticationTypes.Secure;
            return de;
        }


        public JsonResult getUsers(string term)
        {

            DirectoryEntry de = GetDirectoryEntry();
            DirectorySearcher deSearch = new DirectorySearcher();
            List<string> groupMembers = new List<string>();

            de.Password = ConfigurationManager.AppSettings["ADConnectionPassword"];
            de.Username = ConfigurationManager.AppSettings["ADConnectionUserName"];

            deSearch.SearchRoot = de;
            deSearch.Filter = "(&(objectClass=user) (cn=" + "*" + "))";

            foreach (SearchResult sr in deSearch.FindAll())
            {
                foreach (string str in sr.Properties["name"])
                {

                    PrincipalContext ctx = new PrincipalContext(ContextType.Domain, ConfigurationManager.AppSettings["ADDomain"], ConfigurationManager.AppSettings["ADConnectionUserName"], ConfigurationManager.AppSettings["ADConnectionPassword"]);
                    UserPrincipal u = UserPrincipal.FindByIdentity(ctx, str);
                    if (u.EmailAddress != null)
                    { groupMembers.Add(str); }

                }
            }

            return Json(groupMembers, JsonRequestBehavior.AllowGet);
        }
    
    }
}