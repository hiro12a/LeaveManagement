using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace LeaveManagement.Models.ViewModels
{
    // IValidatableObject allows us to implement more validation
    public class LeaveRequestCreateVM : IValidatableObject
    {
        [DisplayName("Start Date")]
        [DataType(DataType.Date)]
        public DateTime StartDate {get;set;}

        [DisplayName("End Date")]
        [DataType(DataType.Date)]
        public DateTime EndDate {get;set;}

        // Foreign Key for leave type
        [Required]
        [DisplayName("Leave Type")]
        public int LeaveTypeId { get; set; }
        public SelectList? LeaveTypes { get; set; }

        public DateTime DateRequested {get;set;}

        [DisplayName("Leave Reason")]
        public string? RequestComments {get;set;}

        // IValidatableObject allows for custom logic
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Makes sure start date is less than end date if detected otherwise
            if(StartDate > EndDate)
            {
                yield return new ValidationResult("The Start Date Must Be Before End Date", new[] {nameof(StartDate), nameof(EndDate)});
            }
        }
    }
}