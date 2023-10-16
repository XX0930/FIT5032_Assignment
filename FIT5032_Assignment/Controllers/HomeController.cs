using FIT5032_Assignment.Models;
using FIT5032_Assignment.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
namespace FIT5032_Assignment.Controllers
{
    [RequireHttps]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [Authorize]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize(Roles = "admin")]
        public ActionResult Send_Email()
        {
            return View(new SendEmailViewModel());
        }


        [HttpPost]
        public async Task<ActionResult> Send_Email(SendEmailViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    string toEmail = model.ToEmail;
                    string subject = model.Subject;
                    string contents = model.Contents;
                    Stream fileStream = null;
                    string fileName = "";

                    // Handle the attachment
                    if (model.Attachment != null && model.Attachment.ContentLength > 0)
                    {
                        fileStream = model.Attachment.InputStream;
                        fileName = Path.GetFileName(model.Attachment.FileName);
                    }

                    EmailSender es = new EmailSender();
                    var response = await es.Send(toEmail, subject, contents, fileStream, fileName);

                    if (response.StatusCode == System.Net.HttpStatusCode.Accepted || response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        ViewBag.Result = "Email has been sent.";
                        ModelState.Clear();
                        return View(new SendEmailViewModel());
                    }
                    else
                    {
                        ViewBag.Result = "Error sending email.";
                        return View(model);
                    }
                }
                catch
                {
                    ViewBag.Result = "Exception occurred.";
                    return View();
                }
            }
            return View();
        }
    }
}