using System;
using Domain.Interface.CRM;

namespace DAL.CRM
{
    public class ContactGroupDAL : Common.CommonDAL<IContactGroup>
    {
        public void AddContact(int groupid, string contactlist)
        {
            IContactGroup bl = GetById(groupid);
            ContactDAL dal = new ContactDAL();
            string[] list = contactlist.Split(',');
            for (int i = 1; i < list.Length; i++)
            {
                int id = Convert.ToInt32(list[i]);
                IContact contact = dal.GetById(id);
                bl.ContactList.Add(contact);
                InsertOrUpdate(bl);
            }
        }
    }
}
