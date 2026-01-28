using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ContosoUniversity.Data;
using System.Linq;

namespace ContosoUniversity.IntegrationTests.TestHelpers
{
    public class ContosoWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        private readonly string _dbName = Guid.NewGuid().ToString();

        public ContosoWebApplicationFactory()
        {
            // Ensure the server is created on initialization
            _ = Server;
            
            // Seed the database
            using (var scope = Services.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<SchoolContext>();
                context.Database.EnsureCreated();

                // Add test data in separate operations to avoid tracking conflicts
                context.Instructors.AddRange(TestDataBuilder.GetTestInstructors());
                context.SaveChanges();
                context.ChangeTracker.Clear();

                context.Departments.AddRange(TestDataBuilder.GetTestDepartments());
                context.SaveChanges();
                context.ChangeTracker.Clear();

                context.Courses.AddRange(TestDataBuilder.GetTestCourses());
                context.SaveChanges();
                context.ChangeTracker.Clear();

                context.Students.AddRange(TestDataBuilder.GetTestStudents());
                context.SaveChanges();
                context.ChangeTracker.Clear();
            }
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            
            builder.ConfigureTestServices(services =>
            {
                // Add InMemory database for testing
                // (Program.cs won't add SQL Server DbContext in Testing environment)
                services.AddDbContext<SchoolContext>(options =>
                {
                    options.UseInMemoryDatabase(_dbName);
                });
            });
        }
    }
}
