using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Application2016.Areas.Admin.Models
{
    public class CostModel
    {
        public Entities.CostSetting CostSetting { get; set; }
        public List<Entities.Cost> ListCost { get; set; }

        // More information
        
        public List<Entities.Item> ListCity { get; set; }
        public List<Entities.Item> ListDistrict { get; set; }
        public List<Entities.Item> ListRegion { get; set; }
        public List<Entities.Item> ListWard { get; set; }

        public List<Entities.Item> ListProductType { get; set; }
        public List<Entities.Item> ListRole { get; set; }
        public List<Entities.Item> ListUser { get; set; }

        public List<Entities.Item> ListCostMaster { get; set; }
        public List<Entities.Item> ListTransactionType { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        public CostModel()
        {
            CostSetting = new Entities.CostSetting();
            ListCost = new List<Entities.Cost>();
            ListCity = new List<Entities.Item>();
            ListDistrict = new List<Entities.Item>();
            ListRegion = new List<Entities.Item>();
            ListWard = new List<Entities.Item>();
            ListProductType = new List<Entities.Item>();
            ListRole = new List<Entities.Item>();
            ListUser = new List<Entities.Item>();
            ListCostMaster = new List<Entities.Item>();
            ListTransactionType = new List<Entities.Item>();
        }
    }

    public class ListCostModel
    {
        public List<Entities.CostSetting> ListCostSetting { get; set; }
        public CostCondition Condition { get; set; }
    }

    public class CostCondition : BaseModel
    {
        public int ProductType { get; set; }
        public int TransactionType { get; set; }

        public List<Entities.Item> ListProductType { get; set; }
        public List<Entities.Item> ListTransactionType { get; set; }

        public CostCondition()
        {
            ListProductType = new List<Entities.Item>();
            ListTransactionType = new List<Entities.Item>();
        }
    }
}