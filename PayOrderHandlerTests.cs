using Microsoft.EntityFrameworkCore;
using OrderManagementApp.Application.Commands;
using OrderManagementApp.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace OrderManagementApp.Tests
{
    public class PayOrderHandlerTests
    {
        private AppDbContext CreateInMemoryDb(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(dbName).Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task PayOrder_Only_When_Placed()
        {
            using var db = CreateInMemoryDb("pay-order-test");
            var customer = new OrderManagementApp.Domain.Customer { Id = Guid.NewGuid(), Name = "C", CreatedAt = DateTime.UtcNow };
            db.Customers.Add(customer);
            var order = new OrderManagementApp.Domain.Order { Id = Guid.NewGuid(), CustomerId = customer.Id, CreatedAt = DateTime.UtcNow, Status = OrderManagementApp.Domain.OrderStatus.Placed, TotalAmount = 10m };
            db.Orders.Add(order);
            await db.SaveChangesAsync();


            var handler = new PayOrderHandler(db);
            await handler.HandleAsync(new PayOrder(order.Id));


            var updated = await db.Orders.FindAsync(order.Id);
            Assert.Equal(OrderManagementApp.Domain.OrderStatus.Paid, updated!.Status);
        }
    }
}
