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
    public class EfCallBackTitleDal : GenericRepository<CallBackTitle>, ICallBackTitleDal
    {
        public EfCallBackTitleDal(CarShopContext context) : base(context)
        {
        }
    }
}
