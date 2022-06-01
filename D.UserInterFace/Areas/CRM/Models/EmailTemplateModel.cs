
using System.Collections.Generic;
using DAL.CRM;
using Domain.Implementation.CRM;
using Domain.Interface.CRM;
using D.UserInterFace.Models;

namespace D.UserInterFace.Areas.CRM.Models
{
    public class EmailTemplateModel : Domain.Implementation.CRM.EmailTemplate
    {

    }

    public class EmailTemplateRepository : Repository<EmailTemplateModel>
    {
        public override EmailTemplateModel GetById(int id)
        {
            EmailTemplateDAL dal = new EmailTemplateDAL();
            AutoMapper.Mapper.CreateMap<EmailTemplate, EmailTemplateModel>();
            EmailTemplateModel model = AutoMapper.Mapper.Map<EmailTemplateModel>(dal.GetById(id));

            return model;
        }

        public override System.Collections.Generic.IList<EmailTemplateModel> GetAll()
        {
            EmailTemplateDAL dal = new EmailTemplateDAL();
            AutoMapper.Mapper.CreateMap<EmailTemplate, EmailTemplateModel>();
            List<EmailTemplateModel> model = AutoMapper.Mapper.Map<List<EmailTemplateModel>>(dal.GetAll());

            return model;
        }

        public override void Edit(EmailTemplateModel obj)
        {
            EmailTemplateDAL dal = new EmailTemplateDAL();
            IEmailTemplate bl = dal.GetById(obj.Id);
            bl.TemplateName = obj.TemplateName;
            bl.UniqueName = obj.UniqueName;
            bl.Encoding = obj.Encoding;
            bl.Description = obj.Description;
            bl.Subject = obj.Subject;
            bl.EmailBody = obj.EmailBody;
            bl.Type = obj.Type;
            dal.InsertOrUpdate(bl);
        }

        public override void Insert(EmailTemplateModel obj)
        {
            EmailTemplateDAL dal = new EmailTemplateDAL();
            IEmailTemplate bl = new EmailTemplate();
            bl.TemplateName = obj.TemplateName;
            bl.UniqueName = obj.UniqueName;
            bl.Encoding = obj.Encoding;
            bl.Description = obj.Description;
            bl.Subject = obj.Subject;
            bl.EmailBody = obj.EmailBody;
            bl.Type = obj.Type;
            dal.InsertOrUpdate(bl);




        }

        public override bool Delete(int id)
        {
            EmailTemplateDAL dal = new EmailTemplateDAL();
            IEmailTemplate bl = dal.GetById(id);
            return dal.DeleteEmailTemplate(bl);
            //return dal.Delete(bl);
        }

        public void AddAttachments(EmailAttachmentModel obj, int id)
        {
            EmailTemplateDAL dal = new EmailTemplateDAL();
            dal.AddAttachment(obj, id);
        }
    }
}