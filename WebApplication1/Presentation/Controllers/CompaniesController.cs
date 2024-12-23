﻿using Microsoft.AspNetCore.Mvc;
using Presentation.ActionFilters;
using Presentation.ModelBinders;
using Service.Contracts;
using Shared.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Presentation.Controllers
{
    [Route("api/companies")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly IServiceManager _service;
        public CompaniesController(IServiceManager service) => _service = service;

        [HttpOptions]
        public IActionResult GetCompaniesOptions()
        {
            Response.Headers.Add("Allow", "GET, OPTIONS, POST, PUT, DELETE");
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            var companies = await
            _service.CompanyService.GetAllCompaniesAsync(trackChanges: false);
            return Ok(companies);
        }

        //[HttpGet("{id:guid}")]
        [HttpGet("{id:guid}", Name = "CompanyById")]
        public async Task<IActionResult> GetCompany(Guid id)
        {
            var company = await _service.CompanyService.GetCompanyAsync(id, trackChanges: false);
            return Ok(company);
        }

        [HttpPost]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> CreateCompany([FromBody] CompanyForCreationDto company)
        {
            if (company is null)
                return BadRequest("CompanyForCreationDto object is null");
            var createdCompany = await _service.CompanyService.CreateCompanyAsync(company);
            return CreatedAtRoute("CompanyById", new { id = createdCompany.Id }, createdCompany);
        }

        [HttpGet("collection/({ids})", Name = "CompanyCollection")]
        public async Task<IActionResult> GetCompanyCollection([ModelBinder(BinderType = typeof(ArrayModelBinder))] IEnumerable<Guid> ids)
        {
            var companies = await _service.CompanyService.GetByIdsAsync(ids, trackChanges:
            false);

            return Ok(companies);
        }

        [HttpPost("collection")]
        public async Task<IActionResult> CreateCompanyCollection([FromBody] IEnumerable<CompanyForCreationDto> companyCollection)
        {
            var result = await _service.CompanyService.CreateCompanyCollectionAsync(companyCollection);
            return CreatedAtRoute("CompanyCollection", new { result.ids },
            result.companies);
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteCompany(Guid id)
        {
            await _service.CompanyService.DeleteCompanyAsync(id, trackChanges: false);

            return NoContent();
        }

        [HttpPut("{id:guid}/with-employees", Name = nameof(UpdateCompanyWithEmployees))]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateCompanyWithEmployees(Guid id, [FromBody] CompanyForUpdateDto company)
        {
            if (company is null)
                return BadRequest("CompanyForUpdateDto object is null");
            await _service.CompanyService.UpdateCompanyAsync(id, company, trackChanges: true);
            return NoContent();
        }

        [HttpPut("{id:guid}", Name =nameof(UpdateCompany))]
        [ServiceFilter(typeof(ValidationFilterAttribute))]
        public async Task<IActionResult> UpdateCompany(Guid id, [FromBody] CompanyForSelfUpdateDto company)
        {
            if (company is null)
                return BadRequest("CompanyForUpdateDto object is null");
            await _service.CompanyService.UpdateCompanyAsync(id, company, trackChanges: true);
            return NoContent();
        }


        //[HttpGet]
        //public IActionResult GetCompanies()
        //{
        //    try
        //    {
        //        var companies =
        //        _service.CompanyService.GetAllCompanies(trackChanges: false);
        //        return Ok(companies);
        //    }
        //    catch
        //    {
        //        return StatusCode(500, "Internal server error");
        //    }
        //}
    }


    //public class CompaniesController : ControllerBase
    //{
    //    private readonly IServiceManager _service;
    //    public CompaniesController(IServiceManager service) => _service = service;

    //    [HttpGet]
    //    public IActionResult GetCompanies()
    //    {
    //        //throw new Exception("Test");
    //        var companies = _service.CompanyService.GetAllCompanies(trackChanges: false);
    //        return Ok(companies);
    //    }

    //    //[HttpGet("{id:guid}")]
    //    [HttpGet("{id:guid}", Name = "CompanyById")]
    //    public IActionResult GetCompany(Guid id)
    //    {
    //        var company = _service.CompanyService.GetCompany(id, trackChanges: false);
    //        return Ok(company);
    //    }

    //    [HttpPost]
    //    public IActionResult CreateCompany([FromBody] CompanyForCreationDto company)
    //    {
    //        if (company is null)
    //            return BadRequest("CompanyForCreationDto object is null");
    //        var createdCompany = _service.CompanyService.CreateCompany(company);
    //        return CreatedAtRoute("CompanyById", new { id = createdCompany.Id }, createdCompany);
    //    }

    //    [HttpGet("collection/({ids})", Name = "CompanyCollection")]
    //    public IActionResult GetCompanyCollection(IEnumerable<Guid> ids)
    //    {
    //        var companies = _service.CompanyService.GetByIds(ids, trackChanges: false);
    //        return Ok(companies);
    //    }

    //    [HttpPost("collection")]
    //    public IActionResult CreateCompanyCollection([FromBody]IEnumerable<CompanyForCreationDto> companyCollection)
    //    {
    //        var result =
    //        _service.CompanyService.CreateCompanyCollection(companyCollection);
    //        return CreatedAtRoute("CompanyCollection", new { result.ids },
    //        result.companies);
    //    }

    //    [HttpDelete("{id:guid}")]
    //    public IActionResult DeleteCompany(Guid id)
    //    {
    //        _service.CompanyService.DeleteCompany(id, trackChanges: false);
    //        return NoContent();
    //    }

    //    [HttpPut("{id:guid}")]
    //    public IActionResult UpdateCompany(Guid id, [FromBody] CompanyForUpdateDto company)
    //    {
    //        if (company is null)
    //            return BadRequest("CompanyForUpdateDto object is null");
    //        _service.CompanyService.UpdateCompany(id, company, trackChanges: true);
    //        return NoContent();
    //    }


    //    //[HttpGet]
    //    //public IActionResult GetCompanies()
    //    //{
    //    //    try
    //    //    {
    //    //        var companies =
    //    //        _service.CompanyService.GetAllCompanies(trackChanges: false);
    //    //        return Ok(companies);
    //    //    }
    //    //    catch
    //    //    {
    //    //        return StatusCode(500, "Internal server error");
    //    //    }
    //    //}
    //}
}
