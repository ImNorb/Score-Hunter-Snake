using System.ComponentModel;

namespace Score_Hunter_Snake {
    class Scoring : INotifyPropertyChanged {
        private int score;
        public int Score {
            get { return score; }
            set {
                score = value;
                OnPropertyChanged("Score");
            }
        }

        private int endScore;
        public int EndScore {
            get { return endScore; }
            set {
                endScore = value;
                OnPropertyChanged("EndScore");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
