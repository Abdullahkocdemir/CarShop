using BusinessLayer.Abstract;
using DataAccessLayer.Abstract; // IFeatureImageDal arayüzü için gerekli
using EntityLayer.Entities;
using System.Collections.Generic;
using System.Linq; // Listeler üzerinde işlem yapmak için (örneğin Where metodu)

namespace BusinessLayer.Concrete
{
    public class FeatureImageManager : IFeatureImageService
    {
        private readonly IFeatureImageDal _featureImageDal;
        public FeatureImageManager(IFeatureImageDal featureImageDal)
        {
            _featureImageDal = featureImageDal;
        }

        public void BAdd(FeatureImage entity)
        {
            _featureImageDal.Add(entity);
        }

        public void BDelete(FeatureImage entity)
        {
            _featureImageDal.Delete(entity);
        }

        public FeatureImage BGetById(int id)
        {
            return _featureImageDal.GetById(id);
        }

        public List<FeatureImage> BGetListAll()
        {
            return _featureImageDal.GetListAll();
        }

        public void BUpdate(FeatureImage entity)
        {
            _featureImageDal.Update(entity);
        }


        public List<FeatureImage> BGetImagesByFeatureId(int featureId)
        {
            return _featureImageDal.GetImagesByFeatureId(featureId);
        }
    }
}