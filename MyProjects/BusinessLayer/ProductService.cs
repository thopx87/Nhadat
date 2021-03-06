using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Entities;
using DataLayer;
using BusinessLayer.Helpers;
using System.Data.Linq;

namespace BusinessLayer
{
    public class ProductService:BaseService<Entities.Product>
    {
        private string className { get { return this.GetType().Name; } }
        public ProductService() : base() { }

        public int Save(Entities.Product e)
        {
            DataLayer.Product p = (from x in Context.Products
                                   where x.Id == e.Id
                                   select x).FirstOrDefault();
            bool isNew = false;
            if (p == null)
            {
                p = new DataLayer.Product();
                isNew = true;
            }
            p.UserId = e.UserId;
            p.Transaction_Type = e.Transaction_Type;
            p.Product_Type = e.Product_Type;
            p.CityId = e.CityId;
            p.DistrictId = e.DistrictId;
            p.WardId = e.WardId;
            p.Text = e.Text;
            p.Text_Alias = e.Text_Alias;
            p.HouseNumber = e.HouseNumber;
            p.Address = e.Address;
            p.Area = e.Area;
            p.Description = e.Description != null ? e.Description : "";
            p.StandardCost = e.StandardCost;
            p.SellStartDate = e.SellStartDate;
            p.SellEndDate = e.SellEndDate;
            p.State = e.State;
            p.AgencyCost = e.AgencyCost;
            p.CostUnit = e.CostUnit;
            p.Tag = e.Tag;
            p.UpdateCost = e.UpdateCost;
            p.UpdateBy = e.UpdateBy;
            p.UpdateTime = DateTime.Now;
            p.UserName = e.UserName;
            p.RoleId = e.RoleId;
            p.RoleText = e.RoleText;
            try
            {
                if (isNew)
                {
                    Context.Products.InsertOnSubmit(p);
                }
                Context.SubmitChanges();
            }
            catch (Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.INSERT_ERROR;
            }
            return p.Id;
        }

        public int UpdateAvatar(int productId, string avatar, string avatar_url)
        {
            DataLayer.Product p = (from x in Context.Products
                                   where x.Id == productId
                                   select x).FirstOrDefault();
            if (p != null)
            {
                p.Avatar = avatar;
                p.Avatar_Url = avatar_url;
                try
                {
                    Context.SubmitChanges();
                    return p.Id;
                }
                catch (Exception ex)
                {
                    string data = className + ex.Message.ToString();
                    Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                    return (int)Enums.Errors.UPDATE_ERROR;
                }
            }
            return (int)Enums.Errors.UPDATE_ERROR;
        }

        public int Delete(int entityId, int userId)
        {
            try
            {
                DataLayer.Product p = (from u in Context.Products
                                           where u.Id == entityId
                                           select u).FirstOrDefault();
                if (p != null)
                {
                    //Context.Products.DeleteOnSubmit(p);
                    p.Delete_Flag = true;
                    p.DeleteBy = userId;
                    p.DeleteDate = DateTime.Now;
                    Context.SubmitChanges();
                    return entityId;
                }
                else
                {
                    return (int)Enums.Errors.NOT_EXIST;
                }

            }
            catch(Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.DELETE_ERROR;
            }
        }

        public Entities.Product GetById(int entityId)
        {
            var result = (from p in Context.Products
                     where p.Id == entityId
                     join placeCity in Context.Places on p.CityId equals placeCity.Id
                     join placeDistrict in Context.Places on p.DistrictId equals placeDistrict.Id
                     select new Entities.Product
                     {
                        Id = p.Id,
                        UserId = p.UserId,
                        Transaction_Type = p.Transaction_Type,
                        Product_Type = p.Product_Type,
                        CityId = p.CityId,
                        CityText = placeCity.Text,
                        DistrictId = p.DistrictId,
                        DistrictText = placeDistrict.Text,
                        WardId = p.WardId,
                        Text = p.Text,
                        Text_Alias = p.Text_Alias,
                        HouseNumber = p.HouseNumber,
                        Address = p.Address,
                        Area = p.Area,
                        Description = p.Description,
                        StandardCost = p.StandardCost,
                        SellStartDate = p.SellEndDate,
                        SellEndDate = p.SellEndDate,
                        State = p.State,
                        AgencyCost = p.AgencyCost,
                        CostUnit = p.CostUnit,
                        Tag = p.Tag,
                        Avatar = p.Avatar,
                        Avatar_Url = p.Avatar_Url,
                        UpdateCost = p.UpdateCost,
                        UpdateBy = p.UpdateBy,
                        UpdateTime = p.UpdateTime,
                        UserName = p.UserName,
                        RoleId = p.RoleId,
                        RoleText = p.RoleText
                     }).FirstOrDefault();
            return result;
        }

        public List<Entities.Product> GetByIds(List<int> productIds)
        {
            var result = (from p in Context.Products
                          join district in Context.Places on p.DistrictId equals(district.Id)
                          where productIds.Contains(p.Id)
                          && (p.Delete_Flag == null || p.Delete_Flag == false)
                          select new Entities.Product
                          {
                              Id = p.Id,
                              UserId = p.UserId,
                              Transaction_Type = p.Transaction_Type,
                              Product_Type = p.Product_Type,
                              CityId = p.CityId,
                              DistrictId = p.DistrictId,
                              DistrictText = district.Text,
                              WardId = p.WardId,
                              Text = p.Text,
                              Text_Alias = p.Text_Alias,
                              HouseNumber = p.HouseNumber,
                              Address = p.Address,
                              Area = p.Area,
                              Description = p.Description,
                              StandardCost = p.StandardCost,
                              SellStartDate = p.SellEndDate,
                              SellEndDate = p.SellEndDate,
                              State = p.State,
                              AgencyCost = p.AgencyCost,
                              CostUnit = p.CostUnit,
                              Tag = p.Tag,
                              Avatar = p.Avatar,
                              Avatar_Url = p.Avatar_Url,
                              UpdateCost = p.UpdateCost,
                              UpdateBy = p.UpdateBy,
                              UpdateTime = p.UpdateTime,
                              UserName = p.UserName,
                              RoleId = p.RoleId,
                              RoleText = p.RoleText,
                              ListRegionSelling = TextRegionByProduct(p.Id)
                          }).ToList();
            return result;
        }

        /// <summary> Lấy email người bán sản phẩm
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public string GetEmailByProductId(int productId)
        {
            var result = (from p in Context.Products
                          join u in Context.Users on p.UserId equals u.Id
                          where p.Id == productId
                          select u.Email).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// List Product Using Stored procedure
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        /// <param name="transactionType">
        /// 1 --> Bán
        /// 2 --> Cho thuê
        /// </param>
        /// <param name="productType">
        /// 1	Biệt thự liền kề
        /// 2	Chung cư
        /// 3	Đất thổ cư
        /// 4	Nhà mặt phố
        /// 5	Đất dịch vụ
        /// 6	Đất đấu giá
        /// </param>
        /// <param name="cityId"></param>
        /// <param name="districtId"></param>
        /// <param name="wardId"></param>
        /// <param name="text"></param>
        /// <param name="minCost"></param>
        /// <param name="maxCost"></param>
        /// <param name="sellFrom"></param>
        /// <param name="sellTo"></param>
        /// <param name="orderBy"></param>
        /// <param name="page"></param>
        /// <param name="recordNum"></param>
        /// <returns></returns>
        public List<Entities.Product> ListOld(int id = 0, int userId = 0, short transactionType = 1, int productType = 0, int cityId = 0, int districtId = 0, int wardId = 0,
                                            string text = "", long minCost = 0, long maxCost = 0, string sellFrom = "", string sellTo = "", string orderBy = "TIME_DESC",
                                            int page = 1, int recordNum = 10)
        {
            // Test SP: Exec GET_PRODUCTS 0,0,1,0,0,0,0,'',0,0,'','','',1,10
            var lstRecordDB = Context.GET_PRODUCTS(id, userId, transactionType, productType, cityId, districtId, wardId, text, minCost, maxCost, sellFrom, sellTo, orderBy, page, recordNum).ToList();
            if (lstRecordDB.Count() <= 0)
            {
                return new List<Entities.Product>();
            }
            var result = (from x in lstRecordDB
                          select new Entities.Product()
                          {
                              Id = x.Id.Value,
                              UserId = x.UserId.Value,
                              Transaction_Type = x.Transaction_Type.Value,
                              CityId = x.CityId.Value,
                              DistrictId = x.DistrictId.Value,
                              WardId = x.WardId.Value,
                              Text = x.Text,
                              HouseNumber = x.HouseNumber,
                              Address = x.Address,
                              Area = x.Area.Value,
                              Description = x.Description,
                              StandardCost = x.StandardCost.Value,
                              SellStartDate = x.SellStartDate,
                              SellEndDate = x.SellEndDate,
                              State = x.State,
                              AgencyCost = x.AgencyCost,
                              CostUnit = x.CostUnit,
                              Tag = x.Tag != null ? x.Tag : "",
                              Approval_Flag = x.Approval_Flag != null ? x.Approval_Flag.Value : false,
                              ApprovalBy = x.ApprovalBy != null ? x.ApprovalBy.Value : 0,
                              ApprovalDate = x.ApprovalDate,
                              Avatar = x.Avatar,
                              Avatar_Url = x.Avatar_Url,
                              UpdateCost = x.UpdateCost,
                              UpdateBy = x.UpdateBy,
                              UpdateTime = x.UpdateTime,
                              DistrictText = x.DistrictText,
                              PTypeText = x.PTypeText,
                              UserName = x.UserName,
                              RoleId = x.RoleId,
                              RoleText = x.RoleText,  
                              Total = x.Total
                              
                          }).ToList();
            return result;
        }

        public List<Entities.Product> List(short state = 0, bool delete = false, int id = 0, int withOutId = 0, int userId = 0, short transactionType = 1, int productType = 0, int cityId = 0, int districtId = 0, int wardId = 0,
                                            string text = "", decimal minArea = 0, decimal maxArea=0, long minCost = 0, long maxCost = 0, string sellFrom = "", string sellTo = "", string orderBy = "TIME_DESC",
                                            int page = 1, int recordNum = 10)
        {

            DateTime sellFromDate = sellFrom != "" ? DateTime.ParseExact(sellFrom, "dd/MM/yyyy", null) : DateTime.Now.AddYears(-1);
            DateTime sellToDate = sellTo != "" ? DateTime.ParseExact(sellTo, "dd/MM/yyyy", null): DateTime.Now;
            if (text == null)
            {
                text = "";
            }
            var query = (from product in Context.Products
                          join pType in Context.ProductTypes on product.Product_Type equals pType.Id
                          join placeCity in Context.Places on product.CityId equals placeCity.Id
                          join placeDistrict in Context.Places on product.DistrictId equals placeDistrict.Id
                          where true
                            && (state == 1 ? product.State == 1 : product.State == null) // Trạng thái sản phẩm: 1 --> đã bán.
                            && (delete ? product.Delete_Flag == true : product.Delete_Flag == null || product.Delete_Flag == false)
                            && (id > 0 ? product.Id == id : true)
                            && (withOutId >0? product.Id != withOutId: true)
                            && (userId > 0 ? product.UserId == userId : true)
                            && (transactionType > 0 ? product.Transaction_Type == transactionType : true)
                            && (productType > 0 ? product.Transaction_Type == productType : true)
                            && (cityId > 0 ? product.CityId == cityId : true)
                            && (districtId > 0 ? product.DistrictId == districtId : true)
                            && (wardId > 0 ? product.WardId == wardId : true)
                            && (!string.IsNullOrEmpty(text) ? product.Text.Contains(text) : true)
                            && (minArea > 0 ? product.Area >= minArea : true)
                            && (maxArea > 0 ? product.Area <= maxArea : true)
                            && (minCost > 0 ? product.UpdateCost >= minCost : true)
                            && (maxCost > 0 ? product.UpdateCost <= maxCost : true)
                            && (sellFrom != "" ? product.SellStartDate >= sellFromDate : true)
                            && (sellTo != "" ? product.SellEndDate <= sellToDate : true)
                          orderby product.UpdateTime descending
                          select new Entities.Product()
                          {
                              Id = product.Id,
                              UserId = product.UserId,
                              Transaction_Type = product.Transaction_Type,
                              Product_Type = product.Product_Type,
                              PTypeText = pType.Text,
                              CityId = product.CityId,
                              CityText = placeCity.Text,
                              DistrictId = product.DistrictId,
                              DistrictText = placeDistrict.Text,
                              WardId = product.WardId,
                              Text = product.Text,
                              Text_Alias = product.Text_Alias,
                              HouseNumber = product.HouseNumber,
                              Address = product.Address,
                              Area = product.Area,
                              Description = product.Description,
                              StandardCost = product.StandardCost,
                              SellStartDate = product.SellStartDate !=null? product.SellStartDate: DateTime.Now,
                              SellEndDate = product.SellEndDate!=null? product.SellEndDate: DateTime.Now,
                              State = product.State,
                              AgencyCost = product.AgencyCost,
                              CostUnit = product.CostUnit,
                              Tag = product.Tag,
                              ApprovalBy = product.ApprovalBy,
                              ApprovalDate = product.ApprovalDate!=null? product.ApprovalDate: DateTime.Now,
                              Delete_Flag = product.Delete_Flag,
                              Avatar = product.Avatar,
                              Avatar_Url = product.Avatar_Url,
                              UpdateCost = product.UpdateCost,
                              UpdateTime = product.UpdateTime !=null? product.UpdateTime: DateTime.Now,
                              UpdateBy = product.UpdateBy,
                              UserName = product.UserName,
                              RoleId = product.RoleId,
                              RoleText = product.RoleText,
                              ListRegionSelling = TextRegionByProduct(product.Id)
                          });
            var result = query.Skip((page - 1) * recordNum).Take(recordNum).ToList();
            if (result != null && result.Count > 0)
            {
                result[0].Total = query.Count();
            }
            return result;

        }

        /// <summary>
        /// Lấy danh sách sản phẩm
        /// </summary>
        /// <param name="query"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="total"></param>
        /// <returns></returns>
        public List<Entities.Product> List(string query, int pageIndex, int pageSize, out int total)
        {
            List<Entities.Product> lst = new List<Entities.Product>();
            total = 0;
            pageIndex = pageIndex <= 0 ? 1 : pageIndex;

            total = (from p in Context.Products
                     select p.Id).Count();
            lst = (from product in Context.Products
                   select new Entities.Product
                   {
                       Id = product.Id,
                       UserId = product.UserId,
                       Transaction_Type = product.Transaction_Type,
                       Product_Type = product.Product_Type,
                       CityId = product.CityId,
                       DistrictId = product.DistrictId,
                       WardId = product.WardId,
                       Text = product.Text,
                       Text_Alias = product.Text_Alias,
                       HouseNumber = product.HouseNumber,
                       Address = product.Address,
                       Area = product.Area,
                       Description = product.Description,
                       StandardCost = product.StandardCost,
                       SellStartDate = product.SellEndDate,
                       SellEndDate = product.SellEndDate,
                       State = product.State,
                       AgencyCost = product.AgencyCost,
                       CostUnit = product.CostUnit,
                       Tag = product.Tag,
                       UpdateCost = product.UpdateCost,
                       UpdateBy = product.UpdateBy,
                       UpdateTime = product.UpdateTime,
                       UserName = product.UserName,
                       RoleId = product.RoleId,
                       RoleText = product.RoleText
                   }).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return lst;
        }

        /// <summary>
        /// Get all product
        /// </summary>
        /// <returns></returns>
        public List<Entities.Product> _List()
        {
            var result = (from product in Context.Products
                          select new Entities.Product
                          {
                              Id = product.Id,
                              UserId = product.UserId,
                              Transaction_Type = product.Transaction_Type,
                              Product_Type = product.Product_Type,
                              CityId = product.CityId,
                              DistrictId = product.DistrictId,
                              WardId = product.WardId,
                              Text = product.Text,
                              HouseNumber = product.HouseNumber,
                              Address = product.Address,
                              Area = product.Area,
                              Description = product.Description,
                              StandardCost = product.StandardCost,
                              SellStartDate = product.SellEndDate,
                              SellEndDate = product.SellEndDate,
                              State = product.State,
                              AgencyCost = product.AgencyCost,
                              CostUnit = product.CostUnit,
                              Tag = product.Tag,
                              Avatar = product.Avatar,
                              Avatar_Url = product.Avatar_Url,
                              UpdateBy = product.UpdateBy,
                              UpdateTime = product.UpdateTime,
                              UserName = product.UserName,
                              RoleId = product.RoleId,
                              RoleText = product.RoleText
                          }).ToList();
            return result;
        }

        public List<Entities.Product> List(Entities.ProductCondtions condition)
        {
            return List(withOutId: condition.withOutId,
                        transactionType: condition.TransactionType, 
                        userId: condition.UserId,
                        productType: condition.ProductType,
                        cityId:condition.City, 
                        districtId:condition.District,
                        wardId:condition.Ward,
                        recordNum: condition.Limit,
                        page: condition.Page, 
                        text:condition.Text, 
                        minCost: condition.MinCost, 
                        maxCost: condition.MaxCost,
                        minArea: condition.MinArea,
                        maxArea: condition.MaxArea);
        }

        /// <summary>
        /// Lấy danh sách loại nhà.
        /// </summary>
        /// <returns></returns>
        public List<Item> ListProductType()
        {
            var result = (from p in Context.ProductTypes
                          select new Entities.Item()
                          {
                              Id = p.Id,
                              Text = p.Text
                          }).ToList();
            return result;
        }
        
        public Item ProductType(int id)
        {
            var result = (from p in Context.ProductTypes
                          where p.Id == id
                          select new Entities.Item()
                          {
                              Id = p.Id,
                              Text = p.Text
                          }).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// Danh sách đơn vị giá
        /// </summary>
        /// <returns></returns>
        public List<Item> ListCostUnit()
        {
            //var result = (from p in Context.CostUnits
            //              select new Entities.Item()
            //              {
            //                  Id = p.Id,
            //                  Text = p.Text
            //              }).ToList();
            //return result;
            return new List<Item>() { new Item{Id = 1, Text = "%"} };
        }

        /// <summary>
        /// Lấy danh sách ảnh nhà theo ID nhà
        /// </summary>
        /// <param name="productId">id nhà</param>
        /// <returns></returns>
        public List<Entities.ProductImage> GetListImage(int productId)
        {
            var result = (from p in Context.ProductImages
                          where p.ProductId == productId
                          select new Entities.ProductImage()
                          {
                              Id = p.Id,
                              Text = p.Text,
                              ImageUrl = p.ImageUrl,
                              Size = p.Size,
                              DateUpload = p.DateUpload,
                              DateModified = p.DateModified
                          }).ToList();
            return result;
        }

        public int UpdateWasSelling(int productId)
        {
            DataLayer.Product product = (from x in Context.Products
                                         where x.Id == productId
                                         select x).FirstOrDefault();
            if (product == null)
            {
                return (int)Enums.Errors.UPDATE_ERROR;
            }
            product.State = 1;
            try
            {
                Context.SubmitChanges();
            }
            catch (Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.UPDATE_ERROR;
            }
            return product.Id;
        }

        #region Product - Image

        public int Insert(Entities.ProductImage e)
        {
            DataLayer.ProductImage p = new DataLayer.ProductImage();
            p.ProductId = e.ProductId;
            p.Text = e.Text;
            p.ImageUrl = e.ImageUrl;
            p.Size = e.Size;
            p.Order = e.Order;
            p.DateUpload = System.DateTime.Now;

            try
            {
                Context.ProductImages.InsertOnSubmit(p);
                Context.SubmitChanges();
            }
            catch (Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.INSERT_ERROR;
            }
            return p.Id;
        }

        public int Update(Entities.ProductImage e)
        {
            DataLayer.ProductImage p = (from x in Context.ProductImages
                                   where x.Id == e.Id
                                   select x).FirstOrDefault();

            p.ProductId = e.ProductId;
            p.Text = e.Text;
            p.ImageUrl = e.ImageUrl;
            p.Size = e.Size;
            p.Order = e.Order;
            p.DateModified = System.DateTime.Now;
            try
            {
                Context.SubmitChanges();
                return p.Id;
            }
            catch (Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.UPDATE_ERROR;
            }
        }

        public int DeleteImage(int entityId)
        {
            try
            {
                DataLayer.ProductImage p = (from u in Context.ProductImages
                                       where u.Id == entityId
                                       select u).FirstOrDefault();
                if (p != null)
                {
                    Context.ProductImages.DeleteOnSubmit(p);
                    Context.SubmitChanges();
                    return entityId;
                }
                else
                {
                    return (int)Enums.Errors.NOT_EXIST;
                }

            }
            catch (Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.DELETE_ERROR;
            }
        }

        public int DeleteImage(string imgName, int productId)
        {
            try
            {
                DataLayer.ProductImage p = (from u in Context.ProductImages
                                            where u.Text == imgName
                                            && u.ProductId == productId
                                            select u).FirstOrDefault();
                if (p != null)
                {
                    Context.ProductImages.DeleteOnSubmit(p);
                    Context.SubmitChanges();

                    // Delete in folder


                    return 1;
                }
                else
                {
                    return (int)Enums.Errors.NOT_EXIST;
                }

            }
            catch (Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.DELETE_ERROR;
            }
        }
        #endregion

        #region Product - Region
        /// <summary>
        /// Lấy danh sách vùng theo id sản phẩm
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public List<Entities.Item> ListNeighborRegion(int productId)
        {
            var result = (from r in Context.Regions
                          join pr in Context.Product_Regions on r.Id equals pr.RegionId
                          where pr.ProductId == productId
                          && pr.Fixed == false
                          && pr.Status == true
                          select new Entities.Item()
                          {
                              Id = r.Id,
                              Text = r.Text,
                              Checked = true
                          }).ToList();
            return result;
        }

        public int Insert(Entities.Product_Region e)
        {
            DataLayer.Product_Region p = new DataLayer.Product_Region();
            p.ProductId = e.ProductId;
            p.RegionId = e.RegionId;
            p.Fixed = e.Fixed;
            p.Status = e.Status;
            try
            {
                Context.Product_Regions.InsertOnSubmit(p);
                Context.SubmitChanges();
            }
            catch (Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.INSERT_ERROR;
            }
            return p.Id;
        }

        public int Update(Entities.Product_Region e)
        {
            DataLayer.Product_Region p = (from x in Context.Product_Regions
                                        where x.Id == e.Id
                                        select x).FirstOrDefault();

            p.ProductId = e.ProductId;
            p.RegionId = e.RegionId;
            p.Fixed = e.Fixed;
            p.Status = e.Status;
            try
            {
                Context.SubmitChanges();
                return p.Id;
            }
            catch (Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.UPDATE_ERROR;
            }
        }

        public int DeleteRegion(int entityId)
        {
            try
            {
                DataLayer.Product_Region p = (from u in Context.Product_Regions
                                            where u.Id == entityId
                                            select u).FirstOrDefault();
                if (p != null)
                {
                    Context.Product_Regions.DeleteOnSubmit(p);
                    Context.SubmitChanges();
                    return entityId;
                }
                else
                {
                    return (int)Enums.Errors.NOT_EXIST;
                }

            }
            catch (Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.DELETE_ERROR;
            }
        }

        /// <summary>
        /// Lấy danh sách vùng nhờ môi giới bán theo Id sản phẩm
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public List<Entities.Item> ListRegionByProduct(int productId)
        {
            List<Entities.Item> list = new List<Item>();
            list = (from pr in Context.Product_Regions
                    join r in Context.Regions on pr.RegionId equals r.Id
                    where pr.Id == productId
                    && pr.Status
                    select new Entities.Item
                    {
                        Id = r.Id,
                        Text = r.Text
                    }).ToList();
            return list;
        }

        public string TextRegionByProduct(int productId)
        {
            var list = (from pr in Context.Product_Regions
                        join r in Context.Regions on pr.RegionId equals r.Id
                        where pr.ProductId == productId
                        && pr.Status
                        select r.Text).ToList();
            string result = "";
            foreach (string str in list)
            {
                if (!result.Contains(str))
                {
                    result += str + ", ";
                }
            }
            return result;
        }

        /// <summary>
        /// Lấy Vùng fixed theo product id
        /// </summary>
        /// <param name="productId">productID</param>
        /// <returns>region fixed (Fixed = true)</returns>
        public int GetRegionFixed(int productId)
        {
            var result = (from pr in Context.Product_Regions
                          where pr.ProductId == productId
                          && pr.Fixed == true
                          select pr.RegionId).FirstOrDefault();
            return result;
        }

        public Entities.Product_Region GetProductRegion(int productId, int regionId)
        {
            var result = (from pr in Context.Product_Regions
                          where pr.RegionId == regionId && pr.ProductId == productId
                          select new Entities.Product_Region() {
                            Id = pr.Id,
                            ProductId = pr.ProductId,
                            RegionId = pr.RegionId,
                            Fixed = pr.Fixed,
                            Status = pr.Status
                          }).FirstOrDefault();
            return result;
        }

        public List<int> ListProductByRegions(int[] regionIds)
        {
            var result = (from pr in Context.Product_Regions
                          where regionIds.Contains(pr.RegionId)
                          && pr.Status
                          select pr.ProductId                          
                          ).ToList();
            return result;
        }

        /// <summary> Lấy danh sách email của những người môi giới liên quan đến sản phẩm.
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public List<string> ListEmailAgency(int productId)
        {
            var query = (from pr in Context.Product_Regions
                         join uir in Context.UserInRegions on pr.RegionId equals uir.RegionId
                         join u in Context.Users on uir.UserId equals u.Id
                         where pr.ProductId == productId
                         && pr.Status == true
                         && uir.RegionType == 2
                         select u.Email).ToList();
            return query;
        }

        #endregion Product - Region

        #region Product - Change Cost
        public int UpdateCost(Entities.Product e)
        {
            DataLayer.Product p = (from x in Context.Products
                                              where x.Id == e.Id
                                              select x).FirstOrDefault();
            p.Id = e.Id;
            p.UpdateBy = e.UpdateBy;
            p.UpdateCost = e.UpdateCost;
            p.UpdateTime = DateTime.Now;
            try
            {
                Context.SubmitChanges();
                return p.Id;
            }
            catch (Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.UPDATE_ERROR;
            }
        }

        public List<Entities.Product_ChangeCost> GetListProductChangeCost(int productId, int userId, int recordNum = 10)
        {
            #region Old Code
            //var resultSP = Context.GetListProductChangeCost(productId, userId, 10);
            //var result = (from x in resultSP
            //              select new Entities.Product_ChangeCost()
            //              {
            //                  Id = x.Id,
            //                  ProductId = x.ProductId,
            //                  UserId = x.UserId,
            //                  UserName = x.UserName,
            //                  Cost = x.Cost,
            //                  UpdateTime = x.UpdateTime
            //              }).ToList();
            //return result;
            #endregion

            var result = (from p in Context.Product_ChangeCosts
                          join u in Context.Users on p.UserId equals u.Id
                          where p.ProductId == productId
                             && p.UserId != userId
                            orderby p.Cost descending
                          select new Entities.Product_ChangeCost()
                          {
                              ProductId = productId,
                              UserId = p.UserId,
                              UserName = p.UserName,
                              PhoneNumber = p.PhoneNumber,
                              Cost = p.Cost,
                              UpdateTime = p.UpdateTime,
                              Email = u.Email
                          }
                        ).Take(recordNum).ToList();
            return result;
        }

        public int UpdateProductChangeCost(Entities.Product_ChangeCost e)
        {
            try
            {
                DataLayer.Product_ChangeCost p = (from x in Context.Product_ChangeCosts
                                                  where x.ProductId == e.ProductId
                                                  && x.UserId == e.UserId
                                       select x).FirstOrDefault();
                bool isNew = false;
                if (p == null)
                {
                    p = new DataLayer.Product_ChangeCost();
                    isNew = true;
                }
                short times = GetMaxTimeUpdateCost(e.ProductId, e.UserId);
                p.ProductId = e.ProductId;
                p.UserId = e.UserId;
                p.UserName = e.UserName;
                p.PhoneNumber = e.PhoneNumber;
                p.Cost = e.Cost;
                p.Times = (short)(times + 1);
                p.UpdateTime = DateTime.Now;

                // Xử lý thêm thông tin vào bảng lịch sử.
                DataLayer.Product_ChangeCost_History pH = new DataLayer.Product_ChangeCost_History();
                pH.ProductId = e.ProductId;
                pH.UserId = e.UserId;
                pH.Cost = e.Cost;
                pH.Times = (short)(times + 1);
                pH.UpdateTime = DateTime.Now;
                pH.Delete_Flag = false;
            
                if (isNew)
                {
                    Context.Product_ChangeCosts.InsertOnSubmit(p);
                }

                Context.Product_ChangeCost_Histories.InsertOnSubmit(pH);

                Context.SubmitChanges();
            }
            catch (Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.INSERT_ERROR;
            }
            return e.Id;
        }

        

        public decimal GetCostChangeByOwn(int productId, int userId)
        {
            decimal result = (from p in Context.Product_ChangeCosts
                              where p.ProductId == productId && p.UserId == userId
                              orderby p.Times descending
                              select p.Cost).FirstOrDefault();
            return result;
        }

        public Entities.Product_ChangeCost GetLastChangeCost(int productId, int userId)
        {
            var result = (from p in Context.Product_ChangeCosts
                          where p.ProductId == productId
                          && p.UserId == userId
                          orderby p.Times descending
                          select new Entities.Product_ChangeCost()
                          {
                              Id = p.Id,
                              ProductId = p.ProductId,
                              UserId = p.UserId,
                              UserName = p.UserName,
                              PhoneNumber = p.PhoneNumber,
                              Cost = p.Cost,
                              Times = p.Times,
                              UpdateTime = p.UpdateTime
                          }).FirstOrDefault();
            return result;
        }

        public int ChangeFollow(int productId, int userId, bool isFollow)
        {
            DataLayer.Product_Follow p = (from x in Context.Product_Follows
                                          where productId == x.ProductId && userId == x.UserId
                                          select x).FirstOrDefault();
            try
            {
                if (p == null)
                {
                    p = new DataLayer.Product_Follow();
                    p.UserId = userId;
                    p.ProductId = productId;
                    p.IsFollow = isFollow;
                    p.UpdateTime = DateTime.Now;
                    Context.Product_Follows.InsertOnSubmit(p);
                    Context.SubmitChanges();
                }
                else
                {
                    p.IsFollow = isFollow;
                    p.UpdateTime = DateTime.Now;
                    Context.SubmitChanges();
                }
                return p.ProductId;
            }
            catch (Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.UPDATE_ERROR;
            }
        }

        public short GetMaxTimeUpdateCost(int productId, int userId)
        {
            short result = (from p in Context.Product_ChangeCost_Histories
                            where p.ProductId == productId && p.UserId == userId
                            orderby p.Times descending
                            select p.Times).FirstOrDefault();
            return result;
        }
        #endregion

        #region Product Want Sell
        public List<Entities.Product_WantSell> GetListProductWantSell()
        {
            var result = (from p in Context.Product_WantSells
                          orderby p.UpdateTime descending
                          select new Entities.Product_WantSell()
                          {
                              Id = p.Id,
                              ProductId = p.ProductId,
                              Text = p.Text,
                              CostSell = p.CostSell,
                              BuyerId = p.BuyerId,
                              CostBuy = p.CostBuy,
                              UpdateTime = p.UpdateTime,
                              Times = p.Times,
                              IsChecked = p.IsChecked
                          }).ToList();
            return result;
        }

        public List<Entities.Product_WantSell> GetListProductWantSell(int buyerId)
        {
            var result = (from p in Context.Product_WantSells
                          where p.BuyerId == buyerId
                          orderby p.UpdateTime descending
                          select new Entities.Product_WantSell()
                          {
                              Id = p.Id,
                              ProductId = p.ProductId,
                              Text = p.Text,
                              CostSell = p.CostSell,
                              BuyerId = p.BuyerId,
                              CostBuy = p.CostBuy,
                              UpdateTime = p.UpdateTime,
                              Times = p.Times,
                              IsChecked = p.IsChecked
                          }).ToList();
            return result;
        }

        public List<Entities.Product_WantSell> GetListProductWantSell(int buyerId, bool isChecked = false)
        {
            var result = (from p in Context.Product_WantSells
                          where p.BuyerId == buyerId && p.IsChecked == isChecked
                          orderby p.UpdateTime descending
                          select new Entities.Product_WantSell()
                          {
                              Id = p.Id,
                              ProductId = p.ProductId,
                              Text = p.Text,
                              CostSell = p.CostSell,
                              BuyerId = p.BuyerId,
                              CostBuy = p.CostBuy,
                              UpdateTime = p.UpdateTime,
                              Times = p.Times,
                              IsChecked = p.IsChecked
                          }).ToList();
            return result;
        }

        public List<Entities.Product_WantSell> GetListProductWantSell(int productId, int buyerId)
        {
            var result = (from p in Context.Product_WantSells
                          where p.ProductId == productId && p.BuyerId == buyerId
                          orderby p.UpdateTime descending
                          select new Entities.Product_WantSell()
                          {
                              Id = p.Id,
                              ProductId = p.ProductId,
                              Text = p.Text,
                              CostSell = p.CostSell,
                              BuyerId = p.BuyerId,
                              CostBuy = p.CostBuy,
                              UpdateTime = p.UpdateTime,
                              Times = p.Times,
                              IsChecked = p.IsChecked
                          }).ToList();
            return result;
        }

        public int InsertProductWantSell(Entities.Product_WantSell e)
        {
            DataLayer.Product_WantSell p = new DataLayer.Product_WantSell();

            p.ProductId = e.ProductId;
            p.Text = e.Text;
            p.CostSell = e.CostSell;
            p.BuyerId = e.BuyerId;
            p.CostBuy = e.CostBuy;
            p.UpdateTime = DateTime.Now;
            p.Times = e.Times;
            p.IsChecked = false;
            try
            {
                Context.Product_WantSells.InsertOnSubmit(p);
                Context.SubmitChanges();
            }
            catch (Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.INSERT_ERROR;
            }
            return p.Id;
        }

        public int UpdateProductWantSell(int productId, int buyerId)
        {
            DataLayer.Product_WantSell p = (from x in Context.Product_WantSells
                                            where x.Id == productId && x.BuyerId == buyerId
                                            select x).FirstOrDefault();
            if (p != null)
            {
                p.IsChecked = true;
                p.CheckTime = DateTime.Now;
                try
                {
                    Context.SubmitChanges();
                    return p.Id;
                }
                catch(Exception ex)
                {
                    string data = className + ex.Message.ToString();
                    Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                    return (int)Enums.Errors.UPDATE_ERROR;
                }
            }
            return (int)Enums.Errors.UPDATE_ERROR;
        }

        #endregion

        #region Product - Follow
        /// <summary>
        /// Lấy danh sách các sản phẩm đang theo dõi của 1 user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Entities.Product> GetListProductFollow(int userId)
        {
            List<Entities.Product> listProduct = new List<Entities.Product>();
            // Get from product follow table
            var listPF = (from p in Context.Product_Follows
                          where p.UserId == userId
                          select p.ProductId).ToList();
            if (listPF != null)
            {
                Entities.Product product;
                foreach (int pId in listPF)
                {
                    product = (from p in Context.Products
                               where p.Id == pId
                               && (p.State == null? true: p.State!=1)
                               && (p.Delete_Flag == null || p.Delete_Flag == false)
                               select new Entities.Product()
                               {
                                   Id = p.Id,
                                   UserId = p.UserId,
                                   Transaction_Type = p.Transaction_Type,
                                   Product_Type = p.Product_Type,
                                   CityId = p.CityId,
                                   DistrictId = p.DistrictId,
                                   WardId = p.WardId,
                                   Text = p.Text,
                                   Text_Alias = p.Text_Alias,
                                   HouseNumber = p.HouseNumber,
                                   Address = p.Address,
                                   Area = p.Area,
                                   Description = p.Description,
                                   StandardCost = p.StandardCost,
                                   SellStartDate = p.SellEndDate,
                                   SellEndDate = p.SellEndDate,
                                   State = p.State,
                                   AgencyCost = p.AgencyCost,
                                   CostUnit = p.CostUnit,
                                   Tag = p.Tag,
                                   Avatar = p.Avatar,
                                   Avatar_Url = p.Avatar_Url,
                                   UpdateBy = p.UpdateBy,
                                   UpdateTime = p.UpdateTime
                               }).FirstOrDefault();
                    if (product != null)
                    {
                        listProduct.Add(product);
                    }
                }
            }
            return listProduct;
        }

        /// <summary>
        /// Lấy danh sách sản phẩm đang theo dõi của 1 user.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<Entities.Product_Follow> ListProductFollowId(int userId)
        {
            var result = (from p in Context.Product_Follows
                          where p.UserId == userId
                          select new Entities.Product_Follow()
                          {
                              UserId = userId,
                              ProductId = p.ProductId,
                              IsFollow = p.IsFollow
                          }).ToList();
            return result;
        }

        /// <summary>
        /// Lấy danh sách người dùng (ID) đang theo dõi 1 sản phẩm.
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public List<int> ListUserIdProductFollow(int productId)
        {
            var result = (from p in Context.Product_Follows
                          where p.ProductId == productId
                          && p.IsFollow
                          select p.UserId).ToList();

            return result;
        }

        /// <summary>
        /// Lấy danh sách email đang theo dõi sản phẩm.
        /// Loại trừ người dùng (withoutId)
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        public List<string> ListUserEmailFollow(int productId, int withoutId = 0)
        {
            var result = (from p in Context.Product_Follows
                          join u in Context.Users on p.UserId equals u.Id
                          where p.ProductId == productId
                          && p.IsFollow
                          && (withoutId>0? p.UserId != withoutId:true)
                          select u.Email).ToList();

            return result;
        }

        #endregion

        #region Product Message

        public int Insert(Entities.ProductMessage e)
        {
            DataLayer.ProductMessage p = new DataLayer.ProductMessage();

            p.From = e.From;
            p.To = e.To;
            p.ProductId = e.ProductId;
            p.ProductText = e.ProductText;
            p.Content = e.Content;
            p.Read_Flag = false;
            p.Delete_Flag = false;
            p.CreateDate = DateTime.Now;
            p.UpdateDate = DateTime.Now;

            try
            {
                Context.ProductMessages.InsertOnSubmit(p);
                Context.SubmitChanges();
            }
            catch (Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.INSERT_ERROR;
            }
            return p.Id;
        }

        public int InsertList(Entities.ProductMessage e, List<int> listUserReceive)
        {
            try
            {
                foreach (int userId in listUserReceive)
                {
                    DataLayer.ProductMessage p = new DataLayer.ProductMessage();
                    p.From = e.From;
                    p.To = userId;
                    p.ProductId = e.ProductId;
                    p.ProductText = e.ProductText;
                    p.Content = e.Content;
                    p.Read_Flag = false;
                    p.Delete_Flag = false;
                    p.CreateDate = DateTime.Now;
                    p.UpdateDate = DateTime.Now;

                    Context.ProductMessages.InsertOnSubmit(p);
                }
                Context.SubmitChanges();
            }
            catch (Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.INSERT_ERROR;
            }
            return e.ProductId;
        }

        public int UpdateReadFlag(int Id)
        {
            DataLayer.ProductMessage p = (from x in Context.ProductMessages
                                        where x.Id == Id
                                        select x).FirstOrDefault();

            p.Read_Flag = true;
            p.UpdateDate = DateTime.Now;
            try
            {
                Context.SubmitChanges();
                return p.Id;
            }
            catch (Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.UPDATE_ERROR;
            }
        }

        public int DeleteMessage(int Id)
        {
            DataLayer.ProductMessage p = (from x in Context.ProductMessages
                                          where x.Id == Id
                                          select x).FirstOrDefault();

            p.Delete_Flag = true;
            p.DeleteDate = DateTime.Now;
            try
            {
                Context.SubmitChanges();
                return p.Id;
            }
            catch (Exception ex)
            {
                string data = className + ex.Message.ToString();
                Logs.LogWrite(string.Format(Configs.ERROR_ACTION, data));
                return (int)Enums.Errors.UPDATE_ERROR;
            }
        }

        public List<Entities.ProductMessage> ListMessage(int ToId)
        {
            List<Entities.ProductMessage> lst = new List<Entities.ProductMessage>();

            lst = (from p in Context.ProductMessages
                   join u in Context.Users on p.From equals u.Id
                   where p.To == ToId
                   orderby p.Read_Flag ascending, p.UpdateDate descending
                   select new Entities.ProductMessage
                   {
                       Id = p.Id,
                       ProductId = p.ProductId,
                       ProductText = p.ProductText,
                       Content = p.Content,
                       CreateDate = p.CreateDate,
                       Read_Flag = p.Read_Flag,
                       Avatar = u.Avatar,
                       Phone = u.Phone,
                       UserName = u.UserName
                   }).ToList();
            
            return lst;
        }
        
        #endregion
    }
}
