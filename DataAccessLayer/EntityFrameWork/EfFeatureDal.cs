// DataAccessLayer.Concrete/EfFeatureDal.cs
using DataAccessLayer.Abstract;
using DataAccessLayer.Context;
using DataAccessLayer.Repositories;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore; // Bunu eklemeyi unutmayın!

public class EfFeatureDal : GenericRepository<Feature>, IFeatureDal
{
    private readonly CarShopContext _context; // DbContext'i doğrudan kullanmak için

    public EfFeatureDal(CarShopContext context) : base(context)
    {
        _context = context; // DbContext'i saklıyoruz
    }

    public List<Feature> GetListWithImage() // Bu metod adını kullanıyorsanız
    {
        return _context.Features.Include(f => f.FeatureImages).ToList();
    }

    public Feature GetByIdWithImage(int id) // Bu metod adını kullanıyorsanız
    {
        return _context.Features.Include(f => f.FeatureImages).FirstOrDefault(f => f.FeatureId == id)!;
    }

    // Eğer GetListAll ve GetById isimleri zaten varsa ve onları kullanmak istiyorsanız,
    // GenericRepository'nizi Include yapabilecek şekilde genişletmeniz gerekebilir
    // veya bu metodları (BGetListAll, BGetById) kendi IFeatureDal'ınızda tanımlayıp
    // Include'u burada kullanın.
}