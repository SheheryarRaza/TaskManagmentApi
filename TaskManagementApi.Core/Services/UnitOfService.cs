using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Core.Interface;


namespace TaskManagementApi.Core.Services
{
    public class UnitOfService : IUnitOfService
    {
        private readonly ITaskItemService _taskItemService;

        public UnitOfService(ITaskItemService taskItemService)
        {
            _taskItemService = taskItemService;
        }

        public ITaskItemService TaskItemService => _taskItemService;
    }
}
