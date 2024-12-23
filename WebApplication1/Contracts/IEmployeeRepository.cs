using Entities.Models;
using Shared.RequestFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IEmployeeRepository
    {
        Task<PagedList<Employee>> GetEmployeesAsync(Guid companyId, EmployeeParameters employeeParameters, bool trackChanges);

        //Task<IEnumerable<Employee>> GetEmployeesAsync(Guid companyId, bool trackChanges);
        Task<Employee> GetEmployeeAsync(Guid companyId, Guid id, bool trackChanges);
        Task CreateEmployeeForCompanyAsync(Guid companyId, Employee employee);

        void DeleteEmployeeAsync(Employee employee);
    }

    //public interface IEmployeeRepository
    //{
    //    IEnumerable<Employee> GetEmployees(Guid companyId, bool trackChanges);
    //    Employee GetEmployee(Guid companyId, Guid id, bool trackChanges);
    //    void CreateEmployeeForCompany(Guid companyId, Employee employee);

    //    void DeleteEmployee(Employee employee);
    //}
}
