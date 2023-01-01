using AutoMapper;
using Core.Entities.OrderAggregate;
using API.Dtos;
using Microsoft.Extensions.Configuration;

namespace API.Helpers
{
    public class OrderITemUrlResolver : IValueResolver<OrderItem,OrderItemDto,string>
    {
        private readonly IConfiguration _config;

        public OrderITemUrlResolver(IConfiguration config)
        {
            _config=config;
        }

        public string Resolve(OrderItem source,OrderItemDto destination,string destMember,ResolutionContext context)
        {
            if(!string.IsNullOrEmpty(source.ItemOrdered.PictureUrl))
            {
                return _config["ApiUrl"] + source.ItemOrdered.PictureUrl;
            }

            return null;
        }
    }
}