using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagement.Models
{
    // Make it partial so people can't accidentally call it
    // Every models will contain things from this class after inheriting it
    // So we don't have to repeat it in the model classes
    public partial class BaseEntity
    {
        public int Id { get; set; }
        public DateTime DateCreated {get;set;}
        public DateTime DateModified {get;set;}
    }
}