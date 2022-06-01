using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using ApplicationServices.Web;
using Domain.Interface.Master;
using D.UserInterFace.Models;
using D.UserInterFace.Models.Masters;
using D.UserInterFace.Areas.SCM.Models.Master;

namespace D.UserInterFace.Areas.SCM.Controllers
{
    [HandleError]
    
    public class ClientController : Controller
    {
        static List<ClientModel> modelList = new List<ClientModel>();

        //
        // GET: /Client/

        //[ActionAuthorize]
        public ActionResult Index()
        {
            CountryRepository dal = new CountryRepository();
            ViewBag.Country = new SelectList(dal.GetAll().OrderBy(x => x.Name), "Id", "Name");
            return View();
        }

        public ActionResult ImportClientData()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FileUpload(HttpPostedFileBase uploadFile)
        {
            string filePath = Path.Combine(HttpContext.Server.MapPath("../Uploads"),
                                               Path.GetFileName("Client.csv"));
            string fileText = string.Empty;
            uploadFile.SaveAs(filePath);

            return RedirectToAction("ImportClientData");
        }

        #region ---- Client Information ----

        [HttpPost]
        public JsonResult List(string col = null, string name = null, int countryId = 0, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                ClientRepository dal = new ClientRepository();
                List<ClientModel> model = new List<ClientModel>();
                if (!string.IsNullOrEmpty(col))
                {
                    switch (col)
                    {
                        case "n":
                            if (!string.IsNullOrEmpty(name))
                            {
                                model = dal.GetAll().Where(x => x.Name.ToLower().Contains(name.ToLower())).ToList();
                            }
                            else
                            {
                                model = dal.GetAll().ToList();
                            }
                            break;
                        case "c":
                            if (countryId != 0)
                            {
                                model = dal.GetAll().Where(x => x.Country.Id == countryId).ToList();
                            }
                            else
                            {
                                model = dal.GetAll().ToList();
                            }
                            break;
                        default:
                            model = dal.GetAll().ToList();
                            break;
                    }
                }
                else
                {
                    model = dal.GetAll().ToList();
                }

                int count = model.Count;
                List<ClientModel> Model1 = model.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        //Create new Client
        //[ActionAuthorize]
        [HttpPost]
        public JsonResult Create(ClientModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                ClientRepository dal = new ClientRepository();
                dal.Insert(model);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        //Update Client details
        //[ActionAuthorize]
        [HttpPost]
        public JsonResult Update(ClientModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                ClientRepository dal = new ClientRepository();
                dal.Edit(model);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        #endregion

        [HttpPost]
        public JsonResult GetClientOptions()
        {
            try
            {
                ClientRepository dal = new ClientRepository();
                var list = dal.GetAll().OrderBy(X => X.Name)
                                .Select(c => new { DisplayText = c.Name, Value = c.Id });
                return Json(new { Result = "OK", Options = list });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SelectClientOptions()
        {
            List<ClientModel> model = new List<ClientModel>();
            model.Add(new ClientModel { Id = 0, Name = "* Select *" });
            var data = model.Select(c => new { DisplayText = c.Name, Value = c.Id });
            try
            {
                ClientRepository dal = new ClientRepository();
                var list = dal.GetAll().OrderBy(X => X.Name)
                                .Select(c => new { DisplayText = c.Name, Value = c.Id });
                return Json(new { Result = "OK", Options = list.Concat(data).OrderBy(x => x.DisplayText) });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ActionResult GetClient()
        {
            try
            {
                ClientRepository dal = new ClientRepository();
                var list = from m in dal.GetAll().OrderBy(x => x.Name)
                           select new { Id = m.Id, Name = m.Name };
                return Json(list, "client", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ActionResult GetCountry()
        {
            try
            {
                CountryRepository dal = new CountryRepository();
                var list = from m in dal.GetAll().OrderBy(x => x.Name)
                           select new { Id = m.Id, Name = m.Name };
                return Json(list.Distinct(), "client", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ActionResult GetRepresentation()
        {
            try
            {
                EnquirySourceRepository dal = new EnquirySourceRepository();
                var list = from m in dal.GetAll().OrderBy(x => x.Name)
                           select new { Id = m.Id, Name = m.Name };
                return Json(list.Distinct(), "client", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        // Get country options
        [HttpPost]
        public JsonResult GetCountryOptions(int clientId = 0)
        {
            CountryRepository dal = new CountryRepository();

            try
            {
                if (clientId == 0)
                {
                    var data = dal.GetAll()
                               .Select(c => new { DisplayText = c.Name, Value = c.Id });
                    return Json(new { Result = "OK", Options = data });
                }
                else
                {
                    ClientRepository cl = new ClientRepository();
                    int id = cl.GetById(clientId).Country.Id;
                    var data = dal.GetAll().Where(x => x.Id == id)
                               .Select(c => new { DisplayText = c.Name, Value = c.Id });
                    return Json(new { Result = "OK", Options = data });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult SelectCountryOptions(int clientId = 0)
        {
            CountryRepository dal = new CountryRepository();
            List<CountryModel> model = new List<CountryModel>();
            model.Add(new CountryModel { Id = 0, Name = "* Select *" });
            var data1 = model.Select(c => new { DisplayText = c.Name, Value = c.Id });

            try
            {
                if (clientId == 0)
                {
                    var data = dal.GetAll()
                               .Select(c => new { DisplayText = c.Name, Value = c.Id });
                    return Json(new { Result = "OK", Options = data.Concat(data1).OrderBy(x => x.DisplayText) });
                }
                else
                {
                    ClientRepository cl = new ClientRepository();
                    int id = cl.GetById(clientId).Country.Id;
                    var data = dal.GetAll().Where(x => x.Id == id)
                               .Select(c => new { DisplayText = c.Name, Value = c.Id });
                    return Json(new { Result = "OK", Options = data.Concat(data1).OrderBy(x => x.DisplayText) });
                }
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ActionResult ExportData()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add(new DataColumn("Id", typeof(int)));
            dt.Columns.Add(new DataColumn("Client", typeof(string)));
            dt.Columns.Add(new DataColumn("CountryId", typeof(int)));
            dt.Columns.Add(new DataColumn("Country", typeof(string)));
            dt.Columns.Add(new DataColumn("Client Group", typeof(string)));
            dt.Columns.Add(new DataColumn("Remarks", typeof(string)));

            GridView gv = new GridView();
            ClientRepository dal = new ClientRepository();
            foreach (var item in dal.GetAll())
            {
                DataRow dr = dt.NewRow();
                dr["Id"] = item.Id;
                dr["Client"] = item.Name;
                dr["CountryId"] = item.Country.Id;
                dr["Country"] = item.Country.Name;
                dr["Client Group"] = item.ClientGroup;
                dr["Remarks"] = item.ClientGroup;
                dt.Rows.Add(dr);
            }

            gv.DataSource = dt;
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=Client.xls");

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

       
        public ActionResult Import()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult ImportClient(HttpPostedFileWrapper imageFile)
        {

            var fileName = "ImportClient.csv";
            var imagePath = Path.Combine(Server.MapPath(Url.Content("~/Upload/")), fileName);
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            imageFile.SaveAs(imagePath);
            List<ClientModel> model = new List<ClientModel>();
            string filename = imagePath;
            using (var reader = new StreamReader(System.IO.File.OpenRead(filename)))
            {
                int ii = 1;

                while (!reader.EndOfStream)
                {
                    ClientRepository dal = new ClientRepository();
                    CountryRepository cont = new CountryRepository();

                    ClientModel bl = new ClientModel();
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    var data = dal.GetAll().Where(x => x.Name == values[1] && x.Country.Id == Convert.ToInt32(values[2])).ToList();
                    if (data.Count > 0)
                    {
                        bl.dup = 1;
                    }
                    bl.Id = ii;
                    bl.Name = values[1].ToString();
                    bl.ClientGroup = values[4].ToString();
                    try
                    {
                        ICountry c = cont.GetById(Convert.ToInt32(values[2]));
                        bl.Country = c;

                    }
                    catch (Exception) { reader.Close(); }

                    model.Add(bl);
                    ii++;
                }
            }
            modelList = model.ToList();
            return View();
        }

        [HttpPost]
        public JsonResult ImportClientList(int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                List<ClientModel> Model = modelList.ToList();
                int count = Model.Count;
                List<ClientModel> Model1 = Model.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.InnerException.Message });
            }
        }

        public ActionResult ProcessImport()
        {
            ClientRepository dal = new ClientRepository();
            dal.InsertBulk(modelList);
            return RedirectToAction("Index");
        }



    }
}
