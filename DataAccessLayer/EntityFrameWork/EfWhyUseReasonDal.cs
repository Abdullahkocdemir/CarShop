using DataAccessLayer.Abstract;
using DataAccessLayer.Context;
using DataAccessLayer.Repositories;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.EntityFrameWork
{
    public class EfWhyUseReasonDal : GenericRepository<WhyUseReason>, IWhyUseReasonDal
    {
        private readonly CarShopContext _context;

        public EfWhyUseReasonDal(CarShopContext context) : base(context)
        {
            _context = context;
        }

    }
}
