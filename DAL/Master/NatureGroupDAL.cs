using System;
using Domain.Interface.Master;

namespace DAL.Master
{
    public class NatureGroupDAL : Common.CommonDAL<INatureGroup>
    {
        public void AddBusinessNature(int groupid, string naturelist)
        {
            INatureGroup bl = GetById(groupid);
            BusinessNatureDAL dal = new BusinessNatureDAL();
            string[] list = naturelist.Split(',');
            for (int i = 1; i < list.Length; i++)
            {
                int id = Convert.ToInt32(list[i]);
                IBusinessNature nature = dal.GetById(id);
                bl.NatureName.Add(nature);
                InsertOrUpdate(bl);
            }
        }
    }
}
