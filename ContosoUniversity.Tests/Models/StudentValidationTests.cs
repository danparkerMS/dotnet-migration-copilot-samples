using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ContosoUniversity.Models;
using FluentAssertions;
using Xunit;

namespace ContosoUniversity.Tests.Models
{
    public class StudentValidationTests
    {
        [Fact]
        public void ValidStudent_ShouldPassValidation()
        {
            var student = new Student
            {
                LastName = "Doe",
                FirstMidName = "John",
                EnrollmentDate = new DateTime(2020, 9, 1)
            };

            var results = ValidateModel(student);

            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void LastName_WhenNullOrEmpty_ShouldFailValidation(string lastName)
        {
            var student = new Student
            {
                LastName = lastName,
                FirstMidName = "John",
                EnrollmentDate = new DateTime(2020, 9, 1)
            };

            var results = ValidateModel(student);

            results.Should().ContainSingle()
                .Which.ErrorMessage.Should().Contain("Last Name");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void FirstMidName_WhenNullOrEmpty_ShouldFailValidation(string firstName)
        {
            var student = new Student
            {
                LastName = "Doe",
                FirstMidName = firstName,
                EnrollmentDate = new DateTime(2020, 9, 1)
            };

            var results = ValidateModel(student);

            results.Should().ContainSingle()
                .Which.ErrorMessage.Should().Contain("First Name");
        }

        [Fact]
        public void LastName_WhenExceedsMaxLength_ShouldFailValidation()
        {
            var student = new Student
            {
                LastName = new string('A', 51),
                FirstMidName = "John",
                EnrollmentDate = new DateTime(2020, 9, 1)
            };

            var results = ValidateModel(student);

            results.Should().ContainSingle()
                .Which.ErrorMessage.Should().Contain("50");
        }

        [Fact]
        public void FirstMidName_WhenExceedsMaxLength_ShouldFailValidation()
        {
            var student = new Student
            {
                LastName = "Doe",
                FirstMidName = new string('A', 51),
                EnrollmentDate = new DateTime(2020, 9, 1)
            };

            var results = ValidateModel(student);

            results.Should().ContainSingle()
                .Which.ErrorMessage.Should().Contain("50");
        }

        [Theory]
        [InlineData(50)]
        [InlineData(25)]
        [InlineData(1)]
        public void LastName_WithinMaxLength_ShouldPassValidation(int length)
        {
            var student = new Student
            {
                LastName = new string('A', length),
                FirstMidName = "John",
                EnrollmentDate = new DateTime(2020, 9, 1)
            };

            var results = ValidateModel(student);

            results.Should().BeEmpty();
        }

        [Theory]
        [InlineData(50)]
        [InlineData(25)]
        [InlineData(1)]
        public void FirstMidName_WithinMaxLength_ShouldPassValidation(int length)
        {
            var student = new Student
            {
                LastName = "Doe",
                FirstMidName = new string('A', length),
                EnrollmentDate = new DateTime(2020, 9, 1)
            };

            var results = ValidateModel(student);

            results.Should().BeEmpty();
        }

        [Fact]
        public void EnrollmentDate_WhenDefault_ShouldFailValidation()
        {
            var student = new Student
            {
                LastName = "Doe",
                FirstMidName = "John",
                EnrollmentDate = default(DateTime)
            };

            var results = ValidateModel(student);

            results.Should().ContainSingle()
                .Which.ErrorMessage.Should().Contain("1753");
        }

        [Theory]
        [InlineData(1752, 12, 31)]
        [InlineData(1753, 1, 0)]
        public void EnrollmentDate_BelowMinimumYear_ShouldFailValidation(int year, int month, int day)
        {
            if (day == 0)
            {
                var student = new Student
                {
                    LastName = "Doe",
                    FirstMidName = "John",
                    EnrollmentDate = new DateTime(1752, 12, 31)
                };

                var results = ValidateModel(student);

                results.Should().Contain(r => r.ErrorMessage.Contains("1753"));
                return;
            }

            var validStudent = new Student
            {
                LastName = "Doe",
                FirstMidName = "John",
                EnrollmentDate = new DateTime(year, month, day)
            };

            var validResults = ValidateModel(validStudent);

            validResults.Should().Contain(r => r.ErrorMessage.Contains("1753"));
        }

        [Theory]
        [InlineData(1753, 1, 1)]
        [InlineData(2020, 9, 1)]
        [InlineData(9999, 12, 31)]
        public void EnrollmentDate_WithinValidRange_ShouldPassValidation(int year, int month, int day)
        {
            var student = new Student
            {
                LastName = "Doe",
                FirstMidName = "John",
                EnrollmentDate = new DateTime(year, month, day)
            };

            var results = ValidateModel(student);

            results.Should().BeEmpty();
        }

        [Fact]
        public void EnrollmentDate_AtMaximumDate_ShouldPassValidation()
        {
            var student = new Student
            {
                LastName = "Doe",
                FirstMidName = "John",
                EnrollmentDate = new DateTime(9999, 12, 31)
            };

            var results = ValidateModel(student);

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
