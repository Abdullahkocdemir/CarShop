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
    public class CalltoActionManager : ICalltoActionService
    {
        private readonly ICalltoActionDal _calltoActionDal;

        public CalltoActionManager(ICalltoActionDal calltoActionDal)
        {
            _calltoActionDal = calltoActionDal;
        }

        public void BAdd(CalltoAction entity)
        {
            _calltoActionDal.Add(entity);
        }

        public void BDelete(CalltoAction entity)
        {
            _calltoActionDal.Delete(entity);
        }

        public CalltoAction BGetById(int id)
        {
            return _calltoActionDal.GetById(id);
        }

        public List<CalltoAction> BGetListAll()
        {
            return _calltoActionDal.GetListAll();
        }

        public void BUpdate(CalltoAction entity)
        {
            _calltoActionDal.Update(entity);
        }
    }
}
