using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Application2016.Models
{
    public class ArticleModel
    {
        public Entities.Article Article { get; set; }
        public List<Entities.Item> ListCategory { get; set; }
    }
}