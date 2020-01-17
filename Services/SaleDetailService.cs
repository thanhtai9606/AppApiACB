using App.Models;
using BecamexIDC.Pattern.EF.Repositories;
using BecamexIDC.Pattern.Services;
namespace App.Services
{
public interface ISaleDetailService : IService<SaleDetail> { }
    public class SaleDetailService : Service<SaleDetail>, ISaleDetailService
    {
        public SaleDetailService(IRepositoryAsync<SaleDetail> repository) : base(repository)
        {
        }
    }
}