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
    public class NewLatestManager : INewLatestService
    {
        private readonly INewLatestDal _newLatestDal;

        public NewLatestManager(INewLatestDal newLatestDal)
        {
            _newLatestDal = newLatestDal;
        }

        public void BAdd(NewLatest entity)
        {
            _newLatestDal.Add(entity);
        }

        public void BDelete(NewLatest entity)
        {
            _newLatestDal.Delete(entity);
        }

        public NewLatest BGetById(int id)
        {
            return _newLatestDal.GetById(id);
        }

        public List<NewLatest> BGetListAll()
        {
            return _newLatestDal.GetListAll();
        }

        public void BUpdate(NewLatest entity)
        {
            _newLatestDal.Update(entity);
        }
    }
}
