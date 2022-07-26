﻿using AutoMapper;
using Core.Entities;
using Infrastructure.DTOs.Account;
using Infrastructure.DTOs.Basket;
using Infrastructure.DTOs.Category;
using Infrastructure.DTOs.Customer;
using Infrastructure.DTOs.Order;
using Infrastructure.DTOs.Product;

namespace Infrastructure.AutoMapperProfile
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Customer, CustomerReadDTO>();
            CreateMap<Product, ProductReadDTO>();
            CreateMap<Category, CategoryReadDTO>();
            CreateMap<RegisterDto, Customer>();
            #region basket
            CreateMap<CustomerBasketDTO, CustomerBasket>();
            CreateMap<BasketItemDTO, BasketItem>();

                #endregion
            CreateMap<Customer, UserDto>();

            //there's no an address class 
            CreateMap<AddressDto, Address>();



        }
    }
}
