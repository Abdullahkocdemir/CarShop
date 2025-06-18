using DataAccessLayer.Abstract;
using DataAccessLayer.Context; // CarShopContext'in bulunduğu namespace
using DataAccessLayer.Repositories; // GenericRepository'nin bulunduğu namespace
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq; // .Where() metodu için bu using'i ekle
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.EntityFrameWork
{
    public class EfFeatureImageDal : GenericRepository<FeatureImage>, IFeatureImageDal
    {
        private readonly CarShopContext _context;

        public EfFeatureImageDal(CarShopContext context) : base(context)
        {
            _context = context;
        }

        // Bu metot IFeatureImageDal arayüzünde artık tanımlı olduğu için hata vermeyecek.
        public List<FeatureImage> GetImagesByFeatureId(int featureId)
        {
            return _context.FeatureImages
                           .Where(x => x.FeatureId == featureId)
                           .ToList();
        }
    }
}