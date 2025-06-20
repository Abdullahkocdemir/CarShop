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
    public class BlogDetailManager : IBlogDetailService
    {
        private readonly IBlogDetailsDal _blogDetailsDal;

        public BlogDetailManager(IBlogDetailsDal blogDetailsDal)
        {
            _blogDetailsDal = blogDetailsDal;
        }

        public void BAdd(BlogDetail entity)
        {
           _blogDetailsDal.Add(entity);
        }

        public void BDelete(BlogDetail entity)
        {
            _blogDetailsDal.Delete(entity);
        }

        public BlogDetail BGetById(int id)
        {
            return _blogDetailsDal.GetById(id);
        }

        public List<BlogDetail> BGetListAll()
        {
            return _blogDetailsDal.GetListAll();
        }

        public void BUpdate(BlogDetail entity)
        {
            _blogDetailsDal.Update(entity);
        }
    }
}
