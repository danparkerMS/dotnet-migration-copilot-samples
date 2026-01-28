using ContosoUniversity;
using FluentAssertions;
using Xunit;

namespace ContosoUniversity.Tests.Data
{
    public class PaginatedListTests
    {
        [Fact]
        public void Create_WithValidData_ReturnsPaginatedList()
        {
            // Arrange
            var items = Enumerable.Range(1, 100).ToList();
            var pageIndex = 1; // 1-indexed
            var pageSize = 10;

            // Act
            var result = PaginatedList<int>.Create(
                items.AsQueryable(),
                pageIndex,
                pageSize
            );

            // Assert
            result.Should().HaveCount(pageSize);
            result.PageIndex.Should().Be(pageIndex);
            result.TotalPages.Should().Be(10);
            result.HasPreviousPage.Should().BeFalse();
            result.HasNextPage.Should().BeTrue();
        }

        [Fact]
        public void Create_LastPage_HasNoPreviousPage()
        {
            // Arrange
            var items = Enumerable.Range(1, 25).ToList();
            var pageIndex = 3; // Third page (1-indexed)
            var pageSize = 10;

            // Act
            var result = PaginatedList<int>.Create(
                items.AsQueryable(),
                pageIndex,
                pageSize
            );

            // Assert
            result.Should().HaveCount(5); // Only 5 items on last page
            result.HasNextPage.Should().BeFalse();
            result.HasPreviousPage.Should().BeTrue();
        }

        [Fact]
        public void Create_FirstPage_HasNoNextPage_WhenOnlyOnePage()
        {
            // Arrange
            var items = Enumerable.Range(1, 5).ToList();
            var pageIndex = 1;
            var pageSize = 10;

            // Act
            var result = PaginatedList<int>.Create(
                items.AsQueryable(),
                pageIndex,
                pageSize
            );

            // Assert
            result.Should().HaveCount(5);
            result.HasNextPage.Should().BeFalse();
            result.HasPreviousPage.Should().BeFalse();
            result.TotalPages.Should().Be(1);
        }

        [Fact]
        public void Create_EmptySource_ReturnsEmptyPaginatedList()
        {
            // Arrange
            var items = new List<int>();

            // Act
            var result = PaginatedList<int>.Create(
                items.AsQueryable(),
                1,
                10
            );

            // Assert
            result.Should().BeEmpty();
            result.TotalPages.Should().Be(0);
            result.HasNextPage.Should().BeFalse();
            result.HasPreviousPage.Should().BeFalse();
        }

        [Fact]
        public void Create_SecondPage_HasBothPreviousAndNextPage()
        {
            // Arrange
            var items = Enumerable.Range(1, 30).ToList();
            var pageIndex = 2;
            var pageSize = 10;

            // Act
            var result = PaginatedList<int>.Create(
                items.AsQueryable(),
                pageIndex,
                pageSize
            );

            // Assert
            result.Should().HaveCount(10);
            result.HasPreviousPage.Should().BeTrue();
            result.HasNextPage.Should().BeTrue();
        }
    }
}
