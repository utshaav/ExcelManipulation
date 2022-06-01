#nullable disable
namespace ExcelManipulation.Models;

public class EmployeeResponse{
    public List<Employee> Employees { get; set; }
    public int CurrentPage { get; set; }
    public int Pages { get; set; }
}