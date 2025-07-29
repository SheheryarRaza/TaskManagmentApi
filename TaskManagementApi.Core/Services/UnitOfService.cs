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
        private readonly ITagService _tagService;

        public UnitOfService(ITaskItemService taskItemService, IAuthService authService, ISubtaskItemService subtaskItemService, ITagService tagService)
        {
            _taskItemService = taskItemService;
            _authService = authService;
            _subtaskItemService = subtaskItemService;
            _tagService = tagService;
        }

        public ITaskItemService TaskItemService => _taskItemService;
        public IAuthService AuthService => _authService;

        public ISubtaskItemService SubtaskItemService => _subtaskItemService;
        public ITagService TagService => _tagService;
    }
}
