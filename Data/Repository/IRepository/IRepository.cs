using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LeaveManagement.Data.Repository.IRepository
{
    // Since we don't know what class will be used, 
    // <T> where T : class allows us to generically call the classes
    public interface IRepository<T> where T : class
    {
        Task<T> GetAsync(int? id); // Get obj
        Task<List<T>> GetAllAsync(); // Get a list of objs
        Task<T> AddAsync(T entity); // Adds obj to database
        Task<bool> Exists(int id); // Make sure obj exists in database by its id
        Task DeleteAsync(int id); // Delete obj by id
        Task UpdateAsync(T entity); // Update objs
    }
}