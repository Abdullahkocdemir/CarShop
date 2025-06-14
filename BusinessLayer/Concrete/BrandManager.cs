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
    public class BrandManager : IBrandService
    {
        private readonly IBrandDal _brandDal;

        public BrandManager(IBrandDal brandDal)
        {
            _brandDal = brandDal;
        }

        public void BAdd(Brand entity)
        {
            _brandDal.Add(entity);
        }

        public void BDelete(Brand entity)
        {
            _brandDal.Delete(entity);
        }

        public Brand BGetById(int id)
        {
            return _brandDal.GetById(id);
        }

        public List<Brand> BGetListAll()
        {
            return _brandDal.GetListAll();
        }

        public void BUpdate(Brand entity)
        {
            _brandDal.Update(entity);
        }
    }
}
