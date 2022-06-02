using ExcelManipulation.Models;

namespace ExcelManipulation.Services;
public interface IDataManipulationService
{
    ExcelParseResult ParseExcel(IFormFile file);
    byte[] ExcelExport(List<string> excel_row, List<string> excel_column);
}