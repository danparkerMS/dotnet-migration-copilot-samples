using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ContosoUniversity.Models;
using FluentAssertions;
using Xunit;

namespace ContosoUniversity.Tests.Models
{
    public class OfficeAssignmentValidationTests
    {
        [Fact]
        public void ValidOfficeAssignment_ShouldPassValidation()
        {
            var officeAssignment = new OfficeAssignment
            {
                InstructorID = 1,
                Location = "Smith Hall 101"
            };

            var results = ValidateModel(officeAssignment);

            results.Should().BeEmpty();
        }

        [Fact]
        public void Location_ExceedsMaximumLength_ShouldFailValidation()
        {
            var officeAssignment = new OfficeAssignment
            {
                InstructorID = 1,
                Location = new string('A', 51)
            };

            var results = ValidateModel(officeAssignment);

            results.Should().ContainSingle()
                .Which.ErrorMessage.Should().Contain("50");
        }

        [Theory]
        [InlineData(50)]
        [InlineData(25)]
        [InlineData(10)]
        [InlineData(1)]
        public void Location_WithinMaxLength_ShouldPassValidation(int length)
        {
            var officeAssignment = new OfficeAssignment
            {
                InstructorID = 1,
                Location = new string('A', length)
            };

            var results = ValidateModel(officeAssignment);

            results.Should().BeEmpty();
        }

        [Fact]
        public void Location_WhenNull_ShouldPassValidation()
        {
            var officeAssignment = new OfficeAssignment
            {
                InstructorID = 1,
                Location = null
            };

            var results = ValidateModel(officeAssignment);

            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void Location_WhenEmpty_ShouldPassValidation(string location)
        {
            var officeAssignment = new OfficeAssignment
            {
                InstructorID = 1,
                Location = location
            };

            var results = ValidateModel(officeAssignment);

            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData("Smith Hall 101")]
        [InlineData("Thompson Hall 201")]
        [InlineData("Building A, Room 305")]
        [InlineData("Office 42")]
        public void Location_WithVariousValidLocations_ShouldPassValidation(string location)
        {
            var officeAssignment = new OfficeAssignment
            {
                InstructorID = 1,
                Location = location
            };

            var results = ValidateModel(officeAssignment);

            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(100)]
        [InlineData(999)]
        public void InstructorID_WithVariousValidValues_ShouldPassValidation(int instructorId)
        {
            var officeAssignment = new OfficeAssignment
            {
                InstructorID = instructorId,
                Location = "Smith Hall 101"
            };

            var results = ValidateModel(officeAssignment);

            results.Should().BeEmpty();
        }

        [Fact]
        public void OfficeAssignment_WithMinimalData_ShouldPassValidation()
        {
            var officeAssignment = new OfficeAssignment
            {
                InstructorID = 1
            };

            var results = ValidateModel(officeAssignment);

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
