using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using TaskManagementApi.Core.DTOs;
using TaskManagementApi.Core.Entities;

namespace TaskManagementApi.Core.Mapping_Profiles
{
    public class TaskMappingProfile : Profile
    {
        public TaskMappingProfile()
        {
            CreateMap<DTO_TaskPost, TaskItem>()
                .ForMember(dest => dest.UserId, opt => opt.Ignore()) // UserId set in service
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // CreatedAt set in service
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()) // UpdatedAt set in service
                .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => false)) // Default for new tasks
                .ForMember(dest => dest.User, opt => opt.Ignore());


            CreateMap<TaskItem, DTO_TaskGet>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName));

            CreateMap<DTO_TaskPut, TaskItem>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id)) // Ensure Id is mapped if present in DTO
                .ForMember(dest => dest.UserId, opt => opt.Ignore()) // UserId should not be updated via PUT
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // CreatedAt should not be updated via PUT
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore()) // UpdatedAt set in service
                .ForMember(dest => dest.User, opt => opt.Ignore());
        }

    }
}
