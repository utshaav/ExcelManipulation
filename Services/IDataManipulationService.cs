using ExcelManipulation.Models;

namespace ExcelManipulation.Services;
public interface IDataManipulationService
{
    ExcelParseResult ParseExcel(IFormFile file);
    public ExcelParseResult ParseCsv(IFormFile file);
    Export ExcelExport(List<string> row, List<string> column);
    Export CsvExport(List<string> row, List<string> column);
    Export PdfExport(List<string> row, List<string> column);


}