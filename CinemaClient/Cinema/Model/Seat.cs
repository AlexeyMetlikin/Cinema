using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Cinema.Model
{
    public class Seat : INotifyPropertyChanged
    {
        private bool _isSelected;

        private bool _isOccupied;

        public bool IsOccupied
        {
            get { return _isOccupied; }
            set
            {
                _isOccupied = value;
                OnPropertyChanged("IsOccupied");
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }

        public int SeatNum { get; }

        public Seat(int seatNum)
        {
            SeatNum = seatNum;
        }


        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
