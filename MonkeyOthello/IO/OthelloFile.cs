using System;
using System.Collections.Generic; 


namespace MonkeyOthello.IO
{
    /// <summary>
    /// 配置文件类
    /// </summary>
    [Serializable]
    class OthelloFile
    {
        private ChessType[] board;
        private ChessType curColor; 
        private OpenType startMode;
        private GameMode gameMode;
        private Stack<DrawBoard.Move> historyMoveList;
        private List<string> historyMoveItems;

        /// <summary>
        /// 棋盘
        /// </summary>
        public ChessType[] Board
        {
            get { return board; }
            set { board = value; }
        }

        /// <summary>
        /// 当前局面的下棋方颜色
        /// </summary>
        public ChessType CurColor
        {
            get { return curColor; }
            set { curColor = value; }
        }

        /// <summary>
        /// 游戏开局模式
        /// </summary>
        public OpenType StartMode
        {
            get { return startMode; }
            set { startMode = value; }
        }

        /// <summary>
        /// 游戏对局模式
        /// </summary>
        public GameMode GameMode
        {
            get { return gameMode; }
            set { gameMode = value; }
        }

        public List<string> HistoryMoveItems
        {
            get { return historyMoveItems; }
            set { historyMoveItems = value; }
        }

        /// <summary>
        /// 历史下棋记录
        /// </summary>
        public Stack<DrawBoard.Move> HistoryMove
        {
            get { return historyMoveList; }
            set { historyMoveList = value; }
        }


        public OthelloFile()
        {
            //默认设置
            //monkeyAI = (ComputerAI) MonkeyOthello.Properties.Settings.Default.MonkeyAI;
            startMode = (OpenType)MonkeyOthello.Properties.Settings.Default.OpenType;
            gameMode = (GameMode)MonkeyOthello.Properties.Settings.Default.GameMode;
            historyMoveList = new Stack<DrawBoard.Move>();
        }
    }
}
