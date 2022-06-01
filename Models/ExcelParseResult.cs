#nullable disable
namespace ExcelManipulation.Models;
public class ExcelParseResult{
    public List<int> EmptyRows { get; set; }
    public List<Employee> Employees { get; set; }
}