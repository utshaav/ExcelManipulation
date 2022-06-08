using System.Security.Claims;
using ExcelManipulation.Enums;
using ExcelManipulation.Models;
using ExcelManipulation.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ExcelManipulation.Controllers;
[Authorize]
public class EmployeeController : Controller
{
    private readonly IDataManipulationService _dataManipulation;
    private readonly IEmployeeDBService _employeeDB;
    private readonly RoleManager<IdentityRole> _roleManager;
    public EmployeeController(IDataManipulationService dataManipulation, IEmployeeDBService employeeDB, RoleManager<IdentityRole> roleManager)
    {
        _roleManager = roleManager;
        _employeeDB = employeeDB;
        _dataManipulation = dataManipulation;
    }

    // public async Task CreateRole()
    // {
    //     if (!await _roleManager.RoleExistsAsync(Roles.Admin.ToString()))
    //     {
    //         await _roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
    //         await _roleManager.CreateAsync(new IdentityRole(Roles.Default.ToString()));

    //     }
    // }

    [HttpGet]
    public async Task<IActionResult> Index(int? page, bool redirected = false)
    {
        Filter filter = new Filter{
            PageNo = page??=1,
            RequireFIlter = false,
        };

        Console.WriteLine(User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (redirected && TempData["Message"] != null)
        {
            ViewBag.Message = TempData["Message"]!.ToString();
        }
        ViewBag.Redirected = redirected;
        int pageNo = page ??= 1;
        var employees = await _employeeDB.GetAllEmployees(filter);
        ViewBag.Count = employees.Employees.Count;
        return View(employees);
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult ImportFile()
    {
        return PartialView();
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ImportFile([FromForm] IFormFile postedFile)
    {
        string message = string.Empty;
        var userId =  User.FindFirstValue(ClaimTypes.NameIdentifier);
        ExcelParseResult result = postedFile.FileName.Contains(".csv")
                ?  _dataManipulation.ParseCsv(postedFile, userId)
                : _dataManipulation.ParseExcel(postedFile, userId);

        if (result.Success)
        {
            await _employeeDB.AddEmployee(result.Employees);
            message = "Imported succesfully.";
            if (result.EmptyRows.Count > 0)
            {
                message += "These rows were empty :";
                foreach (int i in result.EmptyRows)
                {
                    message += $" {i},";
                }
            }
        }
        else
        {
            message += "Failed to import your file.";
            message += result.ErrorMessage;
        }
        TempData["Message"] = message;
        return RedirectToAction("Index", new { page = 1, redirected = true });
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
        TempData["Message"] = $"{employe.FullName} updated succesfully";
        return RedirectToAction("Index", new { redirected = true });
    }
    [HttpGet]
    public IActionResult ExportChoice(){
        List<string> options = new List<string>{"excel", "csv", "pdf"};
        return PartialView(options);
    }

    [HttpPost]
    public IActionResult Download(List<string> excel_row, List<string> excel_column, string fileType)
    {
        Console.WriteLine(fileType);
        Export result = new ();
        if(fileType.ToLower() == "excel")
        result = _dataManipulation.ExcelExport(excel_row, excel_column);
        else if(fileType.ToLower() == "csv")
        result = _dataManipulation.CsvExport(excel_row, excel_column);
        else if(fileType.ToLower() == "pdf")
        result = _dataManipulation.PdfExport(excel_row, excel_column);


        string fileName = "myfile" + result.Extension;
        // string fileName = "myfile.csv";
        return File(result.File, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
    }

    [HttpGet]
    public IActionResult Delete(Guid id)
    {
        var employee = _employeeDB.GetEmployee(id);
        return PartialView(employee);
    }

    [HttpPost]
    public IActionResult ConfirmDelete(Guid id)
    {
        _employeeDB.DeleteEmployee(id);
        TempData["Message"] = "Deleted Succesfully";
        return RedirectToAction("Index", new { redirected = true });
    }

    [HttpPost]
    public async Task<IActionResult> FilterSearch(Filter filter)
    {
        EmployeeResponse response = await _employeeDB.GetAllEmployees(filter);
        return PartialView("_EmployeeTable",response);
    }


}