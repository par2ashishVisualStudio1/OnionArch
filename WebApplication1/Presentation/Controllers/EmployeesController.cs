﻿using Entities.Exceptions;
using Entities.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Route("api/companies/{companyId}/employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IServiceManager _service;
        public EmployeesController(IServiceManager service) => _service = service;

        //[HttpGet]
        //public async Task<IActionResult> GetEmployeesForCompany(Guid companyId)
        //{
        //    var employees = await _service.EmployeeService.GetEmployeesAsync(companyId, trackChanges:
        //    false);
        //    return Ok(employees);
        //}

        //[HttpGet("{id:guid}")]
        [HttpGet("{id:guid}/allemployees", Name = "GetEmployeeForCompany")]
        public async Task<IActionResult> GetEmployeeForCompany(Guid companyId, Guid id)
        {
            var employee = await _service.EmployeeService.GetEmployeeByIdAsync(companyId, id, trackChanges: false);
            return Ok(employee);
        }
        
        [HttpGet("{id:guid}/paging", Name = nameof(GetEmployeesForCompanyPaging))]
        [HttpHead]
        public async Task<IActionResult> GetEmployeesForCompanyPaging(Guid companyId,[FromQuery] EmployeeParameters employeeParameters)
        {
            //var employees = await _service.EmployeeService.GetEmployeesAsync(companyId, employeeParameters, trackChanges: false);
            //return Ok(employees);
            if (!employeeParameters.ValidAgeRange)
                throw new MaxAgeRangeBadRequestException();
            var pagedResult = await _service.EmployeeService.GetEmployeesAsync(companyId, employeeParameters, trackChanges: false);
            Response.Headers.Add("X-Pagination",
            JsonSerializer.Serialize(pagedResult.metaData));
            return Ok(pagedResult.employees);
        }

        //[HttpPost]
        //public IActionResult CreateEmployeeForCompany(Guid companyId, [FromBody]EmployeeForCreationDto employee)
        //{
        //    if (employee is null)
        //        return BadRequest("EmployeeForCreationDto object is null");
        //    var employeeToReturn = _service.EmployeeService.CreateEmployeeForCompany(companyId, employee, trackChanges:
        //    false);
        //    return CreatedAtRoute("GetEmployeeForCompany", new
        //    {
        //        companyId,
        //        id = employeeToReturn.Id
        //    },employeeToReturn);
        //}

        [HttpPost]
        
        public async Task<IActionResult> CreateEmployeeForCompany(Guid companyId, [FromBody] EmployeeForCreationDto employee)
        {
            //if (employee is null)
            //    return BadRequest("EmployeeForCreationDto object is null");
            //if (!ModelState.IsValid)
            //    return UnprocessableEntity(ModelState);
            var employeeToReturn = await _service.EmployeeService.CreateEmployeeForCompanyAsync(companyId, employee, trackChanges: false);
            return CreatedAtRoute("GetEmployeeForCompany", new
            {
                companyId,
                id = employeeToReturn.Id
            }, employeeToReturn);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteEmployeeForCompany(Guid companyId, Guid id)
        {
            await _service.EmployeeService.DeleteEmployeeForCompanyAsync(companyId, id, trackChanges:false);
            return NoContent();
        }

        [HttpPut("{id:guid}")]

        public async Task<IActionResult> UpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] EmployeeForUpdateDto employee)
        {
            //if (employee is null)
            //    return BadRequest("EmployeeForUpdateDto object is null");
            //if (!ModelState.IsValid)
            //    return UnprocessableEntity(ModelState);
            await _service.EmployeeService.UpdateEmployeeForCompanyAsync(companyId, id, employee, compTrackChanges: false, empTrackChanges: true);
            return NoContent();
        }

        [HttpPatch("{id:guid}")]
        public async Task<IActionResult> PartiallyUpdateEmployeeForCompany(Guid companyId, Guid id, [FromBody] JsonPatchDocument<EmployeeForUpdateDto> patchDoc)
        {
            if (patchDoc is null)
                return BadRequest("patchDoc object sent from client is null.");
            var result = await _service.EmployeeService.GetEmployeeForPatchAsync(companyId, id, compTrackChanges: false, empTrackChanges: true);
            patchDoc.ApplyTo(result.employeeToPatch);
            TryValidateModel(result.employeeToPatch);
            if (!ModelState.IsValid)
                return UnprocessableEntity(ModelState);
            await _service.EmployeeService.SaveChangesForPatchAsync(result.employeeToPatch, result.employeeEntity);
            return NoContent();
        }

    }
}
