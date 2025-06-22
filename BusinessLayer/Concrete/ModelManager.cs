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
    public class ModelManager : IModelService
    {
        private readonly IModelDal _modelDal;

        public ModelManager(IModelDal modelDal)
        {
            _modelDal = modelDal;
        }

        public void BAdd(Model entity)
        {
            _modelDal.Add(entity);
        }

        public void BDelete(Model entity)
        {
            _modelDal.Delete(entity);
        }

        public Model BGetById(int id)
        {
            return _modelDal.GetById(id);
        }

        public List<Model> BGetListAll()
        {
            return _modelDal.GetListAll();
        }

        public void BUpdate(Model entity)
        {
            _modelDal.Update(entity);
        }
    }
}
