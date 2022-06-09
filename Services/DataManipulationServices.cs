using System.Globalization;
using System.Text;
using CsvHelper;
using ExcelManipulation.Enums;
using ExcelManipulation.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using OfficeOpenXml;
using SelectPdf;
// using CsvHelper;
// using CsvHelper.Configuration;
using TinyCsvParser;
using TinyCsvParser.Mapping;

namespace ExcelManipulation.Services;

public class DataManipulationService : IDataManipulationService
{
    private readonly IEmployeeDBService _employeeDb;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IRazorViewEngine _viewEngine;
    private readonly IServiceProvider _serviceProvider;
    private readonly ITempDataProvider _tempData;

    public DataManipulationService(IEmployeeDBService employeeDb,
                                    UserManager<IdentityUser> userManager,
                                    IRazorViewEngine viewEngine,
                                    IServiceProvider serviceProvider,
                                    ITempDataProvider tempData)
    {
        _viewEngine = viewEngine;
        _serviceProvider = serviceProvider;
        _tempData = tempData;
        _userManager = userManager;
        _employeeDb = employeeDb;

    }
    public ExcelParseResult ParseExcel(IFormFile file, string userId)
    {
        List<Employee> employees = new List<Employee>();
        List<int> emptyRows = new List<int>();
        using (var package = new ExcelPackage(file.OpenReadStream()))
        {
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            foreach (ExcelWorksheet worksheet in package.Workbook.Worksheets)
            {
                //loop all rows
                for (int i = 2; i <= worksheet.Dimension.End.Row; i++)
                {
                    try
                    {
                        var columnEnd = worksheet.Cells.End.Column;
                        var cellRange = worksheet.Cells[i, 1, i, columnEnd];
                        var isRowEmpty = cellRange.All(c => c.Value == null);
                        if (isRowEmpty)
                        {
                            emptyRows.Add(i);
                            continue;
                        }
                        bool isCellEmpty = cellRange.Any(c => c.Value == null);
                        if (isCellEmpty)
                            throw new Exception($"Import failed because row {i} have a empty cell.");
                        // return new ExcelParseResult
                        // {
                        //     Success = false,
                        //     ErrorMessage = $"Import failed because row {i} have a empty cell."
                        // };

                        var employee = new Employee();
                        var date = worksheet.Cells[i, 5].GetValue<DateTime>();
                        employee.FullName = worksheet.Cells[i, 1].GetValue<string>();
                        employee.Designation = worksheet.Cells[i, 2].GetValue<string>();
                        employee.Gender = worksheet.Cells[i, 3].GetValue<string>();
                        employee.Salary = worksheet.Cells[i, 4].GetValue<float>();
                        employee.DateOfBirth = worksheet.Cells[i, 5].GetValue<DateTime>();
                        // employee.ImportedBy = User
                        employee.ImportedBy = Guid.Parse(userId);
                        employees.Add(employee);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e);
                        return new ExcelParseResult
                        {
                            Success = false,
                            ErrorMessage = e.Message,
                            EmptyRows = emptyRows
                        };
                    }
                }
            }
        }
        return new ExcelParseResult { Employees = employees, EmptyRows = emptyRows };
    }

    // public async Task<ExcelParseResult> ParseCsvAsync(IFormFile file)
    // {
    //     // using var memoryStream = new MemoryStream(new byte[file.Length]);
    //     // await file.CopyToAsync(memoryStream);
    //     // memoryStream.Position = -1;

    //     using (var reader = new StreamReader(file.OpenReadStream()))
    //     using (var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture))
    //     {
    //         List<Employee> list = new();
    //         csvReader.Read();
    //         var records = csvReader.GetRecords<dynamic>().ToList();
    //         Console.WriteLine(records);
    //         foreach (var record in records)
    //         {
    //             Console.WriteLine(record);
    //             foreach(var furtherRecord in record){

    //             }
    //             // if(record == null) continue;
    //             // Employee emp = new Employee{
    //             //     DateOfBirth = record.DateOfBirth,
    //             //     FullName = record.FullName,
    //             //     Salary = record.Salary,
    //             //     Gender = record.Gender,
    //             //     Designation = record.Designation
    //             // };
    //             // list.Add(emp);
    //         }
    //         return new ExcelParseResult
    //         {
    //             Employees = list,
    //             EmptyRows = new List<int>()
    //         };
    //     }

    //     return new ExcelParseResult();
    // }


    public Export ExcelExport(List<string> row, List<string> column)
    {
        List<Employee> employees = new();
        if (row.Count == 0)
        {
            employees = _employeeDb.GetAllEmployees();
        }
        else
        {
            foreach (var item in row)
            {
                employees.Add(_employeeDb.GetEmployee(Guid.Parse(item)));
            }
        }
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (ExcelPackage excelPackage = new ExcelPackage())
        {
            //create the WorkSheet
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
            if (column.Count == 0)
            {
                worksheet.Cells[1, 1].Value = "Full Name";
                worksheet.Cells[1, 2].Value = "Designation";
                worksheet.Cells[1, 3].Value = "Gender";
                worksheet.Cells[1, 4].Value = "Salary";
                worksheet.Cells[1, 5].Value = "Date of birth";

                for (int i = 0; i < employees.Count; i++)
                {
                    worksheet.Cells[i + 2, 1].Value = employees[i].FullName;
                    worksheet.Cells[i + 2, 2].Value = employees[i].Designation;
                    worksheet.Cells[i + 2, 3].Value = employees[i].Gender;
                    worksheet.Cells[i + 2, 4].Value = employees[i].Salary;
                    worksheet.Cells[i + 2, 5].Value = employees[i].DateOfBirth;
                    worksheet.Cells[i + 2, 5].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;

                }
            }

            else
            {
                int i = 0;
                Employee dummyObj = new Employee();
                foreach (var prop in dummyObj.GetType().GetProperties())
                {
                    if (column.Contains(prop.Name))
                    {
                        worksheet.Cells[1, i + 1].Value = prop.Name;
                        i++;
                    }
                }
                // for (int i = 0; i < column.Count; i++)
                // {
                //     worksheet.Cells[1, i + 1].Value = column[i];
                //     // for (int j = 0; j < employees.Count; j++)
                //     // {
                //     //     worksheet.Cells[j + 2, i + 1].Value = employees[j].FullName;
                //     //     worksheet.Cells[j + 2, i + 1].Value = employees[j].Designation;
                //     //     worksheet.Cells[j + 2, i + 1].Value = employees[j].Gender;
                //     //     worksheet.Cells[j + 2, i + 1].Value = employees[j].Salary;
                //     //     worksheet.Cells[j + 2, i + 1].Value = employees[j].DateOfBirth;
                //     // }
                // }
                int k = 0;
                foreach (var employee in employees)
                {
                    int l = 0;
                    foreach (var prop in employee.GetType().GetProperties())
                    {
                        if (column.Contains(prop.Name))
                        {
                            worksheet.Cells[k + 2, l + 1].Value = prop.GetValue(employee);
                            if (prop.Name == "DateOfBirth")
                            {
                                worksheet.Cells[k + 2, l + 1].Style.Numberformat.Format = DateTimeFormatInfo.CurrentInfo.ShortDatePattern;
                            }
                            l++;
                        }
                    }
                    k++;
                }

            }

            //convert the excel package to a byte array
            byte[] bin = excelPackage.GetAsByteArray();
            return new Export
            {
                Success = true,
                File = bin,
                Extension = ".xlsx"
            };
        }
    }

    public Export CsvExport(List<string> row, List<string> column)
    {
        List<Employee> employees = new();
        byte[] file;
        if (row.Count == 0)
        {
            employees = _employeeDb.GetAllEmployees();
        }
        else
        {
            foreach (var item in row)
            {
                employees.Add(_employeeDb.GetEmployee(Guid.Parse(item)));
            }
        }
        using (var memoryStream = new MemoryStream())
        {
            using (TextWriter streamWriter = new StreamWriter(memoryStream))
            using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.InvariantCulture))
            {
                if (column.Count == 0)
                {
                    csvWriter.WriteField("Full Name");
                    csvWriter.WriteField("Designation");
                    csvWriter.WriteField("Gender");
                    csvWriter.WriteField("Salary");
                    csvWriter.WriteField("Date of birth");
                    csvWriter.NextRecord();

                    for (int i = 0; i < employees.Count; i++)
                    {
                        csvWriter.WriteField(employees[i].FullName);
                        csvWriter.WriteField(employees[i].Designation);
                        csvWriter.WriteField(employees[i].Gender);
                        csvWriter.WriteField(employees[i].Salary);
                        csvWriter.WriteField(employees[i].DateOfBirth);
                        csvWriter.NextRecord();

                    }
                }
                else
                {
                    int i = 0;
                    Employee dummyObj = new Employee();
                    foreach (var prop in dummyObj.GetType().GetProperties())
                    {
                        if (column.Contains(prop.Name))
                        {
                            csvWriter.WriteField(prop.Name);
                            i++;
                        }
                    }
                    csvWriter.NextRecord();

                    int k = 0;
                    foreach (var employee in employees)
                    {
                        int l = 0;
                        foreach (var prop in employee.GetType().GetProperties())
                        {
                            if (column.Contains(prop.Name))
                            {
                                csvWriter.WriteField(prop.GetValue(employee));
                                l++;
                            }
                        }
                        csvWriter.NextRecord();
                        k++;
                    }

                }
            } // StreamWriter gets flushed here.

            file = memoryStream.ToArray();
        }
        return new Export
        {
            Success = true,
            File = file,
            Extension = ".csv"
        };
    }

    public async Task<Export> PdfExportAsync(List<string> row, List<string> column, HttpContext httpCtx)
    {
        List<Employee> employees = new();
        byte[] file;
        if (row.Count == 0)
        {
            employees = _employeeDb.GetAllEmployees();
        }
        else
        {
            foreach (var item in row)
            {
                employees.Add(_employeeDb.GetEmployee(Guid.Parse(item)));
            }
        }
        string viewAsHtml;
        using (var sw = new StringWriter())
        {
            httpCtx = new DefaultHttpContext
            {
                RequestServices = _serviceProvider
            };
            var actionContext = new ActionContext(httpCtx, new RouteData(), new ActionDescriptor());
            var view = _viewEngine.FindView(actionContext, "_PdfExport", false);
            Console.WriteLine(view);
            var vieDictionary = new ViewDataDictionary<List<Employee>>(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = employees
            };
            var tempData = new TempDataDictionary(httpCtx, _tempData);
            var viewContext = new ViewContext(actionContext, view.View, vieDictionary, tempData, sw, new HtmlHelperOptions());
            await view.View.RenderAsync(viewContext);
            viewAsHtml = sw.ToString();
        }

        string htmlText = "<h1> This is Sample Pdf file</h1> <p> This is the demo for Csharp Created Pdf using IronPdf </p> <p> IronPdf is a library which provides build in functions for creating, reading <br> and manuplating pdf files with just few lines of code. </p>";
        var HtmlLine = new HtmlToPdf();
        var pdfDoc = HtmlLine.ConvertHtmlString(viewAsHtml);
        file = pdfDoc.Save();
        pdfDoc.Close();

        return new Export
        {
            File = file,
            Success = true,
            Extension = ".pdf"
        };
    }
    public ExcelParseResult ParseCsv(IFormFile file, string userId)
    {
        CsvParserOptions csvParserOptions = new CsvParserOptions(true, ',');
        List<Employee> employees = new List<Employee>();
        CsvEmployeeMapping csvMapper = new CsvEmployeeMapping();
        CsvParser<EmployeeMappingHelper> csvParser = new CsvParser<EmployeeMappingHelper>(csvParserOptions, csvMapper);
        var result = csvParser
                     .ReadFromStream(file.OpenReadStream(), Encoding.ASCII)
                     .ToList();

        Console.WriteLine("Name " + "ID   " + "City  " + "Country");
        foreach (var details in result)
        {
            if (details.IsValid)
            {
                DateTime dob = DateTime.Parse(details.Result.DOB, CultureInfo.InvariantCulture);
                Console.WriteLine(dob);
                Employee employee = new Employee
                {
                    Salary = details.Result.Salary,
                    Designation = details.Result.Designation,
                    FullName = details.Result.FullName,
                    Gender = details.Result.Gender,
                    DateOfBirth = dob,
                    ImportedBy = Guid.Parse(userId)
                };
                employees.Add(employee);
            }
            else
            {
                if (details.RowIndex != 1)
                {
                    return new ExcelParseResult
                    {
                        Success = false,
                        ErrorMessage = details.Error.Value,
                    };
                }
            }
        }

        return new ExcelParseResult { Employees = employees, EmptyRows = new List<int>() };
    }

    public async Task<List<SelectListItem>> ImporterDD()
    {
        List<SelectListItem> res = new List<SelectListItem>();
        var users = await _userManager.GetUsersInRoleAsync(Roles.Admin.ToString());
        res.Add(new SelectListItem { Value = "", Text = "Select Importer" });
        foreach (var item in users)
        {
            res.Add(new SelectListItem { Value = item.Id, Text = item.UserName });
        }

        return res;
    }

    private class CsvEmployeeMapping : CsvMapping<EmployeeMappingHelper>
    {
        public CsvEmployeeMapping()
            : base()
        {
            MapProperty(0, x => x.FullName);
            MapProperty(1, x => x.Designation);
            MapProperty(2, x => x.Gender);
            MapProperty(3, x => x.Salary);
            MapProperty(4, x => x.DOB);

        }
    }

    private class EmployeeMappingHelper : Employee
    {
        public string DOB { get; set; }
    }
}