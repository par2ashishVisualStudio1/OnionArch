﻿using AutoMapper;
using Contracts;
using Entities.Exceptions;
using Entities.Models;
using Service.Contracts;
using Shared.DataTransferObjects;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    internal sealed class EmployeeService : IEmployeeService
    {
        private readonly IRepositoryManager _repository;
        //private readonly ILoggerManager _logger;
        private readonly IMapper _mapper;

        public EmployeeService(IRepositoryManager repository, IMapper mapper)//, ILoggerManager logger)
        {
            _repository = repository;
            //_logger = logger;
            _mapper = mapper;
        }

        //public async Task<IEnumerable<EmployeeDto>> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges)
        //{
        //    await CheckIfCompanyExists(companyId, trackChanges);
        //    var employeesFromDb = await _repository.Employee
        //    .GetEmployeesAsync(companyId, employeeParameters, trackChanges);
        //    var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);
        //    return employeesDto;
        //}

        public async Task<(IEnumerable<EmployeeDto> employees, MetaData metaData)> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges)
        {
            await CheckIfCompanyExists(companyId, trackChanges);
            var employeesWithMetaData = await _repository.Employee.GetEmployeesAsync(companyId, employeeParameters, trackChanges);
            var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesWithMetaData);
            return (employees: employeesDto, metaData: employeesWithMetaData.MetaData);
        }

        public async Task<EmployeeDto> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges)
        {
            await CheckIfCompanyExists(companyId, trackChanges);
            var employeeDb = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id,
            trackChanges);
            var employee = _mapper.Map<EmployeeDto>(employeeDb);
            return employee;
        }


        //public async Task<IEnumerable<EmployeeDto>> GetEmployeesAsync(Guid companyId, bool trackChanges)
        //{
        //    //var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
        //    //if (company is null)
        //    //    throw new CompanyNotFoundException(companyId);
        //    await CheckIfCompanyExists(companyId, trackChanges);
        //    var employeesFromDb = await _repository.Employee.GetEmployeesAsync(companyId, trackChanges);
        //    var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);
        //    return employeesDto;
        //}


        public async Task<EmployeeDto> GetEmployeeByIdAsync(Guid companyId, Guid id, bool trackChanges)
        {
            //var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
            //if (company is null)
            //    throw new CompanyNotFoundException(companyId);
            await CheckIfCompanyExists(companyId, trackChanges);
            var employeeDb = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges);
            if (employeeDb is null)
                throw new EmployeeNotFoundException(id);
            var employee = _mapper.Map<EmployeeDto>(employeeDb);
            return employee;
        }

        public async Task<EmployeeDto> CreateEmployeeForCompanyAsync(Guid companyId, EmployeeForCreationDto employeeForCreation, bool trackChanges)
        {
            //var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
            //if (company is null)
            //    throw new CompanyNotFoundException(companyId);
            await CheckIfCompanyExists(companyId, trackChanges);
            var employeeEntity = _mapper.Map<Employee>(employeeForCreation);
            await _repository.Employee.CreateEmployeeForCompanyAsync(companyId, employeeEntity);
            await _repository.SaveAsync();
            var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);
            return employeeToReturn;
        }

        public async Task DeleteEmployeeForCompanyAsync(Guid companyId, Guid id, bool trackChanges)
        {
            //var company = await _repository.Company.GetCompanyAsync(companyId, trackChanges);
            //if (company is null)
            //    throw new CompanyNotFoundException(companyId);
            //var employeeForCompany = await _repository.Employee.GetEmployeeAsync(companyId, id, trackChanges);
            //if (employeeForCompany is null)
            //    throw new EmployeeNotFoundException(id);
            await CheckIfCompanyExists(companyId, trackChanges);
            var employeeDb = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, trackChanges);
            _repository.Employee.DeleteEmployeeAsync(employeeDb);
            await _repository.SaveAsync();
        }

        public async Task UpdateEmployeeForCompanyAsync(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdate, bool compTrackChanges, bool empTrackChanges)
        {
            //var company = await _repository.Company.GetCompanyAsync(companyId, compTrackChanges);
            //if (company is null)
            //    throw new CompanyNotFoundException(companyId);
            //var employeeEntity = await _repository.Employee.GetEmployeeAsync(companyId, id, empTrackChanges);
            //if (employeeEntity is null)
            //    throw new EmployeeNotFoundException(id);
            await CheckIfCompanyExists(companyId, compTrackChanges);
            var employeeDb = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id, empTrackChanges);

            _mapper.Map(employeeForUpdate, employeeDb);
            await _repository.SaveAsync();
        }

        public async Task<(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)> GetEmployeeForPatchAsync(Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges)
        {
            //var company = await _repository.Company.GetCompanyAsync(companyId, compTrackChanges);
            //if (company is null)
            //    throw new CompanyNotFoundException(companyId);
            //var employeeEntity = await _repository.Employee.GetEmployeeAsync(companyId, id, empTrackChanges);
            //if (employeeEntity is null)
            //    throw new EmployeeNotFoundException(companyId);
            //var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeEntity);
            //return (employeeToPatch, employeeEntity);

            await CheckIfCompanyExists(companyId, compTrackChanges);
            var employeeDb = await GetEmployeeForCompanyAndCheckIfItExists(companyId, id,
            empTrackChanges);
            var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeDb);
            return (employeeToPatch: employeeToPatch, employeeEntity: employeeDb);
        }
        public async Task SaveChangesForPatchAsync(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)
        {
            _mapper.Map(employeeToPatch, employeeEntity);
            await _repository.SaveAsync();
        }

        private async Task CheckIfCompanyExists(Guid companyId, bool trackChanges)
        {
            var company = await _repository.Company.GetCompanyAsync(companyId,
            trackChanges);
            if (company is null) throw new CompanyNotFoundException(companyId);
        }

        private async Task<Employee> GetEmployeeForCompanyAndCheckIfItExists(Guid companyId, Guid id, bool trackChanges)
        {
            var employeeDb = await _repository.Employee.GetEmployeeAsync(companyId, id,
            trackChanges);
            if (employeeDb is null)
                throw new EmployeeNotFoundException(id);
            return employeeDb;
        }

    }

    //internal sealed class EmployeeService : IEmployeeService
    //{
    //    private readonly IRepositoryManager _repository;
    //    //private readonly ILoggerManager _logger;
    //    private readonly IMapper _mapper;

    //    public EmployeeService(IRepositoryManager repository, IMapper mapper)//, ILoggerManager logger)
    //    {
    //        _repository = repository;
    //        //_logger = logger;
    //        _mapper = mapper;
    //    }

    //    public IEnumerable<EmployeeDto> GetEmployees(Guid companyId, bool trackChanges)
    //    {
    //        var company = _repository.Company.GetCompany(companyId, trackChanges);
    //        if (company is null)
    //            throw new CompanyNotFoundException(companyId);
    //        var employeesFromDb = _repository.Employee.GetEmployees(companyId, trackChanges);
    //        var employeesDto = _mapper.Map<IEnumerable<EmployeeDto>>(employeesFromDb);
    //        return employeesDto;
    //    }
    //    public EmployeeDto GetEmployee(Guid companyId, Guid id, bool trackChanges)
    //    {
    //        var company = _repository.Company.GetCompany(companyId, trackChanges);
    //        if (company is null)
    //            throw new CompanyNotFoundException(companyId);
    //        var employeeDb = _repository.Employee.GetEmployee(companyId, id, trackChanges);
    //        if (employeeDb is null)
    //            throw new EmployeeNotFoundException(id);
    //        var employee = _mapper.Map<EmployeeDto>(employeeDb);
    //        return employee;
    //    }

    //    public EmployeeDto CreateEmployeeForCompany(Guid companyId, EmployeeForCreationDto employeeForCreation, bool trackChanges)
    //    {
    //        var company = _repository.Company.GetCompany(companyId, trackChanges);
    //        if (company is null)
    //            throw new CompanyNotFoundException(companyId);
    //        var employeeEntity = _mapper.Map<Employee>(employeeForCreation);
    //        _repository.Employee.CreateEmployeeForCompany(companyId, employeeEntity);
    //        _repository.Save();
    //        var employeeToReturn = _mapper.Map<EmployeeDto>(employeeEntity);
    //        return employeeToReturn;
    //    }

    //    public void DeleteEmployeeForCompany(Guid companyId, Guid id, bool trackChanges)
    //    {
    //        var company = _repository.Company.GetCompany(companyId, trackChanges);
    //        if (company is null)
    //            throw new CompanyNotFoundException(companyId);
    //        var employeeForCompany = _repository.Employee.GetEmployee(companyId, id,
    //        trackChanges);
    //        if (employeeForCompany is null)
    //            throw new EmployeeNotFoundException(id);
    //        _repository.Employee.DeleteEmployee(employeeForCompany);
    //        _repository.Save();
    //    }

    //    public void UpdateEmployeeForCompany(Guid companyId, Guid id, EmployeeForUpdateDto employeeForUpdate, bool compTrackChanges, bool empTrackChanges)
    //    {
    //        var company = _repository.Company.GetCompany(companyId, compTrackChanges);
    //        if (company is null)
    //            throw new CompanyNotFoundException(companyId);
    //        var employeeEntity = _repository.Employee.GetEmployee(companyId, id, empTrackChanges);
    //        if (employeeEntity is null)
    //            throw new EmployeeNotFoundException(id);
    //        _mapper.Map(employeeForUpdate, employeeEntity);
    //        _repository.Save();
    //    }

    //    public (EmployeeForUpdateDto employeeToPatch, Employee employeeEntity) GetEmployeeForPatch (Guid companyId, Guid id, bool compTrackChanges, bool empTrackChanges)
    //    {
    //        var company = _repository.Company.GetCompany(companyId, compTrackChanges);
    //        if (company is null)
    //            throw new CompanyNotFoundException(companyId);
    //        var employeeEntity = _repository.Employee.GetEmployee(companyId, id, empTrackChanges);
    //        if (employeeEntity is null)
    //            throw new EmployeeNotFoundException(companyId);
    //        var employeeToPatch = _mapper.Map<EmployeeForUpdateDto>(employeeEntity);
    //        return (employeeToPatch, employeeEntity);
    //    }
    //    public void SaveChangesForPatch(EmployeeForUpdateDto employeeToPatch, Employee employeeEntity)
    //    {
    //        _mapper.Map(employeeToPatch, employeeEntity);
    //        _repository.Save();
    //    }
    //}

}
