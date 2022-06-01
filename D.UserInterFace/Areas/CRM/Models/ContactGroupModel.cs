using System.Collections.Generic;
using DAL.CRM;
using Domain.Implementation.CRM;
using Domain.Implementation.Master;
using Domain.Interface.CRM;
using Domain.Interface.Master;
using D.UserInterFace.Models;

namespace D.UserInterFace.Areas.CRM.Models
{
    public class ContactGroupModel : Domain.Implementation.CRM.ContactGroup
    {
        public int Empid { get; set; }
    }

    public class ContactGroupRepository : Repository<ContactGroupModel>
    {
        public override ContactGroupModel GetById(int id)
        {
            ContactGroupDAL dal = new ContactGroupDAL();
            AutoMapper.Mapper.CreateMap<ContactGroup, ContactGroupModel>();
            AutoMapper.Mapper.CreateMap<ContactGroup, ContactGroupModel>()
                .ForMember(dest => dest.Empid, opt => opt.MapFrom(scr => scr.GroupOwner.Id));
            ContactGroupModel model = AutoMapper.Mapper.Map<ContactGroupModel>(dal.GetById(id));

            return model;
        }


        public override IList<ContactGroupModel> GetAll()
        {
            ContactGroupDAL dal = new ContactGroupDAL();
            AutoMapper.Mapper.CreateMap<ContactGroup, ContactGroupModel>();
            AutoMapper.Mapper.CreateMap<ContactGroup, ContactGroupModel>()
                .ForMember(dest => dest.Empid, opt => opt.MapFrom(scr => scr.GroupOwner.Id));
            List<ContactGroupModel> model = AutoMapper.Mapper.Map<List<ContactGroupModel>>(dal.GetAll());

            return model;
        }

        public override void Edit(ContactGroupModel obj)
        {
            ContactGroupDAL dal = new ContactGroupDAL();
            IContactGroup bl = dal.GetById(obj.Id);
            bl.Name = obj.Name;
            bl.Note = obj.Note;
            dal.InsertOrUpdate(bl);
        }

        public override void Insert(ContactGroupModel obj)
        {
            ContactGroupDAL dal = new ContactGroupDAL();
            IContactGroup bl = new ContactGroup();
            bl.Name = obj.Name;
            bl.Date = obj.Date;
            bl.Note = obj.Note;
            IEmployee e = new Employee { Id = obj.Empid };
            bl.GroupOwner = e;
            dal.InsertOrUpdate(bl);
        }

        public override bool Delete(int id)
        {
            ContactGroupDAL dal = new ContactGroupDAL();
            IContactGroup bl = dal.GetById(id);
            bl.ContactList.Clear();
            return dal.Delete(bl);
        }

        public void DeleteContact(int id, int groupid)
        {
            ContactGroupDAL dal = new ContactGroupDAL();
            ContactDAL ct = new ContactDAL();
            IContact c = ct.GetById(id);
            IContactGroup bl = dal.GetById(groupid);

            int i = bl.ContactList.IndexOf(c);
            bl.ContactList.RemoveAt(i);
            dal.InsertOrUpdate(bl);


        }

        public static void AddContact(int groupid, string contactlist)
        {
            ContactGroupDAL dal = new ContactGroupDAL();
            dal.AddContact(groupid, contactlist);
        }

        public List<ContactModel> GetContactModel(int id)
        {
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>();
            AutoMapper.Mapper.CreateMap<Contact, ContactModel>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<ContactModel> model = AutoMapper.Mapper.Map<List<ContactModel>>(GetById(id).ContactList);

            return model;
        }
    }
}