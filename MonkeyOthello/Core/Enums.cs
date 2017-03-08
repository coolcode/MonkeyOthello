using System;
using System.Collections.Generic;
using System.Text;

namespace MonkeyOthello.Core
{
    /// <summary>
    /// ��������
    /// </summary>
    public enum ChessType
    {
        /// <summary>
        /// ����
        /// </summary>
        WHITE,
        
        /// <summary>
        /// ��λ
        /// </summary>
        EMPTY,

        /// <summary>
        /// ����
        /// </summary>
        BLACK,

        /// <summary>
        /// �߽�
        /// </summary>
        DUMMY
    }

    /// <summary>
    /// ����AI
    /// </summary>
    public enum ComputerAI
    {
        /// <summary>
        /// ��ͼ���
        /// </summary>
        LOWEST,
        /// <summary>
        /// ��
        /// </summary>
        LOW,
        /// <summary>
        /// һ��
        /// </summary>
        NORMAL,
        /// <summary>
        /// ��
        /// </summary>
        HIGH,
        /// <summary>
        /// ����
        /// </summary>
        MOREHIGH,
        /// <summary>
        /// ����
        /// </summary>
        EVOLUTION
    }

    /// <summary>
    /// ����ģʽ
    /// </summary>
    public enum OpenType
    {
        /// <summary>
        /// �׺ںڰ�
        /// </summary>
        WBBW,
        /// <summary>
        /// �ڰװ׺�
        /// </summary>
        BWWB,
    }

    /// <summary>
    /// ��Ϸģʽ
    /// </summary>
    public enum GameMode
    {
        /// <summary>
        /// ���Զ���(��������)
        /// </summary>
        CvsP,
        /// <summary>
        /// �˶Ե���(������)
        /// </summary>
        PvsC,        
        /// <summary>
        /// ���˶�ս
        /// </summary>
        PvsP,
        /// <summary>
        /// �������Ҷ�ս
        /// </summary>
        CvsC
    }

    /// <summary>
    /// ����״̬
    /// </summary>
    public enum BoardState
    {
        /// <summary>
        /// ����
        /// </summary>
        START,
        /// <summary>
        /// �о�
        /// </summary>
        MIDDLE,
        /// <summary>
        /// �վ�(ʤ��ƽ����)
        /// </summary>
        WLD,
        /// <summary>
        /// �վ��������Ӯ����
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
