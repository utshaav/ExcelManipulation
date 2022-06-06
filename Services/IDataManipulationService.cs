using ExcelManipulation.Models;

namespace ExcelManipulation.Services;
public interface IDataManipulationService
{
    ExcelParseResult ParseExcel(IFormFile file);
    public ExcelParseResult ParseCsv(IFormFile file);
    Export ExcelExport(List<string> excel_row, List<string> excel_column);
    Export CsvExport(List<string> excel_row, List<string> excel_column);


}