using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using TaskManagementApi.Core.DTOs.DTO_Subtask;
using TaskManagementApi.Core.Entities;

namespace TaskManagementApi.Core.Mapping_Profiles
{
    public class SubtaskMappingProfile : Profile
    {
        public SubtaskMappingProfile()
        {
            CreateMap<DTO_SubtaskPost, SubTaskItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ParentTaskId, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.UserId, opt => opt.Ignore()) // Set in service
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.ParentTask, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());

            // Mapping from SubTaskItem entity to SubTaskGetDto for retrieval
            CreateMap<SubTaskItem, DTO_SubtaskGet>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));

            // Mapping from SubTaskPutDto to SubTaskItem entity for updates
            CreateMap<DTO_SubtaskPut, SubTaskItem>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ParentTaskId, opt => opt.Ignore()) // Should not be updated
                .ForMember(dest => dest.UserId, opt => opt.Ignore()) // Should not be updated
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.ParentTask, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore());
        }
    }
}
