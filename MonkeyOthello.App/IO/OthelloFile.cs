using System;
using System.Collections.Generic; 


namespace MonkeyOthello.IO
{
    /// <summary>
    /// �����ļ���
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
        /// ����
        /// </summary>
        public ChessType[] Board
        {
            get { return board; }
            set { board = value; }
        }

        /// <summary>
        /// ��ǰ��������巽��ɫ
        /// </summary>
        public ChessType CurColor
        {
            get { return curColor; }
            set { curColor = value; }
        }

        /// <summary>
        /// ��Ϸ����ģʽ
        /// </summary>
        public OpenType StartMode
        {
            get { return startMode; }
            set { startMode = value; }
        }

        /// <summary>
        /// ��Ϸ�Ծ�ģʽ
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
        /// ��ʷ�����¼
        /// </summary>
        public Stack<DrawBoard.Move> HistoryMove
        {
            get { return historyMoveList; }
            set { historyMoveList = value; }
        }


        public OthelloFile()
        {
            //Ĭ������
            //monkeyAI = (ComputerAI) MonkeyOthello.Properties.Settings.Default.MonkeyAI;
            startMode = (OpenType)MonkeyOthello.Properties.Settings.Default.OpenType;
            gameMode = (GameMode)MonkeyOthello.Properties.Settings.Default.GameMode;
            historyMoveList = new Stack<DrawBoard.Move>();
        }
    }
}
