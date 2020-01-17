using App.Models;
using BecamexIDC.Pattern.EF.Repositories;
using BecamexIDC.Pattern.Services;
namespace App.Services
{
public interface IProductService : IService<Product> { }
    public class ProductService : Service<Product>, IProductService
    {
        public ProductService(IRepositoryAsync<Product> repository) : base(repository)
        {
        }
    }
}