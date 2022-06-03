#nullable disable
namespace ExcelManipulation.Models;
public class Filter
{
    public float startSalary { get; set; }
    public float endSalary { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string EmployeeName { get; set; }
    public string Gender { get; set; }
    public string Designation { get; set; }
}