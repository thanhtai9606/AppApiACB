
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
    public class SaleController : ControllerBase
    {
        private readonly ISaleHeaderService _SaleHeaderService;
        private readonly ISaleDetailService _SaleDetailService;
        private readonly ICustomerService _CustomerService;
        private readonly IProductService _ProductService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private OperationResult operationResult = new OperationResult();
        public SaleController(ISaleHeaderService SaleHeaderService, 
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
        [HttpPost, Route("AddSale")]
        public async Task<IActionResult> AddSale(SaleHeader Sale)
        {
            try
            {
                var sale_detail =  Sale.SaleDetails.ToList();
               // var totalLine = sale_detail.Sum(x=>x.TotalAmount);//from s in sale_detail.Select(x=>x.TotalAmount).Sum();
               // Sale.TotalLine = totalLine == 0 ? 0: totalLine;
                foreach(var sd in sale_detail){
                    var p = _ProductService.Find(sd.ProductId);
                    p.Inventory -= sd.Quantity;
                    sd.WarrantyStart = DateTime.Now;
                    sd.WarrantyEnd = DateTime.Now.AddMonths(p.Warranty);
                }
                _SaleHeaderService.Add(Sale);
                int res = await _unitOfWork.SaveChangesAsync();
                if (res > 0)
                {
                    operationResult.Success = true;
                    operationResult.Message = "Added new record";
                    operationResult.Caption = "Add complete";
                }

            }
            catch (System.Exception ex)
            {
                operationResult.Success = false;
                operationResult.Message = ex.ToString();
                operationResult.Caption = "Add failed!";
            }
            return Ok(operationResult);
        }
        [HttpPut, Route("UpdateSale")]
        public async Task<IActionResult> UpdateSale(SaleHeader Sale)
        {
            try
            {
                _SaleHeaderService.Update(Sale);
                int res =  await _unitOfWork.SaveChangesAsync();
                if (res > 0)
                {
                    operationResult.Success = true;
                    operationResult.Message = "Update success";
                    operationResult.Caption = "Update complete";
                }

            }
            catch (System.Exception ex)
            {
                operationResult.Success = false;
                operationResult.Message = ex.ToString();
                operationResult.Caption = "Update failed!";
            }
            return Ok(operationResult);
        }
        
        [HttpDelete, Route("DeleteSale/{id}")]
        public async Task<IActionResult> DeleteSale(int id)
        {
            try
            { 
                _SaleHeaderService.Delete(id);
               int res =  await _unitOfWork.SaveChangesAsync();
                if (res > 0)
                {
                    operationResult.Success = true;
                    operationResult.Message = "Delete success";
                    operationResult.Caption = "Delete complete";
                }

            }
            catch (System.Exception ex)
            {
                operationResult.Success = false;
                operationResult.Message = ex.ToString();
                operationResult.Caption = "Delete failed!";
            }
            return Ok(operationResult);
        }
        [HttpGet, Route("GetSale")]
        public IActionResult GetSale()
        {
            var sale_header = _SaleHeaderService.Queryable().ToList();
            var customer = _CustomerService.Queryable().ToList();
            var product = _ProductService.Queryable().ToList();

            var result = from s in sale_header
                         join c in customer on s.CustomerId equals c.CustomerId
                         select new
                         {
                             s.SoId,
                             c.CustomerName,
                             c.Phone,       
                             s.TotalLine,                     
                             s.ModifiedDate
                         };
            return Ok(result);
        }

        [HttpGet, Route("GetSaleById")]
        public IActionResult GetSaleById(int soId)
        {
            var sale_detail = _SaleDetailService.Queryable().ToList();
            var product = _ProductService.Queryable().ToList();

            var result = from s in sale_detail
                         join p in product on s.ProductId equals p.ProductId
                         where s.SoId == soId
                         select new
                         {
                             s.SoId,
                             p.ProductName,
                             s.TotalAmount,
                             p.Warranty,    
                             s.WarrantyStart,
                             s.WarrantyEnd,   
                         };
            return Ok(result);
        }

        [HttpGet, Route("GetSaleHeaderById")]
        public IActionResult GetSaleHeaderById(int soId)
        {           
            return Ok(_SaleHeaderService.FindBy(x=>x.SoId == soId));
        }


        [HttpGet, Route("GetProducts")]
        public IActionResult GetProducts()
        {            
            var product = _ProductService.Queryable().ToList();
            var result = from p in product                        
                         select new
                         {
                             id = p.ProductId,
                             text = p.ProductName                            
                         };

            
            return Ok(product);
        }
        [HttpGet, Route("GetCustomers")]
        public IActionResult GetCustomers()
        {            
            var customer = _CustomerService.Queryable().ToList();
            var result = from c in customer                        
                         select new
                         {
                             id = c.CustomerId,
                             text = c.CustomerName                            
                         };

            
            return Ok(customer);
        }

    }
}
