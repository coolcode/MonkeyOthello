using MonkeyOthello.Core;
using MonkeyOthello.Engines;
using MonkeyOthello.Tests.Engines;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Linq;
using System.IO;

namespace MonkeyOthello.Tests
{
    static class Test
    {
        #region test data
        //113
        private static string[] testData = new string[] {
//"........b.wwbw..bbwbbwwwbwbbbwwwbbwbbwwwbbbwbwww..bbwbw....bb...",
//"........b.wwbw..bbwbbwwwbwbbbwwwbbwbbwwwbbbwbwww..bbwbw.........",
"..wwwww.b.wwbw..bbwbbwwwbwbbbwwwbbwbbwwwbbbwbwww..bbwbw....bbbbw",
"wbbw.b...wwbwww.wwwwbwwwwwwbwbbbwwwwbwb.wwwwwbbb..wwbbb..wwwww..",
"..w.bb.bw.wbbbbbwwwwbbbbwbwwwwbbwbbbwwbwwwbbbbbww.wbbbbw......bw",
"bwwwwww..bwwwww..bbbwwww.bbbbbww.bbbbwww.bbbbbww..bbbbw...bbbbbw",
"b.wbw....bwwww.bbbbbbbbbbbbbbbbbbbwbbbbbbbbwwbbbb.wwww....w.bbbb",
"..bwb..b.bbwwwwwbbbwbbwbbwwbbbwbbwwwbwbbb.wwwwbbb.wwwb.b...wbbb.",
"...bwwbb..bbbwwb.bbbbbwwbbwbbbb.bbbwbbbb.bbbwwbb..bbbbbb..bbbbwb",
"..bbbbwb..bbbwbb.bbwwbbbwwwwbbbb.wwbbbbb.wwbbwbb.bbwww...bwwwww.",
"..b.....wwbb.b..wwwbbbbbbwwwbbwwbbwbbwwwbbbwwwwwbwbbwb..bwwwwwww",
"b.w.b...bb.bbw.wbbbbbbwwbbbbbwwwbwwwwwwwbwbwwww.bbwwww.b..wwwww.",
"b..b....b.bbbb.bbbwwwwbbbbbbwbbbbwbwbbbbbwbwbwbbbwwwww..bwwwww..",
"wbbb.ww.wbbbbw.bwbbwwb.bwbbwwbwbwwbwwww.wwbwwwwwwbwwww....ww..w.",
"bbbbbbb...wwww.bbbbbbwwb.bwbwwwbbbbwwwwb.bbbwwwb..wwbww...wwwww.",
"b.w.b...bb.bbw.wbbbbbbwwbbbbbwwwbwwwwwwwbwbwwww.bbwwww.b..wwwww.",
"bbbbbbb...wwww.bbbbbbwwb.bwbwwwbbbbwwwwb.bbbwwwb..wwbww...wwwww.",
"b..b..w.bbbbbw.bbwbwwwbbbwbbwbbbbwbwbbbbbwbbbwbbbbbbww..b.b.ww..",
"b..b....b.bbbb.bbbwwwwbbbbbbwbbbbwbwbbbbbwbwbwbbbwwwww..bwwwww..",
"...bbbb...bbbb.wbbbbbww.wbwbwww.wbbbbwwwbbbbbbww.bbbbbbw.wwwwww.",
"..bbb.....bbbw.wwwwwbwwwbwwbbbbwwwbwwbbw.bwwbwbwb.wwbbbw..wwwwbw",
"w..bbbb.bbbbbbb.wbwbwwbbwwbbwbbbwwbbwbbbw.wwwwbb..wwww.b..wwww..",
"..bbbbb.wwbbbbb..wwbbbbwbbwwwbbwbbwwwbbwbbbbbwbww.wwwwww.....w.w",
".bbb....w.bbbb.wwbbbbbbbwbbwbbbbwbwbwwbbwwbbbbwbw.bbbwww...bww.w",
"..wwwww.bbwwww.bbbbwwwbbbwbbwbwbbwbbwbbbbbwbbbbbb.wwww......www.",
"w..bbb..bbbbbb..wbwbwbbbwbbwwbbbwbwbwbbbwbbwwwbb.bwwww.b..wwww..",
".bbbbbbb..wwbwb.wwwwwbww.wbwbwwb.bwbbwbb.wbbbbbb...bbbbb..wwwwww",
"..bbbbbw..bbbbww..bbbwbw..bbwbbw.wbwwbbwwwbbwwwwwwbbbb.bbbbbbb..",
".wwwww...wwbbbb.wwwwbbbbwwwwbwb.wwwwwww.wbwwwwwwbbbwww..wwww..w.",
"b.w.b...bb.bbb..bbbwwwwwbbbbbwwwbwbbwwbwbbwwbwwwbwwwwwwww....wbw",
"...bbbb...bbbb.wbbbbbww.wbwbwww.wbbbbwwwbbbbbbww.bbbbbbw.wwwwww.",
".wbbbb.w..bbbb.w.wbwwwbw.bbwwwww.bbwwwbw.bwbwbww.bbbbww.wbwwwww.",
"..wbbw.w..wwwwwbwwwwwwbbwwwbwbwbwwbwbwwbwbbbwbbbw.bbbbbb..bb....",
"..bbbbb..bbbbbbbbbbwbbbbwbbbwbbbbbbbbwbb.bbbwww..bbbbwww....bbw.",
"..w..bww...wbbbb..wwbbbwbbbbbbwwbbbbwbwwbwbbwwwwbbwwwww.bbbbb.w.",
".wwwww..b.wbbw.w.bbwwbwwwwbbwwb.wwwbwwbbwwbwbwbb..bbwbb...bbbbbb",
"....w.....wwww.wwwwwbbbbwwwbwbbbwwwbwwbbwwbwwbwb.bbbbbbw.wwwwwww",
".ww.b.....wwbb..wbbwwbbbwbbbwwbbwbbbbbwbwbbbbwbb.bbbwb.b.bbbbbbb",
"..wbbbbww.bbwwwwwbbbwwwwwbbwwbwwwwwwwwwww.wbbwwww.w..www...bbb..",
".wbbbb.w..bbbb.w.wbwwwbw.bbwwwww.bbwwwbw.bwbwbww.bbbbww.wbwwwww.",
"..bbbb..w.bww...wwbbww..wbbwbwwwwbbbwwwwwbbbbwbw.bbwbbbw.bbbbbbw",
".bb.w....bbbww.bwbwbbwbbwwbwwbbbwwbwwwb.wwwbbwww.wwwwwww..bwwww.",
".....w....wwwwwwbbwbbbwb.bwwwwwbbbbwwbwbbbwbbwbbbbbbbbbbwbbbbb..",
"w..bww..bwwbbbbbbbbwbbbbbbbbwbbb..bwwwb...wwbwww.wwwwwww.wwwwww.",
"wwwwwbw...wwwww.bbwbbbbb.bbwwbbbbbbbbbbb.bwwwbbbb.wwww....wwwww.",
"..w.wb..wwbwww..wwbbwbw.wbbwbbw.wbbbwbwwwwbwwwbwwbbbbbbwb.bb..bw",
"..wbbw..wwwbbbbbwwbbbbbbwwwbbbwbwwwbwwww.wbbwb.w.bbbbw...b.wwww.",
"..wbbb.....b.bwwwwbbwbwwwwbbwwww.wbwwbww.wwbbwwb.wwwwwwb.wwwwwww",
"b.wwwww.bbwwwwb.bwbwwbbb.bwbbwbb.wbwbbbbw.bwbbbw...wwbbw..bw.wbw",
"..bwww....bbbb...wbwbb.wwwbwwwwwwwbbwbwwwwbbbwbwwwwbbbbw..wbbbbw",
"bbbbbb...bwbbbbbbbbwwbbwbbwbbbww.bwbbbw..bwbwbw...wwbwb...wwwwwb",
"w.w.....w.wwwb..wwwwbwwbbwbbbwwbbwbwwbwbbbwwbbbbbwwbww.bwwwwww..",
"....b.wb..bbbbbb..bbbbbb..bbbbw.wwbbbwwwwbbbbwwwbbbwwwwwbwwwwwww",
".bbbbbb.b.bbbb.bbbwwwwbbbbwwbwwbbbwbwbwb.wbwwwbbw.wwww.b...wbw..",
".wwwwww...wbbw..w.bwwbw..bbbwbwwbbbwbbwwbbwbbbbw.wbbbbbw..wbbbbw",
"..wbbw...wwwwwbbbbwbwwwbbbbwbbwbbbbbbbwbbbbbbwbb..bbbb.b..bbbb..",
"..wbbw..wwwbbbbbwwbbbbbbwwwbbbwbwwwbwwww.wbbwb.w.bbbbw...b.wwww.",
"b..www...bwwwwwwbwbbbbwwbwbbbwbwbbbbbbbwbbbbbbbwb..bbbbw...w..bw",
".b.www....bbbb...wwbbb.wwwwwbwwwwwwbwbwwwwbbbwbwwwwbbbbw..wbbbbw",
".bbbbb...wwwwbbbwwwbwbb.bwbwwbbwbbwbwbbwbbbbbbww...bbbww....wbbw",
"..bbbb.b..bwbbb.bbbbbbbbbbbbbbbbbbbbwbbbwbbbbbbb..bbbb...wwwwww.",
".wbw.....bbbwb..bwbwbw...wwwwwwbwwwwbwbbwwwwwbbbbwbwbbbb.bwwwwwb",
".wwwww....wbbw..bbbbbww.bbbwbww.bbbwwbwwbbbbwbww.wbbbbbw..bbbbbw",
"wwwbbbbwbbbbbbbwwbwbwwww.wbwwwbwwwwwwbwwb.bbbwww...bbww......bw.",
".b.wwww...bbbw...wwbwb.wwwwwbbb.wwwbwbbbwwbbbwbbwwwbbbbb..wbbbbw",
"..b.b.w.w.bbbb.b.bbbwbbbbbwbbwbbbwwwbwwbbwwwwbwbb.wwwwbb..wwww.b",
".wwwww...bbbbw.bbwbwwwbbbwwbwbbbbwbwbbb.wwwbwb.w.wwbbw...wwwwww.",
"..bbbw...bbbbb..bwwbbbbbbbwwbbbbbwbbwwwwbbwbwwwwb.bwbww..b.wwww.",
"..bbbbb.w.bbwb..wbbwbww.bbbbwww.bwwwwww.bwwwwwwwb.wwwww..bwwwwww",
"..w.wb....wwwb.wbbwwwbwwbwwwbwwwbbbwwbwwwwwbwwbw..wwbbbb..wwwwww",
"..bbbbbbw.bbbb.bwwbwbwwbwbwbwwwbwwbbwbwbwwwwwwbb...w.bwb....bbww",
"...bw....bbwwb.bwwbwwbbbwwwbwbbb.wwbbbbbbwwbbbwb..wwwwwb.bwwwbbb",
"..wbwwwbwbbbbwwbwbbbwbwbwwwwbwwbwbwbwbwbwwbwwwbbw..bww.b.....w..",
"..bbw..w..bwwwbbwwbbwwwbwbbwwwwwwbwwwwwwwwwwwww.wbwww...wwwwww..",
"..wwwww..bbbww.wbbbwwwwwbbbwwwbwbbwbwwwwbwbbbwwwbbbbbb..b.b...b.",
".bbbbbb...bbwb..wwbwbwwbwwwbwwwwwwwwbbwb.wwwbbw..wwbwww...bwwwwb",
".bbb..wwb.bwbwbbwbwbwwbbwbbwwwbwwbwwbw..wwwwwww.wbwww...wwwwww..",
"..wwww....wwwwb.bbwwwbbwbwwwbwbwwwwwwwbbbbbwbwbw.bbbww..bwwwww..",
".bbbbbbb.bbw.wb.wbbwwbwbwbbbbw.wwbwbbww.wbbwwb..wbbbbbb..wwwww..",
"wbbbwb...bbbbw..wbwwwwwbwwbwwwbbwwwwwbwbwwwbwwbb..wbww.b..wwww..",
"..wbb...bbwbbb..bbbwwwbwbbwbbbbbbbbwbwbb.bbbwbbbwwbwbbb..wwbbw..",
"bbbbbbb..bwwwb.w.wbbbbbw.wwbbwbw.wwwwbbw..wwbwbw...wbbbw.wwwwwww",
"b.w..w.b.bwwwwww.bbwbbwwbbbbbwbwwbwwwwww..bbbbbw..bbbbbw..bbbbbw",
".bbbbb..b.bbbbw.bbwbbwb.bwwwbbbbbwwbwbbwbwbbbbb.bbbwww..bb...www",
"wwwwwww..wbbbw.w.wwbbbbw.wbwbbbwwwwwbwwwb.wwbwbw..wwbbbw...wb.bw",
".bbbwww..bbwwwwwwbwwwwwwwwbbbwwwwbwbbbwwwwbbwbbbw.bbb.....bbw...",
"wwwwwww..wbbbw.w.wwbbbbw.wbwbbbwwwwwbwwwb.wwbwbw..wwbbbw...wb.bw",
"..w.wb..w.wwww...wbbwwbb.bwbbbwbwbbbbwwbwwwbbbbbw.bwbbbb.bbbbbwb",
"..ww....w.www...wwwbbwbbwwbbbbwbwbwwbbwwbbwwwbbbb.bwwwbb.bbbbwwb",
".wwwww..b.wbbw..bwbwbbw.bbbwbwwwbbbbwwwwbbbbww.b.bbbbbw...wwwwww",
"...wbb..bb.wbbb.bbbbwwwwbbbbbwwwbbwbwwww.wbwbwwww.bbbb...wwwwwww",
"..wwww.bbbwwwwb.bbbwwbwwwbbbbwbbbbbbbww.bbbwbw.w.bbw.w...bbb.www",
"w.wbbbbw.wbbbbb.bbwbbwb..bwwbwbwbbbwbbbwwbbbbbbw..bbbbb....wwww.",
"..bb.b....bbbb.bwwwwwwbb.bwbwbbbbbbbbbbbbbbbbbwbb.wwwwwb..bwwwwb",
"..wwwb..b.wwbb..bbbbwbbbbbbwbbbbbbwbwwbb.bwbwbwww.bbbbw...wbbbbw",
"bbbbbw..bwwb.w..bbbwww..bbwwwww.bwwbwbw.bwbbwwbww.bbwbb..wwbbbbw",
"..bbbbb.w.bbbb.bwbbbbbbbwbbbbbwbwbwbwww.b.wwwww..bwwww...bbbbbbb",
"w.wbbbbw.wbbbbb.bbwbbwb..bwwbwbwbbbwbbbwwbbbbbbw..bbbbb....wwww.",
"..wbbbbw..bwwwwwwbwbwbwwbbbwwwbww.wbwbww.wbwbwww..bbwww..bbbbw..",
"..bbbb....bbbb..wwwwwbwbwwwwwwbbwwwbwwbbwwbwbwb.wbbbbbbw...bbbbw",
".bbbbbb.wwwwbw..wwwbwbw.wbwwwwb.wbbbbbbbwwwwwww.w.wbww....wbwwww",
".wwwwww...bbbb..b.bbwbbbwwbbwbb.bwbbwbb.bwwbbbb.wwwwbwb..bbbbbbb",

/* some trivial positions: */
"wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwww..",
"wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwb.",
"wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwbw..",
"wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwbww.",
"wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwbw...",
"wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwbwbw....",
"bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbwbw...",
"bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbwb...",
"bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbwb..",
"bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbwb.",

"bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbwbbbbbbbwbbbbbwb.",
"wwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwwbwwwwwwwbwwwwbbb.",
            };
        #endregion

        public static void Fight()
        {
            var engines = new IEngine[] { new Pilot(), new MonkeyOthelloV2Engine() };
            IColosseum game = new Colosseum();
            game.Fight(engines);
        }

        public static void TestEndGameSearch()
        {
            if (File.Exists("test-data.txt"))
            {
                testData = File.ReadAllLines("test-data.txt");
            }

            var color = 'w';
            var sw = new Stopwatch();
            var engines = new IEngine[] {
                    //new MCTSEngine(),
                    new MonkeyEngineV10(),
                    new MonkeyEngine(),
                    //new EndGameEngine(),
                    //new AlphaBetaEngine(),
                };
            Console.WriteLine(string.Join(" vs. ", engines.Select(c => c.Name)));
            var ts = new TimeSpan[engines.Length];
            sw.Start();
            var length = Math.Min(20, testData.Length);// 10;// = bds.Length; 
            for (var i = 0; i < length; i++)
            {
                int empties = 0, white = 0, black = 0;
                var w = 0ul;
                var b = 0ul;
                for (var j = 0; j < 64; j++)
                {
                    var x = 1ul << j;
                    if (testData[i][j] == 'w')
                    {
                        w |= x;
                        white++;
                    }
                    else if (testData[i][j] == 'b')
                    {
                        b |= x;
                        black++;
                    }
                    else if (testData[i][j] == '.')
                    {
                        empties++;
                    }
                }

                var discdiff = (color == 'b' ? black - white : white - black);

                var board = (color == 'b' ? new BitBoard(b, w) : new BitBoard(w, b));

                Console.WriteLine($"{i} (empties={empties:D2} white={white:D2} black={black:D2}  diff={empties})");

                var index = 0;
                while (index < engines.Length)
                {
                    var r = engines[index].Search(board, empties);
                    //Console.WriteLine($"--cache info: {Rule.CacheInfo()}");
                    //Rule.ClearCache();
                    Console.WriteLine($"[{engines[index].Name}] r{index + 1}: {r}");
                    ts[index] += r.TimeSpan;
                    index++;
                }

                Console.WriteLine();
            }

            sw.Stop();


            for (var i = 0; i < ts.Length; i++)
            {
                Console.WriteLine($"[{engines[i].Name}]total (exclude console's time): {ts[i]}");
            }
            Console.WriteLine($"total: {sw.Elapsed}");
        }

        public static void TestFlips()
        {
            var board = new BitBoard(9246146104748420220UL, 8659321588746490112UL);
            var moves = Rule.FindMoves(board);
            var empties = board.EmptyPieces.Indices();
            var invalidMoves = empties.Except(moves);
            foreach (var im in invalidMoves)
            {
                var flipBits = Rule.FindFlips(board, im);
                Console.WriteLine(flipBits);
            }
        }

        public static void TestBitBoard()
        {
            var board1 = new BitBoard(9246146104748420220UL, 8659321588746490112UL);
            var board2 = new BitBoard(8659321588746490112UL, 9246146104748420220UL);
            var board3 = new BitBoard(9246146104748420220UL, 8659321588746490112UL);
            Console.WriteLine($"b1 hash: {board1.GetHashCode()}");
            Console.WriteLine($"b2 hash: {board2.GetHashCode()}");
            Console.WriteLine($"b1==b2? {board1 == board2}");
            Console.WriteLine($"b1==b3? {board1 == board2}");

            var dict = new Dictionary<BitBoard, int>();
            dict.Add(board1, 1);
            dict.Add(board2, 2);
            dict.Add(board3, 3);
        }

        public static void TestV2IndexToV3Index()
        {
            int[] worst2best = new int[]
           { 
/*B2*/      20 , 25 , 65 , 70 ,
/*B1*/      11 , 16 , 19 , 26 , 64 , 71 , 74 , 79 ,
/*C2*/      21 , 24 , 29 , 34 , 56 , 61 , 66 , 69 ,
/*D2*/      22 , 23 , 38 , 43 , 47 , 52 , 67 , 68 ,
/*D3*/      31 , 32 , 39 , 42 , 48 , 51 , 58 , 59 ,
/*D1*/      13 , 14 , 37 , 44 , 46 , 53 , 76 , 77 ,
/*C3*/      30 , 33 , 57 , 60 ,
/*C1*/      12 , 15 , 28 , 35 , 55 , 62 , 75 , 78 ,
/*A1*/      10 , 17 , 73 , 80 , 
/*D4*/      40 , 41 , 49 , 50
          };

            var v3indexList = worst2best.Reverse().Select(V2IndexToV3Index).ToArray();
            var t = string.Join(",", v3indexList);
            Console.WriteLine(t);
            var t2 = string.Join(",", v3indexList.Select(SquareToString));
            Console.WriteLine(t2);
        }

        public static int V2IndexToV3Index(this int index)
        {
            var m = (index - 9) % 9 - 1;
            var n = (index - 9) / 9;
            return n * 8 + m;
        }

        public static string SquareToString(int square)
        {
            var m = square / 8;
            var n = square % 8;
            var y = m + 1;
            var x = Convert.ToChar(n + 65);
            return x.ToString() + y.ToString();
        }

        public static void GenTestData()
        {
            var depth = 20;
            var rand = new Random();
            var lines = new List<string>();
            for (var i = 0; i < 10; i++)
            {
                var board = BitBoard.NewGame();
                while (board.EmptyPieces.CountBits() > depth)
                {
                    var moves = Rule.FindMoves(board);

                    if (moves.Length == 0)
                    {
                        board = board.Switch();
                        moves = Rule.FindMoves(board);
                        if (moves.Length == 0)
                        {
                            break;
                        }
                    }
                    var index = rand.Next(0, moves.Length);
                    var pos = moves[index];
                    board = Rule.MoveSwitch(board, pos);
                }

                if (board.EmptyPieces.CountBits() > depth)
                {
                    continue;
                }
                var btext = board.Draw();
                lines.Add(btext);
                Console.WriteLine(btext);
            }

            File.WriteAllLines("test-data.txt", lines);

        }

        public static void TestLinkedList()
        {
            var squareList = new LinkedList<int>(new[] { 5, 2, 0, 1, 3, 1, 4 });
            Action<LinkedList<int>> printList = list =>
               {
                   Console.WriteLine(string.Join(",", list));
               };

            Console.WriteLine($"now: ");
            printList(squareList);

            for (var current = squareList.First; current != null; current = current.Next)
            {
                var next = current.Next;
                squareList.Remove(current);
                Console.WriteLine($"remove: {current.Value}");
                printList(squareList);

                if (next == null)
                {
                    squareList.AddLast(current);
                }
                else
                {
                    squareList.AddBefore(next, current);
                }

                Console.WriteLine($"add: {current.Value}");
                printList(squareList);

                //Console.WriteLine(current.Value);
            }
        }

    }
}
