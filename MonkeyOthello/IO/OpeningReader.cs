/*----------------------------------------------------------------
          // Copyright (C) 2007 Fengart
          // 版权所有。 
          // 开发者：Fengart
          // 文件名：OpeningReader.cs
          // 文件功能描述：开局库读取类
//----------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.IO;

namespace MonkeyOthello.AI
{
    /// <summary>
    /// 开局库读取类
    /// </summary>
    class OpeningReader
    {
        private readonly int[] worst2best = new int[]
        {
            //前面的注释表示是与该位置类似的位置
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

        /// <summary>
        /// 棋盘的数组大小（91）
        /// </summary>
        private const int BoardSize = 91;
        private const int NoMove = 0;

        /// <summary>
        /// 棋子散列码（索引1：棋盘位置；索引2：棋子颜色（0代表白色，1代表黑色）；）
        /// </summary>
        private long[, ,] hash_code_set_disc = new long[BoardSize, 2, 2];
        private long[] hash_code_swap_player = new long[2];

        /// <summary>
        /// 开局库（32位键代表棋盘散列码，16位值（前8位代表估值[-128,127]，后8位代表下棋位置）
        /// </summary>
        private Dictionary<int, short> book;

        /// <summary>
        /// 作为随机散列码的辅助参数
        /// </summary>
        private long[] x = new long[3] { 0xe66d, 0xdeec, 0x5 };

        private int bestmove;
        private int bestscore;

        /// <summary>
        /// 开局库搜索的最好下棋位置
        /// </summary>
        public int Bestmove
        {
            get { return bestmove; }
        }

        /// <summary>
        /// 开局库搜索的下棋估值
        /// </summary>
        public int Bestscore
        {
            get { return bestscore; }
        }

        public OpeningReader()
        {
            hash_init(Constants.Nbits);
            
        }

        /// <summary>
        /// 随机化散列码
        /// </summary>
        /// <returns></returns>
        private long hash_random()
        {
            long MASK = 0x0000ffff;
            long[] A = { 0xe66d, 0xdeec, 0x5 };
            long B = 0xB;
            long[] product = new long[3];
            product[0] = A[0] * x[0] + B;
            product[1] = A[1] * x[0] + (product[0] >> 16);
            product[2] = A[1] * x[1] + A[0] * x[2] + A[2] * x[0] + (product[1] >> 16);
            product[1] = A[0] * x[1] + (product[1] & MASK);
            product[2] += (product[1] >> 16);
            x[0] = (product[0] & MASK);
            x[1] = (product[1] & MASK);
            x[2] = (product[2] & MASK);
            return x[1] + (x[2] << 16);
        }

        /// <summary>
        /// 初始化散列码
        /// </summary>
        /// <param name="hash_table"></param>
        /// <param name="n_bits"></param>
        private void hash_init(int n_bits)
        {
            int i, j;
            long[] hash_mask = new long[2];
            long size = 1 << n_bits;
            hash_mask[0] = size - 1;
            hash_mask[1] = 0xffffffff;

            for (j = 0; j < 2; j++)
            {
                hash_code_swap_player[j] = (hash_random() & hash_mask[j]);
                for (i = 0; i < 91; i++)
                {
                    hash_code_set_disc[i, 0, j] = (hash_random() & hash_mask[j]);
                    hash_code_set_disc[i, 1, j] = (hash_random() & hash_mask[j]);
                    hash_code_set_disc[i, 0, j] ^= hash_code_swap_player[j];
                    hash_code_set_disc[i, 1, j] ^= hash_code_swap_player[j];
                }
            }
        }

        /// <summary>
        /// 获取棋盘散列码
        /// </summary>
        /// <returns></returns>
        private int getHashCode(ChessType[] board, ChessType color)
        {
            int sqnum;
            //棋盘散列码
            long[] hash_code_board = new long[2];
            for (int i = 0; i < Constants.MaxEmpties; i++)
            {
                sqnum = worst2best[i];
                if (board[sqnum] == ChessType.WHITE)
                {
                    hash_code_board[0] ^= hash_code_set_disc[sqnum, 0, 0];
                    hash_code_board[1] ^= hash_code_set_disc[sqnum, 0, 1];
                }
                else if (board[sqnum] == ChessType.BLACK)
                {
                    hash_code_board[0] ^= hash_code_set_disc[sqnum, 1, 0];
                    hash_code_board[1] ^= hash_code_set_disc[sqnum, 1, 1];
                }
            }
            if (color == ChessType.WHITE)
            {
                hash_code_board[0] ^= hash_code_swap_player[0];
                hash_code_board[1] ^= hash_code_swap_player[1];
            }
            return (int)(hash_code_board[0] + hash_code_board[1] << 16);
        }

        /// <summary>
        /// 读取开局库(初始化)
        /// </summary>
        public bool InitialRead(string bookname)
        {
            int nbits = Constants.Nbits;
            FileStream fileInput = null;
            BinaryReader reader = null;
            try
            {
                if (!File.Exists(bookname))// 是否存在开局库的文件
                    return false;
                fileInput = new FileStream(bookname, FileMode.Open, FileAccess.Read);
                reader = new BinaryReader(fileInput);
                int booksize = 1 << nbits;
                //book = new Dictionary<int,short>(booksize);
                book = new Dictionary<int, short>();
                for (int i = 0; i < booksize; i++)
                {
                    int hash = reader.ReadInt32();
                    short move = reader.ReadInt16();
                    if (book.ContainsKey(hash))
                    {
                        //System.Windows.Forms.MessageBox.Show("存在相同的记录！");
                    }
                    else
                    {
                        book.Add(hash, move);
                    }
                }
            }
            catch// (IOException exp)
            {
                //  System.Windows.Forms.MessageBox.Show(exp.ToString());
            }
            if (fileInput != null)
                fileInput.Close();
            if (reader != null)
                reader.Close();
            return true;
        }

        /// <summary>
        /// 从开局库里找当前局面,找到返回对应的值,否则返回0;
        /// </summary>
        /// <param name="board"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        private short getBookValue(ChessType[] board, ChessType color)
        {
            short bookValue = 0;
            if (book == null)
                return 0 ;
            int hash = getHashCode(board, color);
            if (book.ContainsKey(hash))
            {
                bookValue = book[hash];
            }
            return bookValue;
        }

        
        /// <summary>
        /// 搜索开局库，如果找到当前局面的最佳下棋位置返回true，否则false;
        /// </summary>
        /// <param name="board"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public bool Search(ChessType[] board, ChessType color)
        {
            short value = getBookValue(board, color);
            if (value == 0)
                return false;
            bestmove = value % 256;
            if (bestmove >= 10 && bestmove <= 80 && board[bestmove] == ChessType.EMPTY &&
                Board.AnyFlips(board, bestmove, color, 2 - color))
            {
                bestscore = value >> 8 - 64;
                if (bestscore >= 64)
                    bestscore -= 64;
                return true;
            }
            return false;
        }

       
    }

}
