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
    public class EfWhyUseDal : GenericRepository<WhyUse>, IWhyUseDal
    {
        private readonly CarShopContext _context;

        public EfWhyUseDal(CarShopContext context) : base(context)
        {
            _context = context;
        }

        public List<WhyUse> GetWhyUseWithItem()
        {
            return _context.WhyUses.Include(x => x.Items).ToList();
        }
    }
}
