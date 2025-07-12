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
        private readonly IAuthService _authService;

        public UnitOfService(ITaskItemService taskItemService, IAuthService authService)
        {
            _taskItemService = taskItemService;
            _authService = authService;
        }

        public ITaskItemService TaskItemService => _taskItemService;
        public IAuthService AuthService => _authService;
    }
}
