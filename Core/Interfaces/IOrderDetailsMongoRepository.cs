﻿using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IOrderDetailsMongoRepository
    {
        List<OrderDetail> GetAllOrderDetails();
        public List<OrderDetail> GetOrderDetailByUserId(string userId);
        public OrderDetail GetOrderDetailByOrderId(string orderId);
        void AddOrderDetail(OrderDetail order);
    }
}
