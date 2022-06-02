using System.Globalization;
using ExcelManipulation.Models;
using OfficeOpenXml;

namespace ExcelManipulation.Services;

public class DataManipulationService : IDataManipulationService
{
    private readonly IEmployeeDBService _employeeDb;
    public DataManipulationService(IEmployeeDBService employeeDb)
    {
        _employeeDb = employeeDb;

    }
    public ExcelParseResult ParseExcel(IFormFile file)
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
                    var columnEnd = worksheet.Cells.End.Column;
                    var cellRange = worksheet.Cells[i, 1, i, columnEnd];
                    var isRowEmpty = cellRange.All(c => c.Value == null);
                    if (isRowEmpty)
                    {
                        emptyRows.Add(i);
                        continue;
                    }
                    var employee = new Employee();
                    var date = worksheet.Cells[i, 5].GetValue<DateTime>();
                    employee.FullName = worksheet.Cells[i, 1].GetValue<string>();
                    employee.Gender = worksheet.Cells[i, 2].GetValue<string>();
                    employee.Designation = worksheet.Cells[i, 3].GetValue<string>();
                    employee.Salary = worksheet.Cells[i, 4].GetValue<float>();
                    employee.DateOfBirth = worksheet.Cells[i, 5].GetValue<DateTime>();
                    employees.Add(employee);
                }
            }
        }
        return new ExcelParseResult { Employees = employees, EmptyRows = emptyRows };
    }
    public byte[] ExcelExport(List<string> excel_row, List<string> excel_column)
    {
        List<Employee> employees = new();
        if (excel_row.Count == 0)
        {
            employees = _employeeDb.GetAllEmployees();
        }
        else
        {
            foreach (var item in excel_row)
            {
                employees.Add(_employeeDb.GetEmployee(Guid.Parse(item)));
            }
        }
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
        using (ExcelPackage excelPackage = new ExcelPackage())
        {
            //create the WorkSheet
            ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");
            if (excel_column.Count == 0)
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
                    if (excel_column.Contains(prop.Name))
                    {
                        worksheet.Cells[1, i + 1].Value = prop.Name;
                        i++;
                    }
                }
                // for (int i = 0; i < excel_column.Count; i++)
                // {
                //     worksheet.Cells[1, i + 1].Value = excel_column[i];
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
                        if (excel_column.Contains(prop.Name))
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
            return bin;
        }
    }
}