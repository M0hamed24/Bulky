using BulkyWeb.Data;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SD7501Bulky.DataAccess.Repository.IRepository;
using SD7501Bulky.Models;
using SD7501Bulky.Models.ViewModles;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll().ToList();
            return View(objProductList);
        }

        public IActionResult Upsert(int id)
        {
            ProductVM productVM = new()
            {
                CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                {
                    Text = u.Name,
                    Value = u.Id.ToString()
                }),
                Product = new Product()
            };

            if (id == 0)
            {
                return View(productVM);
            }

            productVM.Product = _unitOfWork.Product.Get(u => u.Id == id);
            return View(productVM);
        }

        [HttpPost]
        public IActionResult Upsert(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
         
                // IMAGE UPLOAD
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                    string productFolder = Path.Combine(wwwRootPath, @"images\products");

                    if (!Directory.Exists(productFolder))
                    {
                        Directory.CreateDirectory(productFolder);
                    }

                    string fullPath = Path.Combine(productFolder, fileName);

                    using (var fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                    productVM.Product.ImageUrl = @"\images\products\" + fileName;
                }
                else if (productVM.Product.Id != 0)
                {
                    // keep old image
                    var objFromDb = _unitOfWork.Product.Get(u => u.Id == productVM.Product.Id);
                    productVM.Product.ImageUrl = objFromDb.ImageUrl;
                }
            
                // VALIDATION (CREATE ONLY)
                if (productVM.Product.Id == 0 && string.IsNullOrEmpty(productVM.Product.ImageUrl))
                {
                    ModelState.AddModelError("file", "Image is required");

                    productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
                    {
                        Text = u.Name,
                        Value = u.Id.ToString()
                    });

                    return View(productVM);
                }

                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                    TempData["success"] = "Product created successfully";
                }
                else
                {
                    var objFromDb = _unitOfWork.Product.Get(u => u.Id == productVM.Product.Id);

                    if (objFromDb != null)
                    {
                        objFromDb.Title = productVM.Product.Title;
                        objFromDb.ISBN = productVM.Product.ISBN;
                        objFromDb.ListPrice = productVM.Product.ListPrice;
                        objFromDb.Author = productVM.Product.Author;
                        objFromDb.CategoryId = productVM.Product.CategoryId;
                        objFromDb.ImageUrl = productVM.Product.ImageUrl;
                    }

                    TempData["success"] = "Product updated successfully";
                }

                _unitOfWork.Save();
                return RedirectToAction("Index");
            }

            productVM.CategoryList = _unitOfWork.Category.GetAll().Select(u => new SelectListItem
            {
                Text = u.Name,
                Value = u.Id.ToString()
            });

            return View(productVM);
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var productToBeDeleted = _unitOfWork.Product.Get(u => u.Id == id);

            if (productToBeDeleted == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            var oldImagePath = Path.Combine(
                _webHostEnvironment.WebRootPath,
                productToBeDeleted.ImageUrl.TrimStart('\\')
            );

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

            _unitOfWork.Product.Remove(productToBeDeleted);
            _unitOfWork.Save();

            return Json(new { success = true, message = "Deleted successfully" });
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            List<Product> objProductList = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
            return Json(new { data = objProductList });
        }
    }
}