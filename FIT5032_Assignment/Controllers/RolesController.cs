using FIT5032_Assignment.Models;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Net;

namespace FIT5032_Assignment.Controllers
{
    public class RolesController : Controller
    {
        private Entities db = new Entities();

        public ActionResult Index()
        {
            var users = db.AspNetUsers.Include("AspNetRoles").ToList();
            return View(users);
        }

        public ActionResult Assign()
        {
            var users = db.AspNetUsers.ToList();
            var roles = db.AspNetRoles.ToList();

            ViewBag.Users = new SelectList(users, "Id", "UserName");
            ViewBag.Roles = new SelectList(roles, "Id", "Name");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Assign(string UserName, string RoleName)
        {
            var user = db.AspNetUsers.Find(UserName);
            var role = db.AspNetRoles.Find(RoleName);
            if (user == null || role == null)
            {
                
                return View();
            }
            if (!user.AspNetRoles.Contains(role))
            {
                // Assign the role
                user.AspNetRoles.Add(role);
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }


        public ActionResult EditRole(string userId)
        {
            var user = db.AspNetUsers.Include("AspNetRoles").FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return HttpNotFound();
            }

            ViewBag.Roles = new SelectList(db.AspNetRoles, "Id", "Name");
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditRole(string userId, string roleId)
        {
            var user = db.AspNetUsers.Include("AspNetRoles").FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return HttpNotFound();
            }

            // Remove existing roles
            var currentRoles = user.AspNetRoles.ToList();
            foreach (var role in currentRoles)
            {
                user.AspNetRoles.Remove(role);
            }

            // Add the new role
            var newRole = db.AspNetRoles.Find(roleId);
            if (newRole != null)
            {
                user.AspNetRoles.Add(newRole);
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }

        //Delete all roles
        public ActionResult DeleteRole(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            // Fetch the user
            var user = db.AspNetUsers.Find(userId);

            if (user == null)
                return HttpNotFound();

            // Remove all roles for the user
            foreach (var role in user.AspNetRoles.ToList())
            {
                user.AspNetRoles.Remove(role);
            }

            db.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}