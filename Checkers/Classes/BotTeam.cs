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
        public BotTeam(ConsoleColor color) : base (color) { }

        public override void Move(ref List<IChecker> pCheckers, Board board)
        {
            Console.WriteLine(TeamColor == ConsoleColor.Black ? "Ход чёрных" : "Ход белых");
            Thread.Sleep(Board.BotWalkTime);

            bool moved = false;
            Random rnd = new Random();

            // Пробуем убить шашку
            List<int> checkers_id = new List<int>();

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
            // Конец блока

            // Пробуем защитить шашки
            List<int> enemy_checkers_id = new List<int>(); // id шашек врагов

            for (int i = 0; i < pCheckers.Count; i++)
                if (pCheckers[i].TeamColor != TeamColor)
                    enemy_checkers_id.Add(i);

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
            // Конец блока

            // Пробуем просто ходить
            var sw = new Stopwatch();
            sw.Start();

            while (!moved)
            {
                int checker_id = checkers_id[rnd.Next(0, checkers_id.Count)];
                IChecker checker = pCheckers[checker_id];// Получаем рандомную шашку

                // Рандомим ход в радиусе 2х клеток от шашки, чтобы она не только ходила но и била
                int min_x = checker.Pos[1] - 2;
                int max_x = checker.Pos[1] + 2;
                if (min_x < 0) min_x = 0;
                if (min_x > 8) min_x = 8;
                if (max_x < 0) max_x = 0;
                if (max_x > 8) max_x = 8;

                int min_y = checker.Pos[0] - 2;
                int max_y = checker.Pos[0] + 2;
                if (min_y < 0) min_y = 0;
                if (min_y > 8) min_y = 8;
                if (max_y < 0) max_y = 0;
                if (max_y > 8) max_y = 8;

                int topx = rnd.Next(min_x, max_x);
                int topy = rnd.Next(min_y, max_y);
                int checkers_count = pCheckers.Count;
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
                    Console.WriteLine("Невозможно найти ход. Поражение " + (TeamColor == ConsoleColor.Black ? "чёрных" : "белых"));
                    Console.ReadKey();
                    Board.GameValid = false;
                    break;
                }
            }
            sw.Stop();
            // Конец блока

            Thread.Sleep(Board.BotWalkTime);
        }
    }
}
