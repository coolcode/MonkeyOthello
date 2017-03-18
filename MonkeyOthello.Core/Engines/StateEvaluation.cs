using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MonkeyOthello.Core;

namespace MonkeyOthello.Engines
{
    /// <summary>
    /// The evaluation based on board state
    /// <see cref="http://radagast.se/othello/Help/strategy.html"/>
    /// </summary>
    public class StateEvaluation : IEvaluation
    {
        public int Eval(BitBoard board)
        {
            var oppBoard = board.Switch();
            var empties = board.EmptyPieces.CountBits();
            //chess count
            var mydisc = board.PlayerPlayerPiecesCount();
            var opdisc = board.OpponentPiecesPiecesCount(); ;
            //mobility
            var mymob = Rule.Mobility(board);
            var opmob = Rule.Mobility(oppBoard);
            //potential mobility
            var mypotmob = Rule.PotentialMobility(board);
            var oppotmob = Rule.PotentialMobility(oppBoard);
            //stable pieces
            var mystab = 0;
            var opstab = 0;

            //unstable: a2,a7,b2,b8,...
            var diffUnstab = Rule.Unstable(board) - Rule.Unstable(oppBoard);
            //edge: a1,a8,h1,h8
            //var myedge = Rule.Edge(board);
            //var opedge = Rule.Edge(oppBoard);
            var diffEdge = Rule.Edge(board) - Rule.Edge(oppBoard);

            var index = empties - 10;

            var eval = mydisc * weights[index, 0] + opdisc * weights[index, 1] +
                 mymob * weights[index, 2] + opmob * weights[index, 3] +
                 mypotmob * weights[index, 4] + oppotmob * weights[index, 5] +
                 mystab * weights[index, 6] + opstab * weights[index, 7] +
                 diffUnstab * weights[index, 8] + diffEdge * weights[index, 9];

            var diffCenterPieces = Rule.CenterPiecesCount(board) - Rule.CenterPiecesCount(oppBoard);

            eval += diffCenterPieces * weight_center;

            eval /= 100;

            return (int)eval;
        }


        #region weight list

        private readonly double[,] weights = new double[,]{
                /*10*/
                {015,   -016,   340,    -260,   029,   -022,    129,    -115,       -355,   300},
                /*11*/
                {015,   -016,   340,    -260,   029,   -022,    129,    -115,       -358,   306},
                 /*12*/
                {015,   -016,   340,    -260,   029,   -022,    129,    -115,       -362,   314},
                /*13*/
                {013,   -014,   330,    -260,   030,   -023,    130,    -121,       -366,   322},
                /*14*/
                {011,   -012,   322,    -259,   031,   -024,    131,    -127,       -370,   334},
                /*15*/
                {009,   -010,   314,    -258,   032,   -025,    132,    -133,       -375,   352},
                /*16*/
                {006,   -005,   306,    -257,   033,   -026,    133,    -138,       -380,   370},
                /*17*/
                {002,   -001,   298,    -256,   034,   -027,    135,    -143,       -388,   394},

                /*18*/
                {-004,  003,    292,    -255,   035,   -028,    137,    -148,       -400,   420},
                /*19*/
                {-008,  005,    288,    -254,   036,   -029,    139,    -152,       -420,   446},
                /*20*/
                {-012,  007,    284,    -253,   037,   -030,    141,    -156,       -440,   472},
                /*21*/
                {-015,  009,    281,    -252,   038,   -031,    143,    -158,       -458,   514},
                /*22*/
                {-019,  011,    278,    -251,   039,   -032,    145,    -160,       -476,   561},
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
                {-044,    030,      260,  -242,     056,   -048,    164,  -164,     -616, 820},
                /*32*/
                {-047,    033,      259,  -241,     058,   -050,    167,  -165,     -630, 862},
                /*33*/
                {-050,    036,      259,  -240,     060,   -052,    170,  -165,     -644, 888},
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
                {-074,    058,      355,  -232,     069,   -061,    140,  -170,     -754, 1028},
                /*42*/
                {-077,    062,      254,  -231,     070,   -062,    136,  -171,     -764, 1043},
                /*43*/
                {-080,    066,      254,  -230,     071,   -063,    133,  -172,     -774, 1058},
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

        private const double weight_center = 24;
    }
}
