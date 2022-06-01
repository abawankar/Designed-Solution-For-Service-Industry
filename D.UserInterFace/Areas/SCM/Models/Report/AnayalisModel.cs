
using D.UserInterFace.Areas.SCM.Models.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;


namespace D.UserInterFace.Areas.SCM.Models.Report
{
    public class AnayalisModel
    {
        public IList<NBOModel> CurrentNBO { get; set; }
        public IList<NBOModel> PriviousNBO { get; set; }
    }

    public class AnaylysisRepository
    {
        public IList<NBOModel> GetCurrentNBO(IList<NBOModel> model, string dateFrom, string dateTo)
        {
            DateTime dtFrom = Convert.ToDateTime(dateFrom);
            DateTime dtTo = Convert.ToDateTime(dateTo);
            NBORepository dal = new NBORepository();
            IList<NBOModel> data = model.
                Where(x => (x.RequestDate >= dtFrom
                         && x.RequestDate <= dtTo)).ToList();
            return data;
        }

        public IList<NBOModel> GetPriviousNBO(IList<NBOModel> model, string dateFrom, string dateTo)
        {
            NBORepository dal = new NBORepository();
            IList<NBOModel> data = model.
                Where(x => (x.RequestDate >= Convert.ToDateTime(dateFrom).AddYears(-1)
                         && x.RequestDate <= Convert.ToDateTime(dateTo).AddYears(-1))).ToList();
            return data;
        }
    }
}