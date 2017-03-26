using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonkeyOthello.Core;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace MonkeyOthello.Engines.X
{
    public class EdaxEngine : BaseEngine, IDisposable
    {
        public const int EndGameDepth = 30;
        public const int WinLoseDepth = 36;

        /// <summary>
        /// default: 30 (seconds)
        /// </summary>
        public int Timeout { get; set; } = 30;

        private static readonly IEngine openingBookEngine = new OpeningBookEngine();

        public EdaxEngine()
        {
            Task.Run(() => CheckEdaxShell());
        }

        public override SearchResult Search(BitBoard board, int depth)
        {
            var empties = board.EmptyPiecesCount();

            //if (empties.InRange(55, 60))
            //{
            //    var engine = openingBookEngine;
            //    return engine.Search(board, 0);
            //}

            var pattern = board.Draw(ownSymbol: "O", oppSymbol: "X", emptySymbol: "-");
            
            var r = (empties <= WinLoseDepth && empties > EndGameDepth ? CallEdax("endgame-search", pattern, -1, 1, empties) :
                (empties <= EndGameDepth ? CallEdax("endgame-search", pattern, -64, 64, empties) :
                (CallEdax("midgame-search", pattern, -64, 64, depth)))
                );

            return r;
        }

        private Process process = null;

        private void CheckEdaxShell()
        {
            try
            {
                lock (this)
                {
                    if (process != null)//TODO: check if close
                    {
                        return;
                    }

                    var edaxPath = Path.Combine(Environment.CurrentDirectory, @"edax\");
                    var edaxFile = Path.Combine(edaxPath, "wEdax-x64.exe");

                    process = new Process();
                    process.StartInfo.FileName = "cmd";
                    //process.StartInfo.WorkingDirectory = edaxPath;
                    //process.StartInfo.Arguments = "";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardInput = true;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.ErrorDataReceived += Process_ErrorDataReceived;
                    process.StartInfo.CreateNoWindow = true;

                    process.Start();

                    process.BeginErrorReadLine();

                    var input = process.StandardInput;
                    var output = process.StandardOutput;

                    input.WriteLine($"cd /d \"{edaxPath}\"\n\n");
                    input.WriteLine($"wEdax-x64.exe -cassio\n\n");

                    while (true)
                    {
                        var line = output.ReadLine();
                        if (line.Contains("ready."))
                        {
                            break;
                        }
                        Thread.Sleep(50);
                    }
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        private void Process_ErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            RestartShell();
        }

        public void RestartShell()
        {
            lock (this)
            {
                if (process != null)
                {
                    process.Close();
                    process.Dispose();
                    process = null;
                }
            }
            CheckEdaxShell();
        }

        private SearchResult CallEdax(string gameMode, string pattern, int alpha, int beta, int depth)
        {
            var sw = Stopwatch.StartNew();

            CheckEdaxShell();

            var result = string.Empty;
            var foundResult = false;
            var reliability = 1.0;
            var retryCount = 10;
            var i = 0;
            do
            {
                var input = process.StandardInput;
                var output = process.StandardOutput;

                //ENGINE-PROTOCOL midgame-search --XO---O--XXO-O--OXOXO-OOXXOOXOX-OOXXXXX-OOOXXX----XOXOO--XXXX-XO 0 1 1 100
                var cmd = $"ENGINE-PROTOCOL {gameMode} {pattern}O {alpha} {beta} {depth} 100";
                input.WriteLine(cmd);
                input.Flush();

                var count = 10;
                var index = 0;
                do//TODO:try only one time
                {
                    var cts = new CancellationTokenSource();
                    var readTask = Task.Factory.StartNew(() =>
                    {
                        try
                        {
                            return output.ReadLine();
                        }
                        catch (Exception ex)
                        {
                            UpdateProgress?.Invoke(new SearchResult
                            {
                                Reliability = 0,
                                Message = $"spent: {sw.Elapsed}, error:{ex}"
                            });
                        }

                        return string.Empty;
                    });

                    if (!readTask.Wait(Timeout * 1000, cts.Token))
                    {
                        cts.Cancel();
                        break;
                    }

                    var line = readTask.Result;

                    if (string.IsNullOrWhiteSpace(line))
                    {
                        break;
                    }
                    //Console.WriteLine(line);
                    if (!string.IsNullOrWhiteSpace(line) && !line.Contains("ready."))
                    {
                        result = line;
                        foundResult = true;
                        break;
                    }
                    if (foundResult && line.Contains("ready."))
                    {
                        break;
                    }
                    // break;
                    Thread.Sleep(100);
                } while (index++ < count && sw.Elapsed.TotalMinutes < 2);

                if (!foundResult)
                {
                    //the result is not reliable
                    reliability -= 1.0 / retryCount;
                    if (gameMode == "endgame-search")
                    {
                        if (alpha == -1)
                        {
                            //give up

                            break;
                        }
                        //use minimum window to research
                        alpha = -1;
                        beta = 1;
                    }
                    else
                    {//midgame
                        //use lower depth to research
                        depth -= 2;
                        if (depth < 2)
                        {
                            break;
                        }
                    }

                    UpdateProgress?.Invoke(new SearchResult
                    {
                        Reliability = reliability,
                        Message = $"restart shell..., new param:[{alpha},{beta},{depth}] {sw.Elapsed}"
                    });

                    RestartShell();
                }

            } while (!foundResult && i++ < retryCount && sw.Elapsed.TotalMinutes<2);

            sw.Stop();
            //analyze output

            if (string.IsNullOrWhiteSpace(result))
            {
                return new SearchResult
                {
                    IsTimeout = true,
                    TimeSpan = sw.Elapsed,
                    Message = "time out!",
                    Process = 1,
                    Reliability = reliability,
                };
            }

            var r = ParseResult(result);
            r.TimeSpan = sw.Elapsed;
            r.Message = $"[{depth}][{gameMode}] {r.Message}";
            r.Process = 1;
            r.Reliability = reliability;

            return r;
        }

        private void CloseEdax()
        {
            if (process == null)
            {
                return;
            }

            var input = process.StandardInput;
            var output = process.StandardOutput;

            input.WriteLine($"ENGINE-PROTOCOL quit");
            input.Flush();

            //process.WaitForExit();

            process.Close();

        }

        /*
        private SearchResult CallEdax(string gameMode, string pattern, int alpha, int beta, int depth)
        {
            var sw = Stopwatch.StartNew();
            var process = new Process();

            var edaxPath = Path.Combine(Environment.CurrentDirectory, @"Tools\edax");
            var edaxFile = Path.Combine(edaxPath, "wEdax-x64.exe");

            process.StartInfo.FileName = "cmd";
            //process.StartInfo.WorkingDirectory = edaxPath;
            //process.StartInfo.Arguments = "";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.CreateNoWindow = true;
            process.Start();

            var input = process.StandardInput;
            var output = process.StandardOutput;

            input.WriteLine($"cd /d \"{edaxPath}\"\n\n");
            input.WriteLine($"wEdax-x64.exe -cassio\n\n");
            while (true)
            {
                var line = output.ReadLine(); 
                if (line.Contains("ready."))
                {
                    break;
                }
                Thread.Sleep(50);
            }
             
            var cmd = $"ENGINE-PROTOCOL {gameMode} {pattern}O {alpha} {beta} {depth} 100";
            input.WriteLine(cmd);
            input.Flush();


            var result = string.Empty;

            var foundResult = false;
            while (true)
            {
                var line = output.ReadLine();
                //Console.WriteLine(line);
                if (!string.IsNullOrWhiteSpace(line) && !line.Contains("ready."))
                {
                    result = line;
                    foundResult = true;
                }
                if (foundResult && line.Contains("ready."))
                {
                    break;
                }

                Thread.Sleep(100);
            }


            input.WriteLine($"ENGINE-PROTOCOL quit");
            input.Flush();

            //process.WaitForExit();

            process.Close();

            sw.Stop();
            //analyze output
            var r = ParseResult(result);
            r.TimeSpan = sw.Elapsed;
            r.Message = $" [{gameMode}] {r.Message}";
            r.Process = 1;

            return r;
        }
        */

        private static SearchResult ParseResult(string result)
        {
            var rs = result.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .ToArray();
            var move = rs[1].Substring(5).ToIndex();
            var score = (int)double.Parse(rs[4].Substring(1).Split(' ')[0]);
            var nodes = ulong.Parse(rs[6].Substring(5));
            var time = rs[rs.Length - 1];

            return new SearchResult
            {
                Move = move.Value,
                Score = score,
                Nodes = nodes,
                Message = $"{time} | {result}",
            };
        }

        public void Dispose()
        {
            CloseEdax();
        }
    }
}
