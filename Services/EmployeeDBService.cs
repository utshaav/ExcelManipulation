using ExcelManipulation.Data;
using ExcelManipulation.Models;
using Microsoft.EntityFrameworkCore;

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

    public async Task<EmployeeResponse> GetAllEmployees(int page = 0)
    {
        var pageResult = 10f;
        var pageCount = Math.Ceiling(_dbContext.Employees.Count() / pageResult);
        var employees = await _dbContext.Employees
            .Skip((page-1)*(int)pageResult)
            .Take((int)pageResult)
            .ToListAsync();
        return new EmployeeResponse{
            CurrentPage = page,
            Pages = (int)pageCount,
            Employees = employees
        };
    }

    public List<Employee> GetAllEmployees()
    {
        var employees =  _dbContext.Employees.ToList();
        return employees;
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
        _dbContext.SaveChanges();
    }

    public void DeleteAllEmployees()
    {
        var employees = _dbContext.Employees.ToList();
        _dbContext.Employees.RemoveRange(employees);
        _dbContext.SaveChanges();
    }
}