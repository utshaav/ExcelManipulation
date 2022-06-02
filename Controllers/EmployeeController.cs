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
        int pageNo = page ??= 1;
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
    public async Task<IActionResult> ImportFile([FromForm] IFormFile postedFile)
    {
        
        ExcelParseResult result = postedFile.FileName.Contains(".csv")
                ? await _dataManipulation.ParseCsvAsync(postedFile) 
                : _dataManipulation.ParseExcel(postedFile);
        await _employeeDB.AddEmployee(result.Employees);
        return RedirectToAction("Index");
    }


    [HttpGet]
    public IActionResult ClearEmployeeTable()
    {
        _employeeDB.DeleteAllEmployees();
        return RedirectToAction("Index");
    }

    [HttpGet]
    public IActionResult Update(Guid Id)
    {
        var employee = _employeeDB.GetEmployee(Id);
        return PartialView(employee);
    }
    [HttpPost]
    public IActionResult Update(Employee employe)
    {
        _employeeDB.UpdateEmployee(employe);
        return RedirectToAction("Index");
    }
    [HttpPost]
    public FileResult Download(List<string> excel_row, List<string> excel_column)
    {
        byte[] fileBytes = _dataManipulation.ExcelExport(excel_row,excel_column);
        string fileName = "myfile.xlsx";
        return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
    }
}