using MovieLibraryApp.Models;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MovieLibraryApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private MovieLibraryManager _library = new MovieLibraryManager();

        public MainWindow()
        {
            InitializeComponent();
            RefreshMovieList();
        }
        private void AddMovie_Click(object sender, RoutedEventArgs e)
        {
            if ((MovieIdBox.Text == null) || (TitleBox.Text == null) || 
            (DirectorBox.Text == null) || (GenreBox.Text == null) || (YearBox.Text == null))
            {
                MessageBox.Show("One of the field is empty please fill in all the boxes");
                return;
            }
            if (!int.TryParse(YearBox.Text, out int year))
            {
                MessageBox.Show("Please enter a valid year.");
                return;
            }

            var movie = new Movie
            {
                MovieId = MovieIdBox.Text,
                Title = TitleBox.Text,
                Director = DirectorBox.Text,
                Genre = GenreBox.Text,
                ReleaseYear = int.Parse(YearBox.Text),
                IsAvailable = true
            };

            _library.AddMovie(movie);
            RefreshMovieList();
        }
        private void RefreshMovieList()
        {
            MoviesListView.ItemsSource = null;
            MoviesListView.ItemsSource = _library.GetAllMovies().ToList();
        }

        private void SearchTitle_Click(object sender, RoutedEventArgs e)
        {
            if(TitleBox.Text == null)
            {
                MessageBox.Show("Please fill in the title box, cannot be empty");
                return;
            }
            var result = _library.SearchByTitle(TitleBox.Text);

            if (result == null)
            {
                MessageBox.Show("No movie found with that title.");
            }
            else
            {
                MessageBox.Show($"Found: {result.Title} ({result.ReleaseYear})");
            }
        }

        private void Borrow_Click(object sender, RoutedEventArgs e)
        {
            if((BorrowReturnMovieIdBox.Text == null) || (UserNameBox.Text == null))
            {
                MessageBox.Show("One of the field is empty please fill in both Movie ID and your username");
                return;
            }
            string movieId = BorrowReturnMovieIdBox.Text;
            string user = UserNameBox.Text;

            var message = _library.BorrowMovie(movieId, user);
            MessageBox.Show(message);
            RefreshMovieList();

        }

        private void Return_Click(object sender, RoutedEventArgs e)
        {
            if((BorrowReturnMovieIdBox.Text == null))
            {
                MessageBox.Show("The Movie ID text box is empty. Please fill in the Movie ID");
                return;
            }
            string movieId = BorrowReturnMovieIdBox.Text;
            var message = _library.ReturnMovie(movieId);
            MessageBox.Show(message);
            RefreshMovieList();

        }

        private void SortByTitle_Click(object sender, RoutedEventArgs e)
        {
            if (!_library.GetAllMovies().Any())
            {
                MessageBox.Show("There are no movies stored to sort, please add some movies");
                return;
            }
            MoviesListView.ItemsSource = null;
            MoviesListView.ItemsSource = _library.BubbleSortByTitle();
        }

        private void SearchId_Click(object sender, RoutedEventArgs e)
        {
            if(SearchIdBox.Text == null)
            {
                MessageBox.Show("Pleae fill in the Movie ID in the search box");
                return;
            }
            var result = _library.BinarySearchById(SearchIdBox.Text);

            if (result == null)
            {
                MessageBox.Show("No movie found with that ID.");
            }
            else
            {
                MoviesListView.ItemsSource = null;
                MoviesListView.ItemsSource = new List<Movie>() { result };
                MessageBox.Show($"Found: {result.Title} ({result.ReleaseYear})");
            }

        }

        private void SortByYear_Click(object sender, RoutedEventArgs e)
        {
            if (!_library.GetAllMovies().Any())
            {
                MessageBox.Show("There are no movies in the list to sort by year please add some movies");
                return;
            }
            MoviesListView.ItemsSource = null;
            MoviesListView.ItemsSource = _library.MergeSortByYear();

        }

        private void SaveJson_Click(object sender, RoutedEventArgs e)
        {
            _library.SaveToJson("movies.json");
            MessageBox.Show("Movies saved to movies.json");
        }

        private void LoadJson_Click(object sender, RoutedEventArgs e)
        {
            _library.LoadFromJson("movies.json");
            RefreshMovieList();
            MoviesListView.ItemsSource = null;
            MoviesListView.ItemsSource = _library.GetAllMovies().ToList();
            MessageBox.Show("Movies loaded from movies.json");
        }
    }
}