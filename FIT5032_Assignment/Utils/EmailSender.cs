﻿using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace FIT5032_Assignment.Utils
{
    public class EmailSender
    {
        private const String API_KEY = "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";
        public async Task<SendGrid.Response> Send(String toEmail, String subject, String contents, Stream attachmentStream = null, string attachmentFilename = "")
        {
            var client = new SendGridClient(API_KEY);
            var from = new EmailAddress("az694341869@gmail.com", "FIT5032 Email");
            var to = new EmailAddress(toEmail, "");
            var plainTextContent = contents;
            var htmlContent = "<p>" + contents + "</p>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);

            if (attachmentStream != null && attachmentFilename != "")
            {
                var attachment = new Attachment()
                {
                    Content = Convert.ToBase64String(ReadFully(attachmentStream)),
                    Filename = attachmentFilename,
                    Type = "application/octet-stream",
                    Disposition = "attachment"
                };
                msg.Attachments = new List<Attachment> { attachment };
            }

            var response = await client.SendEmailAsync(msg);
            return response;
        }

        public static byte[] ReadFully(Stream input)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                input.CopyTo(ms);
                return ms.ToArray();
            }
        }


    }
}