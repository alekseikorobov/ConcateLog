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

namespace ConcatFiles
{
    class Program
    {
        static void RunOptions(Options options)
        {
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
                        throw new ArgumentException("не указан источник файлов");
                    sourcePaths = File.ReadAllLines(options.File);
                }
                var patern = options.Pattern;
                if (options.DefoultPatern1)
                    patern = "^\\d+-\\d+-\\d+ \\d+:\\d+:\\d+.\\d+.*";
                
                Write(sourcePaths, patern, options);
            }
            catch (Exception ex)
            {
                var temp = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
                Console.ForegroundColor = temp;
            }

            Console.WriteLine("Done");
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
                Console.WriteLine($"»спользуетс€ шаблон дл€ поиска файлов - {options.Template}");

                if(Directory.Exists(dirSrc))
                    return Directory.GetFiles(dirSrc, options.Template, searchOption);
                else
                    return new string[0];
            }
            else
            {
                if (!string.IsNullOrEmpty(options.Query))
                {
                    Console.WriteLine($"»спользуетс€ регул€рное выражение дл€ поиска файлов - {options.Query}");
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
            args = new[]
            {
                "-s",@"\MailServiceLog$\logs\",
                "-p","^\\d+-\\d+-\\d+ \\d+:\\d+:\\d+,\\d+.*",
                "-r","res.txt",
                "-e","(^\\d+-\\d+-\\d+).*Message sent with template (.*)\\. From"
            };
#endif
            bool isWait = false;
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

        private static void Write(string[] sourcePaths, string pattern, Options options)
        {
            string targetPath = options.Result;
            if (options.IsDeleteFileBeforeRun)
                File.Delete(targetPath);

            bool skipFirstLine = options.Skip;

            Regex reg = null;
            if (!string.IsNullOrEmpty(pattern))
            {
                reg = new Regex(pattern, RegexOptions.Compiled);
            }

            Console.WriteLine($"Count file {sourcePaths.Length}");
            foreach (var path in sourcePaths)
            {
                try
                {
                    WriteLine(path, targetPath, skipFirstLine, reg, options.ExtractExpression, options.IsWritePath);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        private static void WriteLine(string sourcePath, string targetPath, bool skipFirstLine, Regex reg, string extractExpression, bool isWritePath)
        {
            Console.WriteLine($"{sourcePath}   ->   {targetPath}");
            using (StreamReader sr = new StreamReader(sourcePath, Encoding.GetEncoding(1251)))
            {
                using (StreamWriter sw = new StreamWriter(targetPath, true, Encoding.GetEncoding(1251)))
                {
                    string line;
                    if (skipFirstLine)
                        sr.ReadLine();

                    line = sr.ReadLine();
                    if (line == null)
                        return;

                    if (!string.IsNullOrEmpty(extractExpression))
                    {
                        var r = new Regex(extractExpression);
                        var m = r.Match(line);
                        var newLine = string.Join("\t", m.Groups.Cast<Group>().Skip(1).Select(c => c.Value));
                        line = newLine;
                    }

                    if (!string.IsNullOrEmpty(line))
                        sw.Write(line);
                    string newLinestr = "";
                    bool isWrite = false;
                    while ((line = sr.ReadLine()) != null)
                    {


                        if (!string.IsNullOrEmpty(extractExpression))
                        {
                            var r = new Regex(extractExpression);
                            var m = r.Match(line);
                            var newLine = string.Join("\t", m.Groups.Cast<Group>().Skip(1).Select(c => c.Value));
                            line = newLine;
                            if (string.IsNullOrEmpty(line)) continue;
                        }

                        if (isWrite)
                            newLinestr = reg == null || reg.IsMatch(line) ? sw.NewLine : "</br>";
                        else
                            isWrite = true;

                        string path = "";
                        if (isWritePath)
                        {
                            path = sourcePath + "\t";
                        }
                        sw.Write(newLinestr + path + line);
                    }
                    if(isWrite)
                        sw.Write(sw.NewLine);
                }
            }
        }
    }
}
