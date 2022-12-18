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

        public override bool IsSafeMove(int to_x, int to_y, ref List<IChecker> Checkers, Board board)
        {
            return true;
        }
    }
}
