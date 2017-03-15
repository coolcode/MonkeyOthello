using MonkeyOthello.AI;
using MonkeyOthello.Core;
using System;
using System.Collections.Generic;

using System.Windows.Forms;

namespace MonkeyOthello.Presentation
{    
    /// <summary>
    /// 游戏类
    /// </summary>
    class Game
    {
        /// <summary>
        /// 下了一步棋后轮转
        /// </summary>
        private enum TurnTo
        {
            /// <summary>
            /// 轮到自己下
            /// </summary>
            ME,
            /// <summary>
            /// 轮到对方下 
            /// </summary>
            YOU,
            /// <summary>
            /// 游戏结束
            /// </summary>
            GAMEOVER
        }

        /// <summary>
        /// 棋盘变换类形
        /// </summary>
        private enum TransType
        {
            /// <summary>
            /// 标准
            /// X
            /// XX
            /// XO
            /// </summary>
            STD=31,
            /// <summary>
            /// OX
            /// XXX
            /// </summary>
            T51=51,
            T59=59,
            T39=39,
            T58=58,
            T48=48,
            T32=32,
            T42=42,
        }
        //private readonly int[] transmaskTemp = new int[91] 
        //{
        //    0,0,0,0,0,0,0,0,0,
        //    0,10,11,12,13,14,15,16,17,
        //    0,19,20,21,22,23,24,25,26,
        //    0,28,29,30,31,32,33,34,35,
        //    0,37,38,39,40,41,42,43,44,
        //    0,46,47,48,49,50,51,52,53,
        //    0,55,56,57,58,59,60,61,62,
        //    0,64,65,66,67,68,69,70,71,
        //    0,73,74,75,76,77,78,79,80,
        //    0,0,0,0,0,0,0,0,0,0
        //};

        /// <summary>
        /// 棋盘变换的掩码
        /// </summary>
        private readonly int[,] transmark = new int[,]
        { {
            0,0,0,0,0,0,0,0,0,
            0,73,74,75,76,77,78,79,80,
            0,64,65,66,67,68,69,70,71,
            0,55,56,57,58,59,60,61,62,
            0,46,47,48,49,50,51,52,53,
            0,37,38,39,40,41,42,43,44,
            0,28,29,30,31,32,33,34,35,
            0,19,20,21,22,23,24,25,26, 
            0,10,11,12,13,14,15,16,17,
            0,0,0,0,0,0,0,0,0,0
        },
            {
            0,0,0,0,0,0,0,0,0,
            0,17,16,15,14,13,12,11,10,
            0,26,25,24,23,22,21,20,19,
            0,35,34,33,32,31,30,29,28,
            0,44,43,42,41,40,39,38,37,
            0,53,52,51,50,49,48,47,46,
            0,62,61,60,59,58,57,56,55,
            0,71,70,69,68,67,66,65,64,
            0,80,79,78,77,76,75,74,73,
            0,0,0,0,0,0,0,0,0,0
        },
            {
            0,0,0,0,0,0,0,0,0,
            0,73,64,55,46,37,28,19,10,
            0,74,65,56,47,38,29,20,11,
            0,75,66,57,48,39,30,21,12,
            0,76,67,58,49,40,31,22,13,
            0,77,68,59,50,41,32,23,14,
            0,78,69,60,51,42,33,24,15,
            0,79,70,61,52,43,34,25,16,
            0,80,71,62,53,44,35,26,17,
            0,0,0,0,0,0,0,0,0,0
        },
            {
            0,0,0,0,0,0,0,0,0,
            0,17,26,35,44,53,62,71,80,
            0,16,25,34,43,52,61,70,79,
            0,15,24,33,42,51,60,69,78,
            0,14,23,32,41,50,59,68,77,
            0,13,22,31,40,49,58,67,76,
            0,12,21,30,39,48,57,66,75,
            0,11,20,29,38,47,56,65,74,
            0,10,19,28,37,46,55,64,73,
            0,0,0,0,0,0,0,0,0,0
        }};

        private const int BLACK = 0;
        private const int WHITE = 1;
        private const int BookDepth = 17;
        /// <summary>
        /// 黑白棋棋盘
        /// </summary>
        private ChessType[] board;
        /// <summary>
        /// 画图板
        /// </summary>
        private DrawBoard drawBoard;
        /// <summary>
        /// 游戏是否结束
        /// </summary>
        private bool gameIsOver;
        private bool busy;


        /// <summary>
        /// 棋盘变换类形
        /// </summary>
        private TransType transType;
        /// <summary>
        /// 无效走步
        /// </summary>
        private const int VaildMove=-1;
        private System.Windows.Forms.ListView movesListView;
                
        /// <summary>
        /// 显示下棋历史记录
        /// </summary>
        public System.Windows.Forms.ListView MovesListView
        {
            set { movesListView = value; }
        }

        /// <summary>
        /// 棋盘画图板
        /// </summary>
        public DrawBoard DrawBoard
        {
            get { return drawBoard; }
            set { drawBoard = value; }
        }

        /// <summary>
        /// 棋手-猴子
        /// </summary>
        private MonkeyOthello.AI.Engine monkey;

        /// <summary>
        /// 棋步历史
        /// </summary>
        private List<int> moveList;

        /// <summary>
        /// 对战模式
        /// </summary>
        private GameMode gameMode;

        /// <summary>
        /// 翻棋线程是否运行
        /// </summary>
        private bool run = false;

        public Game()
        {
            drawBoard = new DrawBoard();
            transType = TransType.STD; 
        }

        /// <summary>
        /// 初始化游戏
        /// </summary>
        public void InitialGame()
        {
            //初始化棋步的历史记录为60步
            moveList = new List<int>(60);
            board = new ChessType[91];
        }

        /// <summary>
        /// 新游戏
        /// </summary>
        public void NewGame()
        {
            //gameIsOver = false;
            Board.Initial(board);
            switch (Config.Instance.OpenType)
            {
                case OpenType.WBBW:
                    board[40] = ChessType.WHITE;
                    board[41] = ChessType.BLACK;
                    board[49] = ChessType.BLACK;
                    board[50] = ChessType.WHITE;
                    break;
                case OpenType.BWWB:
                    board[40] = ChessType.BLACK;
                    board[41] = ChessType.WHITE;
                    board[49] = ChessType.WHITE;
                    board[50] = ChessType.BLACK;
                    break;
                default:
                    break;
            }

            transType = TransType.STD;
            drawBoard.SetBoardStruct(board);
            drawBoard.CurColor = ChessType.BLACK;
            drawBoard.SetCanpuSquare();

            //=============modify==========================
            drawBoard.PaintBuffer();
            Utils.RefreshBoard(drawBoard);
            //=============modify==========================

            gameMode = GameMode.PvsC;
            monkey = new Engine();
            monkey.Color = (gameMode == GameMode.CvsP ? ChessType.BLACK : ChessType.WHITE);
            monkey.AISet(Config.Instance.MonkeyAI);

            moveList.Clear();
            //判断是什么游戏模式
            if (gameMode == GameMode.CvsP)
                ComputerDown();
            gameIsOver = false;
            busy = false;
            if (gameMode == GameMode.CvsC)
                ComputervsComputer();

        }

        /// <summary>
        /// 电脑自我对战
        /// </summary>
        private void ComputervsComputer()
        {
            do
            {
                PlayGame();
            } while (!IsGameOver());
            
        }

        /// <summary>
        /// 上一步
        /// </summary>
        public bool StepUp()
        {
            int rebackNum=0;
            if (gameMode != GameMode.PvsP)
            {
                if (moveList.Count >= 2)
                {
                    gameIsOver = false;
                    rebackNum+=drawBoard.Reback();
                    rebackNum+=drawBoard.Reback();
                    for (int i = 1; i <= rebackNum; i++)
                    {
                        moveList.RemoveAt(moveList.Count - 1);
                        RemoveMoveItem();

                    }
                    drawBoard.PaintBuffer();
                    Utils.RefreshBoard(drawBoard);
                }
                if (moveList.Count < 2)
                {
                    return false;
                }
            }
            else
            {
                if (moveList.Count >= 1)
                {
                    gameIsOver = false;
                    rebackNum=drawBoard.Reback();
                    for (int i = 1; i <= rebackNum; i++)
                    {
                        moveList.RemoveAt(moveList.Count - 1);
                        RemoveMoveItem();
                        drawBoard.PaintBuffer();
                        Utils.RefreshBoard(drawBoard);
                    }
                }
                if (moveList.Count < 1)
                {
                    return false;
                }
            }
            //=============modify==========================
            //drawBoard.PaintBuffer();
            //StaticMethod.RefreshBoard(drawBoard);
            //=============modify==========================
            return true;
        }

        /// <summary>
        /// 双方对战
        /// </summary>
        public void PlayGame()
        {
            //轮到电脑
            TurnTo turn = TurnToWhere(TurnTo.ME);
            if (gameMode == GameMode.PvsC || gameMode == GameMode.CvsP)
            {
                if (turn == TurnTo.YOU)
                {
                    MessageBox.Show("电脑无棋可下,\n该你再下一步！");
                    Utils.AllRefreshBoard(drawBoard);
                    return;
                }
                else if (turn == TurnTo.ME)
                {
                    ComputerDown();
                    //轮到棋手(看看棋手是否有棋可下)
                    //turn = TurnToWhere(TurnTo.YOU);

                    //while (turn == TurnTo.ME)//如果还是轮到电脑就让电脑继续下
                    //{
                    //    MessageBox.Show("你无棋可下,\n该电脑再下一步！");
                    //    ComputerDown();
                    //    //轮到棋手(看看棋手是否有棋可下)
                    //    turn = TurnToWhere(TurnTo.YOU);
                    //}
                }
                //当前棋手下
                Utils.UpdateMessage("该你下棋了!");
            }
            else if (gameMode == GameMode.PvsP)
            {
                if (turn == TurnTo.YOU)
                {
                    if (drawBoard.CurColor == ChessType.BLACK)
                    {
                        drawBoard.PaintBuffer();
                        Utils.RefreshBoard(drawBoard);
                        MessageBox.Show("白子无棋可下,\n该黑子再下一步！");
                    }
                    else
                    {
                        drawBoard.PaintBuffer();
                        Utils.RefreshBoard(drawBoard);
                        MessageBox.Show("黑子无棋可下,\n该白子再下一步！");
                    }
                }
                //当前棋手下
                Utils.UpdateMessage( (drawBoard.CurColor== ChessType.BLACK?"该黑子下棋了!":"该白子下棋了!"));
            }
            else//电脑自我对战
            {
                if (turn == TurnTo.YOU)
                {
                    if (drawBoard.CurColor == ChessType.BLACK)                    
                        Utils.UpdateMessage("白子无棋可下,\n该黑子再下一步！");
                    else
                        Utils.UpdateMessage("黑子无棋可下,\n该白子再下一步！");
                }
                else if (turn == TurnTo.ME)
                {
                    ComputerDown();
                    //轮到棋手(看看棋手是否有棋可下)
                    turn = TurnToWhere(TurnTo.YOU);

                    while (turn == TurnTo.ME)//如果还是轮到电脑就让电脑继续下
                    {
                        if (drawBoard.CurColor == ChessType.WHITE)
                            Utils.UpdateMessage("白子无棋可下,\n该黑子再下一步！");
                        else
                            Utils.UpdateMessage("黑子无棋可下,\n该白子再下一步！");
                        ComputerDown();
                        //轮到棋手(看看棋手是否有棋可下)
                        turn = TurnToWhere(TurnTo.YOU);
                    }
                }
            }
            if (turn == TurnTo.GAMEOVER)
            {
                gameIsOver = true;
                drawBoard.PaintBuffer();
                Utils.RefreshBoard(drawBoard);
                Utils.UpdateMessage("游戏结束!");
                //StaticMethod.UpdateMessage(
                MessageBox.Show("游戏结束!结果是:" + drawBoard.GameOverResult((GameMode)Config.Instance.GameMode),"游戏结束!");
            }
        }

        /// <summary>
        /// 游戏是否结束
        /// </summary>
        /// <returns></returns>
        public bool IsGameOver()
        {
            return gameIsOver||drawBoard.IsGameOver();
        }

        private int getRandSquare()
        {
            List<int> moves = new List<int>();
            for (int i = 10; i <= 80; i++)
                if (board[i] == ChessType.EMPTY && Board.AnyFlips(board,i,ChessType.BLACK,ChessType.WHITE))
                    moves.Add(i);
            int square = moves[new Random().Next(moves.Count)];
            try
            {
                transType = (TransType)square;
            }
            catch
            {
            }
            return square;
        }

        /// <summary>
        /// 电脑下棋
        /// </summary>
        private void ComputerDown()
        {
            int square = VaildMove;
            double speed;
            Utils.UpdateMessage("电脑正在思考...");
            busy = true;
            if (drawBoard.Empties == 60)
            {//第一步随机下棋
                square = getRandSquare();                
                moveList.Add(square);
                AddMoveItem(square, 0);
                playerUpdateBoard(square);
               
            }
            else
            {
                /*if (drawBoard.Empties >= 60 - BookDepth && Config.Instance.UseBook)
                {
                    ChessType[] thinkBoard = realBoardTothinkBoard(transType);
                    monkey.Search(thinkBoard, drawBoard.CurColor);
                    square = thinkSquareTorealSquare(monkey.BestMove, transType);
                }
                else*/
                {
                    //电脑搜索
                    monkey.Search(board, drawBoard.CurColor);
                    square = monkey.BestMove;
                }
                //电脑下棋
                moveList.Add(square);
                AddMoveItem(square, monkey.BestScore);
                speed = (monkey.Time > 0 && monkey.Nodes > 0 ? monkey.Nodes / monkey.Time : Constants.MaxSpeed);
                Utils.UpdateMessage(square, monkey.BestScore, monkey.Nodes, monkey.Time, speed, drawBoard);
            }
            //电脑下棋
            if (Config.Instance.GameMode != GameMode.CvsC)
            {
                try
                {                   
                        if (run)
                        {
                           // System.Threading.Monitor.Wait(this);
                            System.Threading.Thread.Sleep(300);
                        }

                        System.Threading.Thread downThread2 = new System.Threading.Thread(
                            new System.Threading.ParameterizedThreadStart(computerUpdateBoard));
                        downThread2.Start(square);
                }
                catch { }               
            }
            else
            {
                playerUpdateBoard(square);
            }
            busy = false;

        }

        /// <summary>
        /// 棋手下棋
        /// </summary>
        /// <param name="point">鼠标位置</param>
        public bool PlayerDown(System.Drawing.Point point)
        {
            if (busy)
                return false;
            int sqnum = drawBoard.PointToSquare(point);
            if (sqnum == -1)
                return false;
            else
            {
                if (board[sqnum] == ChessType.EMPTY)
                {
                    if (!drawBoard.CanPut(sqnum))
                        return false;
                }
                else
                    return false;
            }
            if (drawBoard.Empties == 60)
                try
                {
                    transType = (TransType)sqnum;
                }
                catch
                {
                    transType = TransType.STD;
                }
            moveList.Add(sqnum);
            AddMoveItem(sqnum);

            lock (this)
            {
                if (run)
                    System.Threading.Thread.Sleep(300);
                System.Threading.Thread downThread2 = new System.Threading.Thread(playerUpdateBoard);
                downThread2.Start(sqnum);
                //playerUpdateBoard(sqnum);
            }
            return true;
        }

        private void playerUpdateBoard(object square)
        {
            try
            {
                lock (this)
                {
                    if (run)
                    {
                        System.Threading.Monitor.Wait(this);
                    }

                    run = true;
                    if (drawBoard.Empties == 60)
                        try
                        {
                            transType = (TransType)square;
                        }
                        catch
                        {
                            transType = TransType.STD;
                        }
                    drawBoard.PaintBuffer();
                    drawBoard.DownStone((int)square);
                    Utils.RefreshBoard(drawBoard);
                    /*
                    //播放下棋声音
                    if (Config.Instance.Sound)
                        new System.Threading.Thread(MonkeyOthello.IO.PlaySound.DownStone).Start();
                    if (Config.Instance.Animation)
                        Utils.Flips(drawBoard, Config.Instance.AnimatSpeed);
                        */
                    Utils.RefreshBoard(drawBoard);

                    run = false;

                    System.Threading.Monitor.Pulse(this);
                }
            }
            catch { }
        }

        private void computerUpdateBoard(object square)
        {
            playerUpdateBoard(square);
            //当前棋手下
            Utils.UpdateMessage("该你下棋了!");
            //轮到棋手(看看棋手是否有棋可下)
            TurnTo turn = TurnToWhere(TurnTo.YOU);
            if (turn == TurnTo.GAMEOVER)
            {
                gameIsOver = true;
                drawBoard.PaintBuffer();
                Utils.RefreshBoard(drawBoard);
                Utils.UpdateMessage("游戏结束!");
                MessageBox.Show("游戏结束!结果是:" + drawBoard.GameOverResult((GameMode)Config.Instance.GameMode), "游戏结束!");               
            }
            if (turn == TurnTo.ME)//如果还是轮到电脑就让电脑继续下
            {
                MessageBox.Show("你无棋可下,\n该电脑再下一步！");
                ComputerDown();
            }
        }

        /// <summary>
        /// 下了一步棋后轮转
        /// </summary>
        /// <returns>轮转</returns>
        private TurnTo TurnToWhere(TurnTo turn)
        {
            if (drawBoard.IsGameOver())
            {
                return TurnTo.GAMEOVER;
            }
            else if (!drawBoard.CanDown())//一方无棋可下,轮转
            {
                drawBoard.CurColor = 2 - drawBoard.CurColor;
                drawBoard.SetCanpuSquare();
                Utils.RefreshBoard(drawBoard);

                //另一方也是无棋可下,则游戏结束
                if (!drawBoard.CanDown())
                {
                    drawBoard.CurColor = 2 - drawBoard.CurColor;
                    return TurnTo.GAMEOVER;
                }
                //否则判断是谁没棋可下
                if (turn == TurnTo.ME) //是自己无棋可下
                {
                    return TurnTo.YOU;
                }
                else if (turn == TurnTo.YOU)//如果是对方无棋可下
                {
                    return TurnTo.ME;
                }
            }
            //有棋下
            return turn;
        }

        private ChessType[] TranBoard(ChessType[] board,TransMode mode)
        {
            ChessType[] temp=new ChessType[board.Length];
            int index=(int)mode;
            for (int i = 0; i < board.Length; i++)
                temp[i] = board[transmark[index, i]];
            return temp;
        }

        /// <summary>
        /// 棋盘变换
        /// </summary>
        /// <param name="mode"></param>
        public void TranBoard(TransMode mode)
        {
            board = TranBoard(board, mode);
            drawBoard.SetBoardStruct(board);
            drawBoard.SetCanpuSquare();            
        }

        /// <summary>
        /// 当前棋盘转换为电脑开局思考的棋盘(标准化)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private ChessType[] realBoardTothinkBoard(TransType type)
        {
            ChessType[] temp = new ChessType[board.Length];
            TransMode mode1, mode2;
            int index1, index2;
            switch (type)
            {
                case TransType.STD:
                    return board;
                case TransType.T51:
                    mode1 = TransMode.INV;
                    mode2 = TransMode.VER;
                    break;
                case TransType.T59:
                    mode1 = TransMode.ALG;
                    mode2 = TransMode.ALG;
                    break;
                case TransType.T39:
                    mode1 = TransMode.INV;
                    mode2 = TransMode.LEV;
                    break;
                case TransType.T58:
                    return TranBoard(board, TransMode.VER);
                case TransType.T48:
                    return TranBoard(board, TransMode.ALG);
                case TransType.T32:
                    return TranBoard(board, TransMode.LEV);
                case TransType.T42:
                    return TranBoard(board, TransMode.INV);
                default:
                    return board;
            }
            index1 = (int)mode1;
            index2 = (int)mode2;
            for (int i = 0; i < board.Length; i++)
                temp[i] = board[transmark[index2,transmark[index1, i]]];
            return temp;
        }

        /// <summary>
        /// 电脑使用开局库思考到的下棋位置转换为真正的下棋位置(反标准化)
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private  int thinkSquareTorealSquare(int thinkSquare,TransType type)
        {
            TransMode mode1, mode2;
            int index1, index2;
            switch (type)
            {
                case TransType.STD:
                    return thinkSquare;
                case TransType.T51:
                    mode1 = TransMode.VER;
                    mode2 = TransMode.ALG;
                    break;
                case TransType.T59:
                    mode1 = TransMode.ALG;
                    mode2 = TransMode.ALG;
                    break;
                case TransType.T39:
                    mode1 = TransMode.LEV;
                    mode2 = TransMode.ALG;
                    break;
                case TransType.T58:
                    return transmark[(int)TransMode.VER,thinkSquare];
                case TransType.T48:
                    return transmark[(int)TransMode.ALG, thinkSquare];
                case TransType.T32:
                    return transmark[(int)TransMode.LEV, thinkSquare];
                case TransType.T42:
                    return transmark[(int)TransMode.INV, thinkSquare];
                default:
                    return thinkSquare;
            }
            index1 = (int)mode1;
            index2 = (int)mode2;
            return transmark[index2, transmark[index1, thinkSquare]];
        }

        /// <summary>
        /// 列表控件添加记录(委托)
        /// </summary>
        /// <param name="Item"></param>
        private delegate void ListViewAddEvent(object Item);

        private void ListViewAddItem(object Item)
        {
            movesListView.Items.Add((ListViewItem)Item);
        }

        private void UpdateListView(System.Windows.Forms.ListViewItem Item)
        {
            movesListView.Invoke(new ListViewAddEvent(ListViewAddItem), new object[] { Item });
        }
        /// <summary>
        /// 列表控件移除记录(委托)
        /// </summary>
        /// <param name="Item"></param>
        private delegate void ListViewRemoveEvent();

        private void ListViewRemoveItem()
        {
            movesListView.Items.RemoveAt(movesListView.Items.Count - 1);
        }

        /// <summary>
        /// 增加电脑下棋记录
        /// </summary>
        /// <param name="square">位置</param>
        /// <param name="eval">计分</param>
        private void AddMoveItem(int square, double eval)
        {
            string lastmoveColor = (drawBoard.CurColor == ChessType.WHITE ? "白" : "黑");
            ListViewItem Item = new ListViewItem(new string[] { moveList.Count.ToString(), lastmoveColor, Utils.SquareToString(square), String.Format("{0}", Math.Round(eval)) });
            UpdateListView(Item);
        }

        /// <summary>
        /// 增加玩家下棋记录
        /// </summary>
        /// <param name="square">位置</param>
        private void AddMoveItem(int square)
        {
            string lastmoveColor = (drawBoard.CurColor == ChessType.WHITE ? "白" : "黑");
            ListViewItem Item = new ListViewItem(new string[] { moveList.Count.ToString(), lastmoveColor, Utils.SquareToString(square), "" });
            UpdateListView(Item);
        }

        private void RemoveMoveItem()
        {
            movesListView.Invoke(new ListViewRemoveEvent(ListViewRemoveItem), null);
        }

        public ChessType[] GetBoard()
        {
            return board;
        }

        internal void SetBoard(ChessType[] board)
        {
            this.board = board;
        }
    }
}
