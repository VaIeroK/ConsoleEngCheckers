using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Checkers.Classes
{
    internal class BotTeam : ITeam
    {
        public BotTeam(ConsoleColor color) : base (color) 
        {
            Owner = "Bot";
        }

        public bool TryKill(ref List<IChecker> pCheckers, Board board)
        {
            List<int> checkers_id = new List<int>();
            bool moved = false;

            for (int i = 0; i < pCheckers.Count; i++)
                if (pCheckers[i].TeamColor == TeamColor)
                    checkers_id.Add(i);

            for (int i = 0; i < checkers_id.Count; i++)
            {
                List<int[]> WinMoves;
                IChecker checker = pCheckers[checkers_id[i]];
                checker.GetKillCoords(ref pCheckers, out WinMoves);

                for (int j = 0; j < WinMoves.Count; j++)
                {
                    int[] move = WinMoves[j];
                    if (checker.TryMove(move[0], move[1], ref pCheckers, board))
                    {
                        moved = true;
                        break;
                    }
                }

                if (moved)
                    break;
            }
            return moved;
        }

        public bool TryDefend(ref List<IChecker> pCheckers, Board board)
        {
            List<int> enemy_checkers_id = new List<int>(); // id шашек врагов

            for (int i = 0; i < pCheckers.Count; i++)
                if (pCheckers[i].TeamColor != TeamColor)
                    enemy_checkers_id.Add(i);

            List<int> checkers_id = new List<int>();
            bool moved = false;

            for (int i = 0; i < pCheckers.Count; i++)
                if (pCheckers[i].TeamColor == TeamColor)
                    checkers_id.Add(i);

            for (int i = 0; i < enemy_checkers_id.Count; i++)
            {
                List<int[]> WinMoves;
                IChecker enemy_checker = pCheckers[enemy_checkers_id[i]];
                enemy_checker.GetKillCoords(ref pCheckers, out WinMoves); // Получили победные координаты врагов

                for (int j = 0; j < WinMoves.Count; j++)
                {
                    int[] move = WinMoves[j]; // Координата врага с убийством нашей шашки
                    for (int k = 0; k < checkers_id.Count; k++)
                    {
                        if (checkers_id[k] >= 0 && checkers_id[k] < pCheckers.Count)
                        {
                            IChecker checker = pCheckers[checkers_id[k]]; // Получили нашу шашку
                            if (checker.TryMove(move[0], move[1], ref pCheckers, board)) // И пробуем защитить нашу шашку
                            {
                                moved = true;
                                break;
                            }
                        }
                    }
                    if (moved)
                        break;
                }
                if (moved)
                    break;
            }

            return moved;
        }

        public bool TryWalk(ref List<IChecker> pCheckers, Board board)
        {
            bool moved = false;
            Random rnd = new Random();

            List<int> checkers_id = new List<int>();
            for (int i = 0; i < pCheckers.Count; i++)
                if (pCheckers[i].TeamColor == TeamColor)
                    checkers_id.Add(i);

            var sw = new Stopwatch();
            sw.Start();

            int wcount = 0;

            bool UseSafeLogic = (rnd.Next(0, 3) <= Board.aiLevel);

            while (!moved)
            {
                wcount++;
                int checker_id = checkers_id[rnd.Next(0, checkers_id.Count)];
                IChecker checker = pCheckers[checker_id];// Получаем рандомную шашку

                // Рандомим ход в радиусе 1 клетки от шашки
                int min_x = checker.Pos[1] - 1;
                int max_x = checker.Pos[1] + 1;
                if (min_x < 0) min_x = 0;
                if (min_x > 7) min_x = 7;
                if (max_x < 0) max_x = 0;
                if (max_x > 7) max_x = 7;

                int min_y = checker.Pos[0] - 1;
                int max_y = checker.Pos[0] + 1;
                if (min_y < 0) min_y = 0;
                if (min_y > 7) min_y = 7;
                if (max_y < 0) max_y = 0;
                if (max_y > 7) max_y = 7;

                int topx = rnd.Next(min_x, max_x + 1);
                int topy = rnd.Next(min_y, max_y + 1);
                int checkers_count = pCheckers.Count;

                // Логика отхода при опасности, и запрета идти в клетку где шашку убъют.
                if (wcount < 1500 && checker.TryMove(topx, topy, ref pCheckers, board, false) && (UseSafeLogic && !checker.IsSafeMove(topx, topy, ref pCheckers, board)))
                    continue;

                if (checker.TryMove(topx, topy, ref pCheckers, board))
                {
                    if (checkers_count != pCheckers.Count)
                    {
                        List<int[]> WinMoves;
                        checker.GetKillCoords(ref pCheckers, out WinMoves);
                        if (WinMoves.Count > 0)
                        {
                            board.EndFrame();
                            board.Frame();
                            Move(ref pCheckers, board);
                        }
                    }
                    moved = true;
                }
                else if (sw.Elapsed.Seconds > 3)  // Если цикл вышел в тупик, то скорее всего это поражение черных
                {
                    Console.WriteLine("Невозможно найти ход. Поражение " + (TeamColor == ConsoleColor.Black ? "чёрных   " : "белых   "));
                    Board.DeleteSave();
                    Console.ReadKey();
                    Board.GameValid = false;
                    break;
                }
            }
            sw.Stop();
            return moved;
        }

        public bool TryGoCenter(ref List<IChecker> pCheckers, Board board)
        {
            Random rnd = new Random();

            // Логика рандомна, а то бот постоянно будет ходить за дамки
            if (rnd.Next(0, 2) == 0)
                return false;

            bool moved = false;

            List<int> damka_checkers_id = new List<int>();
            for (int i = 0; i < pCheckers.Count; i++)
                if (pCheckers[i].TeamColor == TeamColor && pCheckers[i].IsDamka)
                    damka_checkers_id.Add(i);

            if (damka_checkers_id.Count == 0)
                return false;

            var sw = new Stopwatch();
            sw.Start();

            int wcount = 0;
            bool UseSafeLogic = (rnd.Next(0, 3) <= Board.aiLevel);

            while (!moved)
            {
                wcount++;
                int checker_id = damka_checkers_id[rnd.Next(0, damka_checkers_id.Count)];
                IChecker checker = pCheckers[checker_id];// Получаем рандомную шашку

                // Рандомим ход в радиусе 1 клетки от шашки
                int min_x = checker.Pos[1] - 1;
                int max_x = checker.Pos[1] + 1;
                if (min_x < 0) min_x = 0;
                if (min_x > 7) min_x = 7;
                if (max_x < 0) max_x = 0;
                if (max_x > 7) max_x = 7;

                int min_y = checker.Pos[0] - 1;
                int max_y = checker.Pos[0] + 1;
                if (min_y < 0) min_y = 0;
                if (min_y > 7) min_y = 7;
                if (max_y < 0) max_y = 0;
                if (max_y > 7) max_y = 7;

                int topx = rnd.Next(min_x, max_x + 1);
                int topy = rnd.Next(min_y, max_y + 1);
                int checkers_count = pCheckers.Count;

                int CenterX = rnd.Next(3, 5);
                int CenterY = rnd.Next(3, 5);
                double OldDistToCenter = Math.Abs(GetDistance(CenterX, CenterY, checker.Pos[1], checker.Pos[0]));
                double NewDistToCenter = Math.Abs(GetDistance(CenterX, CenterY, topx, topy));

                // Логика отхода при опасности, и запрета идти в клетку где шашку убъют.
                if (wcount < 700 && checker.TryMove(topx, topy, ref pCheckers, board, false) && (UseSafeLogic && !checker.IsSafeMove(topx, topy, ref pCheckers, board)))
                    continue;

                // Если наш шаг дальше от центра то скипаем ход
                if (NewDistToCenter < OldDistToCenter)
                {
                    if (checker.TryMove(topx, topy, ref pCheckers, board))
                    {
                        if (checkers_count != pCheckers.Count)
                        {
                            List<int[]> WinMoves;
                            checker.GetKillCoords(ref pCheckers, out WinMoves);
                            if (WinMoves.Count > 0)
                            {
                                board.EndFrame();
                                board.Frame();
                                Move(ref pCheckers, board);
                            }
                        }
                        moved = true;
                    }
                }

                // Выходим из логики если решение не найдено
                if (wcount > 1500)
                    break;
            }
            sw.Stop();
            return moved;
        }

        public override void Move(ref List<IChecker> pCheckers, Board board)
        {
            Console.WriteLine(TeamColor == ConsoleColor.Black ? "Ход чёрных   " : "Ход белых   ");
            Thread.Sleep(Board.BotWalkTime);

            bool moved = false;

            if (!moved)
                moved = TryKill(ref pCheckers, board);

            if (!moved)
                moved = TryDefend(ref pCheckers, board);

            if (!moved)
                moved = TryGoCenter(ref pCheckers, board);

            if (!moved)
                moved = TryWalk(ref pCheckers, board);

            Thread.Sleep(Board.BotWalkTime);
        }

        private double GetDistance(int x1, int y1, int x2, int y2)
        {
            return Math.Sqrt(Math.Pow((x2 - x1), 2) + Math.Pow((y2 - y1), 2));
        }
    }
}
