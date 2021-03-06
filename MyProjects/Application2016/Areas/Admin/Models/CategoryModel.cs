using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Application2016.Areas.Admin.Models
{
    public class CategoryModel
    {
        public Entities.Category Category { get; set; }

        // Lấy ra danh sách cài đặt trước đó để có thể làm thư mục cha.
        public List<Entities.Item> ListCategory { get; set; }

        public List<Entities.Item> ListCategoryType { get; set; }
    }

    public class ListCategoryModel
    {
        public List<Entities.Category> ListCategory { get; set; }
        public CategoryCondition Condition { get; set; }
    }

    public class CategoryCondition: BaseModel
    {

    }
}