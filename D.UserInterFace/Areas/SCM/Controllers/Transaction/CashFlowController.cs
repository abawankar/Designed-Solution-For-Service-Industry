using D.UserInterFace.Areas.SCM.Models.Master;
using D.UserInterFace.Areas.SCM.Models.Report;
using D.UserInterFace.Models.Masters;
using System;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace D.UserInterFace.Areas.SCM.Controllers
{
    public class CashFlowController : Controller
    {
        //
        // GET: /CashFlow/

        public ActionResult Index()
        {
            CompanyRepository c = new CompanyRepository();
            ViewBag.Branch = new SelectList(c.BranchList(), "Id", "Name");

            return View();
        }

        public CashFlowModel CashFlowData(int branid = 0, string client = null, string supplier = null, string dateFrom = null, string dateTo = null)
        {
            CashFlowRepository dal = new CashFlowRepository();
            CashFlowModel model = new CashFlowModel();
            if (!string.IsNullOrEmpty(dateFrom) && !string.IsNullOrEmpty(dateTo))
            {
                model = dal.GetAll(branid);
                model.Receivable = model.Receivable
                    .Where(x => (x.DueDate >= Convert.ToDateTime(dateFrom)
                         && x.DueDate <= Convert.ToDateTime(dateTo))).ToList();
                model.Payable = model.Payable
                    .Where(x => (x.DueDate >= Convert.ToDateTime(dateFrom)
                         && x.DueDate <= Convert.ToDateTime(dateTo))).ToList();
            }
            else
            {
                model = dal.GetAll(branid);
            }
            if (!string.IsNullOrEmpty(client))
            {
                string[] clientid = client.Split(',');
                model.Receivable = model.Receivable.Where(x => clientid.Contains(x.NBO.ClientName.Id.ToString())).ToList();
            }
            if (!string.IsNullOrEmpty(supplier))
            {
                string[] supplierid = supplier.Split(',');
                model.Payable = model.Payable.Where(x => supplierid.Contains(x.PayingTo.Id.ToString())).ToList();
            }
            return model;
        }

        public ActionResult ViewCashFlow(int branid = 0, string client = null, string supplier = null, string dateFrom = null, string dateTo = null)
        {
            CashFlowModel model = CashFlowData(branid, client, supplier, dateFrom, dateTo);
            return PartialView(model);
        }

        public ActionResult Export(int branch = 0, string client = null, string supplier = null, string dateFrom = null, string dateTo = null)
        {
            CashFlowModel model = CashFlowData(branch, client, supplier, dateFrom, dateTo);
            DataTable dt = new DataTable();

            #region -- Filtering data to fill table --

            var incoming = from r in model.Receivable
                           group r by r.NBO into s
                           select s;

            var outgoing = from r in model.Payable
                           group r by r.NBO into s
                           select s;

            #endregion

            #region -- Table Column Declarition --

            string m1 = (MyExtension.UAETime().AddMonths(1).ToString("MMM-yy"));
            string m2 = (MyExtension.UAETime().AddMonths(2).ToString("MMM-yy"));
            string m3 = (MyExtension.UAETime().AddMonths(3).ToString("MMM-yy"));
            string m4 = (MyExtension.UAETime().AddMonths(4).ToString("MMM-yy"));
            string m5 = (MyExtension.UAETime().AddMonths(5).ToString("MMM-yy"));
            string m6 = (MyExtension.UAETime().AddMonths(6).ToString("MMM-yy"));
            string m7 = (MyExtension.UAETime().AddMonths(7).ToString("MMM-yy"));
            string m8 = (MyExtension.UAETime().AddMonths(8).ToString("MMM-yy"));
            string m9 = (MyExtension.UAETime().AddMonths(9).ToString("MMM-yy"));


            dt.Columns.Add(new DataColumn("Project", typeof(string)));
            dt.Columns.Add(new DataColumn("PM", typeof(string)));
            dt.Columns.Add(new DataColumn("Client/Supplier", typeof(string)));
            dt.Columns.Add(new DataColumn("EventName", typeof(string)));
            dt.Columns.Add(new DataColumn("CheckIn", typeof(string)));
            dt.Columns.Add(new DataColumn("CheckOut", typeof(string)));
            dt.Columns.Add(new DataColumn("Incoming/Outgoing", typeof(string)));
            dt.Columns.Add(new DataColumn("Received/Paid", typeof(string)));

            dt.Columns.Add(new DataColumn("Balance", typeof(string)));
            dt.Columns.Add(new DataColumn("DueDate", typeof(string)));
            dt.Columns.Add(new DataColumn("AmountDue", typeof(string)));
            dt.Columns.Add(new DataColumn("OverDue", typeof(string)));

            dt.Columns.Add(new DataColumn(m1, typeof(string)));
            dt.Columns.Add(new DataColumn(m2, typeof(string)));
            dt.Columns.Add(new DataColumn(m3, typeof(string)));
            dt.Columns.Add(new DataColumn(m4, typeof(string)));
            dt.Columns.Add(new DataColumn(m5, typeof(string)));
            dt.Columns.Add(new DataColumn(m6, typeof(string)));
            dt.Columns.Add(new DataColumn(m7, typeof(string)));
            dt.Columns.Add(new DataColumn(m8, typeof(string)));
            dt.Columns.Add(new DataColumn(m9, typeof(string)));

            #endregion

            #region -- Table internal Body --

            DataRow rec = dt.NewRow();
            rec["Project"] = "INCOMING";
            dt.Rows.Add(rec);

            #region -- Incoming Data ---

            foreach (var item in incoming.Where(x => (x.Sum(y => y.Amount) - x.Sum(z => z.AmountReceived)) != 0))
            {
                DataRow dr = dt.NewRow();
                dr["Project"] = item.Key.FileNumber;
                dr["PM"] = item.Key.FileHandler.EmpName;
                dr["Client/Supplier"] = item.Key.ClientName.Name;
                dr["EventName"] = item.Key.EventName;
                dr["CheckIn"] = Convert.ToDateTime(item.Key.CheckinDate).ToString("dd MMM yyyy");
                dr["CheckOut"] = Convert.ToDateTime(item.Key.CheckOutDate).ToString("dd MMM yyyy");
                dr["Incoming/Outgoing"] = item.Sum(x => x.Amount).ToString("##,###");
                dr["Received/Paid"] = item.Sum(x => x.AmountReceived).ToString("##,###");
                dr["Balance"] = item.Sum(x => x.Amount - x.AmountReceived).ToString("##,###");
                dt.Rows.Add(dr);

                foreach (var item1 in item)
                {
                    DataRow inner = dt.NewRow();
                    inner["DueDate"] = Convert.ToDateTime(item1.DueDate).ToString("dd MMM yyyy");

                    if (item.Sum(x => x.Amount - x.AmountReceived) == 0) { }
                    else
                    {
                        string duedate = Convert.ToDateTime(item1.DueDate).ToString("MMM-yy");
                        inner["AmountDue"] = (item1.Amount - item1.AmountReceived);
                        if (Convert.ToDateTime(item1.DueDate) < Convert.ToDateTime(MyExtension.UAETime().ToShortDateString()))
                        {
                            inner["OverDue"] = (item1.Amount - item1.AmountReceived);
                        }
                        if (duedate == m1)
                        {
                            inner[m1] = (item1.Amount - item1.AmountReceived);
                        }
                        if (duedate == m2)
                        {
                            inner[m2] = (item1.Amount - item1.AmountReceived);
                        }
                        if (duedate == m3)
                        {
                            inner[m3] = (item1.Amount - item1.AmountReceived);
                        }
                        if (duedate == m4)
                        {
                            inner[m4] = (item1.Amount - item1.AmountReceived);
                        }
                        if (duedate == m5)
                        {
                            inner[m5] = (item1.Amount - item1.AmountReceived);
                        }
                        if (duedate == m6)
                        {
                            inner[m6] = (item1.Amount - item1.AmountReceived);
                        }
                        if (duedate == m7)
                        {
                            inner[m7] = (item1.Amount - item1.AmountReceived);
                        }
                        if (duedate == m8)
                        {
                            inner[m8] = (item1.Amount - item1.AmountReceived);
                        }
                        if (duedate == m9)
                        {
                            inner[m9] = (item1.Amount - item1.AmountReceived);
                        }
                    }

                    dt.Rows.Add(inner);
                }
            }
            DataRow totIn = dt.NewRow();

            totIn["Project"] = "Total Incoming";
            totIn["Incoming/Outgoing"] = Convert.ToDouble(incoming.Sum(x => x.Sum(y => y.Amount))).ToString("##,##0");
            totIn["Received/Paid"] = Convert.ToDouble(incoming.Sum(x => x.Sum(y => y.AmountReceived))).ToString("##,##0");
            totIn["Balance"] = Convert.ToDouble(incoming.Sum(x => x.Sum(y => y.Amount - y.AmountReceived))).ToString("##,##0");

            totIn["AmountDue"] = Convert.ToDouble(incoming.Sum(x => x.Sum(y => y.Amount - y.AmountReceived))).ToString("##,##0");
            totIn["OverDue"] = Convert.ToDouble(incoming.Sum(x => x.Where(o => o.DueDate < Convert.ToDateTime(MyExtension.UAETime().ToShortDateString())).Sum(y => y.Amount - y.AmountReceived))).ToString("##,##0");
            totIn[m1] = Convert.ToDouble(incoming.Sum(x => x.Where(o => Convert.ToDateTime(o.DueDate).ToString("MMM-yy") == m1).Sum(y => y.Amount - y.AmountReceived))).ToString("##,##0");
            totIn[m2] = Convert.ToDouble(incoming.Sum(x => x.Where(o => Convert.ToDateTime(o.DueDate).ToString("MMM-yy") == m2).Sum(y => y.Amount - y.AmountReceived))).ToString("##,##0");
            totIn[m3] = Convert.ToDouble(incoming.Sum(x => x.Where(o => Convert.ToDateTime(o.DueDate).ToString("MMM-yy") == m3).Sum(y => y.Amount - y.AmountReceived))).ToString("##,##0");
            totIn[m4] = Convert.ToDouble(incoming.Sum(x => x.Where(o => Convert.ToDateTime(o.DueDate).ToString("MMM-yy") == m4).Sum(y => y.Amount - y.AmountReceived))).ToString("##,##0");
            totIn[m5] = Convert.ToDouble(incoming.Sum(x => x.Where(o => Convert.ToDateTime(o.DueDate).ToString("MMM-yy") == m5).Sum(y => y.Amount - y.AmountReceived))).ToString("##,##0");
            totIn[m6] = Convert.ToDouble(incoming.Sum(x => x.Where(o => Convert.ToDateTime(o.DueDate).ToString("MMM-yy") == m6).Sum(y => y.Amount - y.AmountReceived))).ToString("##,##0");
            totIn[m7] = Convert.ToDouble(incoming.Sum(x => x.Where(o => Convert.ToDateTime(o.DueDate).ToString("MMM-yy") == m7).Sum(y => y.Amount - y.AmountReceived))).ToString("##,##0");
            totIn[m8] = Convert.ToDouble(incoming.Sum(x => x.Where(o => Convert.ToDateTime(o.DueDate).ToString("MMM-yy") == m8).Sum(y => y.Amount - y.AmountReceived))).ToString("##,##0");
            totIn[m9] = Convert.ToDouble(incoming.Sum(x => x.Where(o => Convert.ToDateTime(o.DueDate).ToString("MMM-yy") == m9).Sum(y => y.Amount - y.AmountReceived))).ToString("##,##0");

            dt.Rows.Add(totIn);

            #endregion

            DataRow pay = dt.NewRow();
            pay["Project"] = "OUTGOING";
            dt.Rows.Add(pay);

            #region -- Outgoing Data ---

            foreach (var item in outgoing.Where(x => x.Sum(y => y.Amount - y.AmountPaid) != 0))
            {
                DataRow d = dt.NewRow();
                d["Project"] = item.Key.FileNumber;
                d["PM"] = item.Key.FileHandler.EmpName;
                d["EventName"] = item.Key.EventName;
                d["CheckIn"] = Convert.ToDateTime(item.Key.CheckinDate).ToString("dd MMM yyyy");
                d["CheckOut"] = Convert.ToDateTime(item.Key.CheckOutDate).ToString("dd MMM yyyy");
                d["Incoming/Outgoing"] = item.Sum(x => x.Amount).ToString("##,###");
                d["Received/Paid"] = item.Sum(x => x.AmountPaid).ToString("##,###");
                d["Balance"] = item.Sum(x => x.Amount - x.AmountPaid).ToString("##,###");
                dt.Rows.Add(d);

                foreach (var item1 in item)
                {
                    DataRow p = dt.NewRow();
                    p["Client/Supplier"] = item1.PayingTo.Name;
                    p["DueDate"] = Convert.ToDateTime(item1.DueDate).ToString("dd MMM yyyy");

                    if (item.Sum(x => x.Amount - x.AmountPaid) == 0) { }
                    else
                    {
                        string duedate = Convert.ToDateTime(item1.DueDate).ToString("MMM-yy");
                        p["AmountDue"] = (item1.Amount - item1.AmountPaid);
                        if (Convert.ToDateTime(item1.DueDate) < Convert.ToDateTime(MyExtension.UAETime().ToShortDateString()))
                        {
                            p["OverDue"] = (item1.Amount - item1.AmountPaid);
                        }
                        if (duedate == m1)
                        {
                            p[m1] = (item1.Amount - item1.AmountPaid);
                        }
                        if (duedate == m2)
                        {
                            p[m2] = (item1.Amount - item1.AmountPaid);
                        }
                        if (duedate == m3)
                        {
                            p[m3] = (item1.Amount - item1.AmountPaid);
                        }
                        if (duedate == m4)
                        {
                            p[m4] = (item1.Amount - item1.AmountPaid);
                        }
                        if (duedate == m5)
                        {
                            p[m5] = (item1.Amount - item1.AmountPaid);
                        }
                        if (duedate == m6)
                        {
                            p[m6] = (item1.Amount - item1.AmountPaid);
                        }
                        if (duedate == m7)
                        {
                            p[m7] = (item1.Amount - item1.AmountPaid);
                        }
                        if (duedate == m8)
                        {
                            p[m8] = (item1.Amount - item1.AmountPaid);
                        }
                        if (duedate == m9)
                        {
                            p[m9] = (item1.Amount - item1.AmountPaid);
                        }
                    }

                    dt.Rows.Add(p);
                }
            }
            DataRow totOut = dt.NewRow();

            totOut["Project"] = "Total Outgoing";
            totOut["Incoming/Outgoing"] = Convert.ToDouble(outgoing.Sum(x => x.Sum(y => y.Amount))).ToString("##,##0");
            totOut["Received/Paid"] = Convert.ToDouble(outgoing.Sum(x => x.Sum(y => y.AmountPaid))).ToString("##,##0");
            totOut["Balance"] = Convert.ToDouble(outgoing.Sum(x => x.Sum(y => y.Amount - y.AmountPaid))).ToString("##,##0");

            totOut["AmountDue"] = Convert.ToDouble(outgoing.Sum(x => x.Sum(y => y.Amount - y.AmountPaid))).ToString("##,##0");
            totOut["OverDue"] = Convert.ToDouble(outgoing.Sum(x => x.Where(o => o.DueDate < Convert.ToDateTime(MyExtension.UAETime().ToShortDateString())).Sum(y => y.Amount - y.AmountPaid))).ToString("##,##0");
            totOut[m1] = Convert.ToDouble(outgoing.Sum(x => x.Where(o => Convert.ToDateTime(o.DueDate).ToString("MMM-yy") == m1).Sum(y => y.Amount - y.AmountPaid))).ToString("##,##0");
            totOut[m2] = Convert.ToDouble(outgoing.Sum(x => x.Where(o => Convert.ToDateTime(o.DueDate).ToString("MMM-yy") == m2).Sum(y => y.Amount - y.AmountPaid))).ToString("##,##0");
            totOut[m3] = Convert.ToDouble(outgoing.Sum(x => x.Where(o => Convert.ToDateTime(o.DueDate).ToString("MMM-yy") == m3).Sum(y => y.Amount - y.AmountPaid))).ToString("##,##0");
            totOut[m4] = Convert.ToDouble(outgoing.Sum(x => x.Where(o => Convert.ToDateTime(o.DueDate).ToString("MMM-yy") == m4).Sum(y => y.Amount - y.AmountPaid))).ToString("##,##0");
            totOut[m5] = Convert.ToDouble(outgoing.Sum(x => x.Where(o => Convert.ToDateTime(o.DueDate).ToString("MMM-yy") == m5).Sum(y => y.Amount - y.AmountPaid))).ToString("##,##0");
            totOut[m6] = Convert.ToDouble(outgoing.Sum(x => x.Where(o => Convert.ToDateTime(o.DueDate).ToString("MMM-yy") == m6).Sum(y => y.Amount - y.AmountPaid))).ToString("##,##0");
            totOut[m7] = Convert.ToDouble(outgoing.Sum(x => x.Where(o => Convert.ToDateTime(o.DueDate).ToString("MMM-yy") == m7).Sum(y => y.Amount - y.AmountPaid))).ToString("##,##0");
            totOut[m8] = Convert.ToDouble(outgoing.Sum(x => x.Where(o => Convert.ToDateTime(o.DueDate).ToString("MMM-yy") == m8).Sum(y => y.Amount - y.AmountPaid))).ToString("##,##0");
            totOut[m9] = Convert.ToDouble(outgoing.Sum(x => x.Where(o => Convert.ToDateTime(o.DueDate).ToString("MMM-yy") == m9).Sum(y => y.Amount - y.AmountPaid))).ToString("##,##0");

            dt.Rows.Add(totOut);

            #endregion

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
            cell_1.Text = "Branch:";
            cell_1.BackColor = Color.Yellow;
            row1.Cells.Add(cell_1);

            TableCell cell_11 = new TableCell();
            cell_11.Text = CompanyRepository.BranchName(branch);
            cell_11.ColumnSpan = 20;
            row1.Cells.Add(cell_11);

            TableCell cell_2 = new TableCell();
            cell_2.Text = "Client:";
            cell_2.BackColor = Color.Yellow;
            row2.Cells.Add(cell_2);

            TableCell cell_22 = new TableCell();
            cell_22.Text = ClientRepository.ClientName(client);
            cell_22.ColumnSpan = 20;
            row2.Cells.Add(cell_22);

            TableCell cell_3 = new TableCell();
            cell_3.Text = "Supplier:";
            cell_3.BackColor = Color.Yellow;
            row3.Cells.Add(cell_3);

            TableCell cell_33 = new TableCell();
            cell_33.Text = SupplierRepository.SupplierName(supplier);
            cell_33.ColumnSpan = 20;
            row3.Cells.Add(cell_33);

            TableCell cell_4 = new TableCell();
            cell_4.Text = "Due From:";
            cell_4.BackColor = Color.Yellow;
            row4.Cells.Add(cell_4);

            TableCell cell_44 = new TableCell();
            cell_44.Text = (string.IsNullOrEmpty(dateFrom) && string.IsNullOrEmpty(dateTo)) ? "All Period"
                : (Convert.ToDateTime(dateFrom).ToString("dd MMM yyyy") + " To:" +
                Convert.ToDateTime(dateTo).ToString("dd MMM yyyy"));
            cell_44.ColumnSpan = 20;
            row4.Cells.Add(cell_44);

            gv.Controls[0].Controls.AddAt(0, row1);
            gv.Controls[0].Controls.AddAt(1, row2);
            gv.Controls[0].Controls.AddAt(2, row3);
            gv.Controls[0].Controls.AddAt(3, row4);

            #endregion

            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=CashFlow.xls");

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
