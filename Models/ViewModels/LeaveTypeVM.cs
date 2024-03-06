using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagement.Models.ViewModels
{
    public class LeaveTypeVM
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        [DisplayName("Default Number Of Days")]
        public int DefaultDays { get; set; }
    }
}