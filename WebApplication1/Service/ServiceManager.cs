﻿using AutoMapper;
using Contracts;
using Service.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public sealed class ServiceManager : IServiceManager
    {
        private readonly Lazy<ICompanyService> _companyService;
        private readonly Lazy<IEmployeeService> _employeeService;
        public ServiceManager(IRepositoryManager repositoryManager, IMapper mapper)// ILoggerManager logger)
        {
            _companyService = new Lazy<ICompanyService>(() => new CompanyService(repositoryManager, mapper));//, logger);)
            _employeeService = new Lazy<IEmployeeService>(() => new EmployeeService(repositoryManager, mapper));//, logger));
        }
        public ICompanyService CompanyService => _companyService.Value;
        public IEmployeeService EmployeeService => _employeeService.Value;
    }
}
