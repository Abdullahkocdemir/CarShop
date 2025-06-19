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
    public class WhyUseReasonManager : IWhyUseReasonService
    {
        private readonly IWhyUseReasonDal _whyUseReasonDal;

        public WhyUseReasonManager(IWhyUseReasonDal whyUseReasonDal)
        {
            _whyUseReasonDal = whyUseReasonDal;
        }

        public void BAdd(WhyUseReason entity)
        {
            _whyUseReasonDal.Add(entity);
        }

        public void BDelete(WhyUseReason entity)
        {
            _whyUseReasonDal.Delete(entity);
        }

        public WhyUseReason BGetById(int id)
        {
            return _whyUseReasonDal.GetById(id);
        }

        public List<WhyUseReason> BGetListAll()
        {
            return _whyUseReasonDal.GetListAll();
        }

        public void BUpdate(WhyUseReason entity)
        {
            _whyUseReasonDal.Update(entity);
        }
    }
}
