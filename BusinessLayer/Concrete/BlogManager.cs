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
    public class BlogManager : IBlogService
    {
        private readonly IBlogDal _blogDal;

        public BlogManager(IBlogDal blogDal)
        {
            _blogDal = blogDal;
        }

        public void BAdd(Blog entity)
        {
            _blogDal.Add(entity);
        }

        public void BDelete(Blog entity)
        {
            _blogDal.Delete(entity);
        }

        public Blog BGetById(int id)
        {
            return _blogDal.GetById(id);
        }

        public List<Blog> BGetListAll()
        {
            return _blogDal.GetListAll();
        }

        public void BUpdate(Blog entity)
        {
            _blogDal.Update(entity);
        }
    }
}
