using Budgeting.Data;
using Budgeting.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Budgeting.Service
{
    public class CategoryService : BaseService
    {
        public List<CategoryDto> GetUserCategories()
        {
            List<CategoryDto> list = new List<CategoryDto>();
            using (BudgetingEntities db = new BudgetingEntities())
            {
                list = (from c in db.Categories
                        where c.UserId == UserId
                        select new CategoryDto
                        {
                            CategoryId = c.CategoryId,
                            Name = c.Name
                        })
                        .ToList();
            }
            return list;
        }

        public void SaveCategory(CategoryDto dto)
        {
            using (BudgetingEntities db = new BudgetingEntities())
            {
                Category c = new Category();
                c.Name = dto.Name;
                c.UserId = UserId;
                if (dto.CategoryId == 0)
                {
                    // add new
                    db.Categories.Add(c);
                    db.Entry(c).State = System.Data.Entity.EntityState.Added;
                }
                else
                {
                    // update existing
                    c.CategoryId = dto.CategoryId;
                    db.Entry(c).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();
            }
        }

        public void DeleteCategory(int categoryId)
        {
            using (BudgetingEntities db = new BudgetingEntities())
            {
                Category c = (from cat in db.Categories
                                 where cat.CategoryId == categoryId
                                 select cat)
                                 .Single();
                db.Categories.Remove(c);
                db.SaveChanges();
                // TODO catch errors
            }
        }
    }
}
