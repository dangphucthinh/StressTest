using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace ProfileChecker.Authentication
{
    public class Utils
    {
        private static ASCIIEncoding _encoder = new ASCIIEncoding();


        public static byte[] GetBytes(string obj)
        {
            return _encoder.GetBytes(obj);
        }

        public static string Serialize<T>(T obj)
        {
            return JsonSerializer.Serialize(obj);
        }

        public static T Deserialize<T>(string obj)
        {
            return JsonSerializer.Deserialize<T>(obj);
        }



        internal static Random Rnd = new Random((int)DateTime.Now.Ticks);

        internal static string GetStringBetween(string srcString, string startToken, string endToken)
        {
            var rtn = string.Empty;
            try
            {
                if (!srcString.Contains(startToken) || !srcString.Contains(endToken))
                {
                    rtn = string.Empty;
                }
                else
                {
                    var startIdx = srcString.IndexOf(startToken, StringComparison.Ordinal) + startToken.Length;
                    var endIdx = srcString.IndexOf(endToken, startIdx, StringComparison.Ordinal);

                    rtn = srcString.Substring(startIdx, endIdx - startIdx);
                }
            }
            catch
            {
            }

            return rtn;
        }

        internal static void MoveDirectory(string source, string target)
        {
            var sourcePath = source.TrimEnd('\\', ' ');
            var targetPath = target.TrimEnd('\\', ' ');
            var files = Directory.EnumerateFiles(sourcePath, "*", SearchOption.AllDirectories)
                                 .GroupBy(s => Path.GetDirectoryName(s));
            foreach (var folder in files)
            {
                var targetFolder = folder.Key.Replace(sourcePath, targetPath);
                Directory.CreateDirectory(targetFolder);
                foreach (var file in folder)
                {
                    var targetFile = Path.Combine(targetFolder, Path.GetFileName(file));
                    if (File.Exists(targetFile)) File.Delete(targetFile);
                    File.Move(file, targetFile);
                }
            }
            Directory.Delete(source, true);
        }

        /// <summary>
        ///     Depth-first recursive delete, with handling for descendant
        ///     directories open in Windows Explorer.
        /// </summary>
        internal static void DeleteDirectory(string path)
        {
            foreach (var directory in Directory.GetDirectories(path)) DeleteDirectory(directory);

            Directory.Delete(path, true);
        }

        internal static bool EnsureDirectory(string path)
        {
            if (!Directory.Exists(path)) Directory.CreateDirectory(path);
            return true;
        }


        internal static int GetRandom(int from, int to)
        {
            return Rnd.Next(from, to);
        }

        internal static string GetRandomHexString(int length)
        {
            return new string((from x in Enumerable.Repeat("0123456789abcdef", length)
                               select x[Rnd.Next(16)]).ToArray());
        }

        internal static string GetRandomString(int length)
        {
            return new string((from x in Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789abcdefghijklmnopqrstuvwxyz0123456789", length)
                               select x[Rnd.Next(72)]).ToArray());
        }

        internal static string GetRandomAlphabet(int length)
        {
            return new string((from x in Enumerable.Repeat("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz", length)
                               select x[Rnd.Next(52)]).ToArray());
        }

        internal static string HumanrizeSize(long size)
        {
            return size < 1048576L
                ? string.Format("{0} KB", Math.Round(size / 1024.0, 2))
                : string.Format("{0} MB", Math.Round(size / 1024.0 / 1024.0, 2));
        }

        internal static string GetRandomNumber(int length)
        {
            var str = new string((
                from x in Enumerable.Repeat("0123456789", length)
                select x[Rnd.Next(10)]).ToArray());

            return str;
        }

        internal static string RandomNumberWithoutLeadingZero(int length)
        {
            var str = new string((
                from x in Enumerable.Repeat("0123456789", length - 1)
                select x[Rnd.Next(10)]).ToArray());

            return string.Concat(Rnd.Next(9) + 1, str);
        }

        internal static string RandomNumber(string prefix, int length)
        {
            var str = new string((from x in Enumerable.Repeat("0123456789", length - prefix.Length)
                                  select x[Rnd.Next(10)]).ToArray());

            return prefix + str;
        }

        internal static string TryConvertText(object input, string defaultValue = "")
        {
            try
            {
                return input.ToString();
            }
            catch { }

            return defaultValue;
        }

        internal static bool TryConvertBool(object input, bool defaultValue = false)
        {
            try
            {
                return bool.Parse(input.ToString());
            }
            catch { }

            return defaultValue;
        }

        internal static int TryConvertInt(object input, int defaultValue = 0)
        {
            try
            {
                return int.Parse(input.ToString());
            }
            catch { }

            return defaultValue;
        }

        internal static DateTime TryConvertDateTimeddMMMyyyy(object input, DateTime? defaultValue = null)
        {
            if (!defaultValue.HasValue)
                defaultValue = DateTime.UtcNow;
            try
            {
                var dt = input.ToString();
                var day = int.Parse(dt.Substring(0, 2));
                var month = 1;
                var year = int.Parse(dt.Substring(7, 4));

                switch (dt.Substring(3, 3))
                {
                    case "Jan":
                        month = 1;
                        break;
                    case "Feb":
                        month = 2;
                        break;
                    case "Mar":
                        month = 3;
                        break;
                    case "Apr":
                        month = 4;
                        break;
                    case "May":
                        month = 5;
                        break;
                    case "Jun":
                        month = 6;
                        break;
                    case "Jul":
                        month = 7;
                        break;
                    case "Aug":
                        month = 8;
                        break;
                    case "Sep":
                        month = 9;
                        break;
                    case "Oct":
                        month = 10;
                        break;
                    case "Nov":
                        month = 11;
                        break;
                    case "Dec":
                        month = 12;
                        break;
                }

                return new DateTime(year, month, day);
            }
            catch { }

            return (DateTime)defaultValue;
        }
    }
}
