using DataAccessLayer.Abstract;
using DataAccessLayer.Context;
using DataAccessLayer.Repositories;
using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.EntityFrameWork
{
    public class EfCallBackDal : GenericRepository<CallBack>, ICallBackDal
    {
        public EfCallBackDal(CarShopContext context) : base(context)
        {
        }
    }
}
