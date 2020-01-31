
using System;
using System.Linq;
using System.Threading.Tasks;
using App.Models;
using BecamexIDC.Common;
using BecamexIDC.Pattern.EF.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using App.Services;

namespace App.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
   // [Authorize(AuthenticationSchemes = "Bearer")] // waring have to use this
    //[Authorize] This is not working
    public class DashboardController : ControllerBase
    {
        private readonly ISaleHeaderService _SaleHeaderService;
        private readonly ISaleDetailService _SaleDetailService;
        private readonly ICustomerService _CustomerService;
        private readonly IProductService _ProductService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private OperationResult operationResult = new OperationResult();
        public DashboardController(ISaleHeaderService SaleHeaderService, 
                                ICustomerService customerService,
                                IProductService productService,
                                ISaleDetailService saleDetailService,
                              IUnitOfWorkAsync unitOfWork)
        {
            _SaleHeaderService = SaleHeaderService;
            _SaleDetailService = saleDetailService;
            _unitOfWork = unitOfWork;
            _CustomerService = customerService;
            _ProductService = productService;
          
        }
        [HttpGet, Route("GetSaleOrder")]
        public IActionResult GetSaleOrder(string options)
        {
            var result = _SaleHeaderService.Queryable().ToList();
            int count = 0;
            switch(options){
                case "day":
                    count = result.Where(p=>p.ModifiedDate == DateTime.Today).Count();
                    break;
                case "month":
                    count = result.Where(p=>p.ModifiedDate.Month == DateTime.Today.Month).Count();
                    break;
                case "year":
                    count = result.Where(p=>p.ModifiedDate.Year == DateTime.Today.Year).Count();
                    break;
                default:
                    count = result.Where(p=>p.ModifiedDate == DateTime.Today).Count();
                    break;
            }
            return Ok(count);
        }
        [HttpGet, Route("GetProfit")]
        public IActionResult GetProfit(string options)
        {
            var result = _SaleHeaderService.Queryable().ToList();
            int sum = 0;
            switch(options){
                case "day":
                    sum = result.Where(p=>p.ModifiedDate == DateTime.Today).Sum(p=>p.TotalLine);
                    break;
                case "month":
                    sum = result.Where(p=>p.ModifiedDate.Month == DateTime.Today.Month).Sum(p=>p.TotalLine);
                    break;
                case "year":
                    sum = result.Where(p=>p.ModifiedDate.Year == DateTime.Today.Year).Sum(p=>p.TotalLine);
                    break;
                default:
                    sum = result.Where(p=>p.ModifiedDate == DateTime.Today).Sum(p=>p.TotalLine);
                    break;
            }
            return Ok(sum);
        }
        [HttpGet, Route("GetCustomer")]
        public IActionResult GetCustomer()
        {
            return Ok(_CustomerService.Queryable().ToList().Count);
        }

        [HttpGet, Route("GetInventory")]
        public IActionResult GetInventory()
        {
            return Ok(_ProductService.Queryable().ToList().Sum(p=>p.Inventory));
        }

    }
}
