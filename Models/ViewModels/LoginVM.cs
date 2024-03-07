using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Net.Http.Headers;

namespace LeaveManagement.Models.ViewModels
{
    public class LoginVM
    {
        [Required]
        public string? Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string? Password {get;set;}

        public bool RememberMe {get;set;}
        public string? RedirectUrl { get; set; }
    }
}