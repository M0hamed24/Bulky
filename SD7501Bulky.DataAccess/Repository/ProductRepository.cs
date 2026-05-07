using BulkyWeb.Data;
using BulkyWeb.Models;
using SD7501Bulky.DataAccess.Repository.IRepository;
using SD7501Bulky.Models;
using System;

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD7501Bulky.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private ApplicationDbContext _db;

        public ProductRepository(ApplicationDbContext db):base(db)
        {
            _db = db;
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(Product obj)
        {
            var objFromDb = _db.Product.FirstOrDefault(u  => u.Id == obj.Id);
            if (objFromDb != null)
            {
                objFromDb.Title = obj.Title;
                objFromDb.ISBN = obj.ISBN;
                objFromDb.Price = obj.Price;
                objFromDb.ListPrice = obj.ListPrice;
                objFromDb.Price50 = obj.Price50;
                objFromDb.Price100 = obj.Price100;
                objFromDb.Description = obj.Description;
                objFromDb.Category = obj.Category;
                objFromDb.Author = obj.Author;
                if (objFromDb.ImageUrl != null)
                {
                    obj.ImageUrl = objFromDb.ImageUrl;
                }
            }
        }
    }
}
