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
    public class WhyUseManager : IWhyUseService
    {
        private readonly IWhyUseDal _whyUseDal;

        public WhyUseManager(IWhyUseDal whyUseDal)
        {
            _whyUseDal = whyUseDal;
        }

        public void BAdd(WhyUse entity)
        {
            _whyUseDal.Add(entity);
        }

        public void BDelete(WhyUse entity)
        {
            _whyUseDal.Delete(entity);
        }

        public WhyUse BGetById(int id)
        {
            return _whyUseDal.GetById(id);
        }

        public List<WhyUse> BGetListAll()
        {
            return _whyUseDal.GetListAll();
        }

        public void BUpdate(WhyUse entity)
        {
            _whyUseDal.Update(entity);
        }

        // Yeni eklenen metotlar
        public List<WhyUse> BGetListAllWithReasons()
        {
            return _whyUseDal.GetListAllWithReasons();
        }

        public WhyUse? BGetByIdWithReasons(int id)
        {
            return _whyUseDal.GetByIdWithReasons(id);
        }
    }
}