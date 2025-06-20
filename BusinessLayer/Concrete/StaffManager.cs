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
    public class StaffManager : IStaffService
    {
        private readonly IStaffDal _staffDal;

        public StaffManager(IStaffDal staffDal)
        {
            _staffDal = staffDal;
        }

        public void BAdd(Staff entity)
        {
            _staffDal.Add(entity);
        }

        public void BDelete(Staff entity)
        {
            _staffDal.Delete(entity);
        }

        public Staff BGetById(int id)
        {
            return _staffDal.GetById(id);
        }

        public List<Staff> BGetListAll()
        {
            return _staffDal.GetListAll();
        }

        public void BUpdate(Staff entity)
        {
            _staffDal.Update(entity);
        }
    }
}
