using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LeaveManagement.Models.ViewModels
{
    public class RegisterVM
    {
        [Required]
        public string? Email {get;set;}

        [Required]
        [DataType(DataType.Password)] // Hides the password
        public string? Password {get;set;}
        [Required]
        [DataType(DataType.Password)] // Hides the password
        [Compare(nameof(Password))] // Makes sure confirm password is the same as pasword
        [DisplayName("Confirm Password")]
        public string? ConfirmPassword{get;set;}

        [DisplayName("First Name")]
        public string? FirstName {get;set;}
        [DisplayName("Last Name")]
        public string? LastName {get;set;}
        [DisplayName("Phone Number")]
        public string? PhoneNumber {get;set;}
        public string? Address {get;set;}
        public DateTime JoinDate {get;set;}
        public DateTime DOB {get;set;}

         public string? RedirectUrl { get; set; } 

        // For dropdown to display the roles
        public string? Role {get;set;}
        [ValidateNever]
        public IEnumerable<SelectListItem>? RoleList{get;set;}
    }
}