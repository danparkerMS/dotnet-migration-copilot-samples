using Microsoft.EntityFrameworkCore;
using ContosoUniversity.Data;

namespace ContosoUniversity.Tests.TestHelpers
{
    public static class InMemoryDbContextFactory
    {
        public static SchoolContext Create(string? databaseName = null)
        {
            var options = new DbContextOptionsBuilder<SchoolContext>()
                .UseInMemoryDatabase(databaseName ?? Guid.NewGuid().ToString())
                .Options;

            var context = new SchoolContext(options);
            context.Database.EnsureCreated();
            
            return context;
        }

        public static SchoolContext CreateWithData(string? databaseName = null)
        {
            var context = Create(databaseName);
            SeedTestData(context);
            return context;
        }

        private static void SeedTestData(SchoolContext context)
        {
            // Add sample test data
            context.Students.AddRange(TestDataBuilder.GetTestStudents());
            context.Instructors.AddRange(TestDataBuilder.GetTestInstructors());
            context.Departments.AddRange(TestDataBuilder.GetTestDepartments());
            context.Courses.AddRange(TestDataBuilder.GetTestCourses());
            
            context.SaveChanges();
        }
    }
}
