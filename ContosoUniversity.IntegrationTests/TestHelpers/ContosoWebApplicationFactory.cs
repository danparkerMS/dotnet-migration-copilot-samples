using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using ContosoUniversity.Data;

namespace ContosoUniversity.IntegrationTests.TestHelpers
{
    public class ContosoWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        private readonly string _dbName = Guid.NewGuid().ToString();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Replace the SchoolContext with one using InMemory database
                services.RemoveAll<DbContextOptions<SchoolContext>>();
                services.AddDbContext<SchoolContext>(options =>
                {
                    options.UseInMemoryDatabase(_dbName);
                });

                // Build the service provider and seed data
                var serviceProvider = services.BuildServiceProvider();
                using (var scope = serviceProvider.CreateScope())
                {
                    var db = scope.ServiceProvider.GetRequiredService<SchoolContext>();
                    db.Database.EnsureCreated();
                    SeedTestData(db);
                }
            });

            builder.UseEnvironment("Testing");
        }

        private void SeedTestData(SchoolContext context)
        {
            var students = TestDataBuilder.GetTestStudents();
            var instructors = TestDataBuilder.GetTestInstructors();
            var departments = TestDataBuilder.GetTestDepartments();
            var courses = TestDataBuilder.GetTestCourses();

            context.Students.AddRange(students);
            context.Instructors.AddRange(instructors);
            context.Departments.AddRange(departments);
            context.Courses.AddRange(courses);
            
            context.SaveChanges();
        }
    }
}
