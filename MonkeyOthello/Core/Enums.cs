using System;
using System.Collections.Generic;
using System.Text;

namespace MonkeyOthello.Core
{
    /// <summary>
    /// 棋子类型
    /// </summary>
    public enum ChessType
    {
        /// <summary>
        /// 白子
        /// </summary>
        WHITE,
        
        /// <summary>
        /// 空位
        /// </summary>
        EMPTY,

        /// <summary>
        /// 黑子
        /// </summary>
        BLACK,

        /// <summary>
        /// 边界
        /// </summary>
        DUMMY
    }

    /// <summary>
    /// 电脑AI
    /// </summary>
    public enum ComputerAI
    {
        /// <summary>
        /// 最低级别
        /// </summary>
        LOWEST,
        /// <summary>
        /// 低
        /// </summary>
        LOW,
        /// <summary>
        /// 一般
        /// </summary>
        NORMAL,
        /// <summary>
        /// 高
        /// </summary>
        HIGH,
        /// <summary>
        /// 更高
        /// </summary>
        MOREHIGH,
        /// <summary>
        /// 进化
        /// </summary>
        EVOLUTION
    }

    /// <summary>
    /// 开局模式
    /// </summary>
    public enum OpenType
    {
        /// <summary>
        /// 白黑黑白
        /// </summary>
        WBBW,
        /// <summary>
        /// 黑白白黑
        /// </summary>
        BWWB,
    }

    /// <summary>
    /// 游戏模式
    /// </summary>
    public enum GameMode
    {
        /// <summary>
        /// 电脑对人(电脑先下)
        /// </summary>
        CvsP,
        /// <summary>
        /// 人对电脑(人先下)
        /// </summary>
        PvsC,        
        /// <summary>
        /// 人人对战
        /// </summary>
        PvsP,
        /// <summary>
        /// 电脑自我对战
        /// </summary>
        CvsC
    }

    /// <summary>
    /// 棋盘状态
    /// </summary>
    public enum BoardState
    {
        /// <summary>
        /// 开局
        /// </summary>
        START,
        /// <summary>
        /// 中局
        /// </summary>
        MIDDLE,
        /// <summary>
        /// 终局(胜负平搜索)
        /// </summary>
        WLD,
        /// <summary>
        /// 终局搜索最大赢子数
        /// </summary>
        EXCET,
    }

    enum TransMode
    {
        VER,
        LEV,
        ALG,
        INV,
    }
}
