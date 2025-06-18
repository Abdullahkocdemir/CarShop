using EntityLayer.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BusinessLayer.Abstract
{
    public interface IFeatureImageService : IGenericService<FeatureImage>
    {
        List<FeatureImage> BGetImagesByFeatureId(int featureId);
    }
}
