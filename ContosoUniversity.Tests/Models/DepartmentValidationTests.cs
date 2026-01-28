using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ContosoUniversity.Models;
using FluentAssertions;
using Xunit;

namespace ContosoUniversity.Tests.Models
{
    public class DepartmentValidationTests
    {
        [Fact]
        public void ValidDepartment_ShouldPassValidation()
        {
            var department = new Department
            {
                DepartmentID = 1,
                Name = "Engineering",
                Budget = 350000m,
                StartDate = new DateTime(2007, 9, 1)
            };

            var results = ValidateModel(department);

            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData("A")]
        [InlineData("Ab")]
        public void Name_BelowMinimumLength_ShouldFailValidation(string name)
        {
            var department = new Department
            {
                DepartmentID = 1,
                Name = name,
                Budget = 350000m,
                StartDate = new DateTime(2007, 9, 1)
            };

            var results = ValidateModel(department);

            results.Should().ContainSingle()
                .Which.ErrorMessage.Should().Contain("3");
        }

        [Fact]
        public void Name_ExceedsMaximumLength_ShouldFailValidation()
        {
            var department = new Department
            {
                DepartmentID = 1,
                Name = new string('A', 51),
                Budget = 350000m,
                StartDate = new DateTime(2007, 9, 1)
            };

            var results = ValidateModel(department);

            results.Should().ContainSingle()
                .Which.ErrorMessage.Should().Contain("50");
        }

        [Theory]
        [InlineData(3)]
        [InlineData(10)]
        [InlineData(25)]
        [InlineData(50)]
        public void Name_WithinValidLength_ShouldPassValidation(int length)
        {
            var department = new Department
            {
                DepartmentID = 1,
                Name = new string('A', length),
                Budget = 350000m,
                StartDate = new DateTime(2007, 9, 1)
            };

            var results = ValidateModel(department);

            results.Should().BeEmpty();
        }

        [Fact]
        public void Name_WhenNull_ShouldPassValidation()
        {
            var department = new Department
            {
                DepartmentID = 1,
                Name = null,
                Budget = 350000m,
                StartDate = new DateTime(2007, 9, 1)
            };

            var results = ValidateModel(department);

            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData("Engineering")]
        [InlineData("Mathematics")]
        [InlineData("Art")]
        public void Name_WithVariousValidNames_ShouldPassValidation(string name)
        {
            var department = new Department
            {
                DepartmentID = 1,
                Name = name,
                Budget = 350000m,
                StartDate = new DateTime(2007, 9, 1)
            };

            var results = ValidateModel(department);

            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(100.50)]
        [InlineData(1000000)]
        public void Budget_WithVariousValues_ShouldPassValidation(double budgetValue)
        {
            var department = new Department
            {
                DepartmentID = 1,
                Name = "Engineering",
                Budget = (decimal)budgetValue,
                StartDate = new DateTime(2007, 9, 1)
            };

            var results = ValidateModel(department);

            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(2007, 9, 1)]
        [InlineData(2020, 1, 1)]
        [InlineData(1900, 1, 1)]
        public void StartDate_WithVariousValidDates_ShouldPassValidation(int year, int month, int day)
        {
            var department = new Department
            {
                DepartmentID = 1,
                Name = "Engineering",
                Budget = 350000m,
                StartDate = new DateTime(year, month, day)
            };

            var results = ValidateModel(department);

            results.Should().BeEmpty();
        }

        [Fact]
        public void InstructorID_WhenNull_ShouldPassValidation()
        {
            var department = new Department
            {
                DepartmentID = 1,
                Name = "Engineering",
                Budget = 350000m,
                StartDate = new DateTime(2007, 9, 1),
                InstructorID = null
            };

            var results = ValidateModel(department);

            results.Should().BeEmpty();
        }

        [Fact]
        public void InstructorID_WhenSet_ShouldPassValidation()
        {
            var department = new Department
            {
                DepartmentID = 1,
                Name = "Engineering",
                Budget = 350000m,
                StartDate = new DateTime(2007, 9, 1),
                InstructorID = 1
            };

            var results = ValidateModel(department);

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
