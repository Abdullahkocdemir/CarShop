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
    public class CallBackTitleManager : ICallBackTitleService
    {
        private readonly ICallBackTitleDal _callBackTitleDal;

        public CallBackTitleManager(ICallBackTitleDal callBackTitleDal)
        {
            _callBackTitleDal = callBackTitleDal;
        }

        public void BAdd(CallBackTitle entity)
        {
            _callBackTitleDal.Add(entity);
        }

        public void BDelete(CallBackTitle entity)
        {
            _callBackTitleDal.Delete(entity);
        }

        public CallBackTitle BGetById(int id)
        {
            return _callBackTitleDal.GetById(id);
        }

        public List<CallBackTitle> BGetListAll()
        {
            return _callBackTitleDal.GetListAll();
        }

        public void BUpdate(CallBackTitle entity)
        {
            _callBackTitleDal.Update(entity);
        }
    }
}
