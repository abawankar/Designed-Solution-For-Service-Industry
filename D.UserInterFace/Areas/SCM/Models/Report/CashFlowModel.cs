using D.UserInterFace.Areas.SCM.Models.Transaction;
using System.Collections.Generic;
using System.Linq;

namespace D.UserInterFace.Areas.SCM.Models.Report
{
    public class CashFlowModel
    {
        public IList<ReceivableModel> Receivable { get; set; }
        public IList<PayableModel> Payable { get; set; }
    }

    public class CashFlowRepository
    {
        public CashFlowModel GetAll(int branid)
        {
            CashFlowModel model = new CashFlowModel();

            PayableRepository pay = new PayableRepository();
            ReceivableRepository rec = new ReceivableRepository();

            if (branid == 0)
            {
                model.Receivable = rec.GetAll();
                model.Payable = pay.GetAll();
            }
            else
            {
                model.Receivable = rec.GetAll().Where(x => x.NBO.Branch.Id == branid).ToList();
                model.Payable = pay.GetAll().Where(x => x.NBO.Branch.Id == branid).ToList();
            }

            return model;

        }
    }
}