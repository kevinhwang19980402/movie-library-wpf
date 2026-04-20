using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using MovieLibraryApp.Models;

namespace MovieLibraryApp.Models
{
    public class MovieLibraryManager
    {
        private LinkedList<Movie> _movies = new LinkedList<Movie>();
        private Dictionary<string, Movie> _movieById = new Dictionary<string, Movie>();
        private Dictionary<string, Queue<string>> _waitingLists = new Dictionary<string, Queue<string>>();

        public void AddMovie(Movie movie)
        {
            // Handle duplicate ID later (for testing edge cases)
            if (_movieById.ContainsKey(movie.MovieId))
            {
                MessageBox.Show("There is duplicate Movie ID please enter different Movie ID");
                return;
            }
            _movies.AddLast(movie);
            _movieById[movie.MovieId] = movie;
        }

        public IEnumerable<Movie> GetAllMovies()
        {
            return _movies;
        }
        
        //Movie search by title method
        public Movie SearchByTitle(string title)
        {
            foreach (var movie in _movies)
            {
                if (movie.Title.Equals(title, StringComparison.OrdinalIgnoreCase))
                    return movie;
            }
            return null;
        }

        public List<Movie> BubbleSortByTitle()
        {
            var list = _movies.ToList();

            for (int i = 0; i < list.Count - 1; i++)
            {
                for (int j = 0; j < list.Count - i - 1; j++)
                {
                    if (string.Compare(list[j].Title, list[j + 1].Title) > 0)
                    {
                        var temp = list[j];
                        list[j] = list[j + 1];
                        list[j + 1] = temp;
                    }
                }
            }

            return list;
        }

        public string BorrowMovie(string movieId, string user)
        {
            if (!_movieById.ContainsKey(movieId))
            {
                return "Movie not found.";
            }

            var movie = _movieById[movieId];

            if (movie.IsAvailable)
            {
                movie.IsAvailable = false;
                return $"{user} borrowed {movie.Title}.";
            }
            else
            {
                if (!_waitingLists.ContainsKey(movieId))
                {
                    _waitingLists[movieId] = new Queue<string>();
                }

                _waitingLists[movieId].Enqueue(user);
                return $"{movie.Title} is not available. {user} added to waiting list.";
            }
        }

        public string ReturnMovie(string movieId)
        {
            if (!_movieById.ContainsKey(movieId))
            {
                return "Movie not found.";
            }

            var movie = _movieById[movieId];

            if (_waitingLists.ContainsKey(movieId) && _waitingLists[movieId].Count > 0)
            {
                var nextUser = _waitingLists[movieId].Dequeue();
                return $"{movie.Title} assigned to {nextUser} from waiting list.";
            }

            movie.IsAvailable = true;
            return $"{movie.Title} returned and is now available.";
        }

        public List<Movie> SortById()
        {
            return _movies.OrderBy(m => m.MovieId).ToList();
        }

        public Movie BinarySearchById(string movieId)
        {
            var list = SortById(); // must be sorted first

            int left = 0;
            int right = list.Count - 1;

            while (left <= right)
            {
                int mid = (left + right) / 2;
                var midMovie = list[mid];

                int comparison = string.Compare(midMovie.MovieId, movieId, StringComparison.OrdinalIgnoreCase);

                if (comparison == 0)
                    return midMovie;

                if (comparison < 0)
                    left = mid + 1;
                else
                    right = mid - 1;
            }

            return null;
        }

        public List<Movie> MergeSortByYear()
        {
            var list = _movies.ToList();
            return MergeSort(list);
        }

        private List<Movie> MergeSort(List<Movie> list)
        {
            if (list.Count <= 1)
            {
                return list;
            }

            int mid = list.Count / 2;

            var left = MergeSort(list.GetRange(0, mid));
            var right = MergeSort(list.GetRange(mid, list.Count - mid));

            return Merge(left, right);
        }

        private List<Movie> Merge(List<Movie> left, List<Movie> right)
        {
            List<Movie> result = new List<Movie>();

            int i = 0, j = 0;

            while (i < left.Count && j < right.Count)
            {
                if (left[i].ReleaseYear <= right[j].ReleaseYear)
                {
                    result.Add(left[i]);
                    i++;
                }
                else
                {
                    result.Add(right[j]);
                    j++;
                }
            }

            // Add remaining items
            result.AddRange(left.GetRange(i, left.Count - i));
            result.AddRange(right.GetRange(j, right.Count - j));

            return result;
        }

        public void SaveToJson(string filePath)
        {
            if (_movies.Count == 0)
            {
                MessageBox.Show("There is no movies saved in the file, please add movies");
                return;
            }
            var list = _movies.ToList(); // convert LinkedList to List
            var json = JsonSerializer.Serialize(list, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(filePath, json);
        }

        public void LoadFromJson(string filePath)
        {
            if (!File.Exists(filePath))
            {
                MessageBox.Show("Movie file does not exist");
                return;
            }
            if(_movies.Count == 0)
            {
                MessageBox.Show("There are currently no movies in the list to load please add some movies");
                return;
            }

            string json = File.ReadAllText(filePath);
            var list = JsonSerializer.Deserialize<List<Movie>>(json);

            _movies.Clear();

            foreach (var movie in list)
            {
                _movies.AddLast(movie);
            }
        }

    }
}
