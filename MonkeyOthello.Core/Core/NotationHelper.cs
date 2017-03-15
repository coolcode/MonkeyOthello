using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MonkeyOthello.Core
{
	public static class NotationHelper
	{
        public static int? ToIndex(this string algebraicNotation)
        {
			if (string.IsNullOrEmpty(algebraicNotation))
				return null;
			
            var charArray = algebraicNotation.ToCharArray();
            var column = int.Parse(((char)(charArray[0] - 48)).ToString());
            var row = int.Parse(charArray[1].ToString());

            if (column < 1 || column > 8 || row < 1 || row > 8)
                throw new Exception();

            var x = row - 1;
            var y = column - 1;

            return (x * 8 + y);
        }

        public static ulong ToBitBoard(this int index)
        {
            return 1UL << index;
        }

        public static ulong ToBitBoard(this string algebraicNotation)
        {
            if (string.IsNullOrEmpty(algebraicNotation))
                throw new Exception("String is null or empty.");

            var index = (int)algebraicNotation.ToIndex();

            return index.ToBitBoard();
        }

        public static string ToAlgebraicNotation(this int? playIndex)
        {
			if (playIndex == null)
				return "";
				
            if (playIndex < 0 || playIndex > 63)
                throw new Exception();

            var column = playIndex / 8 + 1;
            var row = playIndex % 8 + 1;

            return ((char)(row + 96)).ToString() + column;
        }
		
        public static string ToAlgebraicNotation(this int playIndex)
        {
            return ((int?) playIndex).ToAlgebraicNotation();
        }
 
        public static List<string> ToAlgebraicNotationList(this ulong bitBoard)
        {
            return bitBoard.Indices().Select(x => x.ToAlgebraicNotation()).ToList();
        } 

		public static Point ToCartesianCoordinate(this int index)
		{
			var x = index % 8;
            var y = index / 8;
			return new Point(x, y);
		}

        public static string ToAlgebraicNotation(this byte play)
        {
            var chars = play.ToString().ToCharArray();
            return ((char)(chars[0] + 48)).ToString() + chars[1];
        }
		
		public static char ToChar(this int index)
		{
			return ((char)(index + 48));
		}

        public static int ToIndex(this char character)
        {
            return (int)(Convert.ToInt16(character) - 48);
        }

        public static double RoundToSignificantDigits(this double d, int digits)
        {
            if (d == 0)
				return 0;
			if (d < 0)
                throw new Exception("Number cannot be less than zero.");

            var scale = Math.Pow(10, Math.Floor(Math.Log10(d)) + 1);
            return scale * Math.Round(d / scale, digits);
        }
 
        public static string ToChars(this ICollection<int?> plays)
        {
            if (plays.Count == 0)
                return "";

            var playsAsChar = plays.Where(p => p != null).Select(p => ((int)p).ToChar()).Select(x => x.ToString()).ToArray();
            var serialisedPlays = string.Join("", playsAsChar);

            return serialisedPlays;
        }

        public static ulong ToBitBoard(this IEnumerable<string> locations)
        {
            var list = locations.Select(x => (int)x.ToIndex()).ToList();
            ulong board = 0;
            list.ForEach(x => board |= x.ToBitBoard());
            return board;
        }
	}
}
