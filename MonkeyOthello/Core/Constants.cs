/*----------------------------------------------------------------
          // Copyright (C) 2007 Fengart
          // 版权所有。 
          // 开发者：Fengart
          // 文件名： Constants.cs
          // 文件功能描述：常量类
//----------------------------------------------------------------*/

using System;

namespace MonkeyOthello.Core
{
    /// <summary>
    /// 常量类
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// 最大空位数:64
        /// </summary>
        public const int MaxEmpties = 64;

        /// <summary>
        /// 执行奇偶搜索的空位数:4
        /// </summary>
        public const int Use_Parity = 4;

        /// <summary>
        /// 胜者得空模式（日本的计分法：游戏结束时如果有剩余的空格，就全归为胜者）:1
        /// </summary>
        public const int Winner_Gets_Empties = 1;

        /// <summary>
        /// 最快优先搜索的深度:7
        /// </summary>
        public const int Fastest_First = 7;

        /// <summary>
        /// 最高分:100000000
        /// </summary>
        public const int HighestScore = 100000000;

        ///// <summary>
        ///// 最低分(不可能低于的分):-30000
        ///// </summary>
        //public const int LowestScore = -30000;

        /// <summary>
        /// 棋盘行数
        /// </summary>
        public const int RowsNum = 8;


        #region 各种权值

        /// <summary>
        /// 行动力权值
        /// </summary>
        public const int MobilityWeight = 244;

        /// <summary>
        /// 潜在行动力权值
        /// </summary>
        public const int PotMobilityWeight = 100;

        /// <summary>
        /// 稳定子权值
        /// </summary>
        public const int TwoStabilityWeight = 004, ThreeStabilityWeight = 020, FourStabilityWeight = 120;
       
        /// <summary>
        /// 中心子权值
        /// </summary>
        public const int MiddleWeight = 24;

        /// <summary>
        /// 边角力权值
        /// </summary>
        public const int KeyanoWeight = 800;

        /// <summary>
        /// 不稳定子权值
        /// </summary>
        public const int UnStabilityWeight = -1000;

        /// <summary>
        /// 棋子数的权值
        /// </summary>
        public const int DiscWeight = -120;

        #endregion

        public const int Nbits = 16;

        public const int MaxSpeed = 100000000; 
        
    }
}
