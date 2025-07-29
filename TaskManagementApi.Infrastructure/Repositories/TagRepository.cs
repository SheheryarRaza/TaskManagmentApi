using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TaskManagementApi.Core.Data;
using TaskManagementApi.Core.Entities;
using TaskManagementApi.Core.Interface.IRepositories;

namespace TaskManagementApi.Infrastructure.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly ApplicationDbContext _context;

        public TagRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IQueryable<Tag> GetAllTagsQueryable()
        {
            return _context.Tags.AsQueryable();
        }

        public async Task<Tag?> GetTagByIdAsync(int id)
        {
            return await _context.Tags.FindAsync(id);
        }

        public async Task<Tag?> GetTagByNameAsync(string name)
        {
            return await _context.Tags.FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower());
        }

        public async Task AddTagAsync(Tag tag)
        {
            _context.Tags.Add(tag);
        }

        public async Task UpdateTagAsync(Tag tag)
        {
            _context.Entry(tag).State = EntityState.Modified;
        }

        public async Task DeleteTagAsync(Tag tag)
        {
            _context.Tags.Remove(tag);
        }

        public async Task<bool> TagExistsAsync(int id)
        {
            return await _context.Tags.AnyAsync(e => e.Id == id);
        }
    }
}
