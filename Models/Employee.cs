#nullable disable
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
// using CsvHelper.Configuration.Attributes;

namespace ExcelManipulation.Models;
public class Employee
{
    public Guid Id { get; set; }

    [Required]
    public string FullName { get; set; }

    [Required]
    public string Designation { get; set; }

    [Required]
    public string Gender { get; set; }


    [Required]
    public float Salary { get; set; }

    [Required]
    public DateTime DateOfBirth { get; set; }

    public Photo Photo { get; set; }

    public DateTime ImportedDate { get; set; } = DateTime.Now;
    public Guid ImportedBy { get; set; }
}