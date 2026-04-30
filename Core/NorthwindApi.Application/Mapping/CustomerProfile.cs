using AutoMapper;
using NorthwindApi.Application.Features.Customers.Commands.CreateCustomer;
using NorthwindApi.Application.Features.Customers.Commands.UpdateCustomer;
using NorthwindApi.Domain.Entities;
using NorthwindApi.Application.Features.Customers.Queries.GetCustomers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NorthwindApi.Application.Mapping
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, GetCustomersQueryResponse>();
            CreateMap<CreateCustomerCommand, Customer>();
            CreateMap<UpdateCustomerCommand, Customer>()
                .ForMember(dest => dest.CustomerId, opt => opt.Ignore());
        }
    }
}