using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Checkers.Classes
{
    internal class BotChecker : IChecker
    {

        public BotChecker(int x, int y, ConsoleColor color, bool placeUD) : base(x, y, color, placeUD) { }

        public override bool TryMove(int x, int y, ref List<IChecker> Checkers, Board board, bool move = true, bool sleep = false)
        {
            if (sleep)
            {
                board.EndFrame();
                board.Frame();
                Thread.Sleep(Board.BotWalkTime);
            }

            bool res = false;
            bool killed = false;
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
                        {
                            Board.RemoveChecker(tmpX, tmpY, ref Checkers);
                            killed = true;
                        }
                        res = true;
                    }
                }
            }

            if (res && move)
            {
                Move(x, y);
                if ((PlaceUD ? y == 7 : y == 0))
                    IsDamka = true;

                if (killed)
                {
                    List<int[]> coords;
                    GetKillCoords(ref Checkers, out coords);

                    for (int i = 0; i < coords.Count; i++)
                    {
                        int[] imove = coords[i];
                        if (TryMove(imove[0], imove[1], ref Checkers, board, false))
                        {
                            TryMove(imove[0], imove[1], ref Checkers, board, true, true);
                        }
                    }
                }
            }

            return res;
        }
    }
}
