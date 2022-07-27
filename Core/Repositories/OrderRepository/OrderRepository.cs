﻿using Core.Context;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Repositories.BasketRepository;
using Core.Repositories.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories.OrderRepository
{

    public class OrderRepository : IOrderRepository //GenericRepository<Order>, 

    {
       
        //private readonly IDeliveryMethodRepository _deliveryMethodRepo;
        //private readonly IProductRepository _productRepo;
        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly StoreContext _storeContext;

        public OrderRepository(IBasketRepository basketRepo, IUnitOfWork unitOfWork , StoreContext storeContext)

            ////IDeliveryMethodRepository deliveryMethodRepo,
            ////IProductRepository productRepo,  StoreContext storecontext) : base(storecontext)
        {
            _unitOfWork = unitOfWork;
            _storeContext = storeContext;
            ////_deliveryMethodRepo = deliveryMethodRepo;
            ////_productRepo = productRepo;
            _basketRepo = basketRepo;
        
        }

       

        public async Task<Order> CreateOrderAsync(string buyerEmail, Guid delieveryMethodId, string basketId, Address shippingAddress)
        {
            //check for items in db for actual prices and data from basket

            // get basket from repo
            var basket = await _basketRepo.GetBasket(basketId);

            // get items from the product repo
            var items = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                var itemOrdered = new ProductItemOrdered(productItem.Id, productItem.Name, productItem.PictureUrl);
                var orderItem = new OrderItem(itemOrdered, productItem.Price, item.NumberOfItem);
                items.Add(orderItem);
            }

            // get deliverey method from repo
           var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(delieveryMethodId);

            // calc subtotal (linq)
            var subtotal = items.Sum(item => item.Price * item.Quantity);

            // create order
            var order = new Order(buyerEmail, shippingAddress, deliveryMethod, items, subtotal);
            _unitOfWork.Repository<Order>().Add(order);

            // important nooooote::: we will need to write code to save it to DB later Before Running The project
            var result = await _unitOfWork.Complete();

            if (result <= 0) return null;


            //delete basket
            await _basketRepo.DeleteBasket(basketId);

            // return  order
            return order;
        }

       

        public Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            return _unitOfWork.Repository<DeliveryMethod>().GetAllAsync();
        }

        public  Order GetOrdersForUserAsync(int id, string buyerEmail)
        {
            return _storeContext.Set<Order>().FirstOrDefault(u => u.Id == id && u.BuyerEmail == buyerEmail);
        }

        public IReadOnlyList<Order> GetOrdersForUserAsync(string buyerEmail)
        {
            return _storeContext.Set<Order>().Where(u => u.BuyerEmail == buyerEmail).ToList();

        }
    }
}
