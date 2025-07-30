using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using TaskManagementApi.Core.DTOs.DTO_Tasks;
using TaskManagementApi.Core.Entities;

namespace TaskManagementApi.Core.Mapping_Profiles
{
    public class TaskMappingProfile : Profile
    {
        public TaskMappingProfile()
        {
            CreateMap<DTO_TaskPost, TaskItem>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.AssignedToUserId != null ? src.AssignedToUserId : (string)null))
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedByUser, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore()) // Not set on creation
                .ForMember(dest => dest.IsNotified, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.TaskItemTags, opt => opt.Ignore());


            CreateMap<TaskItem, DTO_TaskGet>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.AssignedToUserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.AssignedByUserName, opt => opt.MapFrom(src => src.AssignedByUser != null ? src.AssignedByUser.UserName : null))
                .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.TaskItemTags.Select(tit => tit.Tag.Name).ToList()));

            CreateMap<DTO_TaskPut, TaskItem>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.AssignedByUser, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsNotified, opt => opt.Ignore())
                .ForMember(dest => dest.TaskItemTags, opt => opt.Ignore());
        }

    }
}
