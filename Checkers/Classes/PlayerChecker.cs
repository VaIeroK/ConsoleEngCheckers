using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Checkers.Classes
{
    internal class PlayerChecker : IChecker
    {
        public PlayerChecker(int x, int y, ConsoleColor color, bool placeUD) : base(x, y, color, placeUD) { }

        public override bool TryMove(int x, int y, ref List<IChecker> Checkers, Board board, bool move = true, bool sleep = false)
        {
            bool res = false;
            if (Board.GetPixel(x, y, ref Checkers) == null)
            {
                if (x >= 0 && x < 8 && y >= 0 && y < 8)
                {
                    bool res1 = (PlaceUD ? (y - 1 == Pos[0] || IsDamka && y + 1 == Pos[0]) : (y + 1 == Pos[0] || IsDamka && y - 1 == Pos[0]));
                    bool res2 = (PlaceUD ? (y - 2 == Pos[0] || IsDamka && y + 2 == Pos[0]) : (y + 2 == Pos[0] || IsDamka && y - 2 == Pos[0]));
                    if (res1 && (x - 1 == Pos[1] || x + 1 == Pos[1])) // обычная ходьба
                        res = true;

                    int tmpX = x + (Pos[1] - x) / 2;
                    int tmpY = y + (Pos[0] - y) / 2;

                    if ((res2 && (x - 2 == Pos[1] || x + 2 == Pos[1])) && Board.GetPixel(tmpX, tmpY, ref Checkers) != null && Board.GetPixel(tmpX, tmpY, ref Checkers).TeamColor != TeamColor) // ходьба с убивством
                    {
                        if (move)
                            Board.RemoveChecker(tmpX, tmpY, ref Checkers);
                        res = true;
                    }
                }
            }

            if (res && move)
            {
                Move(x, y);
                if ((PlaceUD ? y == 7 : y == 0))
                    IsDamka = true;
            }

            return res;
        }

        public override void GetKillCoords(ref List<IChecker> Checkers, out List<int[]> coords)
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
