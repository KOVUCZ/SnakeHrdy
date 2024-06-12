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
using System.Windows.Media.Animation;

namespace SnakeHrdy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private const int SnakeSquareSize = 20;
        private const double SnakeSpeed = 0.2; // movement per frame
        private const int InitialSnakeLength = 5;
        private const int FoodSquareSize = 20;

        private List<Point> snakeParts = new List<Point>();
        private Point snakeDirection = new Point(1, 0);
        private Point currentFoodPosition;
        private Random random = new Random();
        private bool gameIsRunning;
        private bool isPaused;
        private double timeSinceLastMove = 0;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            gameIsRunning = true;
            isPaused = false;
            timeSinceLastMove = 0;

            CompositionTarget.Rendering -= OnRender;
            CompositionTarget.Rendering += OnRender;

            snakeParts.Clear();
            snakeDirection = new Point(1, 0);
            for (int i = 0; i < InitialSnakeLength; i++)
            {
                snakeParts.Add(new Point(100 - i * SnakeSquareSize, 100));
            }

            GenerateFood();
            DrawGame();
            this.KeyDown -= OnKeyDown;
            this.KeyDown += OnKeyDown;
        }

        private void OnRender(object sender, EventArgs e)
        {
            if (!gameIsRunning || isPaused)
                return;

            timeSinceLastMove += SnakeSpeed;

            if (timeSinceLastMove >= 1)
            {
                MoveSnake();
                CheckCollision();
                DrawGame();
                timeSinceLastMove = 0;
            }
        }

        private void MoveSnake()
        {
            Point newHeadPosition = new Point(
                snakeParts[0].X + snakeDirection.X * SnakeSquareSize,
                snakeParts[0].Y + snakeDirection.Y * SnakeSquareSize
            );

            snakeParts.Insert(0, newHeadPosition);

            if (newHeadPosition == currentFoodPosition)
            {
                GenerateFood();
            }
            else
            {
                snakeParts.RemoveAt(snakeParts.Count - 1);
            }
        }

        private void DrawGame()
        {
            GameCanvas.Children.Clear();

            // Draw the snake
            foreach (var part in snakeParts)
            {
                Rectangle rectangle = new Rectangle
                {
                    Width = SnakeSquareSize,
                    Height = SnakeSquareSize,
                    Fill = Brushes.Green,
                    Stroke = Brushes.Black, // Add outline color
                    StrokeThickness = 1
                };
                Canvas.SetLeft(rectangle, part.X);
                Canvas.SetTop(rectangle, part.Y);
                GameCanvas.Children.Add(rectangle);
            }

            // Draw the food
            Ellipse foodEllipse = new Ellipse
            {
                Width = FoodSquareSize,
                Height = FoodSquareSize,
                Fill = Brushes.Red
            };
            Canvas.SetLeft(foodEllipse, currentFoodPosition.X);
            Canvas.SetTop(foodEllipse, currentFoodPosition.Y);
            GameCanvas.Children.Add(foodEllipse);
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
            gameIsRunning = false;
            CompositionTarget.Rendering -= OnRender;
            MessageBox.Show("Game Over! Your score is: " + (snakeParts.Count - InitialSnakeLength));
            InitializeGame();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!gameIsRunning) return;

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
                case Key.Space:
                    isPaused = !isPaused;
                    break;
            }
        }
    }
}
