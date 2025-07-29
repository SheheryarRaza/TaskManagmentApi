using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskManagementApi.Core.Interface.IRepositories
{
    public interface IUnitOfWork
    {
        ITaskItemRepository TaskItemRepository { get; }

        ISubtaskItemRepository SubtaskItemRepository { get; }

        ITagRepository TagRepository { get; }
        int SaveChanges();
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    }
}
