#nullable disable
using System.ComponentModel.DataAnnotations;

namespace ExcelManipulation.Models;
public class Employee{
    [Required]
    public string FullName { get; set; }
    [Required]
    public string Designation { get; set; }
    [Required]
    public float Salary { get; set; }
    [Required]
    public DateTime DateOfBirth { get; set; }
    [Required]
    public string Gender { get; set; }
}