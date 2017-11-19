using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;

namespace CodeBuilder.Common
{
    public enum ObjectModes
    {
        None,
        Databases,
        Objects,
        Table,
        Sp,
        View,
        Function,
        Assembly,
        Trigger,
        Job,
        Server,
        Index
    }

    internal enum WorkModes
    {
        Summary = 0,
        Objects = 1,
        Activities = 2,
        Performance = 3,
        Analysis = 4,
        Alerts = 5,
        Histories = 6,
        Options = 7,
        Query = 8,
        TableData = 9,
        UserPerformance = 10
    }

    class Utils
    {
        internal const int EmptyIndex = -1;
        internal const string FileExtenionSql = ".sql";
        internal const string SizeKb = "KB";
        internal const string SizeMb = "MB";
        internal const string SizeGb = "GB";
        internal const int Size1K = 1024;
        internal const string TimeMs = "ms";
        internal const string MultiCommentStart = "/*";
        internal const string MultiCommentEnd = "*/";
        internal const string SingleCommentStart = "--";

        internal static T CloneObject<T>(T objectInstance)
        {
            var bFormatter = new BinaryFormatter();
            var stream = new MemoryStream();
            bFormatter.Serialize(stream, objectInstance);
            stream.Seek(0, SeekOrigin.Begin);
            return (T)bFormatter.Deserialize(stream);
        }

        internal static void Split(string content, string splitter, out string key, out string value)
        {
            var index = content.IndexOf(splitter);
            if (index != -1)
            {
                key = content.Substring(0, index);
                value = content.Substring(index + 1).Replace(@"\r\n", "\r\n");
            }
            else
            {
                key = string.Empty;
                value = string.Empty;
            }
        }
    }
}
