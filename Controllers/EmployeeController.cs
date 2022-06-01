using ExcelManipulation.Models;
using ExcelManipulation.Services;
using Microsoft.AspNetCore.Mvc;

namespace ExcelManipulation.Controllers;
public class EmployeeController : Controller
{
    private readonly IDataManipulationService _dataManipulation;
    private readonly IEmployeeDBService _employeeDB;
    public EmployeeController(IDataManipulationService dataManipulation, IEmployeeDBService employeeDB)
    {
        _employeeDB = employeeDB;
        _dataManipulation = dataManipulation;

    }

    [HttpGet]
    public IActionResult Index()
    {
        var employees = _employeeDB.GetAllEmployees();
        ViewBag.Count = employees.Count;
        return View(employees);
    }

    [HttpGet]
    public IActionResult ImportFile()
    {
        return View();
    }

    [HttpPost]
    public IActionResult ImportFile([FromForm] IFormFile postedFile)
    {
        ExcelParseResult result = _dataManipulation.ParseExcel(postedFile);
        _employeeDB.AddEmployee(result.Employees);
        return View();
    }


    [HttpGet]
    public void ClearEmployeeTable(){
        _employeeDB.DeleteAllEmployees();
    }
}