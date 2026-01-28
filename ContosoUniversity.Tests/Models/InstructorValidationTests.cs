using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ContosoUniversity.Models;
using FluentAssertions;
using Xunit;

namespace ContosoUniversity.Tests.Models
{
    public class InstructorValidationTests
    {
        [Fact]
        public void ValidInstructor_ShouldPassValidation()
        {
            var instructor = new Instructor
            {
                LastName = "Smith",
                FirstMidName = "Jane",
                HireDate = new DateTime(2005, 3, 15)
            };

            var results = ValidateModel(instructor);

            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void LastName_WhenNullOrEmpty_ShouldFailValidation(string lastName)
        {
            var instructor = new Instructor
            {
                LastName = lastName,
                FirstMidName = "Jane",
                HireDate = new DateTime(2005, 3, 15)
            };

            var results = ValidateModel(instructor);

            results.Should().ContainSingle()
                .Which.ErrorMessage.Should().Contain("Last Name");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void FirstMidName_WhenNullOrEmpty_ShouldFailValidation(string firstName)
        {
            var instructor = new Instructor
            {
                LastName = "Smith",
                FirstMidName = firstName,
                HireDate = new DateTime(2005, 3, 15)
            };

            var results = ValidateModel(instructor);

            results.Should().ContainSingle()
                .Which.ErrorMessage.Should().Contain("First Name");
        }

        [Fact]
        public void LastName_WhenExceedsMaxLength_ShouldFailValidation()
        {
            var instructor = new Instructor
            {
                LastName = new string('A', 51),
                FirstMidName = "Jane",
                HireDate = new DateTime(2005, 3, 15)
            };

            var results = ValidateModel(instructor);

            results.Should().ContainSingle()
                .Which.ErrorMessage.Should().Contain("50");
        }

        [Fact]
        public void FirstMidName_WhenExceedsMaxLength_ShouldFailValidation()
        {
            var instructor = new Instructor
            {
                LastName = "Smith",
                FirstMidName = new string('A', 51),
                HireDate = new DateTime(2005, 3, 15)
            };

            var results = ValidateModel(instructor);

            results.Should().ContainSingle()
                .Which.ErrorMessage.Should().Contain("50");
        }

        [Theory]
        [InlineData(50)]
        [InlineData(25)]
        [InlineData(1)]
        public void LastName_WithinMaxLength_ShouldPassValidation(int length)
        {
            var instructor = new Instructor
            {
                LastName = new string('A', length),
                FirstMidName = "Jane",
                HireDate = new DateTime(2005, 3, 15)
            };

            var results = ValidateModel(instructor);

            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(50)]
        [InlineData(25)]
        [InlineData(1)]
        public void FirstMidName_WithinMaxLength_ShouldPassValidation(int length)
        {
            var instructor = new Instructor
            {
                LastName = "Smith",
                FirstMidName = new string('A', length),
                HireDate = new DateTime(2005, 3, 15)
            };

            var results = ValidateModel(instructor);

            results.Should().BeEmpty();
        }

        [Fact]
        public void HireDate_WhenDefault_ShouldFailValidation()
        {
            var instructor = new Instructor
            {
                LastName = "Smith",
                FirstMidName = "Jane",
                HireDate = default(DateTime)
            };

            var results = ValidateModel(instructor);

            results.Should().ContainSingle()
                .Which.ErrorMessage.Should().Contain("1753");
        }

        [Theory]
        [InlineData(1752, 12, 31)]
        public void HireDate_BelowMinimumYear_ShouldFailValidation(int year, int month, int day)
        {
            var instructor = new Instructor
            {
                LastName = "Smith",
                FirstMidName = "Jane",
                HireDate = new DateTime(year, month, day)
            };

            var results = ValidateModel(instructor);

            results.Should().Contain(r => r.ErrorMessage.Contains("1753"));
        }

        [Theory]
        [InlineData(1753, 1, 1)]
        [InlineData(2005, 3, 15)]
        [InlineData(2020, 1, 1)]
        [InlineData(9999, 12, 31)]
        public void HireDate_WithinValidRange_ShouldPassValidation(int year, int month, int day)
        {
            var instructor = new Instructor
            {
                LastName = "Smith",
                FirstMidName = "Jane",
                HireDate = new DateTime(year, month, day)
            };

            var results = ValidateModel(instructor);

            results.Should().BeEmpty();
        }

        [Fact]
        public void HireDate_AtMaximumDate_ShouldPassValidation()
        {
            var instructor = new Instructor
            {
                LastName = "Smith",
                FirstMidName = "Jane",
                HireDate = new DateTime(9999, 12, 31)
            };

            var results = ValidateModel(instructor);

            results.Should().BeEmpty();
        }

        [Fact]
        public void FullName_ShouldBeConstructedCorrectly()
        {
            var instructor = new Instructor
            {
                LastName = "Smith",
                FirstMidName = "Jane",
                HireDate = new DateTime(2005, 3, 15)
            };

            instructor.FullName.Should().Be("Smith, Jane");
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
