/*----------------------------------------------------------------
          // Copyright (C) 2007 Fengart
          // ��Ȩ���С� 
          // �����ߣ�Fengart
          // �ļ����� Constants.cs
          // �ļ�����������������
//----------------------------------------------------------------*/

using System;

namespace MonkeyOthello.Core
{
    /// <summary>
    /// ������
    /// </summary>
    public class Constants
    {
        /// <summary>
        /// ����λ��:64
        /// </summary>
        public const int MaxEmpties = 64;

        /// <summary>
        /// ִ����ż�����Ŀ�λ��:4
        /// </summary>
        public const int Use_Parity = 4;

        /// <summary>
        /// ʤ�ߵÿ�ģʽ���ձ��ļƷַ�����Ϸ����ʱ�����ʣ��Ŀո񣬾�ȫ��Ϊʤ�ߣ�:1
        /// </summary>
        public const int Winner_Gets_Empties = 1;

        /// <summary>
        /// ����������������:7
        /// </summary>
        public const int Fastest_First = 7;

        /// <summary>
        /// ��߷�:100000000
        /// </summary>
        public const int HighestScore = 100000000;

        ///// <summary>
        ///// ��ͷ�(�����ܵ��ڵķ�):-30000
        ///// </summary>
        //public const int LowestScore = -30000;

        /// <summary>
        /// ��������
        /// </summary>
        public const int RowsNum = 8;


        #region ����Ȩֵ

        /// <summary>
        /// �ж���Ȩֵ
        /// </summary>
        public const int MobilityWeight = 244;

        /// <summary>
        /// Ǳ���ж���Ȩֵ
        /// </summary>
        public const int PotMobilityWeight = 100;

        /// <summary>
        /// �ȶ���Ȩֵ
        /// </summary>
        public const int TwoStabilityWeight = 004, ThreeStabilityWeight = 020, FourStabilityWeight = 120;
       
        /// <summary>
        /// ������Ȩֵ
        /// </summary>
        public const int MiddleWeight = 24;

        /// <summary>
        /// �߽���Ȩֵ
        /// </summary>
        public const int KeyanoWeight = 800;

        /// <summary>
        /// ���ȶ���Ȩֵ
        /// </summary>
        public const int UnStabilityWeight = -1000;

        /// <summary>
        /// ��������Ȩֵ
        /// </summary>
        public const int DiscWeight = -120;

        #endregion

        public const int Nbits = 16;

        public const int MaxSpeed = 100000000; 
        
    }
}
