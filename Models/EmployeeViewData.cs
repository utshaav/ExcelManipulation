#nullable disable
namespace ExcelManipulation.Models;
public class EmployeeViewData : Employee
{
    public IFormFile FormImage { get; set; }
}