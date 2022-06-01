using D.UserInterFace.Areas.SCM.Models.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace D.UserInterFace.Areas.SCM.Models.Report
{
    public class NBOEventRequestModel
    {
        public IList<NBOModel> Request { get; set; }
        public IList<NBOModel> Event { get; set; }
    }

    public class NBOEventRequestRepository
    {
        public IList<NBOModel> GetRequest(string dateFrom = null, string dateTo = null)
        {
            NBORepository dal = new NBORepository();
            IList<NBOModel> list = new List<NBOModel>();
            if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
            {
                list = dal.GetAll().Where(x => x.RequestDate >= Convert.ToDateTime(dateFrom) && x.RequestDate <= Convert.ToDateTime(dateTo)).ToList();
            }
            else
            {
                list = dal.GetAll().Take(0).ToList();
            }

            return list;
        }
        public IList<NBOModel> GetEvent(string dateFrom = null, string dateTo = null)
        {
            NBORepository dal = new NBORepository();
            IList<NBOModel> list = new List<NBOModel>();
            if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
            {
                list = dal.GetAll().Where(x => x.CheckinDate >= Convert.ToDateTime(dateFrom) && x.CheckinDate <= Convert.ToDateTime(dateTo)).ToList();
            }
            else
            {
                list = dal.GetAll().Take(0).ToList();
            }
            return list;
        }

    }
}