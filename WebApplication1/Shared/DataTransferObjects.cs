using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DataTransferObjects
{
    public record CompanyDto(Guid Id, string Name, string FullAddress);
    public record EmployeeDto(Guid Id, string Name, int Age, string Position);

    //public record CompanyForCreationDto(string Name, string Address, string Country);

    public record CompanyForCreationDto
    {
        [Required(ErrorMessage = "Company name is a required field.")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Company address is a required field.")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "Company country is a required field.")]
        public string? Country { get; set; }
    }

    //public record CompanyForCreationDto(string Name, string Address, string Country, IEnumerable<EmployeeForCreationDto> Employees);


    public abstract record EmployeeForManipulationDto
    {
        [Required(ErrorMessage = "Employee name is a required field.")]
        [MaxLength(30, ErrorMessage = "Maximum length for the Name is 30 characters.")]
        public string? Name { get; init; }
        [Range(18, int.MaxValue, ErrorMessage = "Age is required and it can't be lower than 18")]
        public int Age { get; init; }
        [Required(ErrorMessage = "Position is a required field.")]
        [MaxLength(20, ErrorMessage = "Maximum length for the Position is 20 characters.")]
        public string? Position { get; init; }
    }

    //public record EmployeeForCreationDto(string Name, int Age, string Position);
    public record EmployeeForCreationDto : EmployeeForManipulationDto
    {
        [Required(ErrorMessage = "Employee name is a required field.")]
        [MaxLength(30, ErrorMessage = "Maximum length for the Name is 30 characters.")]
        public string? Name { get; init; }

        [Required(ErrorMessage = "Age is a required field.")]
        [Range(18, int.MaxValue, ErrorMessage = "Age is required and it can't be lower than 18")]
        public int Age { get; init; }

        [Required(ErrorMessage = "Position is a required field.")]
        [MaxLength(20, ErrorMessage = "Maximum length for the Position is 20 characters.")]
        public string? Position { get; init; }
    }

    //public record EmployeeForUpdateDto(string Name, int Age, string Position) : EmployeeForManipulationDto;
    public record EmployeeForUpdateDto : EmployeeForManipulationDto
    {

    }

    public record CompanyForSelfUpdateDto(string Name, string Address, string Country);

    public record CompanyForUpdateDto(string Name, string Address, string Country, IEnumerable<EmployeeForCreationDto> Employees);


}
