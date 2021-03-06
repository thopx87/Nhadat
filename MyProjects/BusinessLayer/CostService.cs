using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities;
using BusinessLayer.Helpers;

namespace BusinessLayer
{
    public class CostService: BaseService<Entities.Cost>
    {
        public CostService() : base() { }

        private string className { get { return this.GetType().Name; } }

        #region CodeSetting
        public Entities.CostSetting GetById(int Id)
        {
            var e = (from c in Context.CostSettings
                     where c.Id == Id
                     select new Entities.CostSetting
                     {
                         Id = c.Id,
                         SettingName = c.SettingName,
                         Mode = c.Mode,
                         CityId = c.CityId??-1,
                         DistrictId = c.DistrictId??-1,
                         RegionId = c.RegionId??-1,
                         WardId = c.WardId??-1,
                         TransactionType = c.Transaction_Type??-1,
                         ProductType = c.Product_Type??-1,
                         ProjectCode = c.ProjectCode,
                         UserRole = c.UserRole??-1,
                         UserIds = c.UserIds,
                         TimeStart = c.TimeStart,
                         TimeEnd = c.TimeEnd,
                         IsApply = c.IsApply
                     }).FirstOrDefault();
            return e;
        }

        public int SaveCostSetting(Entities.CostSetting e, int userId, bool isSubmit = true)
        {
            DataLayer.CostSetting cost = (from c in Context.CostSettings
                                          where c.Id == e.Id
                                          select c).FirstOrDefault();
            bool isNew = false;
            if (cost == null)
            {
                cost = new DataLayer.CostSetting();
                isNew = true;
            }
            cost.Id = e.Id;
            cost.SettingName = e.SettingName;
            cost.Mode = e.Mode;
            cost.CityId = e.CityId;
            cost.DistrictId = e.DistrictId;
            cost.RegionId = e.RegionId;
            cost.WardId = e.WardId;
            cost.Transaction_Type = e.TransactionType;
            cost.Product_Type = e.ProductType;
            cost.ProjectCode = e.ProjectCode;
            cost.UserRole = e.UserRole;
            cost.UserIds = e.UserIds;
            cost.TimeStart = e.TimeStart;
            cost.TimeEnd = e.TimeEnd;
            cost.IsApply = e.IsApply;
            cost.UpdateBy = userId;
            cost.UpdateTime = DateTime.Now;
            if (isNew)
            {
                Context.CostSettings.InsertOnSubmit(cost);
            }
            if (isSubmit)
            {
                Context.SubmitChanges();
            }
            return cost.Id;
        }

        public List<CostSetting> ListCostSetting()
        {
            List<CostSetting> lst = new List<CostSetting>();
            lst = (from c in Context.CostSettings
                   select new Entities.CostSetting
                     {
                         Id = c.Id,
                         SettingName = c.SettingName,
                         Mode = c.Mode,
                         CityId = c.CityId??-1,
                         DistrictId = c.DistrictId??-1,
                         RegionId = c.RegionId??-1,
                         WardId = c.WardId??-1,
                         TransactionType = c.Transaction_Type??-1,
                         ProductType = c.Product_Type??-1,
                         ProjectCode = c.ProjectCode,
                         UserRole = c.UserRole??-1,
                         UserIds = c.UserIds,
                         TimeStart = c.TimeStart,
                         TimeEnd = c.TimeEnd,
                         IsApply = c.IsApply
                     }).ToList();
            return lst;
        }

        public List<CostSetting> ListCostSetting(string strName, int transactionType, int productType, int pageIndex, int pageSize, int state, out int total)
        {
            List<CostSetting> lst = new List<CostSetting>();
            if (string.IsNullOrEmpty(strName))
            {
                strName = "";
            }
            total = 0;
            total = (from c in Context.CostSettings
                     where (c.SettingName.ToString().Contains(strName))
                     && (state == 1 ? c.IsApply == true : state == 2 ? c.IsApply == false : true)
                     && (transactionType >0? c.Transaction_Type== transactionType:true)
                     && (productType>0? c.Product_Type == productType:true)
                     select c.Id).Count();
            lst = (from c in Context.CostSettings
                   where (c.SettingName.ToString().Contains(strName))
                     && (state == 1 ? c.IsApply == true : state == 2 ? c.IsApply == false : true)
                     && (transactionType > 0 ? c.Transaction_Type == transactionType : true)
                     && (productType > 0 ? c.Product_Type == productType : true)
                   select new Entities.CostSetting
                   {
                       Id = c.Id,
                       SettingName = c.SettingName,
                       Mode = c.Mode,
                       CityId = c.CityId ?? -1,
                       DistrictId = c.DistrictId ?? -1,
                       RegionId = c.RegionId ?? -1,
                       WardId = c.WardId ?? -1,
                       TransactionType = c.Transaction_Type ?? -1,
                       ProductType = c.Product_Type ?? -1,
                       ProjectCode = c.ProjectCode,
                       UserRole = c.UserRole ?? -1,
                       UserIds = c.UserIds,
                       TimeStart = c.TimeStart,
                       TimeEnd = c.TimeEnd,
                       IsApply = c.IsApply
                   }).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return lst;
        }


        public List<Cost> List(Entities.Cost e)
        {
            return null;
        }

        public int GetMaxId()
        {
            var res = Context.CostSettings.OrderByDescending(c => c.Id).Select(c=>c.Id).FirstOrDefault();
            if (res == null|| res ==0)
            {
                return 1;
            }
            else
            {
                return res + 1;
            }
        }

        #endregion

        #region Code Master
        public List<Entities.Item> ListCostMaster()
        {
            var result = (from c in Context.CostMasters
                          select new Entities.Item()
                          {
                              Id = c.Id,
                              Text = c.Text
                          }).ToList();

            return result;
        }
        #endregion

        #region Cost
        public List<Entities.Cost> ListCost(int costSetting)
        {
            var result = (from c in Context.Costs
                          where c.CostSettingId == costSetting
                          select new Entities.Cost()
                          {
                             CostMasterId = c.CostMasterId,
                             CostSettingId = c.CostSettingId,
                             CostMasterText = c.CostMasterText,
                             IsApply = c.IsApply,
                             SortOrder = c.SortOrder,
                             MoneyCode1 = c.MoneyCode1,
                             MoneyCode2 = c.MoneyCode2,
                             MoneyCode3 = c.MoneyCode3
                          }).ToList();
            return result;
        }

        public void SaveCost(Entities.Cost cost, bool isSubmit = true)
        {
            DataLayer.Cost dbCost = (from c in Context.Costs
                                     where c.CostSettingId == cost.CostSettingId && c.CostMasterId == cost.CostMasterId
                                     select c).FirstOrDefault();
            bool isNew = false;
            if (dbCost == null)
            {
                dbCost = new DataLayer.Cost();
                isNew = true;
            }
            dbCost.CostSettingId = cost.CostSettingId;
            dbCost.CostMasterId = cost.CostMasterId;
            dbCost.CostMasterText = cost.CostMasterText;
            dbCost.IsApply = cost.IsApply;
            dbCost.SortOrder = cost.SortOrder;
            dbCost.MoneyCode1 = cost.MoneyCode1;
            dbCost.MoneyCode2 = cost.MoneyCode2;
            dbCost.MoneyCode3 = cost.MoneyCode3;
            if (isNew)
            {
                Context.Costs.InsertOnSubmit(dbCost);
            }
            if (isSubmit)
            {
                Context.SubmitChanges();
            }
        }

        public void SaveCost(List<Entities.Cost> listCost, bool isSubmit = false)
        {
            foreach (Entities.Cost cost in listCost)
            {
                SaveCost(cost, isSubmit);
            }
            if (isSubmit)
            {
                Context.SubmitChanges();
            }
        }
        #endregion

        public int Save(Entities.CostSetting costSetting, List<Entities.Cost> listCost, int userId)
        {
            try
            {
                int id = 0;
                if (costSetting.Id <= 0)
                {
                    id = GetMaxId();
                    costSetting.Id = id;
                    listCost.Select(c => { c.CostSettingId = id; return c; }).ToList();
                }
                SaveCostSetting(costSetting, userId, false);

                SaveCost(listCost, false);

                Context.SubmitChanges();
            }
            catch(Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.EXIST;
            }
            finally
            {

            }

            return costSetting.Id;
        }
    }
}
