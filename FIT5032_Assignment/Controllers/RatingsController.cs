using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FIT5032_Assignment.Models;
using Microsoft.AspNet.Identity;

namespace FIT5032_Assignment.Controllers
{   
    //[Authorize(Roles ="doctor")]
    public class RatingsController : Controller
    {
        private Entities db = new Entities();

        // GET: Ratings
        public ActionResult Index()
        {
            var ratingSet = db.RatingSet.Include(r => r.AspNetUsersDoctor).Include(r => r.AspNetUsersPatient);
            return View(ratingSet.ToList());
        }

        // GET: Ratings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rating rating = db.RatingSet.Find(id);
            if (rating == null)
            {
                return HttpNotFound();
            }
            return View(rating);
        }

        // GET: Ratings/Create
        public ActionResult Create()
        {
            //ViewBag.AspNetUsersIdDoctor = new SelectList(db.AspNetUsers, "Id", "Email");
            //ViewBag.AspNetUsersIdPatient = new SelectList(db.AspNetUsers, "Id", "Email");
            // 获取所有医生用户的ID列表
            //var doctorUserIds = db.AspNetRoles.Where(r => r.Name == "doctor")
            //                                 .SelectMany(r => r.AspNetUsers)
            //                                 .Select(u => u.Id)
            //                                 .ToList();
            //// 使用上面的ID列表从AspNetUsers表中筛选医生
            //ViewBag.AspNetUsersIdDoctor = new SelectList(db.AspNetUsers.Where(u => doctorUserIds.Contains(u.Id)), "Id", "Email");

            //ViewBag.AspNetUsersIdDoctor = new SelectList(doctorUsers, "Id", "Email");
            var doctorRole = db.AspNetRoles.FirstOrDefault(r => r.Name == "doctor");
            if (doctorRole != null)
            {
                var doctorUsers = doctorRole.AspNetUsers.ToList();
                ViewBag.AspNetUsersIdDoctor = new SelectList(doctorUsers, "Id", "Email");
            }

            return View();
        }

        // POST: Ratings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "RatingId,Score,Comment,AspNetUsersIdDoctor,AspNetUsersIdPatient")] Rating rating)
        {
            var patientId = User.Identity.GetUserId(); // Assuming you are using ASP.NET Identity to get current user ID
            rating.AspNetUsersIdPatient = patientId;
            if (ModelState.IsValid)
            {
                db.RatingSet.Add(rating);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            //ViewBag.AspNetUsersIdDoctor = new SelectList(db.AspNetUsers, "Id", "Email", rating.AspNetUsersIdDoctor);
            //ViewBag.AspNetUsersIdPatient = new SelectList(db.AspNetUsers, "Id", "Email", rating.AspNetUsersIdPatient);
            var doctorRole = db.AspNetRoles.FirstOrDefault(r => r.Name == "doctor");
            if (doctorRole != null)
            {
                var doctorUsers = doctorRole.AspNetUsers.ToList();
                ViewBag.AspNetUsersIdDoctor = new SelectList(doctorUsers, "Id", "Email");
            }

            return View(rating);
        }

        // GET: Ratings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rating rating = db.RatingSet.Find(id);
            if (rating == null)
            {
                return HttpNotFound();
            }
            ViewBag.AspNetUsersIdDoctor = new SelectList(db.AspNetUsers, "Id", "Email", rating.AspNetUsersIdDoctor);
            ViewBag.AspNetUsersIdPatient = new SelectList(db.AspNetUsers, "Id", "Email", rating.AspNetUsersIdPatient);
            return View(rating);
        }

        // POST: Ratings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RatingId,Score,Comment,AspNetUsersIdDoctor,AspNetUsersIdPatient")] Rating rating)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rating).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AspNetUsersIdDoctor = new SelectList(db.AspNetUsers, "Id", "Email", rating.AspNetUsersIdDoctor);
            ViewBag.AspNetUsersIdPatient = new SelectList(db.AspNetUsers, "Id", "Email", rating.AspNetUsersIdPatient);
            return View(rating);
        }

        // GET: Ratings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rating rating = db.RatingSet.Find(id);
            if (rating == null)
            {
                return HttpNotFound();
            }
            return View(rating);
        }

        // POST: Ratings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Rating rating = db.RatingSet.Find(id);
            db.RatingSet.Remove(rating);
            db.SaveChanges();
            return RedirectToAction("Index");
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
