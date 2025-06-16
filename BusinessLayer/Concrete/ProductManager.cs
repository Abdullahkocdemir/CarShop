﻿using BusinessLayer.Abstract;
using DataAccessLayer.Abstract;
using EntityLayer.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Concrete
{
    public class ProductManager : IProductService
    {
        private readonly IProductDal _productDal;

        public ProductManager(IProductDal productDal)
        {
            _productDal = productDal;
        }

        public void BAdd(Product entity)
        {
            _productDal.Add(entity);
        }

        public void BUpdate(Product entity)
        {
            _productDal.Update(entity);
        }

        public void BDelete(Product entity)
        {
            _productDal.Delete(entity);
        }

        public Product BGetById(int id)
        {
            return _productDal.GetById(id);
        }

        public List<Product> BGetListAll()
        {
            return _productDal.GetListAll();
        }

        public List<Product> BGetProductWithCategory()
        {
            return _productDal.GetProductWithBrand();
        }
    }
}