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
using System.Windows.Threading;

namespace SnakeHrdy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int SnakeSquareSize = 20;
        private const int SnakeSpeed = 100; // in milliseconds
        private const int InitialSnakeLength = 5;
        private const int FoodSquareSize = 20;

        private List<Point> snakeParts = new List<Point>();
        private Point snakeDirection = new Point(1, 0);
        private Point currentFoodPosition;
        private DispatcherTimer gameTimer = new DispatcherTimer();
        private Random random = new Random();

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            gameTimer.Tick += GameTick;
            gameTimer.Interval = TimeSpan.FromMilliseconds(SnakeSpeed);
            gameTimer.Start();

            snakeParts.Clear();
            for (int i = 0; i < InitialSnakeLength; i++)
            {
                snakeParts.Add(new Point(100 - i * SnakeSquareSize, 100));
            }

            GenerateFood();
            this.KeyDown += OnKeyDown;
            DrawSnake();
        }

        private void GameTick(object sender, EventArgs e)
        {
            MoveSnake();
            CheckCollision();
            DrawSnake();
        }

        private void MoveSnake()
        {
            // Calculate the new head position
            Point newHeadPosition = new Point(
                snakeParts[0].X + snakeDirection.X * SnakeSquareSize,
                snakeParts[0].Y + snakeDirection.Y * SnakeSquareSize
            );

            // Add the new head position
            snakeParts.Insert(0, newHeadPosition);

            // Check for food collision
            if (newHeadPosition == currentFoodPosition)
            {
                GenerateFood();
            }
            else
            {
                // Remove the tail
                snakeParts.RemoveAt(snakeParts.Count - 1);
            }
        }

        private void DrawSnake()
        {
            GameCanvas.Children.Clear();

            // Draw the snake
            foreach (var part in snakeParts)
            {
                Rectangle rectangle = new Rectangle
                {
                    Width = SnakeSquareSize,
                    Height = SnakeSquareSize,
                    Fill = Brushes.Green
                };
                Canvas.SetLeft(rectangle, part.X);
                Canvas.SetTop(rectangle, part.Y);
                GameCanvas.Children.Add(rectangle);
            }

            // Draw the food
            Rectangle foodRectangle = new Rectangle
            {
                Width = FoodSquareSize,
                Height = FoodSquareSize,
                Fill = Brushes.Red
            };
            Canvas.SetLeft(foodRectangle, currentFoodPosition.X);
            Canvas.SetTop(foodRectangle, currentFoodPosition.Y);
            GameCanvas.Children.Add(foodRectangle);
        }

        private void GenerateFood()
        {
            Point foodPosition;
            do
            {
                foodPosition = new Point(
                    random.Next(0, (int)(GameCanvas.ActualWidth / FoodSquareSize)) * FoodSquareSize,
                    random.Next(0, (int)(GameCanvas.ActualHeight / FoodSquareSize)) * FoodSquareSize
                );
            } while (snakeParts.Contains(foodPosition));

            currentFoodPosition = foodPosition;
        }

        private void CheckCollision()
        {
            Point head = snakeParts[0];

            // Check wall collision
            if (head.X < 0 || head.X >= GameCanvas.ActualWidth ||
                head.Y < 0 || head.Y >= GameCanvas.ActualHeight)
            {
                GameOver();
            }

            // Check self collision
            for (int i = 1; i < snakeParts.Count; i++)
            {
                if (head == snakeParts[i])
                {
                    GameOver();
                }
            }
        }

        private void GameOver()
        {
            gameTimer.Stop();
            MessageBox.Show("Game Over! Your score is: " + (snakeParts.Count - InitialSnakeLength));
            InitializeGame();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Up:
                    if (snakeDirection != new Point(0, 1))
                        snakeDirection = new Point(0, -1);
                    break;
                case Key.Down:
                    if (snakeDirection != new Point(0, -1))
                        snakeDirection = new Point(0, 1);
                    break;
                case Key.Left:
                    if (snakeDirection != new Point(1, 0))
                        snakeDirection = new Point(-1, 0);
                    break;
                case Key.Right:
                    if (snakeDirection != new Point(-1, 0))
                        snakeDirection = new Point(1, 0);
                    break;
            }
        }
    }
}
