using MonkeyOthello.AI;
using MonkeyOthello.Core;
using System;
using System.Collections.Generic;

using System.Windows.Forms;

namespace MonkeyOthello.Presentation
{    
    /// <summary>
    /// ��Ϸ��
    /// </summary>
    class Game
    {
        /// <summary>
        /// ����һ�������ת
        /// </summary>
        private enum TurnTo
        {
            /// <summary>
            /// �ֵ��Լ���
            /// </summary>
            ME,
            /// <summary>
            /// �ֵ��Է��� 
            /// </summary>
            YOU,
            /// <summary>
            /// ��Ϸ����
            /// </summary>
            GAMEOVER
        }

        /// <summary>
        /// ���̱任����
        /// </summary>
        private enum TransType
        {
            /// <summary>
            /// ��׼
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
        /// ���̱任������
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
        /// �ڰ�������
        /// </summary>
        private ChessType[] board;
        /// <summary>
        /// ��ͼ��
        /// </summary>
        private DrawBoard drawBoard;
        /// <summary>
        /// ��Ϸ�Ƿ����
        /// </summary>
        private bool gameIsOver;
        private bool busy;


        /// <summary>
        /// ���̱任����
        /// </summary>
        private TransType transType;
        /// <summary>
        /// ��Ч�߲�
        /// </summary>
        private const int VaildMove=-1;
        private System.Windows.Forms.ListView movesListView;
                
        /// <summary>
        /// ��ʾ������ʷ��¼
        /// </summary>
        public System.Windows.Forms.ListView MovesListView
        {
            set { movesListView = value; }
        }

        /// <summary>
        /// ���̻�ͼ��
        /// </summary>
        public DrawBoard DrawBoard
        {
            get { return drawBoard; }
            set { drawBoard = value; }
        }

        /// <summary>
        /// ����-����
        /// </summary>
        private MonkeyOthello.AI.Engine monkey;

        /// <summary>
        /// �岽��ʷ
        /// </summary>
        private List<int> moveList;

        /// <summary>
        /// ��սģʽ
        /// </summary>
        private GameMode gameMode;

        /// <summary>
        /// �����߳��Ƿ�����
        /// </summary>
        private bool run = false;

        public Game()
        {
            drawBoard = new DrawBoard();
            transType = TransType.STD; 
        }

        /// <summary>
        /// ��ʼ����Ϸ
        /// </summary>
        public void InitialGame()
        {
            //��ʼ���岽����ʷ��¼Ϊ60��
            moveList = new List<int>(60);
            board = new ChessType[91];
        }

        /// <summary>
        /// ����Ϸ
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
            //�ж���ʲô��Ϸģʽ
            if (gameMode == GameMode.CvsP)
                ComputerDown();
            gameIsOver = false;
            busy = false;
            if (gameMode == GameMode.CvsC)
                ComputervsComputer();

        }

        /// <summary>
        /// �������Ҷ�ս
        /// </summary>
        private void ComputervsComputer()
        {
            do
            {
                PlayGame();
            } while (!IsGameOver());
            
        }

        /// <summary>
        /// ��һ��
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
        /// ˫����ս
        /// </summary>
        public void PlayGame()
        {
            //�ֵ�����
            TurnTo turn = TurnToWhere(TurnTo.ME);
            if (gameMode == GameMode.PvsC || gameMode == GameMode.CvsP)
            {
                if (turn == TurnTo.YOU)
                {
                    MessageBox.Show("�����������,\n��������һ����");
                    Utils.AllRefreshBoard(drawBoard);
                    return;
                }
                else if (turn == TurnTo.ME)
                {
                    ComputerDown();
                    //�ֵ�����(���������Ƿ��������)
                    //turn = TurnToWhere(TurnTo.YOU);

                    //while (turn == TurnTo.ME)//��������ֵ����Ծ��õ��Լ�����
                    //{
                    //    MessageBox.Show("���������,\n�õ�������һ����");
                    //    ComputerDown();
                    //    //�ֵ�����(���������Ƿ��������)
                    //    turn = TurnToWhere(TurnTo.YOU);
                    //}
                }
                //��ǰ������
                Utils.UpdateMessage("����������!");
            }
            else if (gameMode == GameMode.PvsP)
            {
                if (turn == TurnTo.YOU)
                {
                    if (drawBoard.CurColor == ChessType.BLACK)
                    {
                        drawBoard.PaintBuffer();
                        Utils.RefreshBoard(drawBoard);
                        MessageBox.Show("�����������,\n�ú�������һ����");
                    }
                    else
                    {
                        drawBoard.PaintBuffer();
                        Utils.RefreshBoard(drawBoard);
                        MessageBox.Show("�����������,\n�ð�������һ����");
                    }
                }
                //��ǰ������
                Utils.UpdateMessage( (drawBoard.CurColor== ChessType.BLACK?"�ú���������!":"�ð���������!"));
            }
            else//�������Ҷ�ս
            {
                if (turn == TurnTo.YOU)
                {
                    if (drawBoard.CurColor == ChessType.BLACK)                    
                        Utils.UpdateMessage("�����������,\n�ú�������һ����");
                    else
                        Utils.UpdateMessage("�����������,\n�ð�������һ����");
                }
                else if (turn == TurnTo.ME)
                {
                    ComputerDown();
                    //�ֵ�����(���������Ƿ��������)
                    turn = TurnToWhere(TurnTo.YOU);

                    while (turn == TurnTo.ME)//��������ֵ����Ծ��õ��Լ�����
                    {
                        if (drawBoard.CurColor == ChessType.WHITE)
                            Utils.UpdateMessage("�����������,\n�ú�������һ����");
                        else
                            Utils.UpdateMessage("�����������,\n�ð�������һ����");
                        ComputerDown();
                        //�ֵ�����(���������Ƿ��������)
                        turn = TurnToWhere(TurnTo.YOU);
                    }
                }
            }
            if (turn == TurnTo.GAMEOVER)
            {
                gameIsOver = true;
                drawBoard.PaintBuffer();
                Utils.RefreshBoard(drawBoard);
                Utils.UpdateMessage("��Ϸ����!");
                //StaticMethod.UpdateMessage(
                MessageBox.Show("��Ϸ����!�����:" + drawBoard.GameOverResult((GameMode)Config.Instance.GameMode),"��Ϸ����!");
            }
        }

        /// <summary>
        /// ��Ϸ�Ƿ����
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
        /// ��������
        /// </summary>
        private void ComputerDown()
        {
            int square = VaildMove;
            double speed;
            Utils.UpdateMessage("��������˼��...");
            busy = true;
            if (drawBoard.Empties == 60)
            {//��һ���������
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
                    //��������
                    monkey.Search(board, drawBoard.CurColor);
                    square = monkey.BestMove;
                }
                //��������
                moveList.Add(square);
                AddMoveItem(square, monkey.BestScore);
                speed = (monkey.Time > 0 && monkey.Nodes > 0 ? monkey.Nodes / monkey.Time : Constants.MaxSpeed);
                Utils.UpdateMessage(square, monkey.BestScore, monkey.Nodes, monkey.Time, speed, drawBoard);
            }
            //��������
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
        /// ��������
        /// </summary>
        /// <param name="point">���λ��</param>
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
                    //������������
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
            //��ǰ������
            Utils.UpdateMessage("����������!");
            //�ֵ�����(���������Ƿ��������)
            TurnTo turn = TurnToWhere(TurnTo.YOU);
            if (turn == TurnTo.GAMEOVER)
            {
                gameIsOver = true;
                drawBoard.PaintBuffer();
                Utils.RefreshBoard(drawBoard);
                Utils.UpdateMessage("��Ϸ����!");
                MessageBox.Show("��Ϸ����!�����:" + drawBoard.GameOverResult((GameMode)Config.Instance.GameMode), "��Ϸ����!");               
            }
            if (turn == TurnTo.ME)//��������ֵ����Ծ��õ��Լ�����
            {
                MessageBox.Show("���������,\n�õ�������һ����");
                ComputerDown();
            }
        }

        /// <summary>
        /// ����һ�������ת
        /// </summary>
        /// <returns>��ת</returns>
        private TurnTo TurnToWhere(TurnTo turn)
        {
            if (drawBoard.IsGameOver())
            {
                return TurnTo.GAMEOVER;
            }
            else if (!drawBoard.CanDown())//һ���������,��ת
            {
                drawBoard.CurColor = 2 - drawBoard.CurColor;
                drawBoard.SetCanpuSquare();
                Utils.RefreshBoard(drawBoard);

                //��һ��Ҳ���������,����Ϸ����
                if (!drawBoard.CanDown())
                {
                    drawBoard.CurColor = 2 - drawBoard.CurColor;
                    return TurnTo.GAMEOVER;
                }
                //�����ж���˭û�����
                if (turn == TurnTo.ME) //���Լ��������
                {
                    return TurnTo.YOU;
                }
                else if (turn == TurnTo.YOU)//����ǶԷ��������
                {
                    return TurnTo.ME;
                }
            }
            //������
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
        /// ���̱任
        /// </summary>
        /// <param name="mode"></param>
        public void TranBoard(TransMode mode)
        {
            board = TranBoard(board, mode);
            drawBoard.SetBoardStruct(board);
            drawBoard.SetCanpuSquare();            
        }

        /// <summary>
        /// ��ǰ����ת��Ϊ���Կ���˼��������(��׼��)
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
        /// ����ʹ�ÿ��ֿ�˼����������λ��ת��Ϊ����������λ��(����׼��)
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
        /// �б�ؼ���Ӽ�¼(ί��)
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
        /// �б�ؼ��Ƴ���¼(ί��)
        /// </summary>
        /// <param name="Item"></param>
        private delegate void ListViewRemoveEvent();

        private void ListViewRemoveItem()
        {
            movesListView.Items.RemoveAt(movesListView.Items.Count - 1);
        }

        /// <summary>
        /// ���ӵ��������¼
        /// </summary>
        /// <param name="square">λ��</param>
        /// <param name="eval">�Ʒ�</param>
        private void AddMoveItem(int square, double eval)
        {
            string lastmoveColor = (drawBoard.CurColor == ChessType.WHITE ? "��" : "��");
            ListViewItem Item = new ListViewItem(new string[] { moveList.Count.ToString(), lastmoveColor, Utils.SquareToString(square), String.Format("{0}", Math.Round(eval)) });
            UpdateListView(Item);
        }

        /// <summary>
        /// ������������¼
        /// </summary>
        /// <param name="square">λ��</param>
        private void AddMoveItem(int square)
        {
            string lastmoveColor = (drawBoard.CurColor == ChessType.WHITE ? "��" : "��");
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
