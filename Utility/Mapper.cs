using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LeaveManagement.Models;
using LeaveManagement.Models.ViewModels;

namespace LeaveManagement.Utility
{
    public class Mapper : Profile
    {
        public Mapper()
        {
            // ReverseMap allows us to automatically reverse it from leavetypeVm to leavetype
            CreateMap<LeaveTypeVM, LeaveType>().ReverseMap();
        }
    }
}