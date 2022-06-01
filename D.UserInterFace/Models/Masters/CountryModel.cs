using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Master;
using Domain.Implementation.Master;
using Domain.Interface.Master;
using D.UserInterFace.Models;

namespace D.UserInterFace.Models.Masters
{
    public class CountryModel : Domain.Implementation.Master.Country
    {
    }

    public class CountryRepository : Repository<CountryModel>
    {
        public override CountryModel GetById(int id)
        {
            CountryDAL dal = new CountryDAL();
            AutoMapper.Mapper.CreateMap<Country, CountryModel>();
            CountryModel model = AutoMapper.Mapper.Map<CountryModel>(dal.GetById(id));

            return model;
        }

        public override IList<CountryModel> GetAll()
        {
            CountryDAL dal = new CountryDAL();
            AutoMapper.Mapper.CreateMap<Country, CountryModel>();
            List<CountryModel> model = AutoMapper.Mapper.Map<List<CountryModel>>(dal.GetAll().OrderBy(x => x.Name));

            return model;
        }

        public override void Edit(CountryModel obj)
        {
            CountryDAL dal = new CountryDAL();
            ICountry bl = dal.GetById(obj.Id);
            bl.Name = obj.Name.ToUpper();
            dal.InsertOrUpdate(bl);
        }

        public override void Insert(CountryModel obj)
        {
            CountryDAL dal = new CountryDAL();
            ICountry bl = new Country();
            bl.Name = obj.Name.ToUpper();
            dal.InsertOrUpdate(bl);
        }

        public override bool Delete(int id)
        {
            throw new NotImplementedException();
        }

        public static string CountryName(string name)
        {
            CountryRepository c = new CountryRepository();
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

        public static string Country(int name)
        {
            CountryRepository c = new CountryRepository();
            string country = "";
            if (name == 0)
            {
                return country = "All";
            }
            else
            {
                country = c.GetById(name).Name;
                return country;
            }
        }
    }
}