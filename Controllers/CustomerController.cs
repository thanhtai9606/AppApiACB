
using System.Threading.Tasks;
using App.Models;
using BecamexIDC.Common;
using BecamexIDC.Pattern.EF.UnitOfWork;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using App.Services;
using App.Routes;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace App.Controllers
{

    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        [HttpPost(ApiRoutes.Business.addCustomer)]
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
        [HttpPut, Route(ApiRoutes.Business.updateCustomer)]
        public async Task<IActionResult> UpdateCustomer([FromBody] Customer entity)
        {
            try
            {
                _CustomerService.Update(entity);
                int res = await _unitOfWork.SaveChangesAsync();
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

        [HttpDelete, Route(ApiRoutes.Business.deleteCustomer)]
        public async Task<IActionResult> DeleteCustomer(int id)
        {
            try
            {
                _CustomerService.Delete(id);
                int res = await _unitOfWork.SaveChangesAsync();
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
        [HttpGet, Route(ApiRoutes.Business.getCustomer)]
        public IActionResult GetCustomer()
        {
            return Ok(_CustomerService.Queryable());
        }

        [HttpGet, Route(ApiRoutes.Business.findCustomerById)]
        public IActionResult GetCustomerById(int id)
        {
            return Ok(_CustomerService.FindBy(x => x.CustomerId == id));
        }


    }
}
