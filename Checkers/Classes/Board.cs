using Checkers.Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Checkers
{
    internal class Board
    {
        public List<IChecker> pCheckers;
        public ITeam[] pTeams;
        static public bool GameValid;
        private bool MoveSwitcher;
        private static string SaveName;
        public static int BotWalkTime = 500;
        public static int aiLevel = 0;
        public static string sAlphabet = "АБВГДЕЖЗ";
        public static int MovesCount = 0;

        public Board()
        {
            pCheckers = new List<IChecker>();
            GameValid = false;
            MoveSwitcher = false;
            SaveName = "save.txt";
        }

        private int menIndex = 0;
        private int selIndex = 0;

        public static void endLine()
        {
            Console.Write(endInLine());
        }

        public static string endInLine()
        {
            return ("                                  \n");
        }

        private void clearEx()
        {
            Console.SetCursorPosition(0, 0);
            Console.Clear();
        }

        private void MenuStart()
        {
            clearEx();
            
            menIndex = 0;
            selIndex = 0;
        }

        private void MenuUpdate()
        {
            Console.ResetColor();
            Console.Clear();
            
            menIndex = 0;
        }

        private void MenuTitle(string title)
        {
            StringBuilder corner = new StringBuilder("=============================");

            if (title.Length % 2 == 0) title += " ";

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            Console.Write(corner);
            Console.ResetColor(); endLine();

            int all_eq = corner.Length - title.Length;
            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;

            for (int i = 0; i < all_eq / 2; i++) { Console.Write(' '); }
            Console.Write(title);
            for (int i = 0; i < all_eq / 2; i++) { Console.Write(' '); }

            Console.ResetColor(); endLine();

            Console.BackgroundColor = ConsoleColor.White;
            Console.ForegroundColor = ConsoleColor.Black;
            Console.Write(corner); Console.ResetColor(); endLine();
            endLine();
        }

        private void MenuButton(string Text)
        {
            Console.ForegroundColor = (menIndex == selIndex) ? ConsoleColor.White : ConsoleColor.DarkGray;

            char c = (menIndex == selIndex) ? '>' : ' ';
            Console.Write($"{c} {Text}"); endLine(); ++menIndex;

        }

        public void MainMenu()
        {
            MenuStart();

            do
            {
                if (GameValid) break;
                MenuUpdate();

                MenuTitle("ШИШКИ");
                MenuButton("Играть");
                MenuButton("Загрузить сохранение"); endLine();
                MenuButton("Выход");

                Console.ForegroundColor = ConsoleColor.Black;

                ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
                switch (consoleKeyInfo.Key) 
                {
                    case ConsoleKey.DownArrow:
                        if (selIndex != menIndex-1) selIndex++;
                        break;
                    case ConsoleKey.UpArrow:
                        if (selIndex != 0) selIndex--;
                        break;
                    case ConsoleKey.Enter:
                        switch (selIndex)
                        {
                            case 0:
                                Console.ResetColor();
                                CreateGame();
                                MenuStart();
                                break;
                            case 1:
                                LoadGame();
                                MenuStart();
                                break;
                            case 2:
                                Environment.Exit(0);
                                break;
                        }
                        break;
                }
            } while (true);
        }

        private void GetStepText(int step)
        {
            int auto_id = 0;
            Console.ForegroundColor = (step == auto_id++) ? ConsoleColor.White : ConsoleColor.DarkGray; Console.Write("Цвет");
            Console.ForegroundColor = ConsoleColor.DarkGray;                                    Console.Write(">");
            Console.ForegroundColor = (step == auto_id++) ? ConsoleColor.White : ConsoleColor.DarkGray; Console.Write("Тип игрока 1");
            Console.ForegroundColor = ConsoleColor.DarkGray;                                    Console.Write(">");
            Console.ForegroundColor = (step == auto_id++) ? ConsoleColor.White : ConsoleColor.DarkGray; Console.Write("Тип игрока 2");
            Console.ForegroundColor = ConsoleColor.DarkGray;                                    Console.Write(">");
            Console.ForegroundColor = (step == auto_id++) ? ConsoleColor.White : ConsoleColor.DarkGray; Console.Write("Сложность");
            Console.Write(endInLine()); endLine();
            Console.ResetColor();
            switch (step)
            {
                case 0:
                    Console.Write("Выберите цвет (Игрок №1)");
                    break;
                case 1:
                    Console.Write("Выберите тип игрока №1");
                    break;
                case 2:
                    Console.Write("Выберите тип игрока №2");
                    break;
                case 3:
                    Console.Write("Выберите сложность");
                    break;
            }
            
            endLine();
            endLine();
        }

        public void CreateGame()
        {
            pCheckers.Clear();
            GameValid = true;
            MovesCount = 0;
            pTeams = new ITeam[2];

            ConsoleColor first_color = new ConsoleColor();
            ConsoleColor second_color = new ConsoleColor();

            bool uB1 = false;
            bool uB2 = false;

            MenuStart();

            int step = 0;

            do
            {
                MenuUpdate();

                MenuTitle("Создание игры");

                GetStepText(step);

                switch (step) {
                    case 0:
                        MenuButton("Чёрные");
                        MenuButton("Белые");

                        endLine();
                        MenuButton("Назад");
                    break;
                    case 1:
                    case 2:
                        MenuButton("Игрок");
                        MenuButton("Бот");

                        endLine();
                        break;
                    case 3:
                        MenuButton("Лёгкая");
                        MenuButton("Средняя");
                        MenuButton("Сложная");
                        break;
                }
                endLine();
                Console.ForegroundColor = ConsoleColor.Black;

                ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();
                switch (consoleKeyInfo.Key)
                {
                    case ConsoleKey.DownArrow:
                        if (selIndex != menIndex - 1) selIndex++;
                        break;
                    case ConsoleKey.UpArrow:
                        if (selIndex != 0) selIndex--;
                        break;
                    case ConsoleKey.Enter:
                        
                        if (step == 3 && uB1 || uB2)
                        {
                            aiLevel = selIndex;
                            step++;
                        }
                        else if (selIndex == 2 && step == 0) 
                        { 
                            GameValid = false;
                            return; 
                        }
                        else if (selIndex != 2)
                        {
                            if (step == 0)
                            {
                                first_color = (selIndex == 0) ? ConsoleColor.Black : ConsoleColor.White;
                                second_color = (selIndex == 1) ? ConsoleColor.Black : ConsoleColor.White;
                            }
                            else if (step == 1 || step == 2)
                            {
                                if (selIndex == 0)
                                {
                                    if (step == 1)
                                    {
                                        uB1 = false;
                                        pTeams[0] = new PlayerTeam(first_color);
                                        for (int i = 5; i <= 7; i++)
                                            for (int j = 0; j <= 7; j += 2)
                                                pCheckers.Add(new PlayerChecker(j + (i % 2 == 0 ? 1 : 0), i, first_color, false));
                                    }
                                    else
                                    {
                                        uB2 = false;
                                        pTeams[1] = new PlayerTeam(second_color);
                                        for (int i = 0; i < 3; i++)
                                            for (int j = 0; j <= 7; j += 2)
                                                pCheckers.Add(new PlayerChecker(j + (i % 2 == 0 ? 1 : 0), i, second_color, true));
                                    }
                                }
                                else
                                {

                                    if (step == 1)
                                    {
                                        uB1 = true;
                                        pTeams[0] = new BotTeam(first_color);
                                        for (int i = 5; i <= 7; i++)
                                            for (int j = 0; j <= 7; j += 2)
                                                pCheckers.Add(new BotChecker(j + (i % 2 == 0 ? 1 : 0), i, first_color, false));
                                    }
                                    else
                                    {
                                        uB2 = true;
                                        pTeams[1] = new BotTeam(second_color);
                                        for (int i = 0; i < 3; i++)
                                            for (int j = 0; j <= 7; j += 2)
                                                pCheckers.Add(new BotChecker(j + (i % 2 == 0 ? 1 : 0), i, second_color, true));
                                    }
                                }
                            }
                            step++;
                        }
                        
                        selIndex = 0;
                        if (step == 4 || (step == 3 && !uB1 && !uB2))
                        {
                            if (first_color == ConsoleColor.White)
                                MoveSwitcher = true;
                            else
                                MoveSwitcher = false;

                            DateTime dateTimea = DateTime.Now;
                            SaveName = "save_" + dateTimea.ToString("HH.mm.ss.dd.MM") + ".txt";

                            return;
                        }

                        break;
                }
            } while (true);
        }

        public void LoadGame()
        {
            string[] saves = Directory.GetFiles("saves");

            MenuStart();

            do
            {
                MenuUpdate();
                MenuTitle("Сохранения");

                for (int i = 0; i < saves.Length; i++)
                {
                    MenuButton(Path.GetFileName(saves[i]));
                }

                MenuButton("Назад");

                Console.ForegroundColor = ConsoleColor.Black;

                ConsoleKeyInfo consoleKeyInfo = Console.ReadKey();

                switch (consoleKeyInfo.Key)
                {
                    case ConsoleKey.DownArrow:
                        if (selIndex != menIndex - 1) selIndex++;
                        break;
                    case ConsoleKey.UpArrow:
                        if (selIndex != 0) selIndex--;
                        break;
                    case ConsoleKey.Backspace:
                        return;
                    case ConsoleKey.Enter:
                        int m = saves.Length;
                        switch (selIndex)
                        {
                            case var value when value == saves.Length:
                                return;
                            default:
                                GameValid = true;
                                Load(saves[selIndex]);
                                return;
                        }
                }

            } while (true);
        }

        public static void ExitGame()
        {
            Console.Write("Выход из игры"); endLine();
            Thread.Sleep(500);
            GameValid = false;
        }

        public void RunGame()
        {
            if (Environment.GetCommandLineArgs().Length > 1)
            {
                try
                {
                    BotWalkTime = Convert.ToInt32(Environment.GetCommandLineArgs()[1]);
                }
                catch (Exception) 
                {
                    BotWalkTime = 500;
                }
            }

            MainMenu();

            while (GameValid)
            {
                Frame();
                Move();
                EndFrame();

                if (!GameValid)
                    MainMenu();
            }
        }

        public static void DeleteSave()
        {
            if (File.Exists("saves\\" + SaveName))
                File.Delete("saves\\" + SaveName);
        }

        public void Load(string filename)
        {
            StreamReader sr = new StreamReader(filename);

            pCheckers.Clear();

            SaveName = sr.ReadLine();
            aiLevel = Convert.ToInt32(sr.ReadLine());
            MovesCount = Convert.ToInt32(sr.ReadLine());

            ConsoleColor first_color = ConvertColor(sr.ReadLine());
            ConsoleColor second_color = (first_color == ConsoleColor.White ? ConsoleColor.Black : ConsoleColor.White);

            pTeams = new ITeam[2];
            if (sr.ReadLine() == "Bot")
                pTeams[0] = new BotTeam(first_color);
            else
                pTeams[0] = new PlayerTeam(first_color);

            if (sr.ReadLine() == "Bot")
                pTeams[1] = new BotTeam(second_color);
            else
                pTeams[1] = new PlayerTeam(second_color);

            MoveSwitcher = !Convert.ToBoolean(sr.ReadLine());

            int CheckersCount = Convert.ToInt32(sr.ReadLine());
            for (int i = 0; i < CheckersCount; i++)
            {
                string Owner = sr.ReadLine();
                int y = Convert.ToInt32(sr.ReadLine());
                int x = Convert.ToInt32(sr.ReadLine());
                ConsoleColor color = ConvertColor(sr.ReadLine());
                bool damka = Convert.ToBoolean(sr.ReadLine());
                bool ud = Convert.ToBoolean(sr.ReadLine());
                IChecker checker;

                if (Owner == "Player")
                    checker = new PlayerChecker(x, y, color, ud);
                else
                    checker = new BotChecker(x, y, color, ud);

                checker.IsDamka = damka;
                pCheckers.Add(checker);
            }

            sr.Close();
        }

        private ConsoleColor ConvertColor(string color)
        {
            if (color == "White")
                return ConsoleColor.White;
            else
                return ConsoleColor.Black;
        }

        public void Save()
        {
            if (!Directory.Exists("saves"))
                Directory.CreateDirectory("saves");

            StreamWriter sw = new StreamWriter("saves\\" + SaveName);
            sw.WriteLine(SaveName);
            sw.WriteLine(aiLevel);
            sw.WriteLine(MovesCount);
            sw.WriteLine(pTeams[0].TeamColor.ToString());
            sw.WriteLine(pTeams[0].Owner);
            sw.WriteLine(pTeams[1].Owner);
            sw.WriteLine(MoveSwitcher);

            sw.WriteLine(pCheckers.Count.ToString());
            for (int i = 0; i < pCheckers.Count; i++)
            {
                sw.WriteLine(pCheckers[i].Owner);
                sw.WriteLine(pCheckers[i].Pos[0].ToString());
                sw.WriteLine(pCheckers[i].Pos[1].ToString());
                sw.WriteLine(pCheckers[i].TeamColor.ToString());
                sw.WriteLine(pCheckers[i].IsDamka.ToString());
                sw.WriteLine(pCheckers[i].PlaceUD.ToString());
            }
            sw.Close();

            var sortedFiles = new DirectoryInfo("saves").GetFiles().OrderBy(f => f.LastWriteTime).ToList();
            while (sortedFiles.Count > 20)
            {
                sortedFiles[0].Delete();
                sortedFiles = new DirectoryInfo("saves").GetFiles().OrderBy(f => f.LastWriteTime).ToList();
            }
        }

        public void Move()
        {
            if (MoveSwitcher)
                pTeams[0].Move(ref pCheckers, this);
            else
                pTeams[1].Move(ref pCheckers, this);

            MoveSwitcher = !MoveSwitcher;
            MovesCount++;
        }

        public void Frame()
        {
            Draw();
            DrawStats();
        }

        public void EndFrame()
        {
            if (GetCheckersCount(ConsoleColor.White) == 0)
            {
                Console.Clear();
                Frame(); // Костыль, повторная перерисовка для убитых шашек в конце игры
                Console.Write("Чёрные победили!"); endLine();
                GameValid = false;
                Board.DeleteSave();
                Console.ReadKey();
            }
            else if (GetCheckersCount(ConsoleColor.Black) == 0)
            {
                Console.Clear();
                Frame(); // Костыль, повторная перерисовка для убитых шашек в конце игры
                Console.Write("Белые победили!"); endLine();
                GameValid = false;
                Board.DeleteSave();
                Console.ReadKey();
            }

            Console.Clear();
            if (GameValid)
                Save();
        }

        public int GetCheckersCount(ConsoleColor color)
        {
            int count = 0;
            foreach (var ch in pCheckers)
                if (ch.TeamColor == color) 
                    count++;

            return count;
        }

        public static void ShowPixelByCoord(int x, int y)
        {
            Console.WriteLine($"Пиксель: {GetPixelByCoord(x, y)}");
        }

        public static void ShowPixelByPixel(IChecker checker)
        {
            Console.WriteLine($"Пиксель: {GetPixelByPixel(checker)}");
        }

        public static string GetPixelByCoord(int x, int y)
        {
            if (x < 0 || y < 0 || x > 7 || y > 7)
            {
                return ($"x{x} y{y}");
            }
            return ($"{sAlphabet[x]}{y + 1}");
        }

        public static string GetPixelByPixel(IChecker checker)
        {
            return ($"{sAlphabet[checker.Pos[1]]}{checker.Pos[0] + 1}");
        }

        public static IChecker GetPixel(int X, int Y, ref List<IChecker> Checkers)
        {
            foreach (var checker in Checkers)
            {
                if (checker.EqualPos(X, Y))
                    return checker;
            }

            return null;
        }

        public IChecker GetPixel(int X, int Y)
        {
            foreach (var checker in pCheckers)
            {
                if (checker.EqualPos(X, Y))
                    return checker;
            }

            return null;
        }

        public static void RemoveChecker(int X, int Y, ref List<IChecker> Checkers)
        {
            if (Checkers.Contains(GetPixel(X, Y, ref Checkers)))
                Checkers.Remove(GetPixel(X, Y, ref Checkers));
        }

        public void DrawStats()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("\nСтатистика:"); endLine();
            Console.Write($"Белых съедено:   {12 - GetCheckersCount(ConsoleColor.White)}{endInLine()}Белых осталось:  {GetCheckersCount(ConsoleColor.White)}"); endLine();
            Console.WriteLine($"Чёрных съедено:  {12 - GetCheckersCount(ConsoleColor.Black)}{endInLine()}Чёрных осталось: {GetCheckersCount(ConsoleColor.Black)}{endInLine()}");
            Console.WriteLine($"Количество ходов: {MovesCount}{endInLine()}");
            Console.ResetColor();
        }

        public void Draw()
        {
            bool WhitePixel;

            Console.Write("  ");
            for (int i = 0; i < 8; i++)
            {
                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write((sAlphabet[i]).ToString() + "  ");
                Console.ResetColor();
            }
            Board.endLine();

            for (int i = 0; i < 8; i++)
            {
                if (i % 2 == 0)
                    WhitePixel = true;
                else
                    WhitePixel = false;

                Console.BackgroundColor = ConsoleColor.Black;
                Console.ForegroundColor = ConsoleColor.Green;

                Console.Write((i + 1).ToString());

                for (int j = 0; j < 8; j++)
                {
                    if (WhitePixel)
                        Console.BackgroundColor = ConsoleColor.DarkYellow;
                    else
                        Console.BackgroundColor = ConsoleColor.DarkGray;

                    DrawResources.WritePixel(pCheckers, j, i);

                    WhitePixel = !WhitePixel;
                }
                Board.endLine();
            }

            Console.ResetColor();
        }
    }
}
