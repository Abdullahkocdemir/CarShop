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
    public class BroadcastManager : IBroadcastService
    {
        private readonly IBroadcastDal _broadcastDal;

        public BroadcastManager(IBroadcastDal broadcastDal)
        {
            _broadcastDal = broadcastDal;
        }

        public void BAdd(Broadcast entity)
        {
            _broadcastDal.Add(entity);
        }

        public void BDelete(Broadcast entity)
        {
            _broadcastDal.Delete(entity);
        }

        public Broadcast BGetById(int id)
        {
            return _broadcastDal.GetById(id);
        }

        public List<Broadcast> BGetListAll()
        {
            return _broadcastDal.GetListAll();
        }

        public void BUpdate(Broadcast entity)
        {
            _broadcastDal.Update(entity);
        }
    }
}
