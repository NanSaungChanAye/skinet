using Core.Interfaces;
using Core.Entities.OrderAggregate;
using System.Threading.Tasks;
using System.Collections.Generic;
using Core.Entities;
using System.Linq;
using Core.Specifications;


namespace Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        /*private readonly IGenericRepository<Order> _orderRepo;
        private readonly IGenericRepository<DeliveryMethod> _dmRepo;
        private readonly IGenericRepository<Product> _productRepo;*/
        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;

        /*public OrderService(IGenericRepository<Order> orderRepo,IGenericRepository<DeliveryMethod> dmRepo,IGenericRepository<Product> productRepo,IBasketRepository basketRepo)
        {
            _orderRepo=orderRepo;
            _dmRepo=dmRepo;
            _productRepo=productRepo;
            _basketRepo=basketRepo;
        }*/

        public OrderService(IBasketRepository basketRepo,IUnitOfWork unitOfWork)
        {
            _basketRepo=basketRepo;
            _unitOfWork=unitOfWork;
        }

        public async Task<Order> CreateOrderAsync(string buyerEmail,int deliveryMethodId,string basketId,Address shippingAddress)
        {
            //get basket form the repo
            var basket= await _basketRepo.GetBasketAsync(basketId);
            //get items from the product repo
            var items=new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                //var productItem=await _productRepo.GetByIdAsync(item.Id);
                var productItem=await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                var itemOrdered=new ProductItemOrdered(productItem.Id,productItem.Name,productItem.PictureUrl);
                var orderItem=new OrderItem(itemOrdered,productItem.Price,item.Quantity);
                items.Add(orderItem);
            }
            //get delivery method from repo
           // var deliveryMethod=await _dmRepo.GetByIdAsync(deliveryMethodId);
            var deliveryMethod=await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);
            //calculate subtotal
            var subtotal=items.Sum(item=>item.Price * item.Quantity);
            //crete order
            var order=new Order(items, buyerEmail, shippingAddress, deliveryMethod,subtotal);
            _unitOfWork.Repository<Order>().Add(order);
            //save to db
            var result=await _unitOfWork.Complete();
            if(result<=0) return null;
            
            //delete basket
            await _basketRepo.DeleteBasketAsync(basketId);
          
            //return order 
            return order;
        }
        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
           var spec=new OrdersWithItemsAndOrderingSpecification(buyerEmail);
           return await _unitOfWork.Repository<Order>().ListAsync(spec);
        }
        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            return await _unitOfWork.Repository<DeliveryMethod>().ListAllAsync(); 
        }

        public async Task<Order> GetOrderByIdAsync(int id,string buyerEmail)
        {
            var spec=new OrdersWithItemsAndOrderingSpecification(id,buyerEmail);
            return await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
        }
    }
}