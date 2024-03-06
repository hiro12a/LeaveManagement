using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using LeaveManagement.Data.Repository.IRepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace LeaveManagement.Data.Repository
{
    // Inherits from IRepository
    // This is like a wrapper and is more secure
    public class Repository<T> : IRepository<T> where T : class
    {
        // Call the DbContext so that we can update our database through the repository
        private readonly ApplicationDbContext _db;
        public Repository(ApplicationDbContext context)
        {
            _db = context;
        }

        // Adds item to database 
        public async Task<T> AddAsync(T entity)
        {
            await _db.AddAsync(entity);
            await _db.SaveChangesAsync();
            
            return entity;
        }

        // Remove item from database
        public async Task DeleteAsync(int id)
        {
            // Allows us to call from whatever table
            // Makes it generic 
            var obj = await GetAsync(id); // Finds obj by using the GetAsync
            _db.Set<T>().Remove(obj);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> Exists(int id)
        {
            var obj = await GetAsync(id);
            return obj != null;
        }

        public async Task<List<T>> GetAllAsync()
        {
            return await _db.Set<T>().ToListAsync();
        }

        public async Task<T> GetAsync(int? id)
        {
            if(id == null)
            {
                return null;
            }
            return await _db.Set<T>().FindAsync(id);
        }

        public async Task UpdateAsync(T entity)
        {
            _db.Update(entity);
            await _db.SaveChangesAsync();
        }
    }
}