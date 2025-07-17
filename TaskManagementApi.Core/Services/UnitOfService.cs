using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagementApi.Core.Interface;
using TaskManagementApi.Core.Interface.IServices;


namespace TaskManagementApi.Core.Services
{
    public class UnitOfService : IUnitOfService
    {
        private readonly ITaskItemService _taskItemService;
        private readonly IAuthService _authService;
        private readonly ISubtaskItemService _subtaskItemService;

        public UnitOfService(ITaskItemService taskItemService, IAuthService authService, ISubtaskItemService subtaskItemService)
        {
            _taskItemService = taskItemService;
            _authService = authService;
            _subtaskItemService = subtaskItemService;
        }

        public ITaskItemService TaskItemService => _taskItemService;
        public IAuthService AuthService => _authService;

        public ISubtaskItemService SubtaskItemService => _subtaskItemService;
    }
}
