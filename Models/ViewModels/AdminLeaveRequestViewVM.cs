using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LeaveManagement.Models.ViewModels
{
    public class AdminLeaveRequestViewVM
    {
        [DisplayName("Total Number Of Requests")]
        public int TotalRequests { get; set; }
        
        [DisplayName("Approved Requests")]
        public int ApprovedRequests { get; set; }
        
        [DisplayName("Pending Requests")]
        public int PendingRequests { get; set; }
        
        [DisplayName("Rejected Requests")]
        public int RejectedRequests { get; set; }

        public List<LeaveRequestVM>? LeaveRequests { get; set; }
    }
}