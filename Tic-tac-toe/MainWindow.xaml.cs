using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Animation;


namespace Tic_tac_toe
{

   

  
    public partial class MainWindow : Window
    { 
        private readonly Dictionary<Player, ImageSource> imageSources = new()
        {

            { Player.X, new BitmapImage(new Uri("pack://application:,,,/Asset/X15.png")) },
            { Player.O, new BitmapImage(new Uri("pack://application:,,,/Asset/O15.png")) }
        
        };

        private readonly Dictionary<Player, ObjectAnimationUsingKeyFrames> animations = new()
        {
            { Player.X, new ObjectAnimationUsingKeyFrames() },
            { Player.O, new ObjectAnimationUsingKeyFrames() },
        };

        private readonly Image[,] imageControls = new Image[3, 3];
        private readonly Gamestate gameState = new Gamestate();
        public MainWindow()
        {
            InitializeComponent();
            SetupGamerGrid();
            SetupAnimations();

            gameState.MoveMade += OnMoveMade;
            gameState.GameEnded += OnGameEnded;
            gameState.GameRestarted += OnGameRestarted;
        }

        private void SetupGamerGrid()
        {
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c <3; c++)
                {
                    Image imageControl = new Image();
                    GameGrid.Children.Add(imageControl);
                    imageControls[r, c] = imageControl;
                }
            }
        }

        private void SetupAnimations()
        {
            animations[Player.X].Duration = TimeSpan.FromSeconds(.25);
            animations[Player.O].Duration = TimeSpan.FromSeconds(.25);

            for (int i = 0; i < 16; i++)
            {
                Uri xUri = new Uri($"pack://application:,,,/Asset/X{i}.png");
                BitmapImage ximg = new BitmapImage(xUri);
                DiscreteObjectKeyFrame xKeyFrame = new DiscreteObjectKeyFrame(ximg);
                animations[Player.X].KeyFrames.Add(xKeyFrame);

                Uri oUri = new Uri($"pack://application:,,,/Asset/O{i}.png");
                BitmapImage oImg = new BitmapImage(oUri);
                DiscreteObjectKeyFrame oKeyFrame = new DiscreteObjectKeyFrame(oImg);
                animations[Player.O].KeyFrames.Add(oKeyFrame);
            }
        }
        private void TransitionToEndSCreen(string text, ImageSource? winnerImage)
        {
            TurnPanel.Visibility = Visibility.Hidden;
            GameCanvas.Visibility = Visibility.Hidden;
            ResultText.Text = text;
            WinnerImage.Source = winnerImage;
            EndScreen.Visibility= Visibility.Visible;
        }

        private void TransitionToGameScreen()
        {
            EndScreen.Visibility = Visibility.Hidden;
            Line.Visibility = Visibility.Hidden;
            TurnPanel.Visibility = Visibility.Visible;
            GameCanvas.Visibility = Visibility.Visible;
        }

        private (Point, Point) FindLinePoints(Wininfo winInfo)
        {
            double squareSize = GameGrid.Width / 3;
            double margin = squareSize / 2;

            if(winInfo.Type == Win.Row)
            {
                double y = winInfo.Number * squareSize + margin;
                return (new Point(0, y), new Point(GameGrid.Width, y));
            }
            if (winInfo.Type == Win.Column)
            {
               double x = winInfo.Number * squareSize + margin;
               return (new Point(x, 0), new Point(x, GameGrid.Height));
            }
            if (winInfo.Type == Win.MainDiagonal)
            {
                return (new Point(0, 0), new Point(GameGrid.Width, GameGrid.Height));
            }

            return (new Point(GameGrid.Width, 0), new Point(0, GameGrid.Height));
        }

        private async Task ShowLine(Wininfo winInfo)
        {
            (Point start, Point end) = FindLinePoints(winInfo);

            Line.X1 = start.X;
            Line.Y1 = start.Y;

            DoubleAnimation x2Animation = new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(.25),
                From = start.X,
                To = end.X,
            };

            DoubleAnimation y2Animation = new DoubleAnimation
            {
                Duration = TimeSpan.FromSeconds(.25),
                From = start.Y,
                To = end.Y,
            };

            Line.Visibility = Visibility.Visible;
            Line.BeginAnimation(Line.X2Property, x2Animation);
            Line.BeginAnimation(Line.Y2Property, y2Animation);
            await Task.Delay(x2Animation.Duration.TimeSpan);
        }

      
        private void OnMoveMade(int r, int c)
        {
            Player player = gameState.GameGrid[r, c];
            imageControls[r, c].BeginAnimation(Image.SourceProperty, animations[player]);
            PlayerImage.Source = imageSources[gameState.CurrentPlayer];
        }

        private async void OnGameEnded(Gameresult gameResult)
        {
            await Task.Delay(1000);
            if (gameResult.Winner == Player.None)
            {
                TransitionToEndSCreen("It's a tie!", null);
            }
            else
            {
#pragma warning disable CS8604 // Possible null reference argument.
                await ShowLine(gameResult.Wininfo);
#pragma warning restore CS8604 // Possible null reference argument.
                await Task.Delay(1000);
                TransitionToEndSCreen("winner:", imageSources[gameResult.Winner]);
                
            }

        }

        private  void OnGameRestarted()
        {
            for (int r = 0; r < 3; r++)
            {
                for (int c = 0; c < 3; c++)
                {
                    imageControls[r, c].Source = null;
                    imageControls[r, c].Source = null;
                }
            }

            PlayerImage.Source = imageSources[gameState.CurrentPlayer];
            TransitionToGameScreen();
        }

        private void GamerGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            double squareSize = GameGrid.Width / 3;
            Point clickPosition = e.GetPosition(GameGrid);
            int row = (int)(clickPosition.Y / squareSize);
            int col = (int)(clickPosition.X / squareSize);
            gameState.MakeMove(row, col);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (gameState.GameOver)
            {
                gameState.Reset();
            }
        }
    }
}
