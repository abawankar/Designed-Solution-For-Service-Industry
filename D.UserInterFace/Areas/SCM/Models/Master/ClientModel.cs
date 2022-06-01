using System;
using System.Collections.Generic;
using System.Linq;
using DAL.CRM;
using DAL.Master;
using Domain.Implementation.Master;
using Domain.Interface.CRM;
using Domain.Interface.Master;
using D.UserInterFace.Models;

namespace D.UserInterFace.Areas.SCM.Models.Master
{
    public class ClientModel : Domain.Implementation.Master.Client
    {
        public int CountryId { get; set; }
        public int dup { get; set; }
    }

    public class ClientRepository : Repository<ClientModel>
    {
        #region ---- Client abstract class -----

        public override ClientModel GetById(int id)
        {
            ClientDAL dal = new ClientDAL();
            AutoMapper.Mapper.CreateMap<Client, ClientModel>();
            AutoMapper.Mapper.CreateMap<Client, ClientModel>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            ClientModel model = AutoMapper.Mapper.Map<ClientModel>(dal.GetById(id));

            return model;
        }

        public override IList<ClientModel> GetAll()
        {
            ClientDAL dal = new ClientDAL();
            AutoMapper.Mapper.CreateMap<Client, ClientModel>();
            AutoMapper.Mapper.CreateMap<Client, ClientModel>()
                .ForMember(dest => dest.CountryId, opt => opt.MapFrom(scr => scr.Country.Id));
            List<ClientModel> model = AutoMapper.Mapper.Map<List<ClientModel>>(dal.GetAll().OrderBy(x => x.Name));

            return model; ;
        }

        public override void Edit(ClientModel obj)
        {
            ClientDAL dal = new ClientDAL();
            IClient bl = dal.GetById(obj.Id);
            bl.Name = obj.Name;
            bl.ClientGroup = obj.ClientGroup;
            bl.Remarks = obj.Remarks;
            ICountry c = new Country { Id = obj.CountryId };
            bl.Country = c;
            dal.InsertOrUpdate(bl);

            NewAccountDAL act = new NewAccountDAL();
            INewAccount blact = act.GetByAcId(obj.Id);
            blact.AccountName = obj.Name;
            act.InsertOrUpdate(blact);

            ContactDAL ct = new ContactDAL();
            foreach (var item in blact.Contact)
            {
                IContact blcont = ct.GetById(item.Id);
                blcont.AccountName = obj.Name;
                ct.InsertOrUpdate(blcont);
            }
        }

        public override void Insert(ClientModel obj)
        {
            ClientDAL dal = new ClientDAL();
            IClient bl = new Client();
            bl.Name = obj.Name;
            bl.ClientGroup = obj.ClientGroup;
            bl.Remarks = obj.Remarks;
            ICountry c = new Country { Id = obj.CountryId };
            bl.Country = c;
            dal.InsertOrUpdate(bl);
        }

        public override bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        #endregion

        public static string ClientName(string name)
        {
            ClientRepository c = new ClientRepository();
            string client = "";
            if (string.IsNullOrEmpty(name))
            {
                return client = "All";
            }
            else
            {
                string[] id = name.Split(',');
                foreach (var item in c.GetAll().Where(x => id.Contains(x.Id.ToString())))
                {
                    if (client == "")
                    {
                        client = item.Name;
                    }
                    else
                    {
                        client = client + "/" + item.Name;
                    }
                }
                return client;
            }
        }

        public static int GetByName(string name)
        {
            ClientDAL dal = new ClientDAL();
            return dal.GetByName(name).Id;
        }

        public void InsertBulk(List<ClientModel> model)
        {
            ClientDAL dal = new ClientDAL();

            List<IClient> list = new List<IClient>();

            foreach (var obj in model.Where(x => x.dup != 1))
            {

                IClient bl = new Client();
                bl.Name = obj.Name;
                bl.ClientGroup = obj.ClientGroup;
                ICountry cont = new Country { Id = obj.Country.Id };
                bl.Country = cont;
                list.Add(bl);
            }
            dal.InsertBulk(list);

        }

    }
}