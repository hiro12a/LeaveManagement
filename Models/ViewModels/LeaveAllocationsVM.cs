using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagement.Models.ViewModels
{
    public class LeaveAllocationsVM
    {
        [Required]
        public int Id {get;set;}

        [DisplayName("Number of Days")]
        [Required]
        [Range(1,50, ErrorMessage = "Invalid Number Entered")]
        public int NumberOfDays {get;set;}

        [Required]
        [DisplayName("Allocation Period")]
        public int Period {get;set;}

        // Never talk to table from VM, only VM to VM
        public LeaveTypeVM? LeaveType {get;set;}
    }
}