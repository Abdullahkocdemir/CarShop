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
    public class AboutFeatureManager : IAboutFeatureService
    {
        private readonly IAboutFeatureDal _aboutFeatureDal;

        public AboutFeatureManager(IAboutFeatureDal aboutFeatureDal)
        {
            _aboutFeatureDal = aboutFeatureDal;
        }

        public void BAdd(AboutFeature entity)
        {
            _aboutFeatureDal.Add(entity);
        }

        public void BDelete(AboutFeature entity)
        {
            _aboutFeatureDal.Delete(entity);
        }

        public AboutFeature BGetById(int id)
        {
            return _aboutFeatureDal.GetById(id);
        }

        public List<AboutFeature> BGetListAll()
        {
            return _aboutFeatureDal.GetListAll();
        }

        public void BUpdate(AboutFeature entity)
        {
            _aboutFeatureDal.Update(entity);
        }
    }
}
