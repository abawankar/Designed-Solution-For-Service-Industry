
using System;
using System.Collections.Generic;
using System.Net.Mail;
using DAL.CRM;
using Domain.Implementation.CRM;
using Domain.Interface.CRM;
using D.UserInterFace.Models;
using D.UserInterFace.Models.Masters;

namespace D.UserInterFace.Areas.CRM.Models
{
    public class OneStayInTouchModel : Domain.Implementation.CRM.OneStayInTouch
    {
        public int to { get; set; }
        public string currentinfo { get; set; }
        public string contactlist { get; set; }
        public string Emp { get; set; }
        public string Replyto { get; set; }
    }

    public class OneStayInTouchRepository : Repository<OneStayInTouchModel>
    {
        public override OneStayInTouchModel GetById(int id)
        {
            OneStayInTouchDAL dal = new OneStayInTouchDAL();
            AutoMapper.Mapper.CreateMap<OneStayInTouch, OneStayInTouchModel>();
            AutoMapper.Mapper.CreateMap<OneStayInTouch, OneStayInTouchModel>()
                .ForMember(dest => dest.to, opt => opt.MapFrom(scr => scr.Contact.Id));
            OneStayInTouchModel model = AutoMapper.Mapper.Map<OneStayInTouchModel>(dal.GetById(id));

            return model;
        }

        public override System.Collections.Generic.IList<OneStayInTouchModel> GetAll()
        {
            OneStayInTouchDAL dal = new OneStayInTouchDAL();
            AutoMapper.Mapper.CreateMap<OneStayInTouch, OneStayInTouchModel>();
            AutoMapper.Mapper.CreateMap<OneStayInTouch, OneStayInTouchModel>()
                .ForMember(dest => dest.to, opt => opt.MapFrom(scr => scr.Contact.Id));
            List<OneStayInTouchModel> model = AutoMapper.Mapper.Map<List<OneStayInTouchModel>>(dal.GetAll());

            return model;
        }

        public override void Edit(OneStayInTouchModel obj)
        {
            throw new System.NotImplementedException();
        }

        public override void Insert(OneStayInTouchModel obj)
        {
            try
            {
                ContactDAL ct = new ContactDAL();
                OneStayInTouchDAL dal = new OneStayInTouchDAL();
                IOneStayInTouch bl = new OneStayInTouch();
                EmployeeRepository e = new EmployeeRepository();
                var usermail = e.GetBy(obj.Emp);
                bl.CC = obj.CC;
                bl.BCC = obj.BCC;
                bl.Notes = obj.Notes;
                bl.Signature = obj.Signature;
                bl.Subject = obj.Subject;
                bl.Contact = ct.GetById(obj.to);
                bl.Date = obj.Date;
                dal.InsertOrUpdate(bl);

                MailMessage msg = new MailMessage();
                msg.To.Add(new MailAddress(bl.Contact.Email));
                msg.From = new MailAddress("AgneloF1@1001events.com", usermail.EmpName);
                if (!string.IsNullOrEmpty(bl.CC))
                {
                    msg.CC.Add(new MailAddress(bl.CC));
                }
                if (!string.IsNullOrEmpty(bl.BCC))
                {
                    msg.Bcc.Add(new MailAddress(bl.BCC));
                }
                msg.Subject = bl.Subject;
                msg.Body = "Dear " + bl.Contact.FirstName + "<br/><br/>" + bl.Notes + "<br/><br/>" +
                    obj.currentinfo + "</br></br>" + bl.Signature;
                msg.IsBodyHtml = true;
                msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
                //msg.Headers.Add("Disposition-Notification-To", usermail.MailId);
                msg.ReplyToList.Add(new MailAddress(usermail.MailId, usermail.EmpName));
                if (!string.IsNullOrEmpty(obj.Replyto))
                {
                    string[] disp = obj.Replyto.Split('@');
                    msg.ReplyToList.Add(new MailAddress(obj.Replyto, disp[0]));
                }
                EmailSetting.SendEmail(msg);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void InsertBulk(OneStayInTouchModel obj)
        {
            try
            {
                ContactDAL ct = new ContactDAL();
                OneStayInTouchDAL dal = new OneStayInTouchDAL();
                IOneStayInTouch bl = new OneStayInTouch();
                EmployeeRepository e = new EmployeeRepository();
                var usermail = e.GetBy(obj.Emp);
                bl.CC = obj.CC;
                bl.BCC = obj.BCC;
                bl.Notes = obj.Notes;
                bl.Signature = obj.Signature;
                bl.Subject = obj.Subject;
                bl.Contact = ct.GetById(obj.to);
                bl.Date = obj.Date;
                dal.InsertOrUpdate(bl);

                MailMessage msg = new MailMessage();
                msg.To.Add(new MailAddress(bl.Contact.Email));
                msg.From = new MailAddress("AgneloF1@1001events.com", usermail.EmpName);
                //msg.From = new MailAddress("arvind@aprilsourcing.com", usermail.EmpName);
                if (!string.IsNullOrEmpty(bl.CC))
                {
                    msg.CC.Add(new MailAddress(bl.CC));
                }
                if (!string.IsNullOrEmpty(bl.BCC))
                {
                    msg.Bcc.Add(new MailAddress(bl.BCC));
                }

                string currentinfo = "<table style=\"width:450px;background-color:#e6e0e0;border:1px solid #015941;\">" +
                    "<tr><td><p style=\"text-align:center\">" +
                    "<strong>" + bl.Contact.FirstName + " " + bl.Contact.LastName + "</strong><br />" +
                    "<strong>" + bl.Contact.Title + "</strong><br />" +
                    "<strong>" + bl.Contact.Department + "</strong><br />" +
                    "<strong>" + bl.Contact.AccountName + "</strong><br /></p><div style=\"float:left;width:40%\">" +
                    bl.Contact.Street + "<br/>" + bl.Contact.City + "-" + bl.Contact.ZipPostalCode + "<br/>" +
                    bl.Contact.StateProvince + "<br/>" + bl.Contact.Country.Name + "<br>" +
                    bl.Contact.Email + "</div><div style=\"float:right;width:40%\">" +
                    "Phone: " + bl.Contact.Phone + "<br/>Mobile: " + bl.Contact.Mobile + "<br>Fax: " +
                    bl.Contact.Fax + "</div><td></tr>" +
                    "<tr style=\"height:10px;background-color:#015941;\"><td> </td></tr>" +
                    "</table>";

                msg.Subject = bl.Subject;
                msg.Body = "Dear " + bl.Contact.FirstName + "<br/><br/>" + bl.Notes + "<br/><br/>" +
                    currentinfo + "</br></br>" + bl.Signature;
                msg.IsBodyHtml = true;
                msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
                //msg.Headers.Add("Disposition-Notification-To", usermail.MailId);
                msg.ReplyToList.Add(new MailAddress(usermail.MailId, usermail.EmpName));
                if (!string.IsNullOrEmpty(obj.Replyto))
                {
                    string[] disp = obj.Replyto.Split('@');
                    msg.ReplyToList.Add(new MailAddress(obj.Replyto, disp[0]));
                }
                EmailSetting.SendEmail(msg);
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public override bool Delete(int id)
        {
            OneStayInTouchDAL dal = new OneStayInTouchDAL();
            IOneStayInTouch bl = dal.GetById(id);
            return dal.Delete(bl);

        }

        
    }
}