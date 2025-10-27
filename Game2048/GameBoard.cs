using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Game2048
{
    public class GameBoard
    {
        public const int Size = 4;
        public Tile[,] Tiles { get; private set; }
        private Random random = new Random();
        public int Score { get; private set; } = 0;

        private bool gameStop = false;

        public bool IsGameOverState => IsGameOver();

        public GameBoard()
        {
            Tiles = new Tile[Size, Size];

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    Tiles[i, j] = new Tile();
                }
            }
            AddRandomTile();
            AddRandomTile();
        }

        public void AddRandomTile()
        {
            var emptyTiles = new List<(int row, int col)>();

            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (Tiles[i, j].IsEmpty)
                        emptyTiles.Add((i, j));
                }
            }

            if (emptyTiles.Count == 0)
                return;

            var (row, col) = emptyTiles[random.Next(emptyTiles.Count)];
            Tiles[row, col].Value = random.NextDouble() < 0.9 ? 2 : 4;
        }

        public bool IsGameOver()
        {
            // Проверка: есть ли хотя бы одна пустая ячейка
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    if (Tiles[i, j].IsEmpty)
                        return false; // ещё можно ставить
                }
            }

            // Проверка: можно ли объединить соседние клетки
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    int value = Tiles[i, j].Value;

                    if (i < Size - 1 && Tiles[i + 1, j].Value == value)
                        return false;

                    if (j < Size - 1 && Tiles[i, j + 1].Value == value)
                        return false;
                }
            }

            return true; // игра окончена: нет ни пустых, ни возможных объединений
        }

        public void Reset()
        {
            Score = 0;
            // Очистка всех плиток
            for (int i = 0; i < Size; i++)
            {
                for (int j = 0; j < Size; j++)
                {
                    Tiles[i, j].Value = 0;
                }
            }

            // Сброс состояния
            gameStop = false;

            // Добавление двух начальных плиток
            AddRandomTile();
            AddRandomTile();
        }


        #region "Движения"

        public bool MoveLeft()
        {
            bool moved = false;
            for (int i = 0; i < Size; i++)
            {
                int[] values = new int[Size];
                int index = 0;
                for (int j = 0; j < Size; j++)
                    if (Tiles[i, j].Value != 0)
                        values[index++] = Tiles[i, j].Value;

                int[] merged = new int[Size];
                index = 0;
                for (int j = 0; j < Size; j++)
                {
                    if (j < Size - 1 && values[j] != 0 && values[j] == values[j + 1])
                    {
                        int mergedValue = values[j] * 2;
                        merged[index++] = mergedValue;
                        Score += mergedValue;
                        j++; // Пропускаем следующую плитку
                        moved = true;
                    }
                    else if (values[j] != 0)
                    {
                        merged[index++] = values[j];
                    }
                }

                for (int j = 0; j < Size; j++)
                {
                    if (Tiles[i, j].Value != merged[j])
                    {
                        Tiles[i, j].Value = merged[j];
                        moved = true;
                    }
                }
            }
            return moved;
        }

        public bool MoveRight()
        {
            bool moved = false;

            for (int i = 0; i < Size; i++)
            {
                int[] values = new int[Size];
                int index = Size - 1;

                // Собираем ненулевые значения справа налево
                for (int j = Size - 1; j >= 0; j--)
                {
                    if (Tiles[i, j].Value != 0)
                        values[index--] = Tiles[i, j].Value;
                }

                // Объединяем и добавляем очки
                for (int j = Size - 1; j > 0; j--)
                {
                    if (values[j] != 0 && values[j] == values[j - 1])
                    {
                        int mergedValue = values[j] * 2;
                        values[j] = mergedValue;
                        values[j - 1] = 0;
                        Score += mergedValue; // Добавляем очки
                        moved = true;
                    }
                }

                // Сдвигаем снова
                int[] newRow = new int[Size];
                index = Size - 1;
                for (int j = Size - 1; j >= 0; j--)
                {
                    if (values[j] != 0)
                        newRow[index--] = values[j];
                }

                // Применяем
                for (int j = 0; j < Size; j++)
                {
                    if (Tiles[i, j].Value != newRow[j])
                    {
                        Tiles[i, j].Value = newRow[j];
                        moved = true;
                    }
                }
            }

            return moved;
        }

        public bool MoveUp()
        {
            bool moved = false;

            for (int j = 0; j < Size; j++)
            {
                int[] values = new int[Size];
                int index = 0;

                for (int i = 0; i < Size; i++)
                {
                    if (Tiles[i, j].Value != 0)
                        values[index++] = Tiles[i, j].Value;
                }

                // Объединяем и добавляем очки
                for (int i = 0; i < Size - 1; i++)
                {
                    if (values[i] != 0 && values[i] == values[i + 1])
                    {
                        int mergedValue = values[i] * 2;
                        values[i] = mergedValue;
                        values[i + 1] = 0;
                        Score += mergedValue; // Добавляем очки
                        moved = true;
                    }
                }

                // Сдвигаем снова
                int[] newCol = new int[Size];
                index = 0;
                for (int i = 0; i < Size; i++)
                {
                    if (values[i] != 0)
                        newCol[index++] = values[i];
                }

                // Применяем
                for (int i = 0; i < Size; i++)
                {
                    if (Tiles[i, j].Value != newCol[i])
                    {
                        Tiles[i, j].Value = newCol[i];
                        moved = true;
                    }
                }
            }

            return moved;
        }

        public bool MoveDown()
        {
            bool moved = false;

            for (int j = 0; j < Size; j++)
            {
                int[] values = new int[Size];
                int index = Size - 1;

                for (int i = Size - 1; i >= 0; i--)
                {
                    if (Tiles[i, j].Value != 0)
                        values[index--] = Tiles[i, j].Value;
                }

                // Объединяем и добавляем очки
                for (int i = Size - 1; i > 0; i--)
                {
                    if (values[i] != 0 && values[i] == values[i - 1])
                    {
                        int mergedValue = values[i] * 2;
                        values[i] = mergedValue;
                        values[i - 1] = 0;
                        Score += mergedValue; // Добавляем очки
                        moved = true;
                    }
                }

                // Сдвигаем снова
                int[] newCol = new int[Size];
                index = Size - 1;
                for (int i = Size - 1; i >= 0; i--)
                {
                    if (values[i] != 0)
                        newCol[index--] = values[i];
                }

                // Применяем
                for (int i = 0; i < Size; i++)
                {
                    if (Tiles[i, j].Value != newCol[i])
                    {
                        Tiles[i, j].Value = newCol[i];
                        moved = true;
                    }
                }
            }

            return moved;
        }



        #endregion

    }
}
