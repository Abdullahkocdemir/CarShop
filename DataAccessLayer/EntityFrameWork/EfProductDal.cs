using DataAccessLayer.Abstract;
using DataAccessLayer.Context;
using DataAccessLayer.Repositories;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            var values = _context.Products.Include(x => x.Brand)
                .ToList();
            return values;
        }
    }
}
