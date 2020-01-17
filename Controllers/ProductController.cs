
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
    public class ProductController : ControllerBase
    {
        private readonly IProductService _ProductService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private OperationResult operationResult = new OperationResult();
        public ProductController(IProductService ProductService, IUnitOfWorkAsync unitOfWork)
        {
            _ProductService = ProductService;
            _unitOfWork = unitOfWork;
          
        }
        [HttpPost, Route("AddProduct")]
        public async Task<IActionResult> AddProduct(Product Product)
        {
            try
            {                
                _ProductService.Add(Product);
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
        [HttpPut, Route("UpdateProduct")]
        public async Task<IActionResult> UpdateProduct(Product Product)
        {
            try
            {
                _ProductService.Update(Product);
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
        
        [HttpDelete, Route("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                _ProductService.Delete(id);
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
        [HttpGet, Route("GetProduct")]
        public IActionResult GetProduct()
        {
            return Ok(_ProductService.Queryable().ToList().Where(p=>p.Inventory >0));
        }


    }
}
