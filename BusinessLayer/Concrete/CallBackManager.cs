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
    public class CallBackManager : ICallBackService
    {
        private readonly ICallBackDal _callBackDal;

        public CallBackManager(ICallBackDal callBackDal)
        {
            _callBackDal = callBackDal;
        }

        public void BAdd(CallBack entity)
        {
            _callBackDal.Add(entity);
        }

        public void BDelete(CallBack entity)
        {
            _callBackDal.Delete(entity);
        }

        public CallBack BGetById(int id)
        {
            return _callBackDal.GetById(id);
        }

        public List<CallBack> BGetListAll()
        {
            return _callBackDal.GetListAll();
        }

        public void BUpdate(CallBack entity)
        {
            _callBackDal.Update(entity);
        }
    }
}
