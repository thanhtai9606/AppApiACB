
using System.Threading.Tasks;
using App.Models;
using BecamexIDC.Common;
using BecamexIDC.Pattern.EF.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using App.Services;

namespace App.Controllers
{
   // [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = "Bearer")] // waring have to use this
    //[Authorize] This is not working
    public class CustomerController : ControllerBase
    {
        private readonly ICustomerService _CustomerService;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private OperationResult operationResult = new OperationResult();
        public CustomerController(ICustomerService CustomerService, IUnitOfWorkAsync unitOfWork)
        {
            _CustomerService = CustomerService;
            _unitOfWork = unitOfWork;
          
        }
        [HttpPost,Route("addCustomer")]
        public async Task<IActionResult> AddCustomer([FromBody] Customer entity)
        {
            try
            {
                _CustomerService.Add(entity);
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
        [HttpPut, Route("UpdateCustomer")]
        public async Task<IActionResult> UpdateCustomer([FromBody] Customer entity)
        {
            try
            {
                _CustomerService.Update(entity);
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
        
        [HttpDelete, Route("DeleteCustomer/{id}")]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                _CustomerService.Delete(id);
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
        [HttpGet, Route("GetCustomer")]
        public IActionResult GetCustomer()
        {
            return Ok(_CustomerService.Queryable());
        }

        [HttpGet, Route("GetCustomerById")]
        public IActionResult GetCustomerById(int id)
        {
            return Ok(_CustomerService.FindBy(x=>x.CustomerId == id));
        }


    }
}
