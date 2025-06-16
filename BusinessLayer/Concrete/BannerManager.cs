using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class BannerManager : IBannerService
    {
        private readonly IBannerDal _bannerDal;

        public BannerManager(IBannerDal bannerDal)
        {
            _bannerDal = bannerDal;
        }

        public void BAdd(Banner entity)
        {
            _bannerDal.Add(entity);
        }

        public void BDelete(Banner entity)
        {
            _bannerDal.Delete(entity);
        }

        public Banner BGetById(int id)
        {
            return _bannerDal.GetById(id);
        }

        public List<Banner> BGetListAll()
        {
            return _bannerDal.GetListAll();
        }

        public void BUpdate(Banner entity)
        {
            _bannerDal.Update(entity);
        }
    }
}
