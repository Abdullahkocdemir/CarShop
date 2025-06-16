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
    public class ContactManager : IContactService
    {
        private readonly IContactDal _contactDal;

        public ContactManager(IContactDal contactDal)
        {
            _contactDal = contactDal;
        }

        public void BAdd(Contact entity)
        {
            _contactDal.Add(entity);
        }

        public void BDelete(Contact entity)
        {
            _contactDal.Delete(entity);
        }

        public Contact BGetById(int id)
        {
            return _contactDal.GetById(id);
        }

        public List<Contact> BGetListAll()
        {
            return _contactDal.GetListAll();
        }

        public void BUpdate(Contact entity)
        {
            _contactDal.Update(entity);
        }
    }
}
