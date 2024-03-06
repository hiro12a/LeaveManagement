using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeaveManagement.Data.Repository.IRepository;

namespace LeaveManagement.Data.Repository.IRepository
{
    // A collection of specific IRepository so we don't cluster up program.cs
    public interface IUnitofWork
    {
        ILeaveTypeRepository LeaveTypes {get;}
    }
}