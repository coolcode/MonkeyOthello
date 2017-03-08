using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MonkeyOthello.IO
{
    /// <summary>
    /// 配置文件管理类
    /// </summary>
    class OthelloFileManager
    {
        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool Save(string filename, OthelloFile content)
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write);
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(stream,content);
                if (stream != null)
                    stream.Close();
                return true;
            }
            catch
            {
                if (stream != null)
                    stream.Close();
                return false;
            }
        }

        /// <summary>
        /// 读取
        /// </summary>
        /// <param name="filename"></param>
        public static OthelloFile Read(string filename)
        {
            OthelloFile content;
            BinaryFormatter formatter = new BinaryFormatter();
            FileStream stream = null;
            if (!File.Exists(filename)) return null;
            try
            {
                stream = new FileStream(filename, FileMode.Open, FileAccess.Read);
                stream.Seek(0, SeekOrigin.Begin);
                content = (OthelloFile)formatter.Deserialize(stream);
                if (stream != null)
                    stream.Close();
                return content;
            }
            catch
            {
                if (stream != null)
                    stream.Close();
                return null;
            }
        }
    }
}
