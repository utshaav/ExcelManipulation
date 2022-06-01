using ExcelManipulation.Data;
using ExcelManipulation.Models;

namespace ExcelManipulation.Services;

public class EmployeeDBService : IEmployeeDBService
{
    private readonly ApplicationDbContext _dbContext;
    public EmployeeDBService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Employee GetEmployee(Guid employeeId)
    {
        var employee = _dbContext.Employees.Where(x => x.Id == employeeId).FirstOrDefault();
        return employee!;
    }

    public List<Employee> GetAllEmployees()
    {
        return _dbContext.Employees.ToList();
    }

    public async Task<int> AddEmployee(Employee employee)
    {
        // _dbContext.Employees.Attach(employee);
        await _dbContext.Employees.AddAsync(employee);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> AddEmployee(List<Employee> employees)
    {
        int i;
        foreach (var employee in employees)
        {
            i = await AddEmployee(employee);
            if (i == 0)
            {
                return 0;
            }
        }
        return 1;
    }



    public async Task<int> UpdateEmployee(Employee employee)
    {
        _dbContext.Employees.Attach(employee);
        _dbContext.Employees.Update(employee);
        return await _dbContext.SaveChangesAsync();
    }

    public void DeleteEmployee(Guid employeeId)
    {
        var employee = _dbContext.Employees.Where(x => x.Id == employeeId).FirstOrDefault();
        _dbContext.Employees.Remove(employee!);
    }

    public void DeleteAllEmployees()
    {
        var employees = _dbContext.Employees.ToList();
        _dbContext.Employees.RemoveRange(employees);
    }

}