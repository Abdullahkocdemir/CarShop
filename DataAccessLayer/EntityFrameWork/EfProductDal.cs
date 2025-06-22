using DataAccessLayer.Abstract;
using DataAccessLayer.Context;
using DataAccessLayer.Repositories;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessLayer.EntityFrameWork
{
    public class EfProductDal : GenericRepository<Product>, IProductDal
    {
        private readonly CarShopContext _context;

        public EfProductDal(CarShopContext context) : base(context)
        {
            _context = context;
        }

        public List<Product> GetProductWithBrand()
        {
            var values = _context.Products.Include(x => x.Brand).ToList();
            return values;
        }

        public List<Product> GetProductsWithDetails()
        {
            var values = _context.Products
                .Include(x => x.Brand)
                .Include(x => x.Color)
                .Include(x => x.Model)
                .Include(x => x.Images)
                .ToList();
            return values;
        }

        public Product GetProductByIdWithDetails(int id)
        {
            var value = _context.Products
                .Where(x => x.ProductId == id)
                .Include(x => x.Brand)
                .Include(x => x.Color)
                .Include(x => x.Model)
                .Include(x => x.Images)
                .FirstOrDefault();
            return value!;
        }
    }
}