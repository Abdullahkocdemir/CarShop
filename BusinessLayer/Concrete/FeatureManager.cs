﻿using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Entities;
using System.Collections.Generic;

namespace BusinessLayer.Concrete
{
    public class FeatureManager : IFeatureService 
    {
        private readonly IFeatureDal _featureDal;

        public FeatureManager(IFeatureDal featureDal)
        {
            _featureDal = featureDal;
        }

        public void BAdd(Feature entity)
        {
           _featureDal.Add(entity);
        }

        public void BDelete(Feature entity)
        {
            _featureDal.Delete(entity);
        }

        public void BUpdate(Feature entity)
        {
            _featureDal.Update(entity);
        }

        public Feature BGetById(int id)
        {
            return _featureDal.GetById(id);
        }

        public List<Feature> BGetListAll()
        {
            return _featureDal.GetListAll();
        }
    }
}