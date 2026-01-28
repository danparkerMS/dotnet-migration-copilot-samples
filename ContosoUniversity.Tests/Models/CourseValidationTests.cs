using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ContosoUniversity.Models;
using FluentAssertions;
using Xunit;

namespace ContosoUniversity.Tests.Models
{
    public class CourseValidationTests
    {
        [Fact]
        public void ValidCourse_ShouldPassValidation()
        {
            var course = new Course
            {
                CourseID = 1050,
                Title = "Chemistry",
                Credits = 3,
                DepartmentID = 1
            };

            var results = ValidateModel(course);

            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData("Ab")]
        [InlineData("A")]
        public void Title_BelowMinimumLength_ShouldFailValidation(string title)
        {
            var course = new Course
            {
                CourseID = 1050,
                Title = title,
                Credits = 3,
                DepartmentID = 1
            };

            var results = ValidateModel(course);

            results.Should().ContainSingle()
                .Which.ErrorMessage.Should().Contain("3");
        }

        [Fact]
        public void Title_ExceedsMaximumLength_ShouldFailValidation()
        {
            var course = new Course
            {
                CourseID = 1050,
                Title = new string('A', 51),
                Credits = 3,
                DepartmentID = 1
            };

            var results = ValidateModel(course);

            results.Should().ContainSingle()
                .Which.ErrorMessage.Should().Contain("50");
        }

        [Theory]
        [InlineData(3)]
        [InlineData(10)]
        [InlineData(50)]
        public void Title_WithinValidLength_ShouldPassValidation(int length)
        {
            var course = new Course
            {
                CourseID = 1050,
                Title = new string('A', length),
                Credits = 3,
                DepartmentID = 1
            };

            var results = ValidateModel(course);

            results.Should().BeEmpty();
        }

        [Fact]
        public void Title_WhenNull_ShouldPassValidation()
        {
            var course = new Course
            {
                CourseID = 1050,
                Title = null,
                Credits = 3,
                DepartmentID = 1
            };

            var results = ValidateModel(course);

            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(6)]
        [InlineData(10)]
        public void Credits_OutsideValidRange_ShouldFailValidation(int credits)
        {
            var course = new Course
            {
                CourseID = 1050,
                Title = "Chemistry",
                Credits = credits,
                DepartmentID = 1
            };

            var results = ValidateModel(course);

            results.Should().ContainSingle()
                .Which.ErrorMessage.Should().Contain("0").And.Contain("5");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(3)]
        [InlineData(5)]
        public void Credits_WithinValidRange_ShouldPassValidation(int credits)
        {
            var course = new Course
            {
                CourseID = 1050,
                Title = "Chemistry",
                Credits = credits,
                DepartmentID = 1
            };

            var results = ValidateModel(course);

            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData("Physics")]
        [InlineData("Introduction to Computer Science")]
        [InlineData("Art")]
        public void Title_WithVariousValidTitles_ShouldPassValidation(string title)
        {
            var course = new Course
            {
                CourseID = 1050,
                Title = title,
                Credits = 3,
                DepartmentID = 1
            };

            var results = ValidateModel(course);

            results.Should().BeEmpty();
        }

        [Fact]
        public void TeachingMaterialImagePath_ExceedsMaximumLength_ShouldFailValidation()
        {
            var course = new Course
            {
                CourseID = 1050,
                Title = "Chemistry",
                Credits = 3,
                DepartmentID = 1,
                TeachingMaterialImagePath = new string('A', 256)
            };

            var results = ValidateModel(course);

            results.Should().ContainSingle()
                .Which.ErrorMessage.Should().Contain("255");
        }

        [Theory]
        [InlineData(255)]
        [InlineData(100)]
        [InlineData(1)]
        public void TeachingMaterialImagePath_WithinMaxLength_ShouldPassValidation(int length)
        {
            var course = new Course
            {
                CourseID = 1050,
                Title = "Chemistry",
                Credits = 3,
                DepartmentID = 1,
                TeachingMaterialImagePath = new string('A', length)
            };

            var results = ValidateModel(course);

            results.Should().BeEmpty();
        }

        private static IList<ValidationResult> ValidateModel(object model)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(model, null, null);
            Validator.TryValidateObject(model, validationContext, validationResults, true);
            return validationResults;
        }
    }
}
