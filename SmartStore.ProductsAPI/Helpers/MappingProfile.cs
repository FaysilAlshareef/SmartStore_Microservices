using AutoMapper;
using SmartStore.ProductsAPI.Dtos;
using SmartStore.ProductsAPI.Entities;

namespace SmartStore.ProductsAPI.Helpers
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductDto>().ReverseMap();
                             
        }
    }
}
