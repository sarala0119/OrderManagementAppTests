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
    public class OrderItemHandlerTests
    {
        private AppDbContext CreateInMemoryDb(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(dbName).Options;
            return new AppDbContext(options);
        }


        [Fact]
        public async Task AddOrderItem_CalculatesLineTotal_And_TotalAmount()
        {
            using var db = CreateInMemoryDb("add-item-test");
            var customer = new OrderManagementApp.Domain.Customer { Id = Guid.NewGuid(), Name = "C", CreatedAt = DateTime.UtcNow };
            var product = new OrderManagementApp.Domain.Product { Id = Guid.NewGuid(), Sku = "SKU1", Name = "P", UnitPrice = 12.5m, CreatedAt = DateTime.UtcNow };
            db.Customers.Add(customer); db.Products.Add(product);
            var order = new OrderManagementApp.Domain.Order { Id = Guid.NewGuid(), CustomerId = customer.Id, CreatedAt = DateTime.UtcNow, Status = OrderManagementApp.Domain.OrderStatus.Draft, TotalAmount = 0m };
            db.Orders.Add(order);
            await db.SaveChangesAsync();


            var handler = new AddOrderItemHandler(db);
            var cmd = new AddOrderItem(order.Id, product.Id, 3);
            await handler.HandleAsync(cmd);


            var updatedOrder = await db.Orders.Include(o => o.Items).FirstOrDefaultAsync(o => o.Id == order.Id);
            Assert.NotNull(updatedOrder);
            Assert.Single(updatedOrder!.Items);
            Assert.Equal(37.5m, updatedOrder.TotalAmount);
        }
    }
}
