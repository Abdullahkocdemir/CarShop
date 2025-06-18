// DataAccessLayer.Abstract/IFeatureDal.cs
using EntityLayer.Entities;
using System.Collections.Generic;

namespace DataAccessLayer.Abstract
{
    public interface IFeatureDal : IGenericDal<Feature>
    {
        List<Feature> GetListWithImage();

        Feature GetByIdWithImage(int id);
    }
}