
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using DAL.CRM;
using Domain.Implementation.CRM;
using Domain.Implementation.Master;
using Domain.Interface.CRM;
using Domain.Interface.Master;
using D.UserInterFace.Models;
using D.UserInterFace.Models.Masters;

namespace D.UserInterFace.Areas.CRM.Models
{
    public class MassEmailingModel : Domain.Implementation.CRM.MassEmailing
    {
        public int MailTempId { get; set; }
        public int EmpId { get; set; }
        public string ContactList { get; set; }
        public string mailtemplist { get; set; }
        public string mailtime { get; set; }
        public string filelocation { get; set; }
        public bool chkbcc { get; set; }
        public string UserName { get; set; }

    }

    public class MassEmailingRepository : Repository<MassEmailingModel>
    {

        public override MassEmailingModel GetById(int id)
        {
            MassEmailingDAL dal = new MassEmailingDAL();
            AutoMapper.Mapper.CreateMap<MassEmailing, MassEmailingModel>();
            AutoMapper.Mapper.CreateMap<MassEmailing, MassEmailingModel>()
                .ForMember(dest => dest.EmpId, opt => opt.MapFrom(scr => scr.Owner.Id))
                .ForMember(dest => dest.MailTempId, opt => opt.MapFrom(scr => scr.EmailTemplate.Id));
            MassEmailingModel model = AutoMapper.Mapper.Map<MassEmailingModel>(dal.GetById(id));

            return model;
        }

        public IList<ContactModel> GetContactById(int id)
        {
            MassEmailingDAL dal = new MassEmailingDAL();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<ContactModel> model = AutoMapper.Mapper.Map<List<ContactModel>>(dal.GetById(id).ContactGroup);

            return model;
        }

        public override System.Collections.Generic.IList<MassEmailingModel> GetAll()
        {
            MassEmailingDAL dal = new MassEmailingDAL();
            AutoMapper.Mapper.CreateMap<MassEmailing, MassEmailingModel>();
            AutoMapper.Mapper.CreateMap<MassEmailing, MassEmailingModel>()
                .ForMember(dest => dest.EmpId, opt => opt.MapFrom(scr => scr.Owner.Id))
                .ForMember(dest => dest.MailTempId, opt => opt.MapFrom(scr => scr.EmailTemplate.Id));
            List<MassEmailingModel> model = AutoMapper.Mapper.Map<List<MassEmailingModel>>(dal.GetAll());

            return model;
        }

        public override void Edit(MassEmailingModel obj)
        {
            throw new System.NotImplementedException();
        }

        public override void Insert(MassEmailingModel obj)
        {
            MassEmailingDAL dal = new MassEmailingDAL();
            IMassEmailing bl = new MassEmailing();
            bl.Name = obj.Name;
            bl.Schedule = obj.Schedule;
            bl.TimeZone = obj.TimeZone;
            bl.ServerTime = obj.ServerTime;
            bl.Bcc = obj.chkbcc;
            bl.Salutation = obj.Salutation;
            bl.ReplyTo = obj.ReplyTo;
            IEmployee emp = new Employee { Id = obj.EmpId };
            bl.Owner = emp;
            bl.Status = 0;
            string[] list2 = obj.mailtemplist.Split(',');

            IEmailTemplate t = new EmailTemplate { Id = Convert.ToInt32(list2[1]) };
            bl.EmailTemplate = t;

            ContactDAL ct = new ContactDAL();
            List<IContact> ctlist = new List<IContact>();
            string[] list = obj.ContactList.Split(',');
            for (int i = 1; i < list.Length; i++)
            {
                int id = Convert.ToInt32(list[i]);
                IContact contact = ct.GetById(id);
                ctlist.Add(contact);
            }
            bl.ContactGroup = ctlist;
            if (bl.Salutation == null || bl.Salutation == "")
            {
                bl.Salutation = "Dear";
            }
            if (obj.mailtime == "sendnow")
            {
                SendMail(bl.Salutation, Convert.ToInt32(list2[1]), ctlist, obj.filelocation, obj.chkbcc, obj.UserName, bl.ReplyTo);
                bl.Status = 1;
                bl.Schedule = MyExtension.UAETime();
            }

            dal.InsertOrUpdate(bl);
        }

        public override bool Delete(int id)
        {
            MassEmailingDAL dal = new MassEmailingDAL();
            IMassEmailing bl = dal.GetById(id);
            bl.ContactGroup.Clear();
            dal.InsertOrUpdate(bl);
            return dal.Delete(bl);
        }

        public void SendMail(string salutation, int id, List<IContact> contact, string filename, bool bcc, string username, string replyto)
        {
            EmailTemplateRepository dal = new EmailTemplateRepository();
            EmployeeRepository e = new EmployeeRepository();
            EmailTemplateModel bl = dal.GetById(id);
            var usermail = e.GetBy(username.ToUpper());

            int i = 1;
            foreach (var item in contact)
            {
                try
                {
                    MailMessage msg = new MailMessage();
                    msg.To.Add(new MailAddress(item.Email));
                    if (bcc == true && i == 1)
                    {
                        msg.Bcc.Add(new MailAddress(usermail.MailId));
                    }
                    msg.From = new MailAddress("AgneloF1@1001events.com", usermail.EmpName);

                    msg.Subject = bl.Subject;
                    string htmlBody = "";
                    string images = "";
                    bool flag = false;
                    foreach (var item1 in bl.Attachment.OrderBy(x => x.AttachType).ThenBy(x => x.Id))
                    {
                        string file = "";
                        file = filename + "/" + item1.FileOnServer;
                        if (item1.AttachType == 0)
                        {
                            msg.Attachments.Add(new Attachment(file));
                            flag = true;
                        }
                        else
                        {
                            flag = false;
                            file = "http://1001eventstourism.com/Upload/EmailAttachments/" + item1.FileOnServer;
                            images = images + "<img style=\"width:100%\" src=\"" + file + "\" /><br/>";
                        }
                    }
                    if (flag == false && bl.Attachment.Count != 0)
                    {
                        if (bl.Attachment.Where(x => x.AttachType == 1).Count() > 2)
                        {
                            using (StreamReader reader = File.OpenText(filename + "/email.html"))
                            {
                                htmlBody = reader.ReadToEnd();

                                htmlBody = htmlBody.Replace("{UserName}", salutation + " " + item.FirstName);
                                htmlBody = htmlBody.Replace("{Insert-Image}", images);
                                htmlBody = htmlBody.Replace("{mailbody}", bl.EmailBody);

                                htmlBody = htmlBody.Replace("{Salutation}", item.Salutation);
                                htmlBody = htmlBody.Replace("{ContactFirstName}", item.FirstName);
                                htmlBody = htmlBody.Replace("{ContactLastName}", item.LastName);
                                htmlBody = htmlBody.Replace("{AccountName}", item.AccountName);
                            }
                        }
                        else
                        {
                            using (StreamReader reader = File.OpenText(filename + "/email2.html"))
                            {
                                htmlBody = reader.ReadToEnd();
                                htmlBody = htmlBody.Replace("{UserName}", salutation + " " + item.FirstName);
                                htmlBody = htmlBody.Replace("{Insert-Image}", images);
                                htmlBody = htmlBody.Replace("{mailbody}", bl.EmailBody);

                                htmlBody = htmlBody.Replace("{Salutation}", item.Salutation);
                                htmlBody = htmlBody.Replace("{ContactFirstName}", item.FirstName);
                                htmlBody = htmlBody.Replace("{ContactLastName}", item.LastName);
                                htmlBody = htmlBody.Replace("{AccountName}", item.AccountName);
                            }
                        }
                    }
                    else
                    {
                        using (StreamReader reader = File.OpenText(filename + "/email2.html"))
                        {
                            htmlBody = reader.ReadToEnd();
                            htmlBody = htmlBody.Replace("{UserName}", salutation + " " + item.FirstName);
                            htmlBody = htmlBody.Replace("{Insert-Image}", "");
                            htmlBody = htmlBody.Replace("{mailbody}", bl.EmailBody);

                            htmlBody = htmlBody.Replace("{Salutation}", item.Salutation);
                            htmlBody = htmlBody.Replace("{ContactFirstName}", item.FirstName);
                            htmlBody = htmlBody.Replace("{ContactLastName}", item.LastName);
                            htmlBody = htmlBody.Replace("{AccountName}", item.AccountName);
                        }
                    }
                    msg.Body = htmlBody;
                    msg.IsBodyHtml = true;
                    msg.DeliveryNotificationOptions = DeliveryNotificationOptions.OnSuccess;
                    //msg.Headers.Add("Disposition-Notification-To", usermail.MailId);
                    msg.ReplyToList.Add(new MailAddress(usermail.MailId, usermail.EmpName));
                    if (!string.IsNullOrEmpty(replyto))
                    {
                        string[] disp = replyto.Split('@');
                        msg.ReplyToList.Add(new MailAddress(replyto, disp[0]));
                    }
                    try { EmailSetting.SendEmail(msg); }
                    catch (Exception) { }

                    i++;
                }
                catch (Exception) { }
            }

        }

        

    }
}