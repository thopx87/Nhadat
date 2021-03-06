using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLayer.Helpers
{
    public class InitData
    {
        public List<Entities.CostMaster> ListCostMaster()
        {
            List<Entities.CostMaster> listCost = new List<Entities.CostMaster>();
            listCost.Add(new Entities.CostMaster() { Id = 1, Text = "Một vùng gửi", Code = "OneSend" });
            listCost.Add(new Entities.CostMaster() { Id = 2, Text = "Một vùng nhận", Code = "OneReceive" });
            listCost.Add(new Entities.CostMaster() { Id = 3, Text = "Sửa giá lên", Code = "ChangeUp" });
            listCost.Add(new Entities.CostMaster() { Id = 4, Text = "Sửa giá xuống", Code = "ChangeDown" });
            listCost.Add(new Entities.CostMaster() { Id = 5, Text = "Phí đăng ký", Code = "Registry" });
            listCost.Add(new Entities.CostMaster() { Id = 6, Text = "Phí duy trì", Code = "HoldOn" });
            listCost.Add(new Entities.CostMaster() { Id = 7, Text = "Tin chính chủ", Code = "OwnerArticle" });
            listCost.Add(new Entities.CostMaster() { Id = 8, Text = "Tin đăng", Code = "Post" });
            return listCost;
        }
    }
}
