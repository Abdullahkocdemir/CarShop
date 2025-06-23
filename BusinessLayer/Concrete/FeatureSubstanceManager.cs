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
    public class FeatureSubstanceManager : IFeatureSubstanceService
    {
        private readonly IFeatureSubstanceDal _featureSubstanceDal;

        public FeatureSubstanceManager(IFeatureSubstanceDal featureSubstanceDal)
        {
            _featureSubstanceDal = featureSubstanceDal;
        }

        public void BAdd(FeatureSubstance entity)
        {
            _featureSubstanceDal.Add(entity);
        }

        public void BDelete(FeatureSubstance entity)
        {
            _featureSubstanceDal.Delete(entity);
        }

        public FeatureSubstance BGetById(int id)
        {
            return _featureSubstanceDal.GetById(id);
        }

        public List<FeatureSubstance> BGetListAll()
        {
            return _featureSubstanceDal.GetListAll();
        }

        public void BUpdate(FeatureSubstance entity)
        {
            _featureSubstanceDal.Update(entity);
        }
    }
}
