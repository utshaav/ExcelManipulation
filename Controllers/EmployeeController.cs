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
        Filter filter = new Filter
        {
            PageNo = page ??= 1,
            RequireFIlter = false,
        };

        ViewBag.ImporterDD = await _dataManipulation.ImporterDD();

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

    #region  Import File
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
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        ExcelParseResult result = postedFile.FileName.Contains(".csv")
                ? _dataManipulation.ParseCsv(postedFile, userId)
                : _dataManipulation.ParseExcel(postedFile, userId);

        if (result.Success)
        {
            await _employeeDB.AddEmployee(result.Employees);
            message = "Imported succesfully.";
            if (result.EmptyRows.Count > 0)
            {
                var emptyRows = string.Empty;
                var first = true;
                foreach (int i in result.EmptyRows)
                {
                    if(!first) emptyRows += ",";
                    emptyRows += i.ToString();
                    first = false;
                }
                message = $"X rows [{emptyRows}] were skipped since they did not have data";
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

    #endregion

    #region Update Employee
    [HttpGet]
    public IActionResult Update(Guid Id)
    {
        var emp = _employeeDB.GetEmployee(Id);
        EmployeeViewData employee = new EmployeeViewData
        {
            DateOfBirth = emp.DateOfBirth,
            Designation = emp.Designation,
            FullName = emp.FullName,
            Id = emp.Id,
            Gender = emp.Gender,
            Photo = emp.Photo,
            Salary = emp.Salary,
            ImportedBy = emp.ImportedBy,
            ImportedDate = emp.ImportedDate
        };
        if (employee.Photo != null && employee.Photo.Bytes != null)
        {
            ViewBag.Photo = $"data:{employee.Photo.FileExtension};base64,{Convert.ToBase64String(employee.Photo.Bytes)}";
        }
        else
        {
            ViewBag.Photo = "image/noimage.jpg";
        }
        return PartialView(employee);
    }
    [HttpPost]
    public IActionResult Update([FromForm] EmployeeViewData employe)
    {
        Console.WriteLine("===============================================");
        Console.WriteLine(employe.FormImage);
        Console.WriteLine("===============================================");
        if (employe.FormImage != null)
        {
            using (var ms = new MemoryStream())
            {
                employe.FormImage.CopyTo(ms);
                var fileBytes = ms.ToArray();
                Photo photo = new Photo
                {
                    Bytes = fileBytes,
                    Size = fileBytes.Length,
                    FileExtension = employe.FormImage.ContentType,
                    EmployeeId = employe.Id
                };
                if (employe.Photo.Id != 0)
                {
                    photo.Id = employe.Photo.Id;
                    _employeeDB.UpdatePhoto(photo);
                }
                else
                {
                    employe.Photo = photo;
                }
                // act on the Base64 data
            }

        }
        Employee emp = employe;
        _employeeDB.UpdateEmployee(emp);
        TempData["Message"] = $"{employe.FullName} updated succesfully";
        return RedirectToAction("Index", new { redirected = true });
    }

    #endregion

    #region Export File
    [HttpGet]
    public IActionResult ExportChoice()
    {
        List<string> options = new List<string> { "excel", "csv", "pdf" };
        return PartialView(options);
    }

    [HttpPost]
    public async Task<IActionResult> Download(List<string> excel_row, List<string> excel_column, string fileType)
    {
        Console.WriteLine(fileType);
        Export result = new();
        if (fileType.ToLower() == "excel")
            result = _dataManipulation.ExcelExport(excel_row, excel_column);
        else if (fileType.ToLower() == "csv")
            result = _dataManipulation.CsvExport(excel_row, excel_column);
        else if (fileType.ToLower() == "pdf")
            result = await _dataManipulation.PdfExportAsync(excel_row, excel_column, HttpContext);


        string fileName = $"myfile-{DateTime.Now.ToShortDateString()}{result.Extension}";
        // string fileName = "myfile.csv";
        return File(result.File, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
    }
    #endregion

    #region Delete Employee
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
    #endregion

    #region Add Employee
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public IActionResult Add()
    {
        return PartialView(new EmployeeViewData{DateOfBirth = DateTime.Now});
    }
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AddAsync([FromForm] EmployeeViewData employe)
    {
        if (ModelState.IsValid)
        {
            using (var ms = new MemoryStream())
            {
                employe.FormImage.CopyTo(ms);
                var fileBytes = ms.ToArray();
                Photo photo = new Photo
                {
                    Bytes = fileBytes,
                    Size = fileBytes.Length,
                    FileExtension = employe.FormImage.ContentType,
                    EmployeeId = employe.Id
                };
                employe.Photo = photo;
            }
            Employee emp = employe;
            await _employeeDB.AddEmployee(emp);
            TempData["Message"] = $"{employe.FullName} Added succesfully";
            return RedirectToAction("Index", new { redirected = true });
        }

        return View(employe);

    }

    #endregion


    #region Others
    [HttpPost]
    public async Task<IActionResult> FilterSearch(Filter filter)
    {
        EmployeeResponse response = await _employeeDB.GetAllEmployees(filter);
        return PartialView("_EmployeeTable", response);
    }

    [HttpGet]
    public IActionResult ClearEmployeeTable()
    {
        _employeeDB.DeleteAllEmployees();
        return RedirectToAction("Index");
    }
    #endregion


}