using AutoMapper;
using SmartStore.ShoppingCartAPI.Dtos;
using SmartStore.ShoppingCartAPI.Models;

namespace SmartStore.ShoppingCartAPI.Helpers
{
    public class MappingProfile:Profile
    {
        public MappingProfile()
        {
            CreateMap<Product,ProductDto>().ReverseMap();   
            CreateMap<CartDetails,CartDetailsDto>().ReverseMap();   
            CreateMap<CartHeader,CartHeaderDto>().ReverseMap();   
            CreateMap<Cart,CartDto>().ReverseMap();   
        }
    }
}
