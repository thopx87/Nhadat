using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Application2016.Areas.Admin.Models
{
    public class ArticleModel
    {
        public Entities.Article Article { get; set; }
        public List<Entities.Item> ListCategory { get; set; }
    }
    public class ListArticleModel
    {
        public List<Entities.Article> ListArticle { get; set; }
        public List<Entities.Item> ListCategory { get; set; }
        public ArticleCondition Condition { get; set; }        
    }
    public class ArticleCondition : BaseModel
    {
        public bool DisplayOnly { get; set; }
        public int CatId { get; set; }

        public ArticleCondition()
        {
            DisplayOnly = true;
        }
    }
}