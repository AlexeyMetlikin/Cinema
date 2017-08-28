using Cinema.Infrastructure.Abstract;
using Cinema.Model;
using Cinema.Serialization;
using Cinema.ViewModel.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace Cinema.ViewModel
{
    public class CinemaHallViewModel : INotifyPropertyChanged
    {
        private static int colRows = 10;    // Количество рядов в зале

        private static int colSeats = 10;   // Количество мест в ряду

        private ICinemaApi _API;

        private DispatcherTimer _timer;

        private List<Movie> _movies;

        private CinemaHall _cinemaHall;

        private Movie _selectedMovie;

        private int _selectedSeats;

        private RelayCommand movieChangeCommand;

        private RelayCommand toBookSeatsCommand;

        private RelayCommand seatSelectCommand;

        public int SelectedSeats
        {
            get { return _selectedSeats; }
            set
            {
                _selectedSeats = value;
                OnPropertyChanged("SelectedSeats");
            }
        }

        public CinemaHall Hall
        {
            get { return _cinemaHall; }
            set
            {
                _cinemaHall = value;
                OnPropertyChanged("Hall");
            }
        }

        public List<Movie> Movies
        {
            get { return _movies; }
            set
            {
                _movies = value;
                OnPropertyChanged("Movies");
            }
        }

        public Movie SelectedMovie
        {
            get { return _selectedMovie; }
            set
            {
                if (_selectedMovie != value)
                {
                    _selectedMovie = value;
                    OnPropertyChanged("SelectedMovie");
                }
            }
        }

        public RelayCommand MovieChangeCommand
        {
            get
            {
                return movieChangeCommand ??
                    (movieChangeCommand = new RelayCommand(c =>
                    {
                        GetOccupiedSeats();
                    }, c => true));
            }
        }

        public RelayCommand ToBookSeatsCommand
        {
            get
            {
                return toBookSeatsCommand ??
                    (toBookSeatsCommand = new RelayCommand(c =>
                    {
                        ToBookSeats();
                    }, c => true));
            }
        }

        public RelayCommand SeatSelectCommand
        {
            get
            {
                return seatSelectCommand ??
                    (seatSelectCommand = new RelayCommand(c =>
                    {
                        SelectSeat(c);
                    }, c => true));
            }
        }

        public CinemaHallViewModel(ICinemaApi API)
        {
            _API = API;

            _timer = new DispatcherTimer(DispatcherPriority.Normal);
            _timer.Interval = new TimeSpan(0, 5, 0);
            _timer.Tick += _timer_Tick;
            _timer.Start();

            Hall = CinemaHall.InitCinemaHall(colRows, colSeats);
            SelectedSeats = 0;
            GetMovies();
        }

        private void _timer_Tick(object sender, EventArgs e)
        {
            var currentMovie = SelectedMovie;
            var currentHall = Hall;

            GetMovies();

            SelectedMovie = currentMovie;
            Hall = currentHall;
        }

        private async void GetMovies()
        {
            string request = "api/getMovies";

            string result = await Send("GET", request, null);

            if (result != null)
            {
                Movies = JsonConvert.DeserializeObject<List<Movie>>(result);

                if (SelectedMovie == null)
                {
                    SelectedMovie = Movies[0];
                }
            }
        }

        private void GetOccupiedSeats()
        {
            if (SelectedMovie != null)
            {
                string request = "api/getMovieBookedSeats";

                var content = JsonConvert.SerializeObject(new { MovieId = SelectedMovie.MovieId });
                string jsonBookings = Send("POST", request, content).Result;

                var bookings = JsonConvert.DeserializeObject<List<Booking>>(jsonBookings);

                Hall = CinemaHall.InitCinemaHall(colRows, colSeats);
                Hall.SetOccupiedSeats(bookings);
            }
        }

        private void ToBookSeats()
        {
            var bookings = GetSelectedSeats();

            string request = "api/toBookSeats";

            var content = JsonConvert.SerializeObject(bookings);
            string result = Send("POST", request, content).Result;

            if (Regex.IsMatch(result, "Seats books"))
            {
                SelectedSeats = 0;
                Hall.SetOccupiedSeats(bookings);
                MessageBox.Show("Места успешно забронированы");
            }
        }

        private List<Booking> GetSelectedSeats()
        {
            var bookings = new List<Booking>();

            foreach (var row in Hall.Rows)
            {
                foreach (var seat in row.Seats)
                {
                    if (seat.IsSelected)
                    {
                        bookings.Add(new Booking { SeatNum = seat.SeatNum, SeatRow = row.RowNum, MovieId = SelectedMovie.MovieId });
                    }
                }
            }

            return bookings;
        }

        private void SelectSeat(object sender)
        {
            var currentSeat = sender as Seat;
            if (currentSeat != null)
            {
                if (!currentSeat.IsOccupied)
                {
                    currentSeat.IsSelected = !currentSeat.IsSelected;
                    SelectedSeats = currentSeat.IsSelected ? SelectedSeats + 1 : SelectedSeats - 1;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }

        private async Task<string> Send(string requestType, string request, string content)
        {
            try
            { 
                return await _API.SendRequest(requestType, request, content);
            }
            catch (FormatException exp)
            {
                MessageBox.Show(exp.Message);
            }
            catch (HttpRequestException exp)
            {
                MessageBox.Show("Ошибка в результате запроса: " + exp.Message);
            }
            catch (Exception exp)
            {
                MessageBox.Show(exp.Message);
            }
            return null;
        }
    }
}
