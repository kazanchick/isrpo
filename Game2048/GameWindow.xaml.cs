using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.IO;

namespace Game2048
{
    public partial class GameWindow : Window
    {
        private GameBoard board;
        private Label[,] labels;
        private int bestScore;
        private string scoreFilePath = "bestscore.txt";

        public GameWindow()
        {
            InitializeComponent();
            board = new GameBoard();
            labels = new Label[GameBoard.Size, GameBoard.Size];
            InitGridUI();
            DrawBoard();
            bestScore = LoadBestScore();
            BestScoreText.Text = bestScore.ToString();
            this.KeyDown += OnKeyDownHandler;
        }

        private void InitGridUI()
        {
            GameGrid.Children.Clear();

            for (int i = 0; i < GameBoard.Size; i++)
            {
                for (int j = 0; j < GameBoard.Size; j++)
                {
                    var label = new Label
                    {
                        FontSize = 32,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        BorderThickness = new Thickness(1),
                        BorderBrush = Brushes.Gray,
                        Background = Brushes.LightGray,
                        Margin = new Thickness(5)
                    };

                    labels[i, j] = label;
                    Grid.SetRow(label, i);
                    Grid.SetColumn(label, j);
                    GameGrid.Children.Add(label);
                }
            }
        }

        private readonly Dictionary<int, Brush> tileColors = new Dictionary<int, Brush>
        {
            { 0, Brushes.LightGray },
            { 2, Brushes.LightYellow },
            { 4, Brushes.Orange },
            { 8, Brushes.Coral },
            { 16, Brushes.Red },
            { 32, Brushes.Magenta },
            { 64, Brushes.Purple },
            { 128, Brushes.Yellow },
            { 256, Brushes.Green },
            { 512, Brushes.Cyan },
            { 1024, Brushes.Blue },
            { 2048, Brushes.Gold }
        };

        private void DrawBoard()
        {
            for (int i = 0; i < GameBoard.Size; i++)
            {
                for (int j = 0; j < GameBoard.Size; j++)
                {
                    int value = board.Tiles[i, j].Value;
                    var label = labels[i, j];

                    label.Content = value == 0 ? "" : value.ToString();
                    label.Background = tileColors.ContainsKey(value) ? tileColors[value] : Brushes.Orange;
                }
            }
            ScoreText.Text = board.Score.ToString();
        }

        private void OnKeyDownHandler(object sender, KeyEventArgs e)
        {
            bool moved = false;

            switch (e.Key)
            {
                case Key.Left:
                    moved = board.MoveLeft();
                    break;
                case Key.Right:
                    moved = board.MoveRight();
                    break;
                case Key.Up:
                    moved = board.MoveUp();
                    break;
                case Key.Down:
                    moved = board.MoveDown();
                    break;
            }

            if (moved)
            {
                board.AddRandomTile();
                DrawBoard();

                bool has2048 = false;
                for (int i = 0; i < GameBoard.Size; i++)
                {
                    for (int j = 0; j < GameBoard.Size; j++)
                    {
                        if (board.Tiles[i, j].Value == 2048)
                        {
                            has2048 = true;
                            break;
                        }
                    }
                    if (has2048) break;
                }
                if (has2048)
                {
                    MessageBox.Show("Поздравляем! Вы собрали плитку 2048!", "Победа!", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                if (board.Score > bestScore)
                {
                    bestScore = board.Score;
                    BestScoreText.Text = bestScore.ToString();
                    SaveBestScore(bestScore);
                }

                if (board.IsGameOverState)
                {
                    MessageBox.Show($"Игра окончена! Ваш счёт: {board.Score}", "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }
            }
           
        }

        private void Reset_Click(object sender, RoutedEventArgs e)
        {
            board.Reset();
            ScoreText.Text = "0";
            DrawBoard();
        }

        private void MenuButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private int LoadBestScore()
        {
            if (File.Exists(scoreFilePath))
            {
                string text = File.ReadAllText(scoreFilePath);
                if (int.TryParse(text, out int savedScore))
                    return savedScore;
            }
            return 0;
        }

        private void SaveBestScore(int score)
        {
            File.WriteAllText(scoreFilePath, score.ToString());
        }
    }
}