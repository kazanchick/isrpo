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
            // ���������, ��� ����� �������������:
            // 1. ���� ����� 0
            Assert.AreEqual(0, board.Score);

            // 2. �� ���� ���� ����� 2 ������
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
            // ����������: ������� �������� � �������� 2-2-0-0
            for (int i = 0; i < GameBoard.Size; i++)
            {
                for (int j = 0; j < GameBoard.Size; j++)
                {
                    board.Tiles[i, j].Value = 0;
                }
            }
            board.Tiles[0, 0].Value = 2;
            board.Tiles[0, 1].Value = 2;

            // ��������: ������� �����
            bool moved = board.MoveLeft();

            // ��������:
            Assert.IsTrue(moved);
            Assert.AreEqual(4, board.Tiles[0, 0].Value); // ������ ������������ � 4
            Assert.AreEqual(0, board.Tiles[0, 1].Value); // ������ ������ ������ ����� ������
            Assert.AreEqual(4, board.Score); // ���� ������ ����������� �� 4
        }

        [TestMethod]
        public void TestReach2048()
        {
            // ����������: ������� �������� � ����� �������� 1024
            for (int i = 0; i < GameBoard.Size; i++)
            {
                for (int j = 0; j < GameBoard.Size; j++)
                {
                    board.Tiles[i, j].Value = 0;
                }
            }
            board.Tiles[0, 0].Value = 1024;
            board.Tiles[0, 1].Value = 1024;

            // ��������: ������� ����� ��� ����������� ������
            bool moved = board.MoveLeft();

            // ��������:
            Assert.IsTrue(moved);
            Assert.AreEqual(2048, board.Tiles[0, 0].Value); // ������ ������������ � 2048
            Assert.AreEqual(0, board.Tiles[0, 1].Value); // ������ ������ ������ ����� ������
            Assert.AreEqual(2048, board.Score); // ���� ������ ����������� �� 2048

            // ���������, ��� ���� �� ��������, ��� ��� ���� ������ ������
            Assert.IsFalse(board.IsGameOverState);
        }
    }
}