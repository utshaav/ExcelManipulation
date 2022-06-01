using ExcelManipulation.Models;
using ExcelManipulation.Services;
using Microsoft.AspNetCore.Authorization;
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
    public async Task<IActionResult> Index(int? page)
    {
        int pageNo = page??1;
        var employees = await _employeeDB.GetAllEmployees(pageNo);
        ViewBag.Count = employees.Employees.Count;
        return View(employees);
    }

    [HttpGet]
    [Authorize]
    public IActionResult ImportFile()
    {
        return PartialView();
    }

    [HttpPost]
    public IActionResult ImportFile([FromForm] IFormFile postedFile)
    {
        ExcelParseResult result = _dataManipulation.ParseExcel(postedFile);
        _employeeDB.AddEmployee(result.Employees);
        return RedirectToAction("Index");
    }


    [HttpGet]
    public IActionResult ClearEmployeeTable(){
        _employeeDB.DeleteAllEmployees();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Update(Guid Id){
        var employee = _employeeDB.GetEmployee(Id);
        return PartialView(employee);
    }
    [HttpPost]
    public IActionResult Update(Employee employe){
        _employeeDB.UpdateEmployee(employe);
        return RedirectToAction("Index");
    }
}