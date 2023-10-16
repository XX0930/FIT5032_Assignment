using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FIT5032_Assignment.Models;
using Microsoft.AspNet.Identity;

namespace FIT5032_Assignment.Controllers
{
    public class BookingsController : Controller
    {
        private Entities db = new Entities();

        // GET: Bookings
        public ActionResult Index()
        {
            var currentUserId = User.Identity.GetUserId();
            bool isDoctor = User.IsInRole("doctor");
            ViewBag.IsDoctor = isDoctor;
            // If user is an admin
            if (User.IsInRole("admin"))
            {
                var bookingSet = db.BookingSet.Include(b => b.AspNetUsers);
                return View(bookingSet.ToList());
            }
            // If user is a patient
            else if (User.IsInRole("patient"))
            {
                var bookingSet = db.BookingSet.Where(b => b.AspNetUsersId == currentUserId).Include(b => b.AspNetUsers);
                return View(bookingSet.ToList());
            }
            // If user is a doctor
            else if (User.IsInRole("doctor"))
            {
                // get doctor name from user table
                var doctorName = db.AspNetUsers.Where(u => u.Id == currentUserId).Select(u => u.UserName).FirstOrDefault();

                // select the booking 
                var bookingSet = db.BookingSet.Where(b => b.DoctorName == doctorName).ToList();

                return View(bookingSet.ToList());
            }
            else
            {
                // Handle other roles or unassigned roles here.
                // You can redirect them to a different view, or display an error message, etc.
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Bookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.BookingSet.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        
        // GET: Bookings/Create
        public ActionResult Create()
        {
            if (User.IsInRole("doctor"))
            {    
                return RedirectToAction("Index");
            }
            var doctorUserIds = db.AspNetRoles.Where(r => r.Name == "doctor")
                                  .SelectMany(r => r.AspNetUsers)
                                  .Select(u => u.Id)
                                  .ToList();

            ViewBag.AspNetUsersIdDoctor = new SelectList(db.AspNetUsers.Where(u => doctorUserIds.Contains(u.Id)), "Id", "Email");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(string Description, DateTime BookingDate, string AspNetUsersIdDoctor)
        {

            var patientId = User.Identity.GetUserId();

            var doctorUserIds = db.AspNetRoles.Where(r => r.Name == "doctor")
                                  .SelectMany(r => r.AspNetUsers)
                                  .Select(u => u.Id)
                                  .ToList();

            var user = db.AspNetUsers.FirstOrDefault(u => u.Id == AspNetUsersIdDoctor);

            // 1. Check the doctor conflict
            var doctorConflict = db.BookingSet.Any(b => b.BookingDate == BookingDate && b.DoctorName == user.UserName);

            // 2. Check the patient conflict
            var patientConflict = db.BookingSet.Any(b => b.BookingDate == BookingDate && b.AspNetUsersId == patientId);

            // 3. occur conflict
            if (doctorConflict || patientConflict)
            {
                ModelState.AddModelError("", "The selected doctor or patient is already booked at the specified time.");
                ViewBag.AspNetUsersIdDoctor = new SelectList(db.AspNetUsers.Where(u => doctorUserIds.Contains(u.Id)), "Id", "Email");
                return View();
            }
            Booking booking = new Booking();
            //booking.AspNetUsersId = patientId;
            if (ModelState.IsValid)
            {
                booking.AspNetUsersId = patientId;
                booking.Description = Description;
                booking.BookingDate = BookingDate;
                booking.DoctorName = user.UserName;
                db.BookingSet.Add(booking);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AspNetUsersIdDoctor = new SelectList(db.AspNetUsers.Where(u => doctorUserIds.Contains(u.Id)), "Id", "Email", booking.AspNetUsersId);
            return View(booking);
        }


        // GET: Bookings/Edit/5
        // GET: Bookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (User.IsInRole("doctor"))
            {
                // doctor cannot edit booking
                return RedirectToAction("Index");
            }
            var doctorUserIds = db.AspNetRoles.Where(r => r.Name == "doctor")
                      .SelectMany(r => r.AspNetUsers)
                      .Select(u => u.Id)
                      .ToList();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Booking booking = db.BookingSet.Find(id);

            if (booking == null)
            {
                return HttpNotFound();
            }

            var currentUserId = User.Identity.GetUserId();

            if (User.IsInRole("admin") || booking.AspNetUsersId == currentUserId)
            {
                ViewBag.AspNetUsersId = new SelectList(db.AspNetUsers, "Id", "Email", booking.AspNetUsersId);
                ViewBag.AspNetUsersIdDoctor = new SelectList(db.AspNetUsers.Where(u => doctorUserIds.Contains(u.Id)), "Id", "Email");
                return View(booking);
            }
            else
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }
        }


        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "BookingId,Description,BookingDate,AspNetUsersIdDoctor")] Booking booking, string AspNetUsersIdDoctor)
        {

            var currentBooking = db.BookingSet.Find(booking.BookingId);

            if (currentBooking == null)
            {
                return HttpNotFound();
            }
            var user = db.AspNetUsers.FirstOrDefault(u => u.Id == AspNetUsersIdDoctor);
            var currentUserId = User.Identity.GetUserId();

            if (!(User.IsInRole("admin") || currentBooking.AspNetUsersId == currentUserId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (ModelState.IsValid)
            {
                currentBooking.Description = booking.Description;
                currentBooking.BookingDate = booking.BookingDate;
                currentBooking.DoctorName = user.UserName;
                db.Entry(currentBooking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AspNetUsersId = new SelectList(db.AspNetUsers, "Id", "Email", currentBooking.AspNetUsersId);
            return View(booking);
        }


        // GET: Bookings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (!User.IsInRole("admin") )
            {
                // only admin can delete booking
                return RedirectToAction("Index");
            }
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Booking booking = db.BookingSet.Find(id);
            if (booking == null)
            {
                return HttpNotFound();
            }
            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Booking booking = db.BookingSet.Find(id);
            db.BookingSet.Remove(booking);
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
