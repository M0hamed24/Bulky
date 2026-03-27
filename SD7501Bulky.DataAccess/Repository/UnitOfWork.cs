using BulkyWeb.Data;
using SD7501Bulky.DataAccess.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SD7501Bulky.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _db;

    public ICategoryRepository Category { get; private set; }

    public UnitOfWork(ApplicationDbContext db)
    {
        _db = db;
    }

    public void Save()
    {
        _db.SaveChanges();
    }
}
}
