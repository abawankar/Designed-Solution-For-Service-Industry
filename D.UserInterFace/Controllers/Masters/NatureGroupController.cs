using D.UserInterFace.Areas.SCM.Models.Master;
using D.UserInterFace.Models.Masters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace D.UserInterFace.Controllers
{
    public class NatureGroupController : Controller
    {
        //
        // GET: /NatureGroup/

        public ActionResult Index()
        {
            return PartialView();
        }

        [HttpPost]
        public JsonResult List(string name = null, int jtStartIndex = 0, int jtPageSize = 0, string jtSorting = null)
        {
            try
            {
                NatureGroupRepository dal = new NatureGroupRepository();
                List<NatureGroupModel> model = new List<NatureGroupModel>();
                if (string.IsNullOrEmpty(name))
                {
                    model = dal.GetAll().ToList();
                }
                else
                {
                    model = dal.GetAll().Where(x => x.Name.Contains(name)).ToList();
                }
                int count = model.Count;
                List<NatureGroupModel> Model1 = model.Skip(jtStartIndex).Take(jtPageSize).ToList();
                return Json(new { Result = "OK", Records = Model1, TotalRecordCount = count });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Create(NatureGroupModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                NatureGroupRepository dal = new NatureGroupRepository();
                dal.Insert(model);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult Update(NatureGroupModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return Json(new { Result = "ERROR", Message = "Form is not valid! Please correct it and try again." });
                }
                NatureGroupRepository dal = new NatureGroupRepository();
                dal.Edit(model);
                return Json(new { Result = "OK", Record = model });

            }
            catch (Exception ex)
            {
                return Json(new { Result = "Error", Message = ex.Message });
            }
        }

        [HttpPost]
        public JsonResult GetBusinessNature(int groupid = 0)
        {
            NatureGroupRepository dal = new NatureGroupRepository();
            try
            {
                var data = dal.GetById(groupid).NatureName.ToList();

                return Json(new { Result = "OK", Records = data });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ActionResult NatureList()
        {
            NatureGroupRepository dal = new NatureGroupRepository();
            ViewBag.natureid = new SelectList(dal.GetAll(), "Id", "Name");
            return PartialView();
        }

        [HttpPost]
        public JsonResult ListOfNature(int id = 0)
        {
            NatureGroupRepository dal = new NatureGroupRepository();
            BusinessNatureRepository naturedal = new BusinessNatureRepository();
            try
            {
                var nature = dal.GetAll().Select(x => x.NatureName).ToList();
                var natureid = from c in nature
                               from s in c
                               select s.Id;

                var list = naturedal.GetAll().Where(c => !natureid.Contains(c.Id)).ToList();

                return Json(new { Result = "OK", Records = list });
            }
            catch (Exception ex)
            {
                return Json(new { Result = "ERROR", Message = ex.Message });
            }
        }

        public ActionResult AddBusinessNature(string id, string list)
        {
            int groupid = Convert.ToInt32(id);
            NatureGroupRepository.AddBusinessNature(groupid, list);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public JsonResult GetNatureGroupOptions()
        {
            try
            {
                NatureGroupRepository dal = new NatureGroupRepository();
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
