﻿namespace Labrary.RESTful.API.AutoMapper
{
    public class AutoMapperConfig:Profile
    {
        public AutoMapperConfig()
        {
            CreateMap<Book, BookDto>().ReverseMap();
        }
    }
}
