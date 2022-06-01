using System;
using System.Collections.Generic;
using DAL.Master;
using Domain.Implementation.Master;
using Domain.Interface.Master;

namespace D.UserInterFace.Models.Masters
{
    public class ContactTypeModel : Domain.Implementation.Master.ContactType
    {

    }

    public class ContactTypeRepository : Repository<ContactTypeModel>
    {

        public override ContactTypeModel GetById(int id)
        {
            ContactTypeDAL dal = new ContactTypeDAL();
            AutoMapper.Mapper.CreateMap<ContactType, ContactTypeModel>();
            ContactTypeModel model = AutoMapper.Mapper.Map<ContactTypeModel>(dal.GetById(id));

            return model;
        }

        public override IList<ContactTypeModel> GetAll()
        {
            ContactTypeDAL dal = new ContactTypeDAL();
            AutoMapper.Mapper.CreateMap<ContactType, ContactTypeModel>();
            List<ContactTypeModel> model = AutoMapper.Mapper.Map<List<ContactTypeModel>>(dal.GetAll());

            return model;
        }

        public override void Edit(ContactTypeModel obj)
        {
            ContactTypeDAL dal = new ContactTypeDAL();
            IContactType bl = dal.GetById(obj.Id);
            bl.Name = obj.Name;
            dal.InsertOrUpdate(bl);
        }

        public override void Insert(ContactTypeModel obj)
        {
            ContactTypeDAL dal = new ContactTypeDAL();
            IContactType bl = new ContactType();
            bl.Name = obj.Name;
            dal.InsertOrUpdate(bl);
        }

        public override bool Delete(int id)
        {
            throw new NotImplementedException();
        }
    }

}