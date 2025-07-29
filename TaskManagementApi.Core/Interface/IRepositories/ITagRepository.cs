using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Core.Entities;

namespace TaskManagementApi.Core.Interface.IRepositories
{
    public interface ITagRepository
    {
        IQueryable<Tag> GetAllTagsQueryable();
        Task<Tag?> GetTagByIdAsync(int id);
        Task<Tag?> GetTagByNameAsync(string name);
        Task AddTagAsync(Tag tag);
        Task UpdateTagAsync(Tag tag);
        Task DeleteTagAsync(Tag tag);
        Task<bool> TagExistsAsync(int id);
    }
}
