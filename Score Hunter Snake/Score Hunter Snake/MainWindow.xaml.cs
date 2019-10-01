using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Score_Hunter_Snake {

    public partial class MainWindow : Window {
        List<Point> snakeTrail = new List<Point>();
        List<Point> apples = new List<Point>();
        Point startingPoint = new Point(280, 180);
        Point currentPosition = new Point();
        Brush snakeColor = Brushes.Green;
        const int snakeSize = 10;
        const int stepSize = 1;
        DIRECTION direction;
        DIRECTION prevDirection;
        int length;
        const int maxLength = 500;
        int maxApples = 10;
        Random rnd = new Random();
        Scoring scoring = new Scoring();
        DispatcherTimer timer;
        bool gameInProgress = false;
        string gameType;
        string playerName = "player one";

        RestAPI DB = new RestAPI();

        enum BORDER {
            XMAX = 574,
            YMAX = 350
        }
        enum DIRECTION {
            STOPPED,
            DOWN,
            LEFT,
            RIGHT,
            UP
        };

        private Brush ColorPicker(int id = -1) {

            if (id == -1) id = rnd.Next(10);

            switch (id) {
                case 0:
                if (snakeColor != Brushes.Green) return Brushes.Green;
                else return ColorPicker(id + 1);

                case 1:
                if (snakeColor != Brushes.Blue) return Brushes.Blue;
                else return ColorPicker(id + 1);

                case 2:
                if (snakeColor != Brushes.Yellow) return Brushes.Yellow;
                else return ColorPicker(id + 1);

                case 3:
                if (snakeColor != Brushes.Purple) return Brushes.Purple;
                else return ColorPicker(id + 1);

                case 4:
                if (snakeColor != Brushes.Pink) return Brushes.Pink;
                else return ColorPicker(id + 1);

                case 5:
                if (snakeColor != Brushes.Cyan) return Brushes.Cyan;
                else return ColorPicker(id + 1);

                case 6:
                if (snakeColor != Brushes.Salmon) return Brushes.Salmon;
                else return ColorPicker(id + 1);

                case 7:
                if (snakeColor != Brushes.GhostWhite) return Brushes.GhostWhite;
                else return ColorPicker(id + 1);

                case 8:
                if (snakeColor != Brushes.Gold) return Brushes.Gold;
                else return ColorPicker(id + 1);

                case 9:
                if (snakeColor != Brushes.Orange) return Brushes.Orange;
                else return ColorPicker(id + 1);

                default: return Brushes.Green;
            }
        }

        public MainWindow() {
            InitializeComponent();

            HideGame();
            HideGameOver();
            HideHighScore();
            HideMenu();
            HideWelcome(false);

            DataContext = scoring;
            InitializeGame();
        }

        #region Game Mechanics
        private void PaintSnake(Point positon) {
            Ellipse newSnake = new Ellipse();
            newSnake.Fill = snakeColor;
            newSnake.Width = snakeSize;
            newSnake.Height = snakeSize;
            Canvas.SetTop(newSnake, positon.Y);
            Canvas.SetLeft(newSnake, positon.X);
            int count = gameCanvas.Children.Count;
            gameCanvas.Children.Add(newSnake);
            snakeTrail.Add(positon);
            if (count > length) {
                gameCanvas.Children.RemoveAt(count - length + apples.Count - 1);
                snakeTrail.RemoveAt(0);
            }
        }

        private void PaintApple(int index) {
            Point apple = new Point(rnd.Next(5, (int)BORDER.XMAX), rnd.Next(5, (int)BORDER.YMAX));
            bool exit = true;
            while (exit && snakeTrail.Count > 0) {
                for (int i = 0; i < snakeTrail.Count; i++) {
                    if (Math.Abs(apple.X - snakeTrail[i].X) < snakeSize && Math.Abs(apple.Y - snakeTrail[i].Y) < snakeSize) {
                        apple = new Point(rnd.Next(5, (int)BORDER.XMAX), rnd.Next(5, (int)BORDER.YMAX));
                        exit = true;
                        break;
                    } else exit = false;
                }
            }

            Ellipse newApple = new Ellipse();
            newApple.Fill = Brushes.Red;
            newApple.Width = snakeSize;
            newApple.Height = snakeSize;
            Canvas.SetTop(newApple, apple.Y);
            Canvas.SetLeft(newApple, apple.X);
            gameCanvas.Children.Insert(index, newApple);
            apples.Insert(index, apple);
        }

        private bool DirectionChange() {
            switch (direction) {
                case DIRECTION.DOWN:
                currentPosition.Y += stepSize;
                PaintSnake(currentPosition);
                break;

                case DIRECTION.UP:
                currentPosition.Y -= stepSize;
                PaintSnake(currentPosition);
                break;

                case DIRECTION.LEFT:
                currentPosition.X -= stepSize;
                PaintSnake(currentPosition);
                break;

                case DIRECTION.RIGHT:
                currentPosition.X += stepSize;
                PaintSnake(currentPosition);
                break;
            }

            if ((currentPosition.X < 0) || (currentPosition.X > (int)BORDER.XMAX) || (currentPosition.Y < 0) || (currentPosition.Y > (int)BORDER.YMAX))
                return true;
            return false;
        }

        private bool SelfCollisionCheck() {
            for (int i = 0; i < (snakeTrail.Count - snakeSize * 2); i++) {
                Point point = new Point(snakeTrail[i].X, snakeTrail[i].Y);
                if ((Math.Abs(point.X - currentPosition.X) < (snakeSize)) && (Math.Abs(point.Y - currentPosition.Y) < (snakeSize)))
                    return true;
            }
            return false;
        }

        private void TickTimerClassic(object sender, EventArgs e) {
            if (DirectionChange()) EndGame();
            for (int i = 0; i < apples.Count; i++) {
                if ((Math.Abs(apples[i].X - currentPosition.X) < snakeSize) && (Math.Abs(apples[i].Y - currentPosition.Y) < snakeSize)) {
                    length += 10;
                    scoring.Score += 10;
                    apples.RemoveAt(i);
                    gameCanvas.Children.RemoveAt(i);
                    PaintApple(i);
                    break;
                }
            }
            if (SelfCollisionCheck()) EndGame();
        }

        private void TickTimerScoreHunt(object sender, EventArgs e) {
            if (DirectionChange()) EndGame();
            for (int i = 0; i < apples.Count; i++) {
                if ((Math.Abs(apples[i].X - currentPosition.X) < snakeSize) && (Math.Abs(apples[i].Y - currentPosition.Y) < snakeSize)) {
                    length += length < 1000 ? 10 : 0;
                    scoring.Score += 10;
                    snakeColor = ColorPicker();
                    apples.RemoveAt(i);
                    gameCanvas.Children.RemoveAt(i);
                    PaintApple(i);
                    break;
                }
            }
            if (SelfCollisionCheck()) EndGame();
        }

        private void OnButtonKeyDown(object sender, KeyEventArgs e) {
            if (gameInProgress) {
                switch (e.Key) {
                    case Key.Up:
                    if (prevDirection != DIRECTION.DOWN)
                        direction = DIRECTION.UP;
                    break;

                    case Key.Down:
                    if (prevDirection != DIRECTION.UP)
                        direction = DIRECTION.DOWN;
                    break;

                    case Key.Left:
                    if (prevDirection != DIRECTION.RIGHT)
                        direction = DIRECTION.LEFT;
                    break;

                    case Key.Right:
                    if (prevDirection != DIRECTION.LEFT)
                        direction = DIRECTION.RIGHT;
                    break;
                }
                prevDirection = direction;
            }
        }
        #endregion

        #region Game Controllers
        private void InitializeGame() {
            gameCanvas.Children.Clear();
            apples.Clear();
            snakeTrail.Clear();
            scoring.Score = 0;
            direction = DIRECTION.STOPPED;
            prevDirection = DIRECTION.STOPPED;
            length = 100;
            snakeColor = Brushes.Green;
            gameInProgress = false;
        }

        private void SetName() {
            if (tbPlayerName.Text.Length >= 3 && tbPlayerName.Text != string.Empty) {
                playerName = tbPlayerName.Text;
                HideWelcome();
                HideMenu(false);
            }
        }

        private void StartTimer(int TimeSpan, EventHandler EventHandler) {
            timer = new DispatcherTimer();
            timer.Interval = new TimeSpan(TimeSpan * 10000);
            timer.Tick += new EventHandler(EventHandler);
            timer.Start();
        }

        private void StartClassic() {

            StartTimer(1, TickTimerClassic);
            gameType = "c";

            this.KeyDown += new KeyEventHandler(OnButtonKeyDown);
            PaintApple(0);
            PaintSnake(startingPoint);
            currentPosition = startingPoint;
            gameInProgress = true;
        }

        private void StartScoreHunt() {

            StartTimer(1, TickTimerScoreHunt);
            gameType = "sh";

            this.KeyDown += new KeyEventHandler(OnButtonKeyDown);
            for (int i = 0; i < maxApples; i++)
                PaintApple(i);
            PaintSnake(startingPoint);
            currentPosition = startingPoint;
            gameInProgress = true;
        }

        private void LoadScores() {
            List<Scoreboard> classicScores = DB.LoadData(true);
            dgClassic.ItemsSource = classicScores;

            List<Scoreboard> scoreHuntScores = DB.LoadData(false);
            dgScoreHunt.ItemsSource = scoreHuntScores;
        }

        private void EndGame() {
            scoring.EndScore = scoring.Score;
            if (gameType == "c" || gameType == "sh")
                if (!DB.SaveData(playerName, scoring.EndScore, gameType == "c")) MessageBox.Show("Saving Error!");
            HideGame();
            HideGameOver(false);
            InitializeGame();
            timer.Stop();
        }
        #endregion

        #region Views
        private void HideGame(bool hide = true) {
            Game.Visibility = hide ? Visibility.Hidden : Visibility.Visible;
        }

        private void HideMenu(bool hide = true) {
            Menu.Visibility = hide ? Visibility.Hidden : Visibility.Visible;
        }

        private void HideHighScore(bool hide = true) {
            Highscore.Visibility = hide ? Visibility.Hidden : Visibility.Visible;
        }

        private void HideGameOver(bool hide = true) {
            GameOver.Visibility = hide ? Visibility.Hidden : Visibility.Visible;
        }

        private void HideWelcome(bool hide = true) {
            Welcome.Visibility = hide ? Visibility.Hidden : Visibility.Visible;
        }
        #endregion

        #region Buttons
        private void btnClassic_Click(object sender, RoutedEventArgs e) {
            StartClassic();
            HideGame(false);
            HideMenu(true);
        }

        private void btnScoreHunt_Click(object sender, RoutedEventArgs e) {
            StartScoreHunt();
            HideGame(false);
            HideMenu(true);
        }

        private void btnHighscore_Click(object sender, RoutedEventArgs e) {
            HideMenu();
            HideHighScore(false);
            LoadScores();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void btnMainMenu_Click(object sender, RoutedEventArgs e) {
            HideGameOver();
            HideHighScore();
            HideMenu(false);
        }

        private void SetName_Click(object sender, RoutedEventArgs e) {
            SetName();
        }

        private void tbPlayerName_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key == Key.Enter)
                SetName();
        }
        #endregion
    }
}
