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
    public class ServiceManager : IServiceService
    {
        private readonly IServiceDal _serviceDal;

        public ServiceManager(IServiceDal serviceDal)
        {
            _serviceDal = serviceDal;
        }

        public void BAdd(Service entity)
        {
            _serviceDal.Add(entity);
        }

        public void BDelete(Service entity)
        {
            _serviceDal.Delete(entity);
        }

        public Service BGetById(int id)
        {
            return _serviceDal.GetById(id);
        }

        public List<Service> BGetListAll()
        {
            return _serviceDal.GetListAll();
        }

        public void BUpdate(Service entity)
        {
            _serviceDal.Update(entity);
        }
    }
}
