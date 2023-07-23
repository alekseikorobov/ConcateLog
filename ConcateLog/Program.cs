using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using CommandLine;
using CommandLine.Text;
using System.Diagnostics;

namespace ConcatFiles
{
    public class Program
    {
        static void RunOptions(Options options)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                string[] sourcePaths = null;
                if (!string.IsNullOrEmpty(options.Source))
                {
                    sourcePaths = GetSourcePathsByPattern(options);
                }
                else
                {
                    if (string.IsNullOrEmpty(options.File))
                        throw new ArgumentException("�� ������ �������� ������");
                    sourcePaths = File.ReadAllLines(options.File);
                }
                var patern = options.Pattern;
                if (options.DefaultPatern1)
                    patern = "^\\d+-\\d+-\\d+ \\d+:\\d+:\\d+.\\d+.*";

                WriteFile(sourcePaths, patern, options);
            }
            catch (Exception ex)
            {
                var temp = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.ForegroundColor = temp;
            }
            stopwatch.Stop();

            Console.WriteLine($"Done - {stopwatch.Elapsed}");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private static string[] GetSourcePathsByPattern(Options options)
        {
            var dirSrc = options.Source;

            SearchOption searchOption = SearchOption.TopDirectoryOnly;
            if (options.AllDirectory)
                searchOption = SearchOption.AllDirectories;
            if (!string.IsNullOrEmpty(options.Template))
            {
                Console.WriteLine($"������������ ������ ��� ������ ������ - {options.Template}");

                if (Directory.Exists(dirSrc))
                    return Directory.GetFiles(dirSrc, options.Template, searchOption);
                else
                    return new string[0];
            }
            else
            {
                if (!string.IsNullOrEmpty(options.Query))
                {
                    Console.WriteLine($"������������ ���������� ��������� ��� ������ ������ - {options.Query}");
                    Regex reg = new Regex(options.Query, RegexOptions.Compiled);

                    var files = new DirectoryInfo(dirSrc).GetFiles("*", searchOption);
                    return files.Where(c => reg.IsMatch(c.Name)).Select(c => c.FullName).ToArray();
                }
            }
            return Directory.GetFiles(dirSrc);
        }

        static void Main(string[] args)
        {
#if DEBUG
            //args = new[]
            //{
            //    "-s",@"c:\myProject\code\GitHub\Log\",
            //    "-p",".*",
            //    "-r",@"c:\myProject\code\GitHub\Log\res.txt",
            //    "-e",".*\t(POST|GET)\t.*"
            //};

            ////getGroupMethod.bat
            //args = new[]
            //{
            //    "-s",@"c:\myProject\code\GitHub\Log\",
            //    "-p",".*",
            //    "-r",@"c:\myProject\code\GitHub\Log\res.txt",
            //    //"-e",".*\t(POST|GET)\t.*"
            //    "--split",
            //    "--skip","4",
            //    "--group",
            //    "--cols","3"
            //};
            //getGroupUserUrlPort.bat
            args = new[]
            {
                "-s",@"c:\myProject\code\GitHub\Log\",
                "-p",".*",
                "-t","*.log",
                "-r",@"c:\myProject\code\GitHub\Log\getGroupUserUrlPort_res_1.txt",
                //"-e",".*\t(POST|GET)\t.*"
                "--split",
                "--skip","4",
                "--group",
                "--cols","4,6,7",
                //"--test","10",
                //"--take","300"
                "--contains","!/LMS,!.png,!.css,!.js,!.mp3,!.jpg,!.gif,!/DesktopApplications,!/DecisionTree",
                "--tolow",
            };
#endif
            bool isWait = true;
            if (args == null || args.Length == 0)
            {
                isWait = true;
                args = new[]
                {
                    "-h",
                };
            }
            var resu = Parser.Default.ParseArguments<Options>(args);

            Console.WriteLine("-----args----");
            foreach (var a in args)
            {
                Console.WriteLine(a);
            }
            Console.WriteLine("-------------");

            resu.WithParsed(RunOptions);

            if (isWait)
                Console.ReadLine();
        }

        public static void WriteFile(string[] sourcePaths, string pattern, Options options)
        {
            if (options.IsDeleteFileBeforeRun)
                File.Delete(options.Result);

            Regex reg = null;
            if (!string.IsNullOrEmpty(pattern))
            {
                reg = new Regex(pattern, RegexOptions.Compiled);
            }

            groups.Clear();

            Console.WriteLine($"Count file {sourcePaths.Length}");
            //foreach (var path in sourcePaths)
            for (int i = 0; i < sourcePaths.Length; i++)
            {
                try
                {
                    bool isBreak = WriteLine(path, reg, options);
                    if (isBreak)
                        break;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            using (StreamWriter sw = new StreamWriter(options.Result, true, Encoding.GetEncoding(1251)))
            {
                CollectStream(sw, options);
            }
        }

        public static bool WriteLine(string sourcePath, Regex reg, Options options)
        {
            Console.WriteLine($"{sourcePath}   ->   {options.Result}");
            using (StreamReader sr = new StreamReader(sourcePath, Encoding.GetEncoding(1251)))
            {
                if (!options.IsGrouping)
                {
                    using (StreamWriter sw = new StreamWriter(options.Result, true, Encoding.GetEncoding(1251)))
                    {
                        return WriteLineStream(sourcePath, reg, options, sr, sw);
                    }
                }
                else
                {
                    return WriteLineStream(sourcePath, reg, options, sr, null);
                }
            }            
        }
        public static char SpliteColumnText = ' ';
        public static Dictionary<string, double> groups = new Dictionary<string, double>();
        public static bool WriteLineStream(string sourcePath, Regex reg, Options options, TextReader sr, TextWriter sw)
        {
            string line;
            for (int i = 0; i < options.Skip; i++)
            {
                var res = sr.ReadLine();
                if (res == null)
                    return false;
            }

            line = sr.ReadLine();
            if(options.ToLower == true)
                line = line.ToLower();

            if (options.Take.HasValue && options.Take-- < 0) return true;
            if (line == null)
                return false;

            bool isFirst = true;

            Regex extractExpressionRegex = null;
            string[] notContains = null;
            string[] contains = null;
            if (!string.IsNullOrEmpty(options.Contains))
            {
                var arr = options.Contains.Split(',');

                notContains = arr.Where(c => c.Length > 1 && c[0] == '!' && c[1] != '!').Select(c => c.TrimStart('!').ToLower()).ToArray();
                contains = arr.Where(c => (c.Length > 0 && c[0] != '!')
                        || (c.Length > 1 && c[1] == '!' && c[0] == '!')).Select(c =>
                            c[0] == '!' ? c.Substring(1) : c
                        ).Select(c=>c.ToLower()).ToArray();

                if (contains.Length == 0) contains = null;

            }


            if (notContains != null || contains != null)
            {
                if (!FilteredByContains(line, contains, notContains))
                {
                    line = "";
                }
            }

            if (!string.IsNullOrEmpty(line) && options.IsBySplit)
            {
                var ar = line.Split(SpliteColumnText);
                var cols = options.GetColumns.Split(',').Select(c => int.Parse(c));

                if (ar.Length < cols.Min())
                    line = "";
                else
                {
                    var vals = cols.Select(c => ar[c]);
                    line = string.Join("\t", vals);
                }
            }
            else if (!string.IsNullOrEmpty(options.ExtractExpression))
            {
                extractExpressionRegex = new Regex(options.ExtractExpression, RegexOptions.Compiled);
                var m = extractExpressionRegex.Match(line);
                line = string.Join("\t", m.Groups.Cast<Group>().Skip(1).Select(c => c.Value));
            }
            if (!string.IsNullOrEmpty(line))
            {
                if (options.IsWritePath)
                {
                    line = sourcePath + "\t" + line;
                }
                if (!options.IsGrouping)
                {
                    if (Write(sw, line, options)) return true;
                }
                else
                {
                    if (groups.ContainsKey(line))
                    {
                        groups[line] += 1.0;
                    }
                    else
                    {
                        groups.Add(line, 1.0);
                    }
                }
                isFirst = false;
            }

            int countRow = 0;
            string newLinestr = "";
            while ((line = sr.ReadLine()) != null)
            {
                if (options.ToLower == true)
                    line = line.ToLower();

                if (options.Take.HasValue && options.Take-- < 0) return true;

                if (countRow++ % 1000 == 0)
                {
                    Console.WriteLine($" rows {countRow}");
                }

                if (notContains != null || contains != null)
                {
                    if (!FilteredByContains(line, contains, notContains))
                    {
                        continue;
                    }
                }

                if (options.IsBySplit)
                {
                    var ar = line.Split(SpliteColumnText);
                    var cols = options.GetColumns.Split(',').Select(c => int.Parse(c));

                    if (ar.Length <= cols.Min())
                        continue;
                    var vals = cols.Select(c => ar[c]);

                    line = string.Join("\t", vals);
                }
                else if (extractExpressionRegex != null)
                {
                    var m = extractExpressionRegex.Match(line);
                    var newLine = string.Join("\t", m.Groups.Cast<Group>().Skip(1).Select(c => c.Value));
                    line = newLine;
                    if (string.IsNullOrEmpty(line)) continue;
                }
                if (!isFirst)
                {
                    if (reg != null)
                        newLinestr = reg == null || reg.IsMatch(line) ? Environment.NewLine : "</br>";
                }
                else
                {
                    isFirst = false;
                }

                if (options.IsWritePath)
                {
                    line = sourcePath + "\t" + line;
                }
                if (!options.IsGrouping)
                {
                    if (Write(sw, newLinestr + line, options)) return true;
                }
                else
                {
                    if (groups.ContainsKey(line))
                    {
                        groups[line] += 1.0;
                    }
                    else
                    {
                        groups.Add(line, 1.0);
                    }
                }
            }

            if (!options.IsGrouping)
                if (Write(sw, Environment.NewLine, options)) return true;

            return false;
        }

        public static bool FilteredByContains(string line, string[] contains, string[] notContains)
        {
            for (int i = 0; i < (contains?.Length ?? 0); i++)
            {
                if (line.ToLower().Contains(contains[i]))
                {
                    return true;
                }
            }
            for (int i = 0; i < (notContains?.Length ?? 0); i++)
            {
                if (line.ToLower().Contains(notContains[i]))
                {
                    return false;
                }
            }
            return contains == null;
        }

        public static void CollectStream(TextWriter sw, Options options)
        {
            bool isFirst = true;
            if (options.IsGrouping)
            {
                foreach (var item in groups)
                {
                    if (isFirst)
                    {
                        if (Write(sw, $"{item.Key}\t{item.Value}", options)) return;
                        isFirst = false;
                    }
                    else
                    {
                        if (Write(sw, $"{Environment.NewLine}{item.Key}\t{item.Value}", options)) return;
                    }
                }
                Write(sw, Environment.NewLine, options);
            }
        }

        public static bool Write(TextWriter sw, string list, Options options)
        {

            if (options.TestRow.HasValue)
            {
                if (options.TestRow-- < 0) return true;
                Console.Write(list);
            }
            else
            {
                sw.Write(list);
            }
            return false;
        }
        /// <summary>
        /// Parse line by seporation using string split and get element by index
        /// </summary>
        /// <param name="line">input string line</param>
        /// <param name="skipStartWith">if start by char then skip line and return string.Empty</param>
        /// <param name="sep">seporator for split line as array</param>
        /// <param name="getIndexs">get element from array line</param>
        /// <returns>return new line joined by \t</returns>
        public static string ParseBySep(string line, char sep, int[] getIndexs)
        {
            if (!line.Contains(sep)) return "";

            var list = line.Split(sep);

            //����� ������ �������� ������
            //return string.Join("\t",getIndexs.Select(i=>list[i]));

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < getIndexs.Length; i++)
            {
                sb.Append(list[getIndexs[i]]);
                if (i != getIndexs.Length - 1)
                    sb.Append('\t');
            }
            return sb.ToString();
        }
    }
}
