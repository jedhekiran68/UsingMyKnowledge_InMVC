using MachinTest_2WAY.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace MachinTest_2WAY.Repositary
{
    public class MainClass
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ConnStringMy"].ConnectionString;
        public List<TblProductList> GetProductList(string connectionString, int pageNumber, int pageSize)
        {
            using (var context = new Model1(connectionString))
            {
                var productList = (from p in context.TblProductLists join c in context.TblCategoryLists on p.categoryId equals c.categoryId
                                   orderby p.ProductId
                                   select new
                                   {
                                       ProductId = p.ProductId,
                                       ProductName = p.ProductName,
                                       categoryId = p.categoryId,
                                       CategoryName = c.CategoryName
                                   })
                                   .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

                return productList.Select(p => new TblProductList
                {
                    ProductId = p.ProductId,
                    ProductName = p.ProductName,
                    categoryId = p.categoryId,
                    CategoryName = p.CategoryName
                }).ToList();
            }
        }

        public int GetProductCount(string connectionString)
        {
            using (var context = new Model1(connectionString))
            {
                return context.TblProductLists.Count();
            }
        }

        public List<TblCategoryList> GetCategoryList(string connectionString)
        {
            List<TblCategoryList> categoryList = new List<TblCategoryList>();
            try
            {
                using (var context = new Model1(connectionString))
                {
                    categoryList = context.TblCategoryLists.OrderBy(P=>P.categoryId).ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return categoryList;
        }

        public TblCategoryList GetCategoryById(int categoryId, string connectionString)
        {
            TblCategoryList category = null;
            using (var context = new Model1(connectionString))
            {
                category = context.TblCategoryLists.FirstOrDefault(c => c.categoryId == categoryId);
            }
            return category;
        }

        public bool SaveProductDetails(TblProductList objproductMaster, string connectionString)
        {
            bool status = false;
            Model1 context = null;
            try
            {
                context = new Model1(connectionString);
                var objProduct = context.TblProductLists.FirstOrDefault(p => p.ProductId == objproductMaster.ProductId);
                if (objProduct != null)
                {
                    objProduct.ProductName = objproductMaster.ProductName;
                    objProduct.categoryId = objproductMaster.categoryId;

                    var category = context.TblCategoryLists.FirstOrDefault(c => c.categoryId == objproductMaster.categoryId);
                    if (category != null)
                    {
                        objProduct.CategoryName = category.CategoryName;
                    }
                }
                else
                {
                    context.TblProductLists.Add(objproductMaster);
                }

                context.SaveChanges();
                status = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in SaveProductDetails: {ex.Message}");
            }
            finally
            {
                context?.Dispose();
            }
            return status;
        }

        public TblProductList GetProductById(int productId, string connectionString)
        {
            TblProductList product = null;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT ProductId, ProductName, categoryId, CategoryName FROM TblProductList WHERE ProductId = @ProductId";

                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@ProductId", productId);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        product = new TblProductList
                        {
                            ProductId = Convert.ToInt32(reader["ProductId"]),
                            ProductName = reader["ProductName"].ToString(),
                            categoryId = Convert.ToInt32(reader["categoryId"]),
                            CategoryName = reader["CategoryName"].ToString()
                        };
                    }
                }
            }
            return product;
        }
        public bool SaveCategoryDetails(TblCategoryList category, string connectionString)
        {
            bool status = false;

            using (var context = new Model1(connectionString))
            {
                context.TblCategoryLists.Add(category);
                context.SaveChanges();
                status = true;
            }
            return status;
        }
        public bool DeleteProduct(int productId, string connectionString)
        {
            bool status = false;

            using (var context = new Model1(connectionString))
            {
                var product = context.TblProductLists.FirstOrDefault(p => p.ProductId == productId);
                if (product != null)
                {
                    context.TblProductLists.Remove(product);
                    context.SaveChanges();
                    status = true;
                }
            }
            return status;
        }
        public bool DeleteCategory(int categoryId, string connectionString)
        {
            bool status = false;
            using (var context = new Model1(connectionString))
            {
                var category = context.TblCategoryLists.FirstOrDefault(c => c.categoryId == categoryId);
                if (category != null)
                {
                    context.TblCategoryLists.Remove(category);
                    context.SaveChanges();
                    status = true;
                }
            }
            return status;
        }
    }
}