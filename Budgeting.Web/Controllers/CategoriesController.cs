using Budgeting.Dto;
using Budgeting.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Budgeting.Web.Controllers
{
    public class CategoriesController : Controller
    {
        // GET: Categories
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CategoryList()
        {
            CategoryService s = new CategoryService();
            return PartialView("CategoryList", s.GetUserCategories());
        }

        public ActionResult SaveCategory(CategoryDto c)
        {
            CategoryService s = new CategoryService();
            s.SaveCategory(c);
            return CategoryList();
        }

        public ActionResult DeleteCategory(int categoryId)
        {
            CategoryService s = new CategoryService();
            s.DeleteCategory(categoryId);
            return CategoryList();
        }
    }
}