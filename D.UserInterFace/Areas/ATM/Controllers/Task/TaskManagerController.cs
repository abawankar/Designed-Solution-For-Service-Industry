using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using DAL.Master;
using DAL.Task;
using Domain.Implementation.Master;
using Domain.Implementation.Task;
using Domain.Interface.Master;
using Domain.Interface.Task;
using D.UserInterFace.Models.Masters;
using D.UserInterFace.Areas.SCM.Models.Transaction;
using D.UserInterFace.Areas.ATM.Models;
using D.UserInterFace.Areas.ATM.Models.Masters;
using System.Configuration;

namespace D.UserInterFace.Areas.ATM.Controllers
{
    [HandleError]
    [Authorize]
    public class TaskManagerController : Controller
    {
        //
        // GET: /TaskManager/

        public ActionResult Index(string subject = null)
        {
            EmployeeRepository e = new EmployeeRepository();
            SelectList data = new SelectList(Type(), "Text", "Value");
            ViewBag.Type = data;
            ViewBag.Emp = new SelectList(e.GetAll().OrderBy(x => x.EmpName), "Id", "EmpName");
            ViewBag.Subject = subject;

            ViewBag.Status = new SelectList(new[] {
                new SelectListItem { Text = "Start", Value = "1" },
                new SelectListItem { Text = "Cancelled", Value = "2" },
                new SelectListItem { Text = "Completed", Value = "3" },
            }, "Value", "Text");

            ViewBag.StartH = new SelectList(Hour(), "Text", "Value");
            ViewBag.StartM = new SelectList(Minute(), "Text", "Value");

            ViewBag.DueH = new SelectList(Hour(), "Text", "Value");
            ViewBag.DueM = new SelectList(Minute(), "Text", "Value");

            ViewBag.ComplH = new SelectList(Hour(), "Text", "Value");
            ViewBag.ComplM = new SelectList(Minute(), "Text", "Value");

            ViewBag.today = MyExtension.UAETime().ToString("yyyy-MM-dd");

            NBORepository dal = new NBORepository();
            var job = from m in dal.GetAll()
                      select new { Job = m.FileNumber };
            ViewBag.job = new SelectList(job, "Job", "Job");

            ViewBag.CompletePercentage = new SelectList(new[] {
                new SelectListItem { Text = "0%", Value = "0%" },
                new SelectListItem { Text = "10%", Value = "10%" },
                new SelectListItem { Text = "25%", Value = "25%" },
                new SelectListItem { Text = "50%", Value = "50%" },
                new SelectListItem { Text = "75%", Value = "75%" },
                new SelectListItem { Text = "100%", Value = "100%" },
            }, "Value", "Text");

            LocationRepository loc = new LocationRepository();
            ViewBag.location = new SelectList(loc.GetAll(), "Id", "Name");
            return View();
        }

        public ActionResult PendingTask()
        {
            SelectList data = new SelectList(Type(), "Text", "Value");
            ViewBag.Type = data;

            ViewBag.Status = new SelectList(new[] {
                new SelectListItem { Text = "Start", Value = "1" },
                new SelectListItem { Text = "Cancelled", Value = "2" },
                new SelectListItem { Text = "Completed", Value = "3" },
            }, "Value", "Text");

            EmployeeRepository e = new EmployeeRepository();
            ViewBag.Emp = new SelectList(e.GetAll().OrderBy(x => x.EmpName), "Id", "EmpName");

            return View();
        }

        public ActionResult RepeatTask(int taskid)
        {
            ViewBag.taskid = taskid;
            return PartialView();
        }

        public ActionResult TodayTask()
        {
            return PartialView();
        }

        private static IEnumerable<SelectListItem> Type()
        {
            List<SelectListItem> type = new List<SelectListItem>();
            type.Add(new SelectListItem { Value = "Task", Text = "Task" });
            type.Add(new SelectListItem { Value = "Meeting", Text = "Meeting" });
            return type;
        }

        private static IEnumerable<SelectListItem> Hour()
        {
            List<SelectListItem> hour = new List<SelectListItem>();
            for (int i = 0; i < 25; i++)
            {
                hour.Add(new SelectListItem { Value = i.ToString().PadLeft(2, '0'), Text = i.ToString().PadLeft(2, '0') });
            }
            return hour;
        }

        private static IEnumerable<SelectListItem> Minute()
        {
            List<SelectListItem> minute = new List<SelectListItem>();
            for (int i = 0; i < 56; i += 15)
            {
                minute.Add(new SelectListItem { Value = i.ToString().PadLeft(2, '0'), Text = i.ToString().PadLeft(2, '0') });
            }
            return minute;
        }

        [HttpPost]
        public ActionResult CreateTask(TaskManagerModel obj, HttpPostedFileBase fileUploader)
        {
            string mailid = "";
            EmployeeRepository e = new EmployeeRepository();
            IEmployee employee = e.GetBy(User.Identity.Name.ToUpper());
            obj.AssignerId = employee.Id;

            obj.Date = Convert.ToDateTime(MyExtension.UAETime().ToString("yyyy-MM-dd HH:mm:ss"));
            TaskManagerDAL dal = new TaskManagerDAL();
            ITaskManager bl = new TaskManager();
            bl.Date = obj.Date;
            bl.Type = obj.Type;
            bl.Task = obj.Task;
            bl.Notes = obj.Notes;
            bl.Status = obj.Status;
            bl.JobNumber = obj.JobNumber;
            bl.EventName = obj.EventName;
            bl.ClientName = obj.ClientName;
            bl.CompletePercentage = obj.CompletePercentage;

            bl.TotalHours = obj.TotalHours;
            TimeSpan actualtime = new TimeSpan(obj.actualH, obj.actualM, 0);
            bl.ActualHours = actualtime.ToString();
            bl.EmpCost = employee.CostPerHour;
            double cost = Convert.ToDouble((employee.CostPerHour * obj.actualH) + (employee.CostPerHour * (obj.actualM == 0 ? 0 : (Convert.ToDouble(obj.actualM) / 60))));
            bl.TotalCost = cost;
            bl.OtherCost = obj.OtherCost;
            bl.GrandTotal = cost + obj.OtherCost;

            ILocation loc = new Location { Id = obj.LocationId };
            bl.Location = loc;

            TimeSpan tstart = new TimeSpan(obj.StartH, obj.StartM, 0);
            bl.Start = Convert.ToDateTime(obj.Start);// + DateTime.Now.TimeOfDay;
            bl.StartTime = tstart.ToString();
            TimeSpan tdue = new TimeSpan(obj.DueH, obj.DueM, 0);
            if (obj.Due == null)
            {
                bl.Due = Convert.ToDateTime(obj.Start);// + DateTime.Now.TimeOfDay;
                bl.DueTime = bl.StartTime;
            }
            else
            {
                bl.Due = Convert.ToDateTime(obj.Due);// + DateTime.Now.TimeOfDay;
            }

            bl.DueTime = tdue.ToString();
            TimeSpan tcompl = new TimeSpan(obj.ComplH, obj.ComplM, 0);
            bl.Compl = obj.Compl != null ? Convert.ToDateTime(obj.Compl)  : obj.Compl;
            bl.ComplTime = tcompl.ToString();
            TimeSpan duration = DateTime.Parse(tcompl.ToString()).Subtract(DateTime.Parse(tstart.ToString()));
            bl.TotalHours = duration.ToString();
            IEmployee emp = new Employee { Id = obj.AssignerId };
            bl.Assigneer = emp;
            bl.TaskRepeat = obj.TaskRepeat;
            string toName = "";

            if (!string.IsNullOrEmpty(obj.Assignerlist))
            {
                EmployeeDAL edal = new EmployeeDAL();
                string[] assign = obj.Assignerlist.Split(',');
                List<IEmployee> emplist = new List<IEmployee>();
                foreach (var item in assign)
                {
                    IEmployee b = edal.GetById(Convert.ToInt32(item));
                    mailid = mailid + "," + b.MailId;
                    if (toName == "")
                    {
                        toName = b.EmpName;
                    }
                    else
                    {
                        toName = toName + "/" + b.EmpName;
                    }
                    emplist.Add(b);
                }
                bl.Contacts = emplist;

            }
            dal.InsertOrUpdate(bl);

            if (!string.IsNullOrEmpty(obj.Assignerlist) && obj.IsMail == 1)
            {

                string subject = obj.Type + " Assigned - " + obj.Task;
                string body = "<p> Dear " + toName + "</p> Kindly note that the following " + obj.Type +
                    " has been assigned to you and the due date for completion and reporting is fixed for : " +
                Convert.ToDateTime(bl.Due).ToString("dd MMM yyyy") + " " + bl.DueTime;

                body = body + "</p><br/><p><b>" + obj.Task;
                body = body + "</b></p><p>" + " Regards <br/> This is an automatic mail </p>";

                #region  -- sending mail -----

                MailMessage msg = new MailMessage();
                string[] mail = mailid.Split(',');
                for (int i = 1; i < mail.Length; i++)
                {
                    msg.To.Add(new MailAddress(mail[i].ToString()));
                }
                msg.CC.Add(new MailAddress(employee.MailId));
                msg.From = new MailAddress(ConfigurationManager.AppSettings["mail"], ConfigurationManager.AppSettings["frmMsgTaskReport"]);
                msg.Subject = subject;
                msg.Body = body;
                msg.IsBodyHtml = true;

                if (fileUploader != null)
                {
                    string filename = Path.GetFileName(fileUploader.FileName);
                    var path = Path.Combine(Server.MapPath(Url.Content("~/Upload/BMSTaskMail")), filename);
                    fileUploader.SaveAs(path);
                    msg.Attachments.Add(new Attachment(path));
                }

                try
                {
                    //client.Send(msg);
                    EmailSetting.SendEmail(msg);
                    TempData["message"] = "Mail Sending in Backgournd";
                }
                catch (Exception ex)
                {
                    throw ex;
                }

                #endregion
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult Update(TaskManagerModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                TaskManagerRepository task = new TaskManagerRepository();
                ITaskManager bl = task.GetById(model.Id);
                EmployeeRepository e = new EmployeeRepository();
                if (e.GetById(bl.Assigneer.Id).AppLogin.ToUpper() == User.Identity.Name.ToUpper())
                {
                    TaskManagerRepository dal = new TaskManagerRepository();
                    dal.Edit(model);
                    model = dal.GetById(model.Id);
                    return Json(new { Result = "OK", Record = model });
                }
                else
                {
                    return Json(new { Result = "Not Authorised", Message = "You are not owner" });
                }



            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Delete(TaskManagerModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                EmployeeRepository e = new EmployeeRepository();
                if (e.GetById(e.GetByName(User.Identity.Name.ToUpper())).AppLogin.ToUpper() == User.Identity.Name.ToUpper())
                {
                    TaskManagerRepository dal = new TaskManagerRepository();
                    dal.Delete(model.Id);
                    return Json(new { Result = "OK", Record = model });
                }
                else
                {
                    return Json(new { Result = "Not Authorised", Message = "You are not owner" });
                }



            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult List(string col = null, string name = null, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                TaskManagerRepository dal = new TaskManagerRepository();
                List<TaskManagerModel> model = new List<TaskManagerModel>();
                //model = dal.GetAll().Where(x => x.Assigneer.AppLogin == User.Identity.Name.ToUpper() || x.Contacts.Any(y => y.AppLogin.Contains(User.Identity.Name.ToUpper()))).OrderByDescending(x => x.Id).ToList();
                model = dal.GetAllByEmp(User.Identity.Name).ToList();
                switch (col)
                {
                    case "type":
                        model = model.Where(x => x.Type == name).ToList();
                        break;
                    case "status":
                        model = model.Where(x => x.Status == Convert.ToInt32(name)).ToList();
                        break;
                    case "emp":
                        model = model.Where(x => x.Contacts.Any(y => y.Id == Convert.ToInt32(name))).ToList();
                        break;
                    case "date":
                        model = model.Where(x => x.Start.ToShortDateString() == Convert.ToDateTime(name).ToShortDateString()).ToList();
                        break;
                    default:
                        break;
                }


                int count = model.Count;
                List<TaskManagerModel> Model1 = model.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        public JsonResult TodayList(string date = null)
        {
            try
            {
                TaskManagerRepository dal = new TaskManagerRepository();
                List<TaskManagerModel> model = new List<TaskManagerModel>();
                model = dal.GetTodayTask(User.Identity.Name, date).ToList();
                model = model.OrderBy(x => x.StartTime).ToList();
                return Json(new { Result = "OK", Records = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        public JsonResult CompletedList()
        {
            try
            {
                TaskManagerRepository dal = new TaskManagerRepository();
                List<TaskManagerModel> model = new List<TaskManagerModel>();
                string date = string.Empty;
                model = dal.GetTodayTask(User.Identity.Name,date).ToList();

                model = model.OrderBy(x => x.StartTime).ToList();
                return Json(new { Result = "OK", Records = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        public JsonResult PendingList()
        {
            try
            {
                TaskManagerRepository dal = new TaskManagerRepository();
                List<TaskManagerModel> model = new List<TaskManagerModel>();
                //model = dal.GetAll().Where(x => x.Assigneer.AppLogin == User.Identity.Name.ToUpper() || x.Contacts.Any(y => y.AppLogin.Contains(User.Identity.Name.ToUpper()))).OrderByDescending(x => x.Id).ToList();
                model = dal.GetAllByEmp(User.Identity.Name).ToList();
                model = model.Where(x => x.Status != 3 && x.Start.ToShortDateString() != MyExtension.UAETime().ToShortDateString()).ToList();
                return Json(new { Result = "OK", Records = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        private void CreateEmailItem(string mailid, string subject, string body, string cc)
        {
            EmployeeRepository e = new EmployeeRepository();
            MailMessage msg = new MailMessage();
            string[] mail = mailid.Split(',');
            for (int i = 1; i < mail.Length; i++)
            {
                msg.To.Add(new MailAddress(mail[i].ToString()));
            }
            msg.From = new MailAddress(ConfigurationManager.AppSettings["mail"], ConfigurationManager.AppSettings["msgTask"]);
            msg.Subject = subject;
            msg.Body = body;
            msg.IsBodyHtml = true;

            //SmtpClient client = new SmtpClient();
            //client.UseDefaultCredentials = false;
            //client.Credentials = new System.Net.NetworkCredential("BMS1001@1001events.com", "B10M01S@@");
            //client.Port = 587;
            //client.Host = "smtp.office365.com";
            //client.DeliveryMethod = SmtpDeliveryMethod.Network;
            //client.EnableSsl = true;
            try
            {
                EmailSetting.SendEmail(msg);
                TempData["message"] = "Mail Sending in Background";
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPost]
        public JsonResult RepeatTaskList(int taskid = 0, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                TaskManagerRepository dal = new TaskManagerRepository();
                List<RepeatTaskModel> model = dal.RepeatTaskList(taskid).ToList();
                int count = model.Count;
                List<RepeatTaskModel> Model1 = model.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult UpdateRepeat(RepeatTaskModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                RepeatTaskRepository dal = new RepeatTaskRepository();
                dal.Edit(model);
                return Json(new { Result = "OK", Record = model });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        public string GenrateTaskReport(string empName, string date)
        {
            TaskManagerRepository dal = new TaskManagerRepository();
            List<TaskManagerModel> model = new List<TaskManagerModel>();
            EmployeeRepository e = new EmployeeRepository();
            IEmployee emp = e.GetBy(empName.ToUpper());
            if (string.IsNullOrEmpty(date))
            {
                model = dal.GetByEmployee(empName)
               .Where(x => x.Start.ToShortDateString() == MyExtension.UAETime().ToShortDateString() ||
                   x.RepeatTask.Any(y => y.TaskDate.ToShortDateString() == MyExtension.UAETime().ToShortDateString()))
               .ToList();
            }
            else
            {
                model = dal.GetByEmployee(empName)
                .Where(x => x.Start.ToShortDateString() == Convert.ToDateTime(date).ToShortDateString() ||
                    x.RepeatTask.Any(y => y.TaskDate.ToShortDateString() == Convert.ToDateTime(date).ToShortDateString()))
                .ToList();
            }

            DataTable dt = new DataTable();

            #region -- Table Column Declarition --

            dt.Columns.Add(new DataColumn("SrNo", typeof(string)));
            dt.Columns.Add(new DataColumn("Assigner", typeof(string)));
            dt.Columns.Add(new DataColumn("StartDate", typeof(string)));
            dt.Columns.Add(new DataColumn("Description", typeof(string)));
            dt.Columns.Add(new DataColumn("Notes", typeof(string)));
            dt.Columns.Add(new DataColumn("StartTime", typeof(string)));
            dt.Columns.Add(new DataColumn("EndTime", typeof(string)));
            dt.Columns.Add(new DataColumn("Duration", typeof(string)));
            dt.Columns.Add(new DataColumn("Status", typeof(string)));

            #endregion
            int i = 1;
            foreach (var data in model.OrderBy(x => x.StartTime))
            {
                DataRow dr = dt.NewRow();
                dr["SrNo"] = i;
                dr["Assigner"] = data.Assigneer.EmpName;
                dr["StartDate"] = Convert.ToDateTime(data.Start).ToString("dd MMM yyyy");
                dr["Description"] = data.Task;
                dr["Notes"] = data.Notes;
                dr["StartTime"] = data.StartTime;
                dr["EndTime"] = data.ComplTime;
                try
                {
                    TimeSpan duration = DateTime.Parse(data.ComplTime).Subtract(DateTime.Parse(data.StartTime));
                    dr["Duration"] = duration;
                }
                catch (Exception)
                {
                    dr["Duration"] = 0;
                }

                dr["Status"] = data.Status == 1 ? "Start" : data.Status == 2 ? "Cancelled" : "Completed";
                dt.Rows.Add(dr);
                i++;
            }

            GridView gv = new GridView();
            gv.DataSource = dt;
            gv.DataBind();

            #region -- Report Filter --

            GridViewRow row1 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);
            GridViewRow row2 = new GridViewRow(1, 0, DataControlRowType.Header, DataControlRowState.Insert);

            TableCell cell_1 = new TableCell();
            cell_1.Text = "DAILY TASK REPORTING OF :" + emp.EmpName;
            cell_1.BackColor = Color.Yellow;
            cell_1.ColumnSpan = 9;
            cell_1.HorizontalAlign = HorizontalAlign.Center;
            row1.Cells.Add(cell_1);

            TableCell cell_2 = new TableCell();
            cell_2.Text = "Date:";
            cell_2.BackColor = Color.Yellow;
            row2.Cells.Add(cell_2);

            TableCell cell_22 = new TableCell();
            cell_22.Text = System.DateTime.Now.ToString("dd MMM yyyy");
            cell_22.ColumnSpan = 8;
            row2.Cells.Add(cell_22);

            gv.Controls[0].Controls.AddAt(0, row1);
            gv.Controls[0].Controls.AddAt(1, row2);

            #endregion

            gv.HeaderRow.ForeColor = Color.Blue;
            gv.HeaderRow.Font.Bold = true;

            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            string renderGrid = sw.ToString();
            string filename = System.IO.Path.GetTempPath() + "TaskReport-" + emp.EmpName + ".xls";
            System.IO.File.WriteAllText(filename, renderGrid);
            return filename;

        }

        [Authorize]
        public ActionResult SendTaskReport(string list, string date = null)
        {
            EmployeeRepository e = new EmployeeRepository();
            IEmployee emp = e.GetBy(User.Identity.Name.ToUpper());

            MailMessage msg = new MailMessage();
            string[] mailid = list.Split(',');
            string name = "";
            foreach (var item in mailid)
            {
                IEmployee assinger = e.GetById(Convert.ToInt32(item));
                if (name == "")
                {
                    name = assinger.EmpName;
                }
                else
                {
                    name = name + " / " + assinger.EmpName;
                }
                msg.To.Add(new MailAddress(assinger.MailId));
            }
            msg.CC.Add(new MailAddress(emp.MailId));
            msg.From = new MailAddress(ConfigurationManager.AppSettings["mail"], ConfigurationManager.AppSettings["frmMsgTaskReport"]);
            msg.Subject = emp.EmpName + "-Task Report";
            msg.Body = "<p>Dear " + name + "</p><p>Please find attached task report </p><p> Regards </p>" + emp.EmpName;
            try
            {
                msg.Attachments.Add(new Attachment(GenrateTaskReport(User.Identity.Name, date)));
            }
            catch (Exception) { };

            msg.IsBodyHtml = true;

            try
            {
                EmailSetting.SendEmail(msg);
                TempData["message"] = "Mail Send";
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public JsonResult GetJobDetails(string jobnumber)
        {
            NBORepository dal = new NBORepository();
            var job = dal.GetByFile(jobnumber);
            return Json(new { success = true, results = job }, JsonRequestBehavior.AllowGet);
        }
    }
}
