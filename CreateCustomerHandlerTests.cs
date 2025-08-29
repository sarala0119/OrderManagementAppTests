using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
    public class CreateCustomerHandlerTests
    {
        private AppDbContext CreateInMemoryDb(string dbName)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().UseInMemoryDatabase(dbName).Options;
            return new AppDbContext(options);
        }

        [Fact]
        public async Task Create_Customer_HandleAsync()
        {
            using var dbContext = CreateInMemoryDb("add-item-test");
            
            var handler = new CreateCustomerHandler(dbContext);
            var command = new CreateCustomer("John Doe", "john@example.com");

            // Act
            await handler.HandleAsync(command);

            // Assert
            var customer = await dbContext.Customers.FirstOrDefaultAsync(c => c.Email == "john@example.com");
            Assert.NotNull(customer);
            Assert.Equal("John Doe", customer!.Name);
            Assert.Equal("john@example.com", customer.Email);
            Assert.True(customer.CreatedAt <= System.DateTime.UtcNow);

        }

    }
}
