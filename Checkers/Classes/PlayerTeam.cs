using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Checkers.Classes
{
    internal class PlayerTeam : ITeam
    {
        public PlayerTeam(ConsoleColor color) : base(color) 
        {
            LastX = -1;
            LastY = -1;
            Owner = "Player";
        }

        private int LastX, LastY;

        private void CheckMove(ref List<IChecker> pCheckers, Board board)
        {
            List<int> checkers_id = new List<int>();
            Random rnd = new Random();

            for (int i = 0; i < pCheckers.Count; i++)
                if (pCheckers[i].TeamColor == TeamColor)
                    checkers_id.Add(i);

            var sw = new Stopwatch();
            sw.Start();

            while (true)
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
                if (checker.TryMove(topx, topy, ref pCheckers, board, false))
                    break;
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
        }

        public override void Move(ref List<IChecker> pCheckers, Board board)
        {
            Console.WriteLine(TeamColor == ConsoleColor.Black ? "Ход чёрных   " : "Ход белых   ");

            CheckMove(ref pCheckers, board);

input:
            Console.Write("Введите позицию шашки и позицию хода через пробел (пример: Б3 А4): ");
            string Result = Console.ReadLine();

            if (Result == null)
            {
                Board.ExitGame();
                return;
            }

            Result = Result.ToUpper();

            string FirstPlace = "", SecondPlace = "";

            for (int i = 0; i < Result.Length; i++) // Очистка пробелов перед командой
            {
                if (Result[i] != ' ')
                {
                    if (FirstPlace.Length < 2)
                        FirstPlace += Result[i];
                    else
                        SecondPlace += Result[i];
                }
            }

            if (FirstPlace.Length != 2 || SecondPlace.Length != 2)
            {
                Console.WriteLine("Ход введён неверно!");
                goto input;
            }

            int X, Y;
            int NewX, NewY;

            try
            {
                X = Board.sAlphabet.IndexOf(FirstPlace[0]);
                Y = Convert.ToInt32(FirstPlace[1].ToString()) - 1;

                NewX = Board.sAlphabet.IndexOf(SecondPlace[0]);
                NewY = Convert.ToInt32(SecondPlace[1].ToString()) - 1;
            }
            catch (Exception)
            {
                Console.Write("Ход введён неверно!"); Board.endLine();
                goto input;
            }

            if (Board.GetPixel(X, Y, ref pCheckers) == null)
            {
                Console.Write("В ведённой клетке нет шашки!"); Board.endLine();
                goto input;
            }
            else if (Board.GetPixel(X, Y, ref pCheckers).TeamColor != TeamColor)
            {
                Console.Write("Нельзя ходить за чужие шашки!"); Board.endLine();
                goto input;
            }
            else if (LastX != -1 && LastX != X && LastY != -1 && LastY != Y)
            {
                Console.Write("Вам необходимо побить шашку, шашкой из прошлого хода!"); Board.endLine();
                goto input;
            }

            // Блок проверки на то что можем ли мы сейчас убить кого то?
            List<int> checkers_id = new List<int>();

            for (int i = 0; i < pCheckers.Count; i++)
                if (pCheckers[i].TeamColor == TeamColor)
                    checkers_id.Add(i);

            bool can_kill = false, killed = false;
            for (int i = 0; i < checkers_id.Count; i++)
            {
                List<int[]> WinMoves;
                pCheckers[checkers_id[i]].GetKillCoords(ref pCheckers, out WinMoves);

                if (WinMoves.Count > 0)
                {
                    can_kill = true;
                    for (int j = 0; j < WinMoves.Count; j++)
                    {
                        int[] move = WinMoves[j];
                        if (move[0] == NewX && move[1] == NewY)
                            killed = true;
                    }
                }
            }

            if (can_kill && !killed)
            {
                Console.Write("Вам необходимо побить шашку!"); Board.endLine();
                goto input;
            }
            // Конец блока

            foreach (var checker in pCheckers)
            {
                if (checker.EqualPos(X, Y))
                {
                    int checkers_count = pCheckers.Count;
                    if (checker.TryMove(NewX, NewY, ref pCheckers, board))
                    {
                        LastX = NewX;
                        LastY = NewY;
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

                        LastX = -1;
                        LastY = -1;
                        return;
                    }
                    else
                        break;
                }
            }

            Console.Write("Ход введён не верно! 3"); Board.endLine();
            goto input;
        }
    }
}
