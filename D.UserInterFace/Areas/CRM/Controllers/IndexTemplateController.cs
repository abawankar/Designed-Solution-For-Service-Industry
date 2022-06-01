using System;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.Mvc;
using ApplicationServices.Web;
using DAL.CRM;
using Domain.Interface.CRM;
using Domain.Interface.Master;
using D.UserInterFace.Areas.CRM.Models;
using D.UserInterFace.Models.Masters;

namespace D.UserInterFace.Areas.CRM.Controllers
{
    [HandleError]
    
    public class IndexTemplateController : Controller
    {
        //
        // GET: /IndexTemplate/

       
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult ListTemplate(string name = null, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                EmailTemplateRepository dal = new EmailTemplateRepository();
                var model = dal.GetAll();
                int count = model.Count;
                var Model1 = model.OrderBy(x => x.TemplateName).Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.InnerException.Message });
            }
        }

        public ActionResult SendTestMail(int id)
        {
            EmailTemplateRepository dal = new EmailTemplateRepository();
            EmployeeRepository e = new EmployeeRepository();
            EmailTemplateModel bl = dal.GetById(id);
            MailMessage msg = new MailMessage();
            IEmployee emp = e.GetBy(User.Identity.Name.ToUpper());
            string to = emp.MailId;
            msg.To.Add(new MailAddress(to));
            msg.From = new MailAddress("AgneloF1@1001events.com", "Template");
            msg.Subject = bl.Subject;
            string htmlBody = "";
            string images = "";
            bool flag = false;
            foreach (var att in bl.Attachment)
            {
                string file = "";
                file = Path.Combine(Server.MapPath(Url.Content("~/Upload/EmailAttachments")), att.FileOnServer);

                if (att.AttachType == 0)
                {
                    msg.Attachments.Add(new Attachment(file));
                    flag = true;
                }
                else
                {
                    flag = false;
                    file = "http://1001eventstourism.com/Upload/EmailAttachments/" + att.FileOnServer;
                    images = images + "<img style=\"width:100%\" src=\"" + file + "\" /><br/>";
                }
            }

            if (flag == false && bl.Attachment.Count != 0)
            {
                if (bl.Attachment.Where(x => x.AttachType == 1).Count() > 2)
                {
                    using (StreamReader reader = System.IO.File.OpenText(Path.Combine(Server.MapPath(Url.Content("~/Upload/EmailAttachments")), "email.html")))
                    {
                        htmlBody = reader.ReadToEnd();
                        htmlBody = htmlBody.Replace("{UserName}", "Dear " + " " + emp.EmpName);
                        htmlBody = htmlBody.Replace("{Insert-Image}", images);
                        htmlBody = htmlBody.Replace("{mailbody}", bl.EmailBody);

                        htmlBody = htmlBody.Replace("{Salutation}", "Dear ");
                        htmlBody = htmlBody.Replace("{ContactFirstName}", emp.EmpName);
                        htmlBody = htmlBody.Replace("{AccountName}", emp.Department.Name);
                    }
                }
                else
                {
                    using (StreamReader reader = System.IO.File.OpenText(Path.Combine(Server.MapPath(Url.Content("~/Upload/EmailAttachments")), "email2.html")))
                    {
                        htmlBody = reader.ReadToEnd();
                        htmlBody = htmlBody.Replace("{UserName}", "Dear " + " " + emp.EmpName);
                        htmlBody = htmlBody.Replace("{Insert-Image}", images);
                        htmlBody = htmlBody.Replace("{mailbody}", bl.EmailBody);

                        htmlBody = htmlBody.Replace("{Salutation}", "Dear ");
                        htmlBody = htmlBody.Replace("{ContactFirstName}", emp.EmpName);
                        htmlBody = htmlBody.Replace("{AccountName}", emp.Department.Name);
                    }
                }

            }
            else
            {
                using (StreamReader reader = System.IO.File.OpenText(Path.Combine(Server.MapPath(Url.Content("~/Upload/EmailAttachments")), "email2.html")))
                {
                    htmlBody = reader.ReadToEnd();
                    htmlBody = htmlBody.Replace("{UserName}", "Dear " + " " + emp.EmpName);
                    htmlBody = htmlBody.Replace("{Insert-Image}", "");
                    htmlBody = htmlBody.Replace("{mailbody}", bl.EmailBody);

                    htmlBody = htmlBody.Replace("{Salutation}", "Dear ");
                    htmlBody = htmlBody.Replace("{ContactFirstName}", emp.EmpName);
                    htmlBody = htmlBody.Replace("{AccountName}", emp.Department.Name);
                }

            }

            msg.Body = htmlBody;
            msg.IsBodyHtml = true;
            //msg.Headers.Add("Disposition-Notification-To", to);
            msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
            EmailSetting.SendEmail(msg);

            return RedirectToAction("Index");
        }

        public ActionResult SendTestMail1(int id)
        {
            EmailTemplateRepository dal = new EmailTemplateRepository();
            EmployeeRepository e = new EmployeeRepository();
            EmailTemplateModel bl = dal.GetById(id);
            MailMessage msg = new MailMessage();
            msg.To.Add(new MailAddress("arvind@aprilsourcing.com"));
            msg.From = new MailAddress("arvind@aprilsourcing.com", "Template");
            msg.Subject = bl.Subject;
            string tempfile = "";
            string htmlBody = "";
            bool flag = false;
            foreach (var item in bl.Attachment)
            {
                var filename = Path.Combine(Server.MapPath(Url.Content("~/Upload/EmailAttachments")), item.FileOnServer);
                tempfile = System.IO.Path.GetTempPath() + item.FileName;
                if (System.IO.File.Exists(tempfile))
                {
                    System.IO.File.Delete(tempfile);
                }
                System.IO.File.Copy(filename, tempfile);
                if (item.AttachType == 0)
                {
                    msg.Attachments.Add(new Attachment(tempfile));
                    flag = true;
                }
            }

            if (flag == false && bl.Attachment.Count() != 0)
            {
                htmlBody = "<html><body>" + bl.EmailBody + "<br><img src=\"cid:filename\"></body></html>";
                AlternateView htmlview = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
                LinkedResource img = new LinkedResource(tempfile, MediaTypeNames.Image.Jpeg);
                img.ContentId = "filename";
                htmlview.LinkedResources.Add(img);
                msg.AlternateViews.Add(htmlview);

            }
            else
            {
                htmlBody = "<html><body>" + bl.EmailBody + "</body></html>";
                AlternateView htmlview = AlternateView.CreateAlternateViewFromString(htmlBody, null, MediaTypeNames.Text.Html);
                msg.AlternateViews.Add(htmlview);
            }
            msg.Body = bl.EmailBody;
            //msg.Headers.Add("Disposition-Notification-To", "arvind@aprilsourcing.com");
            msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
            msg.IsBodyHtml = true;
            EmailSetting.SendEmail(msg);

            return RedirectToAction("Index");
        }

        

        public ActionResult AddAttachments(int id)
        {
            ViewBag.sampId = id;
            ViewBag.type = new SelectList(new[]{
            new SelectListItem{Text="Attachment",Value="0"},
            new SelectListItem{Text="Email Body",Value="1"}}, "Value", "Text");
            return PartialView();
        }

        [HttpPost]
        public ActionResult UploadImage(int sampId, int type, HttpPostedFileWrapper imageFile)
        {
            EmailTemplateRepository dal = new EmailTemplateRepository();
            int tot = dal.GetById(sampId).Attachment.Count();

            string[] filetypestirng = imageFile.FileName.Split('.');
            string filetype = filetypestirng[filetypestirng.Length - 1];

            var fileName = String.Format("{0}." + filetype, sampId + "-" + tot);
            var imagePath = Path.Combine(Server.MapPath(Url.Content("~/Upload/EmailAttachments")), fileName);
            imageFile.SaveAs(imagePath);


            EmailAttachmentModel bl = new EmailAttachmentModel();
            bl.FileName = imageFile.FileName;
            if (filetype == "jpg" || filetype == "jpeg")
            {
                System.Drawing.Image img = System.Drawing.Image.FromFile(imagePath);
                bl.Width = img.Width;
                bl.Height = img.Height;
            }

            bl.Filetype = filetype;
            bl.AttachType = type;
            bl.FileOnServer = fileName;
            bl.FileSize = (Convert.ToDouble(imageFile.ContentLength) * 0.000000954).ToString() + "MB";
            dal.AddAttachments(bl, sampId);

            return RedirectToAction("Index", "IndexTemplate");
        }

        [HttpPost]
        public JsonResult AttachmentsList(int id)
        {
            try
            {
                EmailAttachmentRepository dal = new EmailAttachmentRepository();
                var model = dal.GetAll(id);
                int count = model.Count;
                return Json(new { Result = "OK", Records = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

       
        [HttpPost]
        public JsonResult DeleteAttachment(EmailAttachmentModel model)
        {
            EmailAttachmentDAL dal = new EmailAttachmentDAL();
            IEmailAttachment bl = dal.GetById(model.Id);
            if (dal.Delete(bl) == true)
            {
                var imagePath = Path.Combine(Server.MapPath(Url.Content("~/Upload/EmailAttachments")), bl.FileOnServer);
                System.IO.File.Delete(imagePath);
            }

            return Json(new { Result = "OK", Record = model });
        }

       
        [HttpPost]
        public JsonResult updateAttachment(EmailAttachmentModel model)
        {
            EmailAttachmentRepository dal = new EmailAttachmentRepository();
            dal.Edit(model);
            return Json(new { Result = "OK", Record = model });
        }

       
        [HttpPost]
        public JsonResult Delete(EmailTemplateModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                EmailTemplateRepository dal = new EmailTemplateRepository();
                dal.Delete(model.Id);
                return Json(new { Result = "OK", Record = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

    }
}
