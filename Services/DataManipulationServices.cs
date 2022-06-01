using ExcelManipulation.Models;
using OfficeOpenXml;

namespace ExcelManipulation.Services;



public class DataManipulationService : IDataManipulationService
{
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
}