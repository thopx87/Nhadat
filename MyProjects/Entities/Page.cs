using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Entities
{
    public class Page
    {

    }

    public class PageMaster
    {
        public int Id { get; set; }
        public string Text { get; set; }
    }

    public class PageCategory
    {
        public int Id { get; set; }
        public int PageId { get; set; }
        public int CategoryId { get; set; }

        // More
        public string PageMasterText { get; set; }
        public string CategoryText { get; set; }
    }

    public class PageArticle
    {
        public int Id { get; set; }
        public int PageId { get; set; }
        public int ArticleId { get; set; }

        // More
        public string PageMasterText { get; set; }
        public string ArticleText { get; set; }
    }
}
