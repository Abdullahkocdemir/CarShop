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
    public class ShowroomManager : IShowroomService
    {
        private readonly IShowroomDal _showroomDal;

        public ShowroomManager(IShowroomDal showroomDal)
        {
            _showroomDal = showroomDal;
        }

        public void BAdd(Showroom entity)
        {
            _showroomDal.Add(entity);
        }

        public void BDelete(Showroom entity)
        {
            _showroomDal.Delete(entity);
        }

        public Showroom BGetById(int id)
        {
            return _showroomDal.GetById(id);
        }

        public List<Showroom> BGetListAll()
        {
            return _showroomDal.GetListAll();
        }

        public void BUpdate(Showroom entity)
        {
            _showroomDal.Update(entity);
        }
    }
}
