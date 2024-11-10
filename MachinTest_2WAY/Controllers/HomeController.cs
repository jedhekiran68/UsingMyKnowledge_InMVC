using MachinTest_2WAY.Models;
using MachinTest_2WAY.Repositary;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace MachinTest_2WAY.Controllers
{
    public class HomeController : Controller
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["ConnStringMy"].ConnectionString;
        MainClass ITCSR = new MainClass();
        public ActionResult Index(int pageNumber = 1, int pageSize = 10)
        {
            var productList = ITCSR.GetProductList(connectionString, pageNumber, pageSize);
            ViewBag.ProductList = productList;
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;

            var totalRecords = ITCSR.GetProductCount(connectionString);
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalRecords / pageSize);

            return View();
        }


        public ActionResult AddProduct()
        {
            var categories = ITCSR.GetCategoryList(connectionString);
            ViewBag.CategoryList = categories;

            return View();
        }

        [HttpPost]
        public ActionResult AddProduct(string ProductName, int CategoryId)
        {
            bool isSaved = false;
            var category = ITCSR.GetCategoryById(CategoryId, connectionString);
            if (category == null)
            {
                return Json(new { success = false, message = "Invalid Category ID." });
            }
            TblProductList newProduct = new TblProductList
            {
                ProductName = ProductName,
                categoryId = CategoryId,
                CategoryName = category.CategoryName
            };
            try
            {
                isSaved = ITCSR.SaveProductDetails(newProduct, connectionString);
                if (isSaved)
                {
                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = "Product could not be added." });
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception in AddProduct: {ex.Message}");
                return Json(new { success = false, message = "An error occurred while adding the product." });
            }
        }

        [HttpGet]
        public ActionResult AddCategory()
        {
            var categories = ITCSR.GetCategoryList(connectionString);
            ViewBag.CategoryList = categories;
            return View();
        }

        [HttpPost]
        public JsonResult AddCategory(string CategoryName)
        {
            bool status = false;
            TblCategoryList newCategory = new TblCategoryList
            {
                CategoryName = CategoryName
            };
            status = ITCSR.SaveCategoryDetails(newCategory, connectionString);
            return Json(new { success = status });
        }

        [HttpGet]
        public JsonResult GetCategories()
        {
            var categories = ITCSR.GetCategoryList(connectionString);
            return Json(categories, JsonRequestBehavior.AllowGet);
        }

        public ActionResult EditProduct(int id)
        {
            var product = ITCSR.GetProductById(id, connectionString);
            if (product == null)
            {
                return HttpNotFound();
            }
            var categories = ITCSR.GetCategoryList(connectionString);
            ViewBag.CategoryList = categories;

            return View(product);
        }

        [HttpPost]
        public ActionResult EditProduct(TblProductList updatedProduct)
        {
            bool isUpdated = ITCSR.SaveProductDetails(updatedProduct, connectionString);
            if (isUpdated)
            {
                return RedirectToAction("Index");
            }
            return View(updatedProduct);  
        }

        [HttpPost]
        public ActionResult DeleteProduct(int id)
        {
            bool isDeleted = ITCSR.DeleteProduct(id, connectionString);
            if (isDeleted)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "Failed to delete product." });
        }
        [HttpPost]
        public ActionResult UpdateCategory(TblCategoryList category)
        {
            bool status = false;
            using (var context = new Model1(connectionString))
            {
                var existingCategory = context.TblCategoryLists.FirstOrDefault(c => c.categoryId == category.categoryId);
                if (existingCategory != null)
                {
                    existingCategory.CategoryName = category.CategoryName;
                    context.SaveChanges();
                    status = true;
                }
            }
            return Json(new { success = status });
        }

        [HttpPost]
        public JsonResult DeleteCategory(int categoryId)
        {
            bool status = ITCSR.DeleteCategory(categoryId, connectionString);
            return Json(new { success = status });
        }
    }
}