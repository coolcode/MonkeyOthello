using System.Runtime.InteropServices;

namespace MonkeyOthello.Engines.V2
{
    /// <summary>
    /// Zebra end game engine
    /// </summary>
    internal class EGEngineNativeMethods
    {
        [DllImport("EGEngine", EntryPoint = "SetDepth", SetLastError = true)]
        public static extern void SetDepth(int mid, int wld, int end);

        /// <summary>
        /// Search
        /// </summary>
        /// <param name="board">array 1*91</param>
        /// <param name="color">0:black, 1:white</param>
        /// <param name="mode">-1:default, engine decides by itself; 0:return winning or losing pieces; 1:return win or lose only</param>
        /// <param name="n_bits">hashtable size: 15≈1M,16≈2M,17≈4M,...,20≈32M</param>
        [DllImport("EGEngine", EntryPoint = "AI_Slove", SetLastError = true)]
        public static extern void Slove(int[] board, int color, int mode, int nbits);

        [DllImport("EGEngine", EntryPoint = "AI_GetEval", SetLastError = true)]
        public static extern int GetEval();

        [DllImport("EGEngine", EntryPoint = "AI_GetNodes", SetLastError = true)]
        public static extern int GetNodes();

        [DllImport("EGEngine", EntryPoint = "AI_GetBestMove", SetLastError = true)]
        public static extern int GetBestMove();
    }
}
