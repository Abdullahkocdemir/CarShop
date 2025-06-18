// BusinessLayer.Abstract/IFeatureService.cs
using EntityLayer.Entities;
using System.Collections.Generic;

namespace BusinessLayer.Abstract
{
    public interface IFeatureService : IGenericService<Feature>
    {
        // Resimlerle birlikte tüm Feature'ları getiren metot
        List<Feature> BGetListWithImage(); // Genellikle servis katmanında 'B' öneki kullanılır

        // Resimlerle birlikte belirli bir Feature'ı ID'ye göre getiren metot
        Feature BGetByIdWithImage(int id); // Genellikle servis katmanında 'B' öneki kullanılır
    }
}