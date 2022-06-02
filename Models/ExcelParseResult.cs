#nullable disable
namespace ExcelManipulation.Models;
public class ExcelParseResult{
    public List<int> EmptyRows { get; set; }
    public List<Employee> Employees { get; set; }
    public List<string> ProblematicCell { get; set; }
    public string ErrorMessage { get; set; }
    public bool Success {get; set;} = true;
}