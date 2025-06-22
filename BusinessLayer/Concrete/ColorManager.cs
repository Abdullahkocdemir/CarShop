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
    public class ColorManager : IColorService
    {
        private readonly IColorDal _colorDal;

        public ColorManager(IColorDal colorDal)
        {
            _colorDal = colorDal;
        }

        public void BAdd(Color entity)
        {
            _colorDal.Add(entity);
        }

        public void BDelete(Color entity)
        {
            _colorDal.Delete(entity);
        }

        public Color BGetById(int id)
        {
            return _colorDal.GetById(id);
        }

        public List<Color> BGetListAll()
        {
            return _colorDal.GetListAll();
        }

        public void BUpdate(Color entity)
        {
            _colorDal.Update(entity);
        }
    }
}
