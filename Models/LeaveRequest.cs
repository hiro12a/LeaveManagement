using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagement.Models
{
    public class LeaveRequest : BaseEntity
    {
        [DisplayName("Start Date")]
        public DateTime StartDate {get;set;}

        [DisplayName("End Date")]
        public DateTime EndDate {get;set;}

        // Foreign Key for leave type
        public int LeaveTypeId {get;set;}
        [ForeignKey(nameof(LeaveTypeId))]
        public LeaveType? LeaveType {get;set;}

        public DateTime DateRequested {get;set;}
        public string? RequestComments {get;set;}

        public bool? Approved {get;set;}
        public bool Cancelled {get;set;}

        public string? RequestEmployeeId {get;set;}
    }
}