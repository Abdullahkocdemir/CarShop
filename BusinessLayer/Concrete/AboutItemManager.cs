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
    public class AboutItemManager : IAboutItemService
    {
        private readonly IAboutItemDal _aboutItemDal;

        public AboutItemManager(IAboutItemDal aboutItemDal)
        {
            _aboutItemDal = aboutItemDal;
        }

        public void BAdd(AboutItem entity)
        {
            _aboutItemDal.Add(entity);
        }

        public void BDelete(AboutItem entity)
        {
            _aboutItemDal.Delete(entity);
        }

        public AboutItem BGetById(int id)
        {
            return _aboutItemDal.GetById(id);
        }

        public List<AboutItem> BGetListAll()
        {
            return _aboutItemDal.GetListAll();
        }

        public void BUpdate(AboutItem entity)
        {
            _aboutItemDal.Update(entity);
        }
    }
}
