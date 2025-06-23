using DataAccessLayer.Abstract;
using DataAccessLayer.Context;
using DataAccessLayer.Repositories;
using EntityLayer.Entities;
using Microsoft.EntityFrameworkCore; 

public class EfFeatureDal : GenericRepository<Feature>, IFeatureDal
{
    private readonly CarShopContext _context; 

    public EfFeatureDal(CarShopContext context) : base(context)
    {
        _context = context; 
    }

}