using DataAccessLayer.Abstract;
using DataAccessLayer.Context;
using DataAccessLayer.Repositories;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore; // Bunu ekleyin

namespace DataAccessLayer.EntityFrameWork
{
    public class EfWhyUseDal : GenericRepository<WhyUse>, IWhyUseDal
    {
        private readonly CarShopContext _context; // Context'i kullanmak için ekleyin

        public EfWhyUseDal(CarShopContext context) : base(context)
        {
            _context = context; // Context'i atayın
        }

        // WhyUseReasons'ı da içeren WhyUse listesini getiren metot
        public List<WhyUse> GetListAllWithReasons()
        {
            return _context.WhyUses.Include(w => w.WhyUseReasons).ToList();
        }

        // Belirli bir WhyUse'u WhyUseReasons ile birlikte getiren metot
        public WhyUse? GetByIdWithReasons(int id)
        {
            return _context.WhyUses.Include(w => w.WhyUseReasons).FirstOrDefault(w => w.WhyUseId == id);
        }
    }
}