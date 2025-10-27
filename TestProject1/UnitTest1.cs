using Game2048;

namespace TestProject1
{
    [TestClass]
    public class UnitTest1
    {
        private GameBoard board;

        [TestInitialize]
        public void Setup()
        {
            board = new GameBoard();
        }

        [TestMethod]
        public void TestInitialState()
        {
            // Проверяем, что после инициализации:
            // 1. Счет равен 0
            Assert.AreEqual(0, board.Score);

            // 2. На поле есть ровно 2 плитки
            int tileCount = 0;
            for (int i = 0; i < GameBoard.Size; i++)
            {
                for (int j = 0; j < GameBoard.Size; j++)
                {
                    if (!board.Tiles[i, j].IsEmpty)
                        tileCount++;
                }
            }
            Assert.AreEqual(2, tileCount);
        }

        [TestMethod]
        public void TestMoveLeft()
        {
            // Подготовка: создаем ситуацию с плитками 2-2-0-0
            for (int i = 0; i < GameBoard.Size; i++)
            {
                for (int j = 0; j < GameBoard.Size; j++)
                {
                    board.Tiles[i, j].Value = 0;
                }
            }
            board.Tiles[0, 0].Value = 2;
            board.Tiles[0, 1].Value = 2;

            // Действие: двигаем влево
            bool moved = board.MoveLeft();

            // Проверка:
            Assert.IsTrue(moved);
            Assert.AreEqual(4, board.Tiles[0, 0].Value); // Должны объединиться в 4
            Assert.AreEqual(0, board.Tiles[0, 1].Value); // Вторая плитка должна стать пустой
            Assert.AreEqual(4, board.Score); // Счет должен увеличиться на 4
        }

        [TestMethod]
        public void TestReach2048()
        {
            // Подготовка: создаем ситуацию с двумя плитками 1024
            for (int i = 0; i < GameBoard.Size; i++)
            {
                for (int j = 0; j < GameBoard.Size; j++)
                {
                    board.Tiles[i, j].Value = 0;
                }
            }
            board.Tiles[0, 0].Value = 1024;
            board.Tiles[0, 1].Value = 1024;

            // Действие: двигаем влево для объединения плиток
            bool moved = board.MoveLeft();

            // Проверка:
            Assert.IsTrue(moved);
            Assert.AreEqual(2048, board.Tiles[0, 0].Value); // Должны объединиться в 2048
            Assert.AreEqual(0, board.Tiles[0, 1].Value); // Вторая плитка должна стать пустой
            Assert.AreEqual(2048, board.Score); // Счет должен увеличиться на 2048

            // Проверяем, что игра не окончена, так как есть пустые клетки
            Assert.IsFalse(board.IsGameOverState);
        }
    }
}