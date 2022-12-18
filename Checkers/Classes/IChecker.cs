using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Classes
{
    abstract class IChecker
    {
        public IChecker(int x, int y, ConsoleColor color, bool placeUD)
        {
            Position = new int[2] { y, x };
            IsDamka = false;
            TeamColor = color;
            PlaceUD = placeUD;
        }

        private int[] Position;
        public ConsoleColor TeamColor;
        public bool IsDamka;
        public bool PlaceUD;

        public int[] Pos
        {
            get { return Position; }
            set { Position = value; }
        }

        public bool EqualPos(int x, int y)
        {
            return (Pos[1] == x && Pos[0] == y);
        }

        public void Move(int x, int y)
        {
            Pos[0] = y;
            Pos[1] = x;
        }

        public abstract bool TryMove(int x, int y, ref List<IChecker> Checkers, Board board, bool move = true, bool sleep = false);
        public abstract bool IsSafeMove(int to_x, int to_y, ref List<IChecker> Checkers, Board board);
        public void GetKillCoords(ref List<IChecker> Checkers, out List<int[]> coords)
        {
            coords = new List<int[]>();
            int CheckerX = Pos[1];
            int CheckerY = Pos[0];

            List<IChecker> EnemyCheckersAround = new List<IChecker>(); // Вражеские шашки в радиусе 1 пиксель

            for (int i = CheckerX - 1; i < CheckerX + 2; i++)
            {
                for (int j = CheckerY - 1; j < CheckerY + 2; j++)
                {
                    if (Board.GetPixel(i, j, ref Checkers) != null && Board.GetPixel(i, j, ref Checkers).TeamColor != TeamColor)
                        EnemyCheckersAround.Add(Board.GetPixel(i, j, ref Checkers));
                }
            }

            for (int i = 0; i < EnemyCheckersAround.Count; i++)
            {
                int EnemyX = EnemyCheckersAround[i].Pos[1];
                int EnemyY = EnemyCheckersAround[i].Pos[0];

                int tmpX = CheckerX - (CheckerX - EnemyX) * 2;
                int tmpY = CheckerY - (CheckerY - EnemyY) * 2;

                if (Board.GetPixel(tmpX, tmpY, ref Checkers) == null && TryMove(tmpX, tmpY, ref Checkers, null, false))
                {
                    int[] pos = new int[2] { tmpX, tmpY };
                    coords.Add(pos);
                }
            }
        }
    }
}
