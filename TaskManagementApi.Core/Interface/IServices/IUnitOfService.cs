using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Core.Interface;

namespace TaskManagementApi.Core.Interface
{
    public interface IUnitOfService
    {
        ITaskItemService TaskItemService { get; }
    }
}
