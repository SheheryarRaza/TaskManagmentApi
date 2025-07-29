using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using TaskManagementApi.Core.DTOs.DTO_Tag;
using TaskManagementApi.Core.Entities;

namespace TaskManagementApi.Core.Mapping_Profiles
{
    public class TagMappingProfile : Profile
    {
        public TagMappingProfile()
        {
            CreateMap<Tag , DTO_Tag>().ReverseMap();
        }
    }
}
