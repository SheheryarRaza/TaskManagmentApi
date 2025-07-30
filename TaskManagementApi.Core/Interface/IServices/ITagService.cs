using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Core.DTOs.DTO_Tag;

namespace TaskManagementApi.Core.Interface.IServices
{
    public interface ITagService
    {
        Task<IEnumerable<DTO_Tag>> GetAllTagsAsync();

        Task<DTO_Tag?> GetTagByIdAsync(int id);

        Task<DTO_Tag> CreateTagAsync(DTO_Tag tagDto);

        Task<bool> UpdateTagAsync(int id, DTO_Tag tagDto);

        Task<bool> DeleteTagAsync(int id);
    }
}
