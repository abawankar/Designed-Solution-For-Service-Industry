using D.UserInterFace.Areas.SCM.Models.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace D.UserInterFace.Areas.SCM.Models.Report
{
    public class NboCalendarModel
    {
        public IList<NBOModel> CurrentNBO { get; set; }
        public IList<NboReceivableModel> Incoming { get; set; }
    }

    public class NboReceivableModel
    {
        public int nboid { get; set; }
        public int branchid { get; set; }
        public double Received { get; set; }
        public double Balance { get; set; }
    }

    public class NboCalendarRepository
    {
        public static IList<NboReceivableModel> GetReceivable()
        {
            ReceivableRepository dal = new ReceivableRepository();
            var data = from m in dal.GetAll()
                       group m by m.NBO into g
                       select new NboReceivableModel
                       {
                           nboid = g.Key.Id,
                           branchid = g.Key.Branch.Id,
                           Received = g.Sum(x => x.AmountReceived),
                           Balance = g.Sum(x => x.Amount - x.AmountReceived)
                       };

            return data.ToList();

        }
    }
}