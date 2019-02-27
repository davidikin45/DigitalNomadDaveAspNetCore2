﻿using AspNetCore.Base.Mapping;
using AspNetCore.Base.ModelMetadataCustom.DisplayAttributes;
using AutoMapper;
using System;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace AspNetCore.Base.Dtos
{
    public class FileMetadataDto : DtoBase<string>, IHaveCustomMappings
    {
        //[Render(ShowForDisplay = false, ShowForEdit = false, ShowForGrid = false)]
        public FileInfo File { get; set; }

        [Render(AllowSortForGrid = true)]
        [Required]
        public DateTime CreationTime { get; set; }

        [Render(ShowForGrid = false), Required]
        public string Caption { get; set; }


        public void CreateMappings(IMapperConfigurationExpression configuration)
        {
            configuration.CreateMap<FileInfo, FileMetadataDto>()
            .ForMember(dto => dto.Id, bo => bo.MapFrom(s => s.FullName))
            .ForMember(dto => dto.Caption, bo => bo.MapFrom(s => Path.GetFileNameWithoutExtension(s.Name)))
            .ForMember(dto => dto.CreationTime, bo => bo.MapFrom(s => s.LastWriteTime))
            .ForMember(dto => dto.File, bo => bo.MapFrom(s => s));
        }
    }
}
