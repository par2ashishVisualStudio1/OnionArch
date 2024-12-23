using Contracts;
using Entities.Models;
using Microsoft.EntityFrameworkCore;
using Repository.Extensions;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(RepositoryContext repositoryContext)
        : base(repositoryContext)
        {
        }

        //public async Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges) =>
        //    await FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
        //    .OrderBy(e => e.Name)
        //    .Skip((employeeParameters.PageNumber - 1) * employeeParameters.PageSize)
        //    .Take(employeeParameters.PageSize)
        //    .ToListAsync();

        public async Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges)
        {
            //Norma
            //var employees = await FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
            //    .OrderBy(e => e.Name)
            //    .ToListAsync();
            //Filter
            //var employees = 
            //    await FindByCondition(e => 
            //    e.CompanyId.Equals(companyId) && 
            //    (e.Age >= employeeParameters.MinAge && e.Age <= employeeParameters.MaxAge), trackChanges)
            //    .OrderBy(e => e.Name)
            //    .ToListAsync();
            //Searching
            var employees = await FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
                .FilterEmployees(employeeParameters.MinAge, employeeParameters.MaxAge)
                .Search(employeeParameters.SearchTerm)
                .Sort(employeeParameters.OrderBy)
                //.OrderBy(e => e.Name)
                .ToListAsync();

            return PagedList<Employee>.ToPagedList(employees, employeeParameters.PageNumber, employeeParameters.PageSize);
        }


        public async Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, bool trackChanges) =>
            await FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
            .OrderBy(e => e.Name).ToListAsync();

        public async Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges) =>
            await FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id), trackChanges)
            .SingleOrDefaultAsync();

        public async Task CreateEmployeeForCompanyAsync(Guid companyId, Employee employee)
        {
            employee.CompanyId = companyId;
            await Create(employee);
        }

        public void DeleteEmployeeAsync(Employee employee) => Delete(employee);

    }

    //public class EmployeeRepository : RepositoryBase<Employee>, IEmployeeRepository
    //{
    //    public EmployeeRepository(RepositoryContext repositoryContext)
    //    : base(repositoryContext)
    //    {
    //    }

    //    public IEnumerable<Employee> GetEmployees(Guid companyId, bool trackChanges) =>
    //        FindByCondition(e => e.CompanyId.Equals(companyId), trackChanges)
    //        .OrderBy(e => e.Name).ToList();

    //    public Employee GetEmployee(Guid companyId, Guid id, bool trackChanges) =>
    //        FindByCondition(e => e.CompanyId.Equals(companyId) && e.Id.Equals(id),trackChanges)
    //        .SingleOrDefault();

    //    public void CreateEmployeeForCompany(Guid companyId, Employee employee)
    //    {
    //        employee.CompanyId = companyId;
    //        Create(employee);
    //    }

    //    public void DeleteEmployee(Employee employee) => Delete(employee);

    //}
}
