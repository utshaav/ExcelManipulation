#nullable disable
namespace ExcelManipulation.Models;
public class Filter : EmployeeResponse
{
    public float StartSalary { get; set; }
    public float EndSalary { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public string EmployeeName { get; set; }
    public string Gender { get; set; }
    public string Designation { get; set; }
    public int PageNo { get; set; } = 1;
    public Guid ImportedBy { get; set; }
    public bool RequireFIlter { get; set; } = true;
    
}