using ExcelManipulation.Models;

namespace ExcelManipulation.Services;

public interface IEmployeeDBService
{
    Task<int> AddEmployee(Employee employee);
    Task<int> AddEmployee(List<Employee> employees);
    void DeleteEmployee(Guid employeeId);
    void DeleteAllEmployees();
    Task<EmployeeResponse> GetAllEmployees(int page);
    List<Employee> GetAllEmployees();
    Employee GetEmployee(Guid employeeId);
    Task<int> UpdateEmployee(Employee employee);
}