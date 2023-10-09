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
    public class BookingsController : Controller
    {
        private Entities db = new Entities();

        // GET: Bookings
        public ActionResult Index()
        {
            var currentUserId = User.Identity.GetUserId();

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
                var bookingSet = db.BookingSet.Where(b => b.AspNetUsersId == currentUserId).Include(b => b.AspNetUsers);
                return View(bookingSet.ToList());
            }
            else
            {
                // Handle other roles or unassigned roles here.
                // You can redirect them to a different view, or display an error message, etc.
                return RedirectToAction("Unauthorized", "Home"); // Assuming you have an Unauthorized action in your Home controller.
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
        public ActionResult Create([Bind(Include = "BookingId,Description,BookingDate,AspNetUsersId")] Booking booking)
        {
            var patientId = User.Identity.GetUserId();
            booking.AspNetUsersId = patientId;

            if (ModelState.IsValid)
            {
                db.BookingSet.Add(booking);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            var doctorUserIds = db.AspNetRoles.Where(r => r.Name == "doctor")
                                              .SelectMany(r => r.AspNetUsers)
                                              .Select(u => u.Id)
                                              .ToList();

            ViewBag.AspNetUsersIdDoctor = new SelectList(db.AspNetUsers.Where(u => doctorUserIds.Contains(u.Id)), "Id", "Email", booking.AspNetUsersId);
            return View(booking);
        }


        // GET: Bookings/Edit/5
        // GET: Bookings/Edit/5
        public ActionResult Edit(int? id)
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

            var currentUserId = User.Identity.GetUserId();

            if (User.IsInRole("admin") || booking.AspNetUsersId == currentUserId)
            {
                ViewBag.AspNetUsersId = new SelectList(db.AspNetUsers, "Id", "Email", booking.AspNetUsersId);
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
        public ActionResult Edit([Bind(Include = "BookingId,Description,BookingDate")] Booking booking)
        {
            var currentBooking = db.BookingSet.Find(booking.BookingId);

            if (currentBooking == null)
            {
                return HttpNotFound();
            }

            var currentUserId = User.Identity.GetUserId();

            if (!(User.IsInRole("admin") || currentBooking.AspNetUsersId == currentUserId))
            {
                return new HttpStatusCodeResult(HttpStatusCode.Forbidden);
            }

            if (ModelState.IsValid)
            {
                currentBooking.Description = booking.Description;
                currentBooking.BookingDate = booking.BookingDate;
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
