using ExcelManipulation.Models;

namespace ExcelManipulation.Services;
public interface IDataManipulationService
{
    ExcelParseResult ParseExcel(IFormFile file);
}