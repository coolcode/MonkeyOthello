/*----------------------------------------------------------------
          // Copyright (C) 2007 Fengart
//----------------------------------------------------------------*/

using System;

namespace MonkeyOthello.Engines.V2.AI
{     
    class Evalation
    {

        #region weights

        private readonly int[,] weight_train = new int[,]{
                // /*10*/
                //{015,	-016, 	340,	-260,   029,   -022,	129,	-115,	    -300,	165},
                ///*11*/
                //{015,	-016, 	340,	-260,   029,   -022,	129,	-115,	    -300,	165},
                // /*12*/
                //{015,	-016, 	340,	-260,   029,   -022,	129,	-115,	    -300,	165},
                ///*13*/
                //{013,	-014, 	330,	-260,   030,   -023,	130,	-121,	    -300,	170},
                ///*14*/
                //{011,	-012,	322,	-259, 	031,   -024,	131,	-127,	    -320,	234},
                ///*15*/
                //{009,	-010,	314,	-258,	032,   -025,	132,	-133,	    -340,	285},
                ///*16*/
                //{006,	-005, 	306,	-257,	033,   -026,	133,	-138,	    -360,	352},
                ///*17*/
                //{002,	-001, 	298,	-256,	034,   -027,	135,	-143,	    -380,	394},
            /*10*/
                {015,	-016, 	340,	-260,   029,   -022,	129,	-115,	    -355,	300},
                /*11*/
                {015,	-016, 	340,	-260,   029,   -022,	129,	-115,	    -358,	306},
                 /*12*/
                {015,	-016, 	340,	-260,   029,   -022,	129,	-115,	    -362,	314},
                /*13*/
                {013,	-014, 	330,	-260,   030,   -023,	130,	-121,	    -366,	322},
                /*14*/
                {011,	-012,	322,	-259, 	031,   -024,	131,	-127,	    -370,	334},
                /*15*/
                {009,	-010,	314,	-258,	032,   -025,	132,	-133,	    -375,	352},
                /*16*/
                {006,	-005, 	306,	-257,	033,   -026,	133,	-138,	    -380,	370},
                /*17*/
                {002,	-001, 	298,	-256,	034,   -027,	135,	-143,	    -388,	394},

                /*18*/
                {-004,	003,	292,	-255, 	035,   -028,	137,	-148,	    -400,	420},
                /*19*/
                {-008,	005,	288,	-254,   036,   -029,	139,	-152,	    -420,	446},
                /*20*/
                {-012,	007,	284,	-253,	037,   -030,	141,	-156,	    -440,	472},
                /*21*/
                {-015,	009,	281,	-252, 	038,   -031,	143,	-158, 	    -458,	514},
                /*22*/
                {-019,	011,	278,	-251,	039,   -032,	145,	-160,	    -476,	561},
                /*23*/
                {-023,    013 ,     275,  -250,     040,   -033,    147,  -161 ,    -494, 597},
                 /*24*/
                {-026,    015,      272,  -249,     042,   -035,    149,  -161,     -512, 628},
                 /*25*/
                {-028,    017,      269,  -248,     044,   -037,    151,  -161,     -528, 659},
                 /*26*/
                {-030,    019,      267,  -247,     046,   -039,    153,  -162,     -544, 696},
                 /*27*/
                {-033,    021,      265,  -246,     048,   -040,    155,  -162,     -560, 711},
                 /*28*/
                {-036,    023,      263,  -245,     050,   -042,    157,  -163,     -574, 747},
                 /*29*/
                {-038,    025,      262,  -244,     052,   -044,    159,  -163,     -588, 773},
                 /*30*/
                {-041,    028,      261,  -243,     054,   -046,    161,  -164,     -602, 794},
                /*31*/
                {-044,	  030,	    260,  -242, 	056,   -048,	164,  -164,	    -616, 820},
                /*32*/
                {-047,	  033,	    259,  -241,	    058,   -050,	167,  -165,	    -630, 862},
                /*33*/
                {-050,	  036,	    259,  -240,	    060,   -052,	170,  -165,	    -644, 888},
                 /*34*/
                {-053,    039,      258,  -239,     062,   -054,    167,  -166,     -658, 900},
                 /*35*/
                {-056,    042,      258,  -238,     063,   -055,    164,  -166,     -672, 929},
                 /*36*/
                {-059,    045,      257,  -237,     064,   -056,    160,  -167,     -686, 950},
                 /*37*/
                {-062,    048,      257,  -236,     065,   -057,    156,  -167,     -700, 971},
                 /*38*/
                {-065,    050,      256,  -235,     066,   -058,    152,  -168,     -714, 985},
                 /*39*/
                {-068,    052,      256,  -234,     067,   -059,    148,  -168,     -728, 1000},
                 /*40*/
                {-071,    055,      255,  -233,     068,   -060,    144,  -169,     -742, 1014},
                 /*41*/
                {-074,	  058,	    355,  -232,	    069,   -061,	140,  -170,	    -754, 1028},
                /*42*/
                {-077,	  062,	    254,  -231,	    070,   -062,	136,  -171, 	-764, 1043},
                /*43*/
                {-080,	  066,	    254,  -230,	    071,   -063,	133,  -172,	    -774, 1058},
                 /*44*/
                {-083,    070,      253,  -229,     072,   -064,    131,  -173,     -784, 1072},
                 /*45*/
                {-088,    074,      253,  -228,     073,   -065,    130,  -174,     -792, 1087},
                 /*46*/
                {-091,    078,      252,  -227,     074,   -066,    129,  -176,     -800, 1100},
                 /*47*/
                {-095,    083,      251,  -226,     075,   -067,    128,  -178,     -809, 1109},
                 /*48*/
                {-099,    086,      250,  -225,     076,   -068,    127,  -180,     -818, 1118},
                 /*49*/ 
                {-111,    089,      249,  -224,     077,   -069,    126,  -182,     -828, 1128},
                 /*50*/
                {-113,    092,      248,  -223,     078,   -070,    126,  -185,     -942, 1142},
                /*51*/
                {-115,    095,      247,  -222,     079,   -071,    125,  -188,     -956, 1156},
                 /*52*/
                {-116,    098,      246,  -221,     079,   -071,    125,  -190,     -970, 1170},
                 /*53*/
                {-117,    111,      245,  -220,     080,   -072,    124,  -192,     -985, 1185},
                 /*54*/
                {-118,    114,      244,  -219,     080,   -074,    124,  -194,     -1000,1200},

                 /*55*/
                {-119,    116,      243,-218,      080,   -074,    124,  -194,     -1000,1200},
                 /*56*/
                {-120,    118,      242,-217,      080,   -074,    124,  -194,     -1000,1200},
                /*57*/
                {-120,    118,      242,-217,      080,   -074,    124,  -194,     -1000,1200},
                /*58*/
                {-120,    118,      242,-217,      080,   -074,    124,  -194,     -1000,1200},                
        };

        #endregion

        private readonly int[] midBoard = new int[]
            { 21,22,23,24, 
            29,30,31,32,33,34,
            38,39,40,41,42,43,
            47,48,49,50,51,52,
            56,57,58,59,60,61,
             66,67,68,69 }; 

        private const int midBoardLength = 6 * 6-4;
        private const int midGameLength=49;
        private const int WHITE = 0;
        private const int EMPTY = 1;
        private const int BLACK = 2;

        private const int IND_MYDISC = 0;
        private const int IND_OPDISC = 1;
        private const int IND_MYMOB = 2;
        private const int IND_OPMOB = 3;
        private const int IND_MYPOTMOB = 4;
        private const int IND_OPPOTMOB = 5;
        private const int IND_MYSTAB = 6;
        private const int IND_OPSTAB = 7;
        private const int IND_UNSTAB = 8;
        private const int IND_KEYNO = 9;
         
        private readonly int[,] score_edge=new int[6561,midGameLength];
         
        private static bool edge_IsStab(int[] edge, int sq, int color)
        {
            int pt;
            int curcol;
            bool maybe = false;
            if (sq == 7 || sq == 0)
                return true;
            if (edge[7] == color)
            {
                pt = sq;
                do
                {
                    pt++;
                    curcol = edge[pt];
                    if (pt == 7)
                        return true;
                } while (curcol == color);
                if (edge[pt] == EMPTY)
                    return false;
                do
                {
                    if (pt == 7)
                    {
                        maybe = true;
                        break;
                    }
                    pt++;
                } while (edge[pt] != EMPTY);
                if (!maybe)
                    return false;
            }
            if (edge[0] == color)
            {
                pt = sq;
                do
                {
                    pt--;
                    curcol = edge[pt];
                    if (pt == 0)
                        return true;
                } while (curcol == color);
                if (edge[pt] == EMPTY)
                    return false;
                do
                {
                    if (pt == 0)
                    {
                        return maybe;
                    }
                    pt--;
                } while (edge[pt] != EMPTY);
            }
            return false;
        }
 
        private int get_score_edge(int[] edge, int emptiesIndex, int color)
        {
            int score = 0;
            int oppcolor=2-color;

            if ((edge[0] == color) && (edge[7] == color))
            {
                score += weight_train[emptiesIndex, IND_KEYNO];
                for (int i = 1; i <= 6; i++)
                    if (edge[i] == color)
                        score += (edge_IsStab(edge,i,color)?weight_train[emptiesIndex, IND_MYSTAB]:weight_train[emptiesIndex, IND_MYSTAB]/2);
                return score;
            }
            else if ((edge[0] == color) && (edge[7] == EMPTY))
            {
                score += weight_train[emptiesIndex, IND_KEYNO];
                for (int i = 1; i <= 6; i++)
                {
                    if (edge[i] == color)
                        if(edge_IsStab(edge,i,color))
                            score+=weight_train[emptiesIndex, IND_MYSTAB];
                }
                for (int i = 1; i <= 6; i++)
                {
                    if (edge[i] == color)
                            continue;
                    else if (edge[i] == EMPTY)
                    {
                        int emptiesNum = 1;
                        for (int j = i + 1; j <= 6; j++)
                            if (edge[j] == EMPTY)
                                emptiesNum++;
                            else if (edge[j] == color)
                            {
                                if ((emptiesNum & 1) == 0)// 
                                    return score + weight_train[emptiesIndex, IND_MYSTAB] * emptiesNum / 2;
                                return score - weight_train[emptiesIndex, IND_MYSTAB] * emptiesNum / 2;
                            }
                            else
                            {
                                if ((emptiesNum & 1) == 0)// 
                                    return score - weight_train[emptiesIndex, IND_MYSTAB] * emptiesNum / 2;
                                return score + weight_train[emptiesIndex, IND_MYSTAB] * emptiesNum / 3;
                            }
                    }
                    else
                    {
                        int k=i+1;
                        for (int j = i + 1; j <= 6; j++)
                            if (edge[j] == EMPTY)
                                return score;
                            else if (edge[j] == color)
                            {
                                k=j+1;
                                break;
                            }
                        for (; k <= 6; k++)
                            if (edge[k] != color)
                                return score;
                        if(edge[6]==color)
                            return score - weight_train[emptiesIndex, IND_MYSTAB] * (6-i+1)/ 2;
                    }
                }                   
                return score;
            }
            else if ((edge[7] == color) && (edge[0] == EMPTY))
            {
                score += weight_train[emptiesIndex, IND_KEYNO] ;
                for (int i = 6; i <= 1; i--)
                {
                     if (edge[i] == color)
                        if(edge_IsStab(edge,i,color))
                            score+=weight_train[emptiesIndex, IND_MYSTAB];
                }
                for (int i = 6; i <= 1; i--)
                {
                    if (edge[i] == color)
                            continue;
                    else if (edge[i] == EMPTY)
                    {
                        int emptiesNum = 1;
                        for (int j = i - 1; j >=1; j--)
                            if (edge[j] == EMPTY)
                                emptiesNum++;
                            else if (edge[j] == color)
                            {
                                if ((emptiesNum & 1) == 0)// 
                                    return score + weight_train[emptiesIndex, IND_MYSTAB] * emptiesNum / 2;
                                return score - weight_train[emptiesIndex, IND_MYSTAB] * emptiesNum / 2;
                            }
                            else
                            {
                                if ((emptiesNum & 1)== 0)// 
                                    return score - weight_train[emptiesIndex, IND_MYSTAB] * emptiesNum / 2;
                                return score + weight_train[emptiesIndex, IND_MYSTAB] * emptiesNum / 2;
                            }
                    }
                    else
                    {
                        int k = i - 1;
                        for (int j = i - 1; j >=1; j--)
                            if (edge[j] == EMPTY)
                                return score;
                            else if (edge[j] == color)
                            {
                                k = j - 1;
                                break;
                            }
                        for (; k >=1; k--)
                            if (edge[k] != color)
                                return score;
                        if (edge[1] == color)
                            return score - weight_train[emptiesIndex, IND_MYSTAB] * (i + 1) / 2;
                    }
                }
                return score;
            }
            else if ((edge[0] == color) && (edge[7] == oppcolor))
            {
                for (int i = 1; i <= 6; i++)
                {
                    if (edge[i] == color)
                    {
                        if (edge_IsStab(edge, i, color))
                            score += weight_train[emptiesIndex, IND_MYSTAB];
                    }
                    else if (edge[i] == oppcolor)
                    {
                        if (edge_IsStab(edge, i, oppcolor))
                            score += weight_train[emptiesIndex, IND_OPSTAB];
                    }
                }
                for (int i = 1; i <= 6; i++)
                {
                    if (edge[i] == oppcolor)
                    {
                        if(i>=1 && i<=4)
                            if(edge[i+1]==color&&edge[5]==color && edge[6]==EMPTY)
                                return score - weight_train[emptiesIndex, IND_MYSTAB] * (6-i) / 2;
                    }
                    else if (edge[i] == EMPTY)
                    {
                        int k = i + 1;
                        if (edge[k] == EMPTY || edge[k] == oppcolor)
                            return score;
                        else if (edge[k] == color)
                        {
                            int j = k;
                            for (; j <= 6; j++)
                                if (edge[j] != color)
                                    break;
                            return score - weight_train[emptiesIndex, IND_MYSTAB] * (j - k + 1) / 2;

                        }
                    }
                }
                return score;
            }
            else if ((edge[0] == EMPTY) && (edge[7] == EMPTY))
            {
                int  hold = 0;
                if ((edge[1] == color) && (edge[6] == color))
                    return weight_train[emptiesIndex, IND_UNSTAB] / 2;
                else if ((edge[1] == color) && (edge[6] == EMPTY))
                {
                    score += weight_train[emptiesIndex, IND_UNSTAB] / 4;
                    for (int i = 2; i <= 5; i++)
                    {
                        if (edge[i] == color)
                        {
                            continue;
                        }
                        if (edge[i] == oppcolor)
                        {
                            int j= i + 1;
                            if (i < 4)
                            {
                                while (j <= 4 && edge[j] == oppcolor)
                                    j++;
                            }
                            if (j <= 5 && edge[j]== color)
                                return weight_train[emptiesIndex, IND_UNSTAB] / 2;
                            if (j == 6)
                                return weight_train[emptiesIndex, IND_UNSTAB] / 2;
                        }
                        else
                            return score;
                    }
                    return score;
                }
                else if ((edge[6] == color) && (edge[1] == EMPTY))
                {
                    score += weight_train[emptiesIndex, IND_UNSTAB] / 4;
                    for (int i = 5; i <= 2; i--)
                    {
                        if(edge[i] == color)
                        {
                            continue;
                        }
                        if (edge[i] == oppcolor)
                        {
                            int j=i - 1;
                            if (i > 3)
                            {
                                while (j >= 3 && edge[j] == oppcolor)
                                    j--;
                            }
                            if (j >=2 && edge[j] == color)
                                return weight_train[emptiesIndex, IND_UNSTAB] / 2;
                            if (j ==1)
                                return weight_train[emptiesIndex, IND_UNSTAB] / 2;
                        }
                        else 
                           return score;
                    }
                    return score;
                }
                else if ((edge[1] == color) && (edge[6] == oppcolor))
                {
                }
                else
                {
                    if (edge[2] == color)
                        score += weight_train[emptiesIndex, IND_MYSTAB] / 3;
                    else
                        hold++;
                    if (edge[3] == color)
                        score += weight_train[emptiesIndex, IND_MYSTAB] / 4;
                    else
                        hold++;
                    if (edge[4] == color)
                        score += weight_train[emptiesIndex, IND_MYSTAB] / 4;
                    else
                        hold++;
                    if (edge[5] == color)
                        score += weight_train[emptiesIndex, IND_MYSTAB] / 3;
                    else
                        hold++;
                    //if ((hold & 1) != 0)//
                    //    score -= weight_train[emptiesIndex, IND_UNSTAB] / 4;
                }
            }
            return score;
        }
         
        private int get_score_edge_all(ChessType[] board,ChessType color,int empties)
        {
            int score = 0;
            int index = (int)board[10] * 2187 + (int)board[11] * 729 + (int)board[12] * 243 +
                  (int)board[13] * 81 + (int)board[14] * 27 + (int)board[15] * 9 + (int)board[16] * 3 + (int)board[17];
            score += score_edge[index, empties];

            index = (int)board[10] * 2187 + (int)board[19] * 729 + (int)board[28] * 243 +
                  (int)board[37] * 81 + (int)board[46] * 27 + (int)board[55] * 9 + (int)board[64] * 3 + (int)board[73];
            score += score_edge[index, empties];

            index = (int)board[17] * 2187 + (int)board[26] * 729 + (int)board[35] * 243 +
                  (int)board[44] * 81 + (int)board[53] * 27 + (int)board[62] * 9 + (int)board[71] * 3 + (int)board[80];
            score += score_edge[index, empties];

            index = (int)board[73] * 2187 + (int)board[74] * 729 + (int)board[75] * 243 +
                  (int)board[76] * 81 + (int)board[77] * 27 + (int)board[78] * 9 + (int)board[79] * 3 + (int)board[80];
            score += score_edge[index, empties];

            return score;
            
        }

        public Evalation()
        {
            Initial();
        }
        
        private void Initial()
        {
            int[]edge=new int[8];
            int index;
            for (int i1 = 0; i1 <= 2; i1++)
            {
                edge[0] = i1;
                for (int i2 = 0; i2 <= 2; i2++)
                {
                    edge[1] = i2;
                    for (int i3 = 0; i3 <= 2; i3++)
                    {
                        edge[2] = i3;
                        for (int i4 = 0; i4 <= 2; i4++)
                        {
                            edge[3] = i4;
                            for (int i5 = 0; i5 <= 2; i5++)
                            {
                                edge[4] = i5;
                                for (int i6 = 0; i6 <= 2; i6++)
                                {
                                    edge[5] = i6;
                                    for (int i7 = 0; i7 <= 2; i7++)
                                    {
                                        edge[6] = i7;
                                        for (int i8 = 0; i8 <= 2; i8++)
                                        {
                                            edge[7] = i8;
                                            index = i1 * 2187 + i2 * 729 + i3 * 243 +
                                                i4 * 81 + i5 * 27 + i6 * 9 + i7 * 3 + i8;
                                            for (int k = 0; k < midGameLength; k++)
                                            {
                                                score_edge[index, k] = get_score_edge(edge, k,BLACK)-get_score_edge(edge,k,WHITE);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

         
        public int StaEval2(ChessType[] board, ChessType color, ChessType oppcolor, int empties, Empties EmHead)
        {
            int eval = 0;
            //disc
            int mydisc = 0;
            int opdisc = 0;
            //mobility
            int mymob = 0;
            int opmob = 0;
            //
            int mypotmob = 0;
            int oppotmob = 0;
            //stable
            int mystab = 0;
            int opstab = 0;

            //corners
            int deltakeyano = 0;
            //unstable
            int unstab = 0;
            int sqnum;
            int index = empties - 10;
            Empties em = EmHead.Succ;
            for (; em != null; em = em.Succ)
            {
                sqnum = em.Square;
                if (RuleUtils.AnyFlips(board, sqnum, color, oppcolor))
                    mymob++;
                else if (RuleUtils.AnyPotMobility(board, sqnum, oppcolor))
                    mypotmob++;
                if (RuleUtils.AnyFlips(board, sqnum, oppcolor, color))
                    opmob++;
                else if (RuleUtils.AnyPotMobility(board, sqnum, color))
                    oppotmob++;
            }

            for (int i = 0; i < midBoardLength; i++)
            {
                sqnum = midBoard[i];
                if (board[sqnum] == color)
                {
                    mydisc++;
                    if (RuleUtils.AnyStab(board, sqnum, color))
                        mystab++;
                }
                else if (board[sqnum] == oppcolor)
                {
                    opdisc++;
                    if (RuleUtils.AnyStab(board, sqnum, oppcolor))
                        opstab++;
                }
            }

            deltakeyano = getDeltaKeyano(board, color, oppcolor);
            unstab = getUnStab(board, color) - getUnStab(board, oppcolor);

            eval = mydisc * weight_train[index, 0] + opdisc * weight_train[index, 1] +
                mymob * weight_train[index, 2] + opmob * weight_train[index, 3] +
                mypotmob * weight_train[index, 4] + oppotmob * weight_train[index, 5] +
                mystab * weight_train[index, 6] + opstab * weight_train[index, 7] +
                unstab * weight_train[index, 8] + deltakeyano * weight_train[index, 9];
            eval +=get_score_edge_all(board, color, index) / 4;

            if (board[40] == color)
                eval += Constants.MiddleWeight;
            else if (board[40] == oppcolor)
                eval -= Constants.MiddleWeight;
            if (board[41] == color)
                eval += Constants.MiddleWeight;
            else if (board[41] == oppcolor)
                eval -= Constants.MiddleWeight;
            if (board[49] == color)
                eval += Constants.MiddleWeight;
            else if (board[49] == oppcolor)
                eval -= Constants.MiddleWeight;
            if (board[50] == color)
                eval += Constants.MiddleWeight;
            else if (board[50] == oppcolor)
                eval -= Constants.MiddleWeight;
            return eval;
        }
 
        public int MidEval2(ChessType[] board, ChessType color, ChessType oppcolor, int empties,Empties EmHead)
        {
            int eval = 0;
            // 
            int mydisc = 0;
            int opdisc = 0;
            // 
            int mymob = 0;
            int opmob = 0;
            // 
            int mypotmob = 0;
            int oppotmob = 0;
            // 
            int mystab = 0;
            int opstab = 0;

            // 
             int deltakeyano = 0;
            // 
            int unstab = 0;
            int sqnum;
            int index = empties - 10;
            Empties em=EmHead.Succ;
            for (; em != null; em = em.Succ)
            {
                sqnum = em.Square;
                if (RuleUtils.AnyFlips(board, sqnum, color, oppcolor))
                    mymob++;
                else if (RuleUtils.AnyPotMobility(board, sqnum, oppcolor))
                    mypotmob++;
                if (RuleUtils.AnyFlips(board, sqnum, oppcolor, color))
                    opmob++;
                else if (RuleUtils.AnyPotMobility(board, sqnum, color))
                    oppotmob++;
            }

            for (int i = 0; i <midBoardLength; i++)
            {
                sqnum = midBoard[i];
                if (board[sqnum] == color)
                {
                    mydisc++;
                    if (RuleUtils.AnyStab(board, sqnum, color))
                        mystab++;
                }
                else if (board[sqnum] == oppcolor)
                {
                    opdisc++;
                    if (RuleUtils.AnyStab(board, sqnum, oppcolor))
                        opstab++;
                }
            }

            deltakeyano = getDeltaKeyano(board, color, oppcolor);
            unstab = getUnStab(board, color) - getUnStab(board, oppcolor);

            eval = mydisc * weight_train[index, 0] + opdisc * weight_train[index, 1] +
                mymob * weight_train[index, 2] + opmob * weight_train[index, 3] +
                mypotmob * weight_train[index, 4] + oppotmob * weight_train[index, 5] +
                mystab * weight_train[index, 6] + opstab * weight_train[index, 7] +
                unstab * weight_train[index, 8] +deltakeyano * weight_train[index, 9];
            eval +=get_score_edge_all(board, color, index) /3;

            return eval;
        }
         
        private static int getDeltaKeyano(ChessType[] board, ChessType color, ChessType oppcolor)
        {
            int num = 0;
            if (board[10] == color)
                num++;
            else if (board[10] == oppcolor)
                num--;
            if (board[17] == color)
                num++;
            else if (board[17] == oppcolor)
                num--;
            if (board[73] == color)
                num++;
            else if (board[73] == oppcolor)
                num--;
            if (board[80] == color)
                num++;
            else if (board[80] == oppcolor)
                num--;
            return num;
        }
 
        private static int getUnStab(ChessType[] board, ChessType color)
        {
            int num = 0;
            int half = 0;
            //if (board[10] != color && board[11] == color && (board[17] != color || !(board[12] == color && board[13] == color && board[14] == color && board[15] == color && board[16] == color)))
            //    num++;
            //if (board[17] != color && board[16] == color && (board[10] != color || !(board[11] == color && board[12] == color && board[13] == color && board[14] == color && board[15] == color)))
            //    num++;
            //if (board[10] != color && board[19] == color && (board[73] != color || !(board[28] == color && board[37] == color && board[46] == color && board[55] == color && board[64] == color)))
            //    num++;
            //if (board[20] == color)
            //    num++;
            //if (board[25] == color)
            //    num++;
            //if (board[65] == color)
            //    num++;
            //if (board[70] == color)
            //    num++;
            //return num;
            if (board[10] != color && board[20] == color)
            {
                if ((board[80] == color) && (board[30] == color && board[40] == color && board[50] == color && board[60] == color && board[70] == color))
                    half++;
                else
                    num++;
            }
            if (board[17] != color && board[25] == color)
            {
                if ((board[73] == color) && (board[33] == color && board[41] == color && board[49] == color && board[57] == color && board[65] == color))
                    half++;
                else
                    num++;
            }
            //if (board[17] != color && board[26] == color && (board[80] != color || !(board[35] == color && board[44] == color && board[53] == color && board[62] == color && board[71] == color)))
            //    num++;
            //if (board[73] != color && board[64] == color && (board[10] != color || !(board[19] == color && board[28] == color && board[37] == color && board[46] == color && board[55] == color)))
            //    num++;
            if (board[73] != color && board[65] == color)
            {
                if ((board[17] == color) && (board[25] == color && board[33] == color && board[41] == color && board[49] == color && board[57] == color))
                    half++;
                else
                    num++;
            }
            if (board[80] != color && board[70] == color)
            {
                if ((board[10] == color) && (board[20] == color && board[30] == color && board[40] == color && board[50] == color && board[60] == color))
                    half++;
                else
                    num++;
            }
            //if (board[80] != color && board[71] == color && (board[17] != color || !(board[26] == color && board[35] == color && board[44] == color && board[53] == color && board[62] == color)))
            //    num++;
            //if (board[73] != color && board[74] == color && (board[80] != color || !(board[75] == color && board[76] == color && board[77] == color && board[78] == color && board[79] == color)))
            //    num++;
            //if (board[80] != color && board[79] == color && (board[73] != color || !(board[74] == color && board[75] == color && board[76] == color && board[77] == color && board[78] == color)))
            //    num++;
            return num + num +num+ half;
        }
 
        private static int getUnStab_Start(ChessType[] board, ChessType color)
        {
            int num = 0;
            if (board[10] != color && board[11] == color && (board[17] != color || !(board[12] == color && board[13] == color && board[14] == color && board[15] == color && board[16] == color)))
                num++;
            if (board[17] != color && board[16] == color && (board[10] != color || !(board[11] == color && board[12] == color && board[13] == color && board[14] == color && board[15] == color)))
                num++;
            if (board[10] != color && board[19] == color && (board[73] != color || !(board[28] == color && board[37] == color && board[46] == color && board[55] == color && board[64] == color)))
                num++;
            if (board[10] != color && board[20] == color && (board[80] != color || !(board[30] == color && board[40] == color && board[50] == color && board[60] == color && board[70] == color)))
                num++;
            if (board[17] != color && board[25] == color && (board[73] != color || !(board[33] == color && board[41] == color && board[49] == color && board[57] == color && board[65] == color)))
                num++;
            if (board[17] != color && board[26] == color && (board[80] != color || !(board[35] == color && board[44] == color && board[53] == color && board[62] == color && board[71] == color)))
                num++;
            if (board[73] != color && board[64] == color && (board[10] != color || !(board[19] == color && board[28] == color && board[37] == color && board[46] == color && board[55] == color)))
                num++;
            if (board[73] != color && board[65] == color && (board[17] != color || !(board[25] == color && board[33] == color && board[41] == color && board[49] == color && board[57] == color)))
                num++;
            if (board[80] != color && board[70] == color && (board[10] != color || !(board[20] == color && board[30] == color && board[40] == color && board[50] == color && board[60] == color)))
                num++;
            if (board[80] != color && board[71] == color && (board[17] != color || !(board[26] == color && board[35] == color && board[44] == color && board[53] == color && board[62] == color)))
                num++;
            if (board[73] != color && board[74] == color && (board[80] != color || !(board[75] == color && board[76] == color && board[77] == color && board[78] == color && board[79] == color)))
                num++;
            if (board[80] != color && board[79] == color && (board[73] != color || !(board[74] == color && board[75] == color && board[76] == color && board[77] == color && board[78] == color)))
                num++;
            return num;
        }
        
    }
}
