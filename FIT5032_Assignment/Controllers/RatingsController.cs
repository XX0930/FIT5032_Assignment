﻿using System;
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
    [Authorize]
    public class RatingsController : Controller
    {
        private Entities db = new Entities();

        // GET: Ratings
        public ActionResult Index()
        {
            if (!User.IsInRole("admin")&& !User.IsInRole("doctor")&& !User.IsInRole("patient"))
            {
                return RedirectToAction("Index", "Home");
            }
            var ratingSet = db.RatingSet.Include(r => r.AspNetUsersDoctor).Include(r => r.AspNetUsersPatient);
            var userId = User.Identity.GetUserId();
            var doctorAverageRatings = db.RatingSet
                        .GroupBy(r => r.AspNetUsersIdDoctor)
                        .Join(db.AspNetUsers,
                        rating => rating.Key,
                        user => user.Id,
                        (rating, user) => new
                        {
                            DoctorId = user.Id,
                            DocName = user.UserName,
                            AverageRating = rating.Average(r => r.Score)
                        })
                .ToList();
            ViewBag.doctorNameAverageRatings = doctorAverageRatings;

            if (User.IsInRole("doctor"))
            {
                var doctorId = User.Identity.GetUserId();
                var doctorAverageRating = doctorAverageRatings.FirstOrDefault(d => d.DoctorId == doctorId);
                if (doctorAverageRating != null)
                {
                    ViewBag.doctorAverageRating = doctorAverageRating.AverageRating;
                }
                else
                {
                    ViewBag.doctorAverageRating = null;
                }

                // if user is doctor, display what rating him
                ratingSet = ratingSet.Where(r => r.AspNetUsersIdDoctor == userId);
            }
            if (User.IsInRole("patient"))
            {
                // if user is patient display what his rating.
                ratingSet = ratingSet.Where(r => r.AspNetUsersIdPatient == userId);
            }
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
            if (User.IsInRole("doctor"))
            {
                // doctor cannot create rating
                return RedirectToAction("Index");
            }

            var doctorRole = db.AspNetRoles.FirstOrDefault(r => r.Name == "doctor");
            var patientRole = db.AspNetRoles.FirstOrDefault(r => r.Name == "patient");

            var doctorsUsers = doctorRole.AspNetUsers.ToList();
            var patientUsers = patientRole.AspNetUsers.ToList();


            //return View();
            var patientId = User.Identity.GetUserId();

            // get the patient's doctor name
            var bookedDoctorNames = db.BookingSet.Where(b => b.AspNetUsersId == patientId).Select(b => b.DoctorName).ToList();

            // get doctor id from user table
            var bookedDoctorIds = db.AspNetUsers.Where(u => bookedDoctorNames.Contains(u.UserName) && u.AspNetRoles.Any(r => r.Name == "doctor")).Select(u => u.Id).ToList();

            // has been rate doctor
            var ratedDoctors = db.RatingSet.Where(r => r.AspNetUsersIdPatient == patientId).Select(r => r.AspNetUsersIdDoctor).ToList();

            // select has been booked and bot rating doctor
            var doctorsToRate = bookedDoctorIds.Except(ratedDoctors).ToList();

            var doctorUsers = db.AspNetUsers.Where(u => doctorsToRate.Contains(u.Id)).ToList();
            if (User.IsInRole("patient"))
            {
                ViewBag.AspNetUsersIdDoctor = new SelectList(doctorUsers, "Id", "Email");
            }
            else if (User.IsInRole("admin"))
            {
                ViewBag.AspNetUsersIdDoctor = new SelectList(doctorsUsers, "Id", "Email");
                ViewBag.AspNetUsersIdPatient = new SelectList(patientUsers, "Id", "Email");
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
            var patientId = User.Identity.GetUserId();
            if (!User.IsInRole("admin"))
            {
                rating.AspNetUsersIdPatient = patientId;
            }

            if (ModelState.IsValid)
            {
                db.RatingSet.Add(rating);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

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

            var doctorUserIds = db.AspNetRoles.Where(r => r.Name == "doctor")
                                  .SelectMany(r => r.AspNetUsers)
                                  .Select(u => u.Id)
                                  .ToList();

            var patientUserIds = db.AspNetRoles.Where(r => r.Name == "patient")
                      .SelectMany(r => r.AspNetUsers)
                      .Select(u => u.Id)
                      .ToList();

            var patientId = User.Identity.GetUserId();

            // get the patient's doctor name
            var bookedDoctorNames = db.BookingSet.Where(b => b.AspNetUsersId == patientId).Select(b => b.DoctorName).ToList();

            // get doctor id from user table
            var bookedDoctorIds = db.AspNetUsers.Where(u => bookedDoctorNames.Contains(u.UserName) && u.AspNetRoles.Any(r => r.Name == "doctor")).Select(u => u.Id).ToList();

            // has been rate doctor
            var ratedDoctors = db.RatingSet.Where(r => r.AspNetUsersIdPatient == patientId).Select(r => r.AspNetUsersIdDoctor).ToList();

            // select has been booked and bot rating doctor
            var doctorsToRate = bookedDoctorIds.Except(ratedDoctors).ToList();

            var doctorUsers = db.AspNetUsers.Where(u => doctorsToRate.Contains(u.Id)).ToList();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Rating rating = db.RatingSet.Find(id);
            if (rating == null)
            {
                return HttpNotFound();
            }
            if (User.IsInRole("admin"))
            {
                ViewBag.AspNetUsersIdDoctor = new SelectList(db.AspNetUsers.Where(u => doctorUserIds.Contains(u.Id)), "Id", "Email");
                ViewBag.AspNetUsersIdPatient = new SelectList(db.AspNetUsers.Where(u => patientUserIds.Contains(u.Id)), "Id", "Email");
            }
            if (User.IsInRole("patient"))
            {
                ViewBag.AspNetUsersIdDoctor = new SelectList(doctorUsers, "Id", "Email");
                ViewBag.AspNetUsersIdPatient = new SelectList(db.AspNetUsers.Where(u => patientUserIds.Contains(u.Id)), "Id", "Email");
            }

            return View(rating);
        }

        // POST: Ratings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "RatingId,Score,Comment,AspNetUsersIdDoctor,AspNetUsersIdPatient")] Rating rating)
        {
            var userid = User.Identity.GetUserId();
            if (!User.IsInRole("admin"))
            {
                rating.AspNetUsersIdPatient = userid;
            }
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
            if (User.IsInRole("doctor"))
            {
                // doctor cannot delete rating
                return RedirectToAction("Index");
            }
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
