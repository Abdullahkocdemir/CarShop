using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccessLayer.Abstract
{
    public interface IFeatureImageDal : IGenericDal<FeatureImage>
    {
        List<FeatureImage> GetImagesByFeatureId(int featureId);
    }
}
