using MovieLibraryApp.Models;
using Xunit;
namespace MovieLibrary.Tests
{
    public class MovieLibraryTest
    {
        [Fact]
        public void AddMovie_ShouldIncreaseCount()
        {
            // Arrange
            var lib = new MovieLibraryManager();
            var movie = new Movie
            {
                MovieId = "M001",
                Title = "Test",
                Director = "Dir",
                Genre = "Drama",
                ReleaseYear = 2000,
                IsAvailable = true
            };

            // Act
            lib.AddMovie(movie);
            var all = lib.GetAllMovies();

            // Assert
            Assert.Single(all); // exactly 1 movie
        }

        [Fact]
        public void AddMovie_DuplicateId_ShouldNotAddSecond()
        {
            var lib = new MovieLibraryManager();

            var m1 = new Movie { MovieId = "M001", Title = "First" };
            var m2 = new Movie { MovieId = "M001", Title = "Second" };

            lib.AddMovie(m1);
            lib.AddMovie(m2);

            var all = lib.GetAllMovies();
            var all1 = lib.GetAllMovies().ToList();

            Assert.Single(all1);
            Assert.Equal("First", all1[0].Title);
        }

        [Fact]
        public void SearchByTitle_ShouldReturnCorrectMovie()
        {
            var lib = new MovieLibraryManager();

            lib.AddMovie(new Movie { MovieId = "M001", Title = "Inception" });
            lib.AddMovie(new Movie { MovieId = "M002", Title = "Interstellar" });

            var result = lib.SearchByTitle("Interstellar");

            Assert.NotNull(result);
            Assert.Equal("M002", result.MovieId);
        }

        [Fact]
        public void BinarySearchById_ShouldFindMovie()
        {
            var lib = new MovieLibraryManager();

            lib.AddMovie(new Movie { MovieId = "M002", Title = "B" });
            lib.AddMovie(new Movie { MovieId = "M001", Title = "A" });
            lib.AddMovie(new Movie { MovieId = "M003", Title = "C" });

            var result = lib.BinarySearchById("M002");

            Assert.NotNull(result);
            Assert.Equal("M002", result.MovieId);
        }

        [Fact]
        public void BubbleSortByTitle_ShouldSortAlphabetically()
        {
            var lib = new MovieLibraryManager();

            lib.AddMovie(new Movie { MovieId = "1", Title = "C" });
            lib.AddMovie(new Movie { MovieId = "2", Title = "A" });
            lib.AddMovie(new Movie { MovieId = "3", Title = "B" });

            var sorted = lib.BubbleSortByTitle();

            Assert.Collection(sorted,
                m => Assert.Equal("A", m.Title),
                m => Assert.Equal("B", m.Title),
                m => Assert.Equal("C", m.Title));
        }

        [Fact]
        public void MergeSortByYear_ShouldSortByReleaseYear()
        {
            var lib = new MovieLibraryManager();

            lib.AddMovie(new Movie { MovieId = "1", Title = "Old", ReleaseYear = 1990 });
            lib.AddMovie(new Movie { MovieId = "2", Title = "New", ReleaseYear = 2020 });
            lib.AddMovie(new Movie { MovieId = "3", Title = "Middle", ReleaseYear = 2005 });

            var sorted = lib.MergeSortByYear();

            Assert.Collection(sorted,
                m => Assert.Equal(1990, m.ReleaseYear),
                m => Assert.Equal(2005, m.ReleaseYear),
                m => Assert.Equal(2020, m.ReleaseYear));
        }

        [Fact]
        public void BorrowMovie_Available_ShouldMarkUnavailable()
        {
            var lib = new MovieLibraryManager();
            lib.AddMovie(new Movie { MovieId = "M001", Title = "Test", IsAvailable = true });

            var message = lib.BorrowMovie("M001", "Kevin");
            var movie = lib.BinarySearchById("M001");

            Assert.False(movie.IsAvailable);
        }

        [Fact]
        public void BorrowMovie_Unavailable_ShouldAddToWaitingQueue()
        {
            var lib = new MovieLibraryManager();
            lib.AddMovie(new Movie { MovieId = "M001", Title = "Test", IsAvailable = false });

            var message = lib.BorrowMovie("M001", "Kevin");

            // You might expose a method or property to inspect queue count
            // e.g., lib.GetWaitingCount("M001")
            // For now, just assert message contains something like "added to waiting list"
            Assert.Contains("waiting", message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ReturnMovie_WithWaitingUser_ShouldAssignToNextUser()
        {
            var lib = new MovieLibraryManager();
            lib.AddMovie(new Movie { MovieId = "M001", Title = "Test", IsAvailable = false });

            lib.BorrowMovie("M001", "Kevin"); // goes to waiting list

            var message = lib.ReturnMovie("M001");

            Assert.Contains("Kevin", message);
        }

        [Fact]
        public void SearchOnEmptyCollection_ShouldReturnNull()
        {
            var lib = new MovieLibraryManager();
            
            lib.AddMovie(new Movie { MovieId = "M001", Title = "Test", IsAvailable = false });

            var result = lib.SearchByTitle("Hi");

            Assert.Null(result);
        }

        [Fact]
        public void Borrow_InvalidId_ShouldReturnErrorMessage()
        {
            var lib = new MovieLibraryManager();

            lib.AddMovie(new Movie { MovieId = "M001", Title = "Test", IsAvailable = false });

            var message = lib.BorrowMovie("M002", "Kevin");

            Assert.Contains("not found", message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Return_InvalidId_ShouldReturnErrorMessage()
        {
            var lib = new MovieLibraryManager();
            lib.AddMovie(new Movie { MovieId = "M001", Title = "Test", IsAvailable = false });

            var message = lib.ReturnMovie("M004");

            Assert.Contains("not found", message, StringComparison.OrdinalIgnoreCase);
        }


    }
}
