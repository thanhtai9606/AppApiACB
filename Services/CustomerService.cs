using App.Models;
using BecamexIDC.Pattern.EF.Repositories;
using BecamexIDC.Pattern.Services;
namespace App.Services
{
public interface ICustomerService : IService<Customer> { }
    public class CustomerService : Service<Customer>, ICustomerService
    {
        public CustomerService(IRepositoryAsync<Customer> repository) : base(repository)
        {
        }
    }
}