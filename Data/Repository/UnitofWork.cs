using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LeaveManagement.Data.Repository.IRepository;
using LeaveManagement.Models;

namespace LeaveManagement.Data.Repository
{
    public class UnitofWork : IUnitofWork
    {
        private readonly ApplicationDbContext _db;
        public ILeaveTypeRepository LeaveTypes {get; private set;}

        public UnitofWork(ApplicationDbContext context){
            _db = context; 
            LeaveTypes = new LeaveTypeRepository(_db);
        }
    }
}