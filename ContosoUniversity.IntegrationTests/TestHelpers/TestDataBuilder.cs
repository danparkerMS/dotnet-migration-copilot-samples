using ContosoUniversity.Models;

namespace ContosoUniversity.IntegrationTests.TestHelpers
{
    public static class TestDataBuilder
    {
        public static List<Student> GetTestStudents()
        {
            return new List<Student>
            {
                new Student
                {
                    ID = 1,
                    FirstMidName = "Carson",
                    LastName = "Alexander",
                    EnrollmentDate = DateTime.Parse("2019-09-01")
                },
                new Student
                {
                    ID = 2,
                    FirstMidName = "Meredith",
                    LastName = "Alonso",
                    EnrollmentDate = DateTime.Parse("2017-09-01")
                },
                new Student
                {
                    ID = 3,
                    FirstMidName = "Arturo",
                    LastName = "Anand",
                    EnrollmentDate = DateTime.Parse("2018-09-01")
                }
            };
        }

        public static List<Instructor> GetTestInstructors()
        {
            return new List<Instructor>
            {
                new Instructor
                {
                    ID = 1,
                    FirstMidName = "Kim",
                    LastName = "Abercrombie",
                    HireDate = DateTime.Parse("1995-03-11")
                },
                new Instructor
                {
                    ID = 2,
                    FirstMidName = "Fadi",
                    LastName = "Fakhouri",
                    HireDate = DateTime.Parse("2002-07-06")
                }
            };
        }

        public static List<Department> GetTestDepartments()
        {
            return new List<Department>
            {
                new Department
                {
                    DepartmentID = 1,
                    Name = "English",
                    Budget = 350000,
                    StartDate = DateTime.Parse("2007-09-01"),
                    InstructorID = 1
                },
                new Department
                {
                    DepartmentID = 2,
                    Name = "Mathematics",
                    Budget = 100000,
                    StartDate = DateTime.Parse("2007-09-01"),
                    InstructorID = 2
                }
            };
        }

        public static List<Course> GetTestCourses()
        {
            return new List<Course>
            {
                new Course
                {
                    CourseID = 1050,
                    Title = "Chemistry",
                    Credits = 3,
                    DepartmentID = 2
                },
                new Course
                {
                    CourseID = 4022,
                    Title = "Microeconomics",
                    Credits = 3,
                    DepartmentID = 1
                }
            };
        }
    }
}
