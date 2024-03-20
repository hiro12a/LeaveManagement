using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagement.Models.ViewModels
{
    public class EmployeeListVM
    {
        public string? Id {get;set;}
        [Display(Name = "First Name")]
        public string? FirstName {get;set;}

        [Display(Name = "Last Name")]
        public string? LastName {get;set;}

        [Display(Name = "Date Joined")]
        [DataType(DataType.Date)]
        public DateTime DateJoined {get;set;}

        [Display(Name = "Email Address")]
        public string? Email {get;set;}
    }
}