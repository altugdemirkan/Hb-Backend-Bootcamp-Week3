﻿using Core.Interfaces;
using Core.Entities;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;

namespace Domain.Commands.Orders
{
  public class OrderCommandHandler : IRequestHandler<CreateOrderCommand>
  {
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderDetailsMongoRepository _orderdetailsRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public OrderCommandHandler(IMapper mapper, IOrderItemRepository orderItemRepository,
        IOrderRepository orderRepository, IProductRepository productRepository,
        IOrderDetailsMongoRepository orderdetailRepository, IUserRepository userRepository)
    {
      _orderItemRepository = orderItemRepository;
      _orderRepository = orderRepository;
      _productRepository = productRepository;
      _orderdetailsRepository = orderdetailRepository;
      _userRepository = userRepository;
      _mapper = mapper;
    }
    public Task<Unit> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
      Order order = new Order(request.UserId);

      OrderDetail orderDetail = new OrderDetail();
      MongoOrderItem mongoOrderItem;
      MongoProduct mongoProduct;
      List<MongoOrderItem> mongoOrderItemList = new List<MongoOrderItem>();


      _orderRepository.CreateDapper(order);

      foreach(var item in request.OrderItems)
      {
        mongoProduct = new MongoProduct();
        mongoOrderItem = new MongoOrderItem();
        var dbProduct = _productRepository.GetById(item.ProductId);
        OrderItem orderItem = new OrderItem(item.ProductId, order.Id, dbProduct.Price, item.Quantity);
        mongoProduct.Name = dbProduct.Name;
        mongoProduct.Price = dbProduct.Price;
        mongoOrderItem.Product = mongoProduct;
        mongoOrderItem.Quantity = item.Quantity;
        mongoOrderItemList.Add(mongoOrderItem);

        _orderItemRepository.Create(orderItem);
      }

      orderDetail.User = _mapper.Map<UserDetail>(_userRepository.GetById(request.UserId));
      orderDetail.OrderItems = mongoOrderItemList;
      orderDetail.OrderId = order.Id.ToString();
      orderDetail.OrderDate = DateTime.Now;

      _orderdetailsRepository.AddOrderDetail(orderDetail);

      return Unit.Task;
    }
  }
}
