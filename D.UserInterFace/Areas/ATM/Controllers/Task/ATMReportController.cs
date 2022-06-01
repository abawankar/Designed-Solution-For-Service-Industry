using D.UserInterFace.Areas.ATM.Models;
using D.UserInterFace.Areas.SCM.Models.Master;
using D.UserInterFace.Areas.SCM.Models.Transaction;
using D.UserInterFace.Models.Masters;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace D.UserInterFace.Areas.ATM.Controllers
{
    public class ATMReportController : Controller
    {
        //
        // GET: /ATMReport/

        public ActionResult Index()
        {
            NBORepository dal = new NBORepository();
            var job = from m in dal.GetAll()
                      select new { Job = m.FileNumber };
            ViewBag.job = new SelectList(job, "Job", "Job");

            ClientRepository c = new ClientRepository();
            var client = from d in dal.GetAll()
                         select new { client = d.ClientName.Name };
            ViewBag.client = new SelectList(client.Distinct(), "client", "client");

            return View();
        }

        public List<TaskManagerModel> GetTimeAndCostReport(string dateFrom = null, string dateTo = null, string job = null, string client = null)
        {
            TaskManagerRepository dal = new TaskManagerRepository();
            List<TaskManagerModel> model = new List<TaskManagerModel>();
            EmployeeRepository e = new EmployeeRepository();
            model = dal.GetByEmployee(User.Identity.Name).ToList();

            // Job Number Only
            if (string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo) && string.IsNullOrEmpty(client) && !string.IsNullOrEmpty(job))
            {
                model = model.Where(x => x.JobNumber == job).ToList();
            }
            // Date only
            if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo) && string.IsNullOrEmpty(client) && string.IsNullOrEmpty(job))
            {
                model = model.Where(x => Convert.ToDateTime(x.Start) >= Convert.ToDateTime(dateFrom) &&
                    Convert.ToDateTime(x.Compl) <= Convert.ToDateTime(dateTo)).ToList();
            }
            //Client Only
            if (string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo) && !string.IsNullOrEmpty(client) && string.IsNullOrEmpty(job))
            {
                model = model.Where(x => x.ClientName != null && x.ClientName.ToLower() == client.ToLower()).ToList();
            }
            //Date and Client only
            if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo) && !string.IsNullOrEmpty(client) && string.IsNullOrEmpty(job))
            {
                model = model.Where(x => Convert.ToDateTime(x.Start) >= Convert.ToDateTime(dateFrom) &&
                    Convert.ToDateTime(x.Compl) <= Convert.ToDateTime(dateTo)).ToList();
                model = model.Where(x => x.ClientName != null && x.ClientName.ToLower() == client.ToLower()).ToList();
            }
            //Date and Job only
            if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo) && string.IsNullOrEmpty(client) && !string.IsNullOrEmpty(job))
            {
                model = model.Where(x => Convert.ToDateTime(x.Start) >= Convert.ToDateTime(dateFrom) &&
                    Convert.ToDateTime(x.Compl) <= Convert.ToDateTime(dateTo)).ToList();
                model = model.Where(x => x.JobNumber == job).ToList();
            }
            return model;
        }

        public ActionResult TimeAndCostReport(string dateFrom = null, string dateTo = null, string job = null, string client = null)
        {
            List<TaskManagerModel> model = new List<TaskManagerModel>();
            model = GetTimeAndCostReport(dateFrom, dateTo, job, client);
            return PartialView(model);
        }

        public ActionResult ExportTimeAndCost(string dateFrom = null, string dateTo = null, string job = null, string client = null)
        {
            #region -- Filtering data to fill table --

            List<TaskManagerModel> model = new List<TaskManagerModel>();
            model = GetTimeAndCostReport(dateFrom, dateTo, job, client);

            #endregion

            DataTable dt = new DataTable();

            #region -- Table Column Declarition --

            dt.Columns.Add(new DataColumn("Jobnumber", typeof(string)));
            dt.Columns.Add(new DataColumn("ClientName", typeof(string)));
            dt.Columns.Add(new DataColumn("Assigner", typeof(string)));
            dt.Columns.Add(new DataColumn("Assigne", typeof(string)));
            dt.Columns.Add(new DataColumn("TaskDescription", typeof(string)));
            dt.Columns.Add(new DataColumn("Notes", typeof(string)));
            dt.Columns.Add(new DataColumn("StartDate", typeof(string)));
            dt.Columns.Add(new DataColumn("EndDate", typeof(string)));
            dt.Columns.Add(new DataColumn("ActualHours", typeof(string)));
            dt.Columns.Add(new DataColumn("CostPerHour", typeof(string)));
            dt.Columns.Add(new DataColumn("TotalCost", typeof(string)));
            dt.Columns.Add(new DataColumn("OtherCost", typeof(string)));
            dt.Columns.Add(new DataColumn("GrandTotal", typeof(string)));

            #endregion

            #region -- Table internal Body --

            TimeSpan time = new TimeSpan();

            foreach (var item in model.OrderBy(x => x.Start))
            {
                DataRow dr = dt.NewRow();
                dr["Jobnumber"] = item.JobNumber;
                dr["ClientName"] = item.ClientName;
                dr["Assigner"] = item.Assigneer.EmpName;
                dr["Assigne"] = "";
                dr["TaskDescription"] = item.Task;
                dr["Notes"] = item.Notes;
                dr["StartDate"] = Convert.ToDateTime(item.Start).ToString("dd MMM yyyy");
                dr["EndDate"] = Convert.ToDateTime(item.Compl).ToString("dd MMM yyyy");
                dr["ActualHours"] = item.ActualHours;
                dr["CostPerHour"] = Convert.ToDouble(item.EmpCost).ToString("##,##0.00");
                dr["TotalCost"] = Convert.ToDouble(item.TotalCost).ToString("##,##0.00");
                dr["OtherCost"] = Convert.ToDouble(item.OtherCost).ToString("##,##0.00");
                dr["GrandTotal"] = Convert.ToDouble(item.GrandTotal).ToString("##,##0.00");

                dt.Rows.Add(dr);

                if (!string.IsNullOrEmpty(item.ActualHours))
                {
                    string[] ts = item.ActualHours.Split(':');
                    time = time.Add(new TimeSpan(Convert.ToInt32(ts[0]), Convert.ToInt32(ts[1]), 0));
                }
            }

            #endregion

            #region --- table footer --

            DataRow f = dt.NewRow();
            f["Jobnumber"] = "Total";
            f["ActualHours"] = time;
            f["TotalCost"] = Convert.ToDouble(model.Sum(x => x.TotalCost)).ToString("##,##0.00");
            f["OtherCost"] = Convert.ToDouble(model.Sum(x => x.OtherCost)).ToString("##,##0.00");
            f["GrandTotal"] = Convert.ToDouble(model.Sum(x => x.GrandTotal)).ToString("##,##0.00");

            dt.Rows.Add(f);

            #endregion

            GridView gv = new GridView();
            gv.DataSource = dt;
            gv.DataBind();

            #region -- Report Filter --

            GridViewRow row1 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
            GridViewRow row2 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
            GridViewRow row3 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
            GridViewRow row4 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);

            TableCell cell_1 = new TableCell();
            cell_1.Text = "Date From:";
            cell_1.BackColor = Color.Gray;
            row1.Cells.Add(cell_1);

            TableCell cell_11 = new TableCell();
            cell_11.Text = dateFrom;
            row1.Cells.Add(cell_11);

            TableCell cell_2 = new TableCell();
            cell_2.Text = "Date To:";
            cell_2.BackColor = Color.Gray;
            row2.Cells.Add(cell_2);

            TableCell cell_22 = new TableCell();
            cell_22.Text = dateTo;
            row2.Cells.Add(cell_22);

            TableCell cell_3 = new TableCell();
            cell_3.Text = "Job#:";
            cell_3.BackColor = Color.Gray;
            row3.Cells.Add(cell_3);

            TableCell cell_33 = new TableCell();
            cell_33.Text = job;
            row3.Cells.Add(cell_33);

            TableCell cell_4 = new TableCell();
            cell_4.Text = "Client Name:";
            cell_4.BackColor = Color.Gray;
            row4.Cells.Add(cell_4);

            TableCell cell_44 = new TableCell();
            cell_44.Text = client;
            row4.Cells.Add(cell_44);

            gv.Controls[0].Controls.AddAt(0, row1);
            gv.Controls[0].Controls.AddAt(1, row2);
            gv.Controls[0].Controls.AddAt(2, row3);
            gv.Controls[0].Controls.AddAt(3, row4);

            #endregion

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=TimeAndAction.xls");

            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();

            return RedirectToAction("Index");
        }

    }
}
