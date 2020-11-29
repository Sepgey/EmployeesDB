using System;
using System.Linq;
using System.Text;
using EmployeesDB.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeesDB
{
    class Program
    {
        static private EmployeesContext _context = new EmployeesContext();
        static void Main(string[] args)
        {
            //Console.WriteLine(GetHighlyPaidEmployees());
            //Console.WriteLine(AddNewAddressForBrown());
            Console.WriteLine(ProjectAudit());
            //Console.WriteLine(EmployeeDossier());
            //Console.WriteLine(SmallDepartments());
            //Console.WriteLine(SalaryIncrease());
            //DeleteDepartment(int id);
            //Town404("Berlin");
        }
        
        //Task 1
        static string GetHighlyPaidEmployees()
        {
            var employees = _context.Employees
                .OrderBy(e => e.LastName)
                .Where(e => e.Salary > 48000)
                .Select(e => new
                {
                    e.EmployeeId,
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle,
                    e.DepartmentId,
                    e.ManagerId,
                    e.AddressId
                })
                .ToList();
            var sb = new StringBuilder();
            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.DepartmentId} {e.ManagerId} {e.AddressId}");
            }
            return sb.ToString().TrimEnd();
        }

        //Task 2
        static string AddNewAddressForBrown()
        {
            var addressBrown = new Addresses()
            {
                AddressText = "Brown street"
            };
            _context.Addresses.Add(addressBrown);
            _context.SaveChanges();
         
            var employees = _context.Employees
                .Where(e => e.LastName == "Brown")
                .ToList();

            employees.ForEach(e => e.Address = addressBrown);
            var sb = new StringBuilder();
            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.DepartmentId} {e.ManagerId} {e.Address.AddressText}");
            }
            return sb.ToString().TrimEnd();
        }

        //Task 3
        static string ProjectAudit()
        {
            DateTime startDate = new DateTime(2002, 1, 1, 00, 00, 00);
            DateTime endDate = new DateTime(2005, 12, 31, 23, 59, 59);

            var employees = _context.Employees.Join(_context.EmployeesProjects,
                e => e.EmployeeId,
                p => p.EmployeeId,
                (e, p) => new { FirstName = e.FirstName, LastName = e.LastName, Manager = e.Manager, ProjectId = p.ProjectId, ProjectName = p.Project.Name, StartDate = p.Project.StartDate, EndDate = p.Project.EndDate })
                .Where(p => p.StartDate >= startDate && p.StartDate <= endDate || p.EndDate <= endDate && p.EndDate >= startDate)
                .Take(5)
                .ToList();
            var sb = new StringBuilder();
            foreach(var e in employees)
            {
                if(e.EndDate == null)
                    sb.AppendLine($"Employee: {e.FirstName} {e.LastName}  Manager: {e.Manager.FirstName} {e.Manager.LastName} \n Name project: {e.ProjectName} Start date: {e.StartDate} НЕ ЗАВЕРШЁН \n ");
                else
                    sb.AppendLine($"Employee: {e.FirstName} {e.LastName}  Manager: {e.Manager.FirstName} {e.Manager.LastName} \n Name project: {e.ProjectName} Start date: {e.StartDate} End date: {e.EndDate} \n ");
            }
            return sb.ToString().TrimEnd();
        }

        //Task 4
        static string EmployeeDossier()
        {
            Console.WriteLine("Enter employee id: ");
            int id = Convert.ToInt32(Console.ReadLine());

            var employeeProjects = _context.EmployeesProjects
                .Include(ep => ep.Employee)
                .Include(ep => ep.Project)
                .Where(ep => ep.EmployeeId == id)
                .ToList();

            var employee = _context.Employees
                .Where(e => e.EmployeeId == id)
                .ToList();
            
            var sb = new StringBuilder();
            foreach(var e in employee)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} - {e.JobTitle}\n");
            }

            sb.AppendLine("Projects: \n");
            foreach(var ep in employeeProjects)
            {
                sb.AppendLine($"{ep.Project.Name}");
            }
            return sb.ToString().TrimEnd();
        }

        //Task 5
        static string SmallDepartments()
        {

            var departments = _context.Departments
                .Select(d => new
                {
                    d.Name,
                    d.Employees
                })
                .Where(d => d.Employees.Count() < 5)
                .ToList();

            var sb = new StringBuilder();
            foreach (var d in departments)
            {
                sb.AppendLine($"{d.Name}");
            }
            return sb.ToString().TrimEnd();
        }

        //Task 6
        static string SalaryIncrease()
        {
            Console.WriteLine("Enter department name: ");
            string name = Console.ReadLine();

            Console.WriteLine("Enter %: ");
            Decimal percent = Convert.ToDecimal(Console.ReadLine());

            var department = _context.Departments
                .Select(d => new
                {
                    d.Name,
                    d.Employees
                })
                .Where(d => d.Name == name)
                .FirstOrDefault();

            var employees = _context.Employees
                .Where(e => e.Department.Name == name)
                .ToList();

            var sb = new StringBuilder();
            foreach (var e in employees)
            {
                Decimal salaryBeforeIncreasing = e.Salary;
                e.Salary = salaryBeforeIncreasing + salaryBeforeIncreasing * percent / 100;
                sb.AppendLine($"{e.FirstName} {e.LastName} Зарплата: {e.Salary}");
            }
            _context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        //Task 7
         static void DeleteDepartment(int id)
         {
            var department = _context.Departments
                .Where(d => d.DepartmentId == id)
                .FirstOrDefault();
            _context.Departments.Remove(department);

            _context.SaveChanges();
         }

        //Task 8
        

        static void Town404(string nameOfTownToRemove)
        {            
                var town = _context.Towns.
                    Where(t => t.Name.Equals(nameOfTownToRemove)).
                    FirstOrDefault();

                _context.Towns.Remove(town);
                _context.SaveChanges();
        }

    }
}