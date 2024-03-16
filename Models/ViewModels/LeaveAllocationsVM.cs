using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagement.Models.ViewModels
{
    public class LeaveAllocationsVM
    {
        public int Id {get;set;}
        public int NumberOfDays {get;set;}
        public int Period {get;set;}

        // Never talk to table from VM, only VM to VM
        public LeaveTypeVM? LeaveType {get;set;}
    }
}