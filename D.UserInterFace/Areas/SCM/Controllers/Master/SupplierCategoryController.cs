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
    
    public class SupplierCategoryController : Controller
    {
        //
        // GET: /SupplierCategory/

        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult List(string name = null, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                SupplierCategoryRepository dal = new SupplierCategoryRepository();
                List<SupplierCategoryModel> model = new List<SupplierCategoryModel>();
                if (string.IsNullOrEmpty(name))
                {
                    model = dal.GetAll().ToList();
                }
                else
                {
                    model = dal.GetAll().Where(x => x.Name.Contains(name)).ToList();
                }
                int count = model.Count;
                List<SupplierCategoryModel> Model1 = model.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        //Create new Supplier
       
        [HttpPost]
        public JsonResult Create(SupplierCategoryModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                SupplierCategoryRepository dal = new SupplierCategoryRepository();
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
        public JsonResult Update(SupplierCategoryModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                SupplierCategoryRepository dal = new SupplierCategoryRepository();
                dal.Edit(model);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetCatOptions()
        {
            try
            {
                SupplierCategoryRepository dal = new SupplierCategoryRepository();
                var list = dal.GetAll()
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
