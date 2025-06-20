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
    public class PartnerManager : IPartnerService
    {
        private readonly IPartnerDal _partnerDal;

        public PartnerManager(IPartnerDal partnerDal)
        {
            _partnerDal = partnerDal;
        }

        public void BAdd(Partner entity)
        {
            _partnerDal.Add(entity);
        }

        public void BDelete(Partner entity)
        {
            _partnerDal.Delete(entity);
        }

        public Partner BGetById(int id)
        {
            return _partnerDal.GetById(id);
        }

        public List<Partner> BGetListAll()
        {
            return _partnerDal.GetListAll();
        }

        public void BUpdate(Partner entity)
        {
            _partnerDal.Update(entity);
        }
    }
}
