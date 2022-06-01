using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ApplicationServices.Web;
using D.UserInterFace.Models;
using D.UserInterFace.Models.Masters;
using D.UserInterFace.Areas.SCM.Models.Master;

namespace D.UserInterFace.Areas.SCM.Controllers
{
    [HandleError]
    
    public class SupplierController : Controller
    {
        //
        // GET: /Supplier/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult List(string name = null, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                SupplierRepository dal = new SupplierRepository();
                List<SupplierModel> model = new List<SupplierModel>();
                if (string.IsNullOrEmpty(name))
                {
                    model = dal.GetAll().ToList();
                }
                else
                {
                    model = dal.GetAll().Where(x => x.Name.Contains(name)).ToList();
                }
                int count = model.Count;
                List<SupplierModel> Model1 = model.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        //Create new Supplier
       
        [HttpPost]
        public JsonResult Create(SupplierModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                SupplierRepository dal = new SupplierRepository();
                dal.Insert(model);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        //Update Supplier details
       
        [HttpPost]
        public JsonResult Update(SupplierModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                SupplierRepository dal = new SupplierRepository();
                dal.Edit(model);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetSupplierOptions()
        {
            try
            {
                SupplierRepository dal = new SupplierRepository();
                var list = dal.GetAll()
                                .Select(c => new { DisplayText = c.Name, Value = c.Id });
                return Json(new { Result = "OK", Options = list });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ActionResult GetSupplier()
        {
            try
            {
                SupplierRepository dal = new SupplierRepository();
                var list = from m in dal.GetAll()
                           select new { Id = m.Id, Name = m.Name };
                return Json(list, "client", JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetHotelName()
        {
            try
            {
                SupplierRepository dal = new SupplierRepository();
                var list = dal.GetAll().Where(x => x.Category.Name.ToLower().StartsWith("hotel"))
                                .Select(c => new { DisplayText = c.Name, Value = c.Id });
                return Json(new { Result = "OK", Options = list });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

    }
}
