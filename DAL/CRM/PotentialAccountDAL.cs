using Domain.Interface.CRM;

namespace DAL.CRM
{
    public class PotentialAccountDAL : Common.CommonDAL<IPotentialAccount>
    {
        public void AddContact(IPotentialContact obj, int id)
        {
            IPotentialAccount bl = GetById(id);
            obj.AccountName = bl.AccountName;
            bl.Contact.Add(obj);
            InsertOrUpdate(bl);
        }

    }
}
