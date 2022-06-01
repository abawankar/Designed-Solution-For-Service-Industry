
using System.Collections.Generic;
using DAL.CRM;
using Domain.Implementation.CRM;
using Domain.Interface.CRM;
using D.UserInterFace.Models;

namespace D.UserInterFace.Areas.CRM.Models
{
    public class EmailAttachmentModel : Domain.Implementation.CRM.EmailAttachment
    {
    }

    public class EmailAttachmentRepository : Repository<EmailAttachmentModel>
    {

        public override EmailAttachmentModel GetById(int id)
        {
            EmailAttachmentDAL dal = new EmailAttachmentDAL();
            AutoMapper.Mapper.CreateMap<EmailAttachment, EmailAttachmentModel>();
            EmailAttachmentModel model = AutoMapper.Mapper.Map<EmailAttachmentModel>(dal.GetById(id));

            return model;
        }

        public override IList<EmailAttachmentModel> GetAll()
        {
            EmailAttachmentDAL dal = new EmailAttachmentDAL();
            AutoMapper.Mapper.CreateMap<EmailAttachment, EmailAttachmentModel>();
            List<EmailAttachmentModel> model = AutoMapper.Mapper.Map<List<EmailAttachmentModel>>(dal.GetAll());

            return model;
        }

        public IList<EmailAttachmentModel> GetAll(int id)
        {
            EmailTemplateDAL dal = new EmailTemplateDAL();
            AutoMapper.Mapper.CreateMap<EmailAttachment, EmailAttachmentModel>();
            List<EmailAttachmentModel> model = AutoMapper.Mapper.Map<List<EmailAttachmentModel>>(dal.GetById(id).Attachment);

            return model;
        }

        public override void Edit(EmailAttachmentModel obj)
        {
            EmailAttachmentDAL dal = new EmailAttachmentDAL();
            IEmailAttachment bl = dal.GetById(obj.Id);
            bl.AttachType = obj.AttachType;
            dal.InsertOrUpdate(bl);
        }

        public override void Insert(EmailAttachmentModel obj)
        {
            throw new System.NotImplementedException();
        }

        public override bool Delete(int id)
        {
            throw new System.NotImplementedException();
        }

        public static void Insert(EmailAttachmentModel obj, int sampid)
        {
            EmailTemplateRepository dal = new EmailTemplateRepository();
            IEmailAttachment bl = new EmailAttachment();
            bl.FileName = obj.FileName;
            bl.Filetype = obj.Filetype;
            bl.FileOnServer = obj.FileOnServer;

        }
    }
}