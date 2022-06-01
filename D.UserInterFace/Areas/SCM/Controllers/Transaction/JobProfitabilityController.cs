using D.UserInterFace.Areas.SCM.Models.Report;
using D.UserInterFace.Areas.SCM.Models.Transaction;
using D.UserInterFace.Models.Masters;
using System.Linq;
using System.Web.Mvc;

namespace D.UserInterFace.Areas.SCM.Controllers
{
    public class JobProfitabilityController : Controller
    {
        //
        // GET: /JobProfitability/

        public ActionResult Index()
        {
            CompanyRepository c = new CompanyRepository();
            ViewBag.Branch = new SelectList(c.BranchList(), "Id", "Name");
            return View();
        }

        public ActionResult GenrateReport(string list = null, int branch = 0)
        {
            NBORepository dal = new NBORepository();
            NboCalendarModel model = new NboCalendarModel();


            if (!string.IsNullOrEmpty(list))
            {
                string[] nboid = list.Split(',');
                model.CurrentNBO = dal.GetAll()
                                      .Where(x => nboid.Contains(x.Id.ToString())).ToList();
                model.Incoming = NboCalendarRepository.GetReceivable().Where(x => nboid.Contains(x.nboid.ToString())).ToList();
                return PartialView(model);
            }
            else
            {
                if (branch != 0)
                {
                    model.CurrentNBO = dal.GetAll().Where(x => x.Status.Id == 5 || x.Status.Id == 6 || x.Status.Id == 7)
                                    .Where(x => x.Branch.Id == branch).ToList();
                    model.Incoming = NboCalendarRepository.GetReceivable().Where(x => x.branchid == branch).ToList();
                    return PartialView(model);
                }
                else
                {
                    return new EmptyResult();
                }

            }


        }


    }
}
