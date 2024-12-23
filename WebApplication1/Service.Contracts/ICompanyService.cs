﻿using Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.DataTransferObjects;

namespace Service.Contracts
{
    public interface ICompanyService
    {
        Task<IEnumerable<CompanyDto>> GetAllCompaniesAsync(bool trackChanges);
        Task<CompanyDto> GetCompanyAsync(Guid companyId, bool trackChanges);
        Task<CompanyDto> CreateCompanyAsync(CompanyForCreationDto company);
        Task<IEnumerable<CompanyDto>> GetByIdsAsync(IEnumerable<Guid> ids, bool trackChanges);
        Task<(IEnumerable<CompanyDto> companies, string ids)>
        CreateCompanyCollectionAsync(IEnumerable<CompanyForCreationDto> companyCollection);
        Task DeleteCompanyAsync(Guid companyId, bool trackChanges);
        Task UpdateCompanyAsync(Guid companyid, CompanyForUpdateDto companyForUpdate, bool trackChanges);
        Task UpdateCompanyAsync(Guid companyId, CompanyForSelfUpdateDto companyForSelfUpdate, bool trackChanges);
    }


    //public interface ICompanyService
    //{
    //    IEnumerable<CompanyDto> GetAllCompanies(bool trackChanges);

    //    CompanyDto GetCompany(Guid companyId, bool trackChanges);
    //    CompanyDto CreateCompany(CompanyForCreationDto company);

    //    IEnumerable<CompanyDto> GetByIds(IEnumerable<Guid> ids, bool trackChanges);

    //    (IEnumerable<CompanyDto> companies, string ids) CreateCompanyCollection(IEnumerable<CompanyForCreationDto> companyCollection);

    //    void DeleteCompany(Guid companyId, bool trackChanges);

    //    void UpdateCompany(Guid companyid, CompanyForUpdateDto companyForUpdate, bool trackChanges);
    //}
}
