using Budgeting.Dto;
using Budgeting.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Budgeting.Web.Controllers
{
    public class ActivityController : Controller
    {
        // GET: Activity
        public ActionResult Index()
        {
            return View("Index", GetBlankActivity());
        }

        public ActionResult SaveActivity(ActivityDto dto)
        {
            if (ModelState.IsValid)
            {
                ActivityService s = new ActivityService();
                s.SaveActivity(dto);
                //return PartialView("Index", GetBlankActivity());
                return RedirectToAction("Index");
            }
            //return PartialView("Index", dto);
            return View("Index", dto);
        }

        private ActivityDto GetBlankActivity()
        {
            ActivityDto a = new ActivityDto();
            LookupService s = new LookupService();
            a.CategoryOptions = s.GetCategoryList(0, false);
            a.CategoryOptions.Add(new LookupDto { Id = -1, Name = "Category" });
            a.DateOfActivity = DateTime.Now.Date;
            return a;
        }
    }
}