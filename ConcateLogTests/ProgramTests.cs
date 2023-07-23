using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConcatFiles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.IO;

namespace ConcatFiles.Tests
{
    [TestClass()]
    public class ProgramTests
    {
        private static IEnumerable<object[]> Texts
        {
            get
            {
                yield return new object[] { new string[]{
                        "line1",
                        "line2",
                    },
                    "line1line2\r\n",new Options(),null
                };

                yield return new object[] {new string[]{
                        "line1",
                        "line2",
                        "line3",
                    },
                    "line1line2line3\r\n",new Options(),null
                };

                yield return new object[] {new string[]{
                        "line1",
                        "line2",
                    },
                    "line2\r\n",new Options{Skip = 1,},null
                };
                yield return new object[] {new string[]{
                        "line1",
                        "line2",
                        "line3",
                    },
                    "line2line3\r\n",new Options{Skip = 1,},null
                };


                yield return new object[] { new string[]{
                        "line1",
                        "line2",
                    },
                    "line1\r\nline2\r\n",new Options(),new Regex(@".*")
                };

                yield return new object[] {new string[]{
                        "line1",
                        "line2",
                        "line3",
                    },
                    "line1\r\nline2\r\nline3\r\n",new Options(),new Regex(@".*")
                };

                yield return new object[] {new string[]{
                        "line1",
                        "line2",
                    },
                    "line2\r\n",new Options{Skip = 1,},new Regex(@".*")
                };
                yield return new object[] {new string[]{
                        "line1",
                        "line2",
                        "line3",
                    },
                    "line2\r\nline3\r\n",new Options{Skip = 1,},new Regex(@".*")
                };

                yield return new object[] {new string[]{
                        "123 line1",
                        "123 line2",
                        "123 line3",
                    },
                    "123\r\n123\r\n123\r\n",new Options{Skip = 0,
                    ExtractExpression = @"(\d+)"
                    },new Regex(@".*")
                };


                yield return new object[] {new string[]{
                        "123 line1",
                        "123 line2",
                        "123 line3",
                    },
                    "123123123\r\n",new Options{Skip = 0,
                    ExtractExpression = @"(\d+)"
                    },null
                };

                yield return new object[] {new string[]{
                        "123 line1",
                        "line2",
                        "456 line3",
                        "789 line4",
                    },
                    "123 line1</br>line2\r\n456 line3\r\n789 line4\r\n",new Options{Skip = 0,
                    },new Regex(@"^\d+.*")
                };



                yield return new object[] {new string[]{
                        "123 line1",
                        "line2",
                        "456 line3",
                        "789 line4",
                    },
                    "123\r\n456\r\n789\r\n",new Options{Skip = 0,
                    ExtractExpression = @"^(\d+)"
                    },new Regex(@"^\d+.*")
                };

                yield return new object[] {new string[]{
                        "123 line1",
                        "line2",
                        "456 line3",
                        "789 line4",
                    },
                    "123\r\n2\r\n456\r\n789\r\n",new Options{Skip = 0,
                    ExtractExpression = @"(\d+)"
                    },new Regex(@"^\d+.*")
                };

                yield return new object[] {new string[]{
                        "123 line1",
                        "line2",
                        "456 line3",
                        "789 line4",
                    },
                    "123\r\n2\r\n456\r\n789\r\n",new Options{Skip = 0,
                    ExtractExpression = @"(\d+)"
                    },new Regex(@".*")
                };


                //by spliting

                yield return new object[] {new string[]{
                        $"line1{Program.SpliteColumnText}line2{Program.SpliteColumnText}line3",
                        $"line4{Program.SpliteColumnText}line5{Program.SpliteColumnText}line6",
                        $"line7{Program.SpliteColumnText}line8{Program.SpliteColumnText}line9",
                    },
                    "line1\tline3\r\nline4\tline6\r\nline7\tline9\r\n",
                    new Options{
                    //IsGrouping = true,
                    IsBySplit = true,
                    GetColumns="0,2",
                    },new Regex(@".*")
                };


                //grouping!

                yield return new object[] {new string[]{
                        "line",
                        "line",
                        "line",
                    },
                    "line\t3\r\n",new Options{
                    IsGrouping = true,
                    },new Regex(@".*")
                };
                yield return new object[] {new string[]{
                        "line",
                        "line",
                        "line1",
                    },
                    "line\t2\r\nline1\t1\r\n",new Options{
                    IsGrouping = true,
                    },new Regex(@".*")
                };

                yield return new object[] {new string[]{
                        "line 123",
                        "line 125",
                        "line1 126",
                    },
                    "line\t2\r\nline1\t1\r\n",new Options{
                    IsGrouping = true,
                    ExtractExpression = "^(.*?) ",
                    },new Regex(@".*")
                };
            }
        }


        [DataTestMethod()]
        [DynamicData(nameof(Texts), DynamicDataSourceType.Property)]
        public void WriteLineTest_imitation_one_File(string[] list, string expected, Options options, Regex reg)
        {
            TextReader sr = new TextReaderTest(list);
            TextWriter sw = new TextWriterTest();
            Program.groups.Clear();
            Program.WriteLineStream("sourcePath", reg, options, sr, sw);

            Program.CollectStream(sw, options);

            var result = sw.ToString();

            Assert.AreEqual(expected, result);
        }


        private static IEnumerable<object[]> TextsReal
        {
            get
            {
                ////get all POST requst
                //yield return new object[] {new string[]{
                //        "#Software: Microsoft Internet Information Services 10.0",
                //        "#Version: 1.0",
                //        "#Date: 2022-03-05 00:00:11",
                //        "#Fields: date time s-ip cs-method cs-uri-stem cs-uri-query s-port cs-username c-ip cs(User-Agent) cs(Referer) sc-status sc-substatus sc-win32-status time-taken X-Forwarded-For",
                //        @"2022-03-05 00:01:24 10.207.32.142 POST /Services/Activity/Track/ScormService.asmx/SetData - 80 RU\ekaterinaklimova 10.207.32.141 Mozilla/5.0+(Windows+NT+10.0;+Win64;+x64)+AppleWebKit/537.36+(KHTML,+like+Gecko)+Chrome/98.0.4758.102+Safari/537.36+Edg/98.0.1108.62 http://applications.ru.kworld.kpmg.com/LMS/Tracks/RM/ActingWithIntegrity/2021new/Module/index_lms.html 200 0 0 140 10.207.222.232",
                //        @"2022-03-05 00:01:24 10.207.32.142 GET /Services/Activity/Track/ScormService.asmx/SetData - 80 RU\ekaterinaklimova 10.207.32.141 Mozilla/5.0+(Windows+NT+10.0;+Win64;+x64)+AppleWebKit/537.36+(KHTML,+like+Gecko)+Chrome/98.0.4758.102+Safari/537.36+Edg/98.0.1108.62 http://applications.ru.kworld.kpmg.com/LMS/Tracks/RM/ActingWithIntegrity/2021new/Module/index_lms.html 200 0 0 140 10.207.222.232",
                //    },
                //    "2022-03-05 00:01:24 10.207.32.142 POST /Services/Activity/Track/ScormService.asmx/SetData - 80 RU\\ekaterinaklimova 10.207.32.141 Mozilla/5.0+(Windows+NT+10.0;+Win64;+x64)+AppleWebKit/537.36+(KHTML,+like+Gecko)+Chrome/98.0.4758.102+Safari/537.36+Edg/98.0.1108.62 http://applications.ru.kworld.kpmg.com/LMS/Tracks/RM/ActingWithIntegrity/2021new/Module/index_lms.html 200 0 0 140 10.207.222.232\r\n",new Options{Skip = false,
                //    ExtractExpression = @"(.*POST.*)"
                //    },new Regex(@".*")
                //};

                ////get times
                //yield return new object[] {new string[]{
                //        "#Software: Microsoft Internet Information Services 10.0",
                //        "#Version: 1.0",
                //        "#Date: 2022-03-05 00:00:11",
                //        "#Fields: date time s-ip cs-method cs-uri-stem cs-uri-query s-port cs-username c-ip cs(User-Agent) cs(Referer) sc-status sc-substatus sc-win32-status time-taken X-Forwarded-For",
                //        @"2022-03-05 00:01:24 10.207.32.142 POST /Services/Activity/Track/ScormService.asmx/SetData - 80 RU\ekaterinaklimova 10.207.32.141 Mozilla/5.0+(Windows+NT+10.0;+Win64;+x64)+AppleWebKit/537.36+(KHTML,+like+Gecko)+Chrome/98.0.4758.102+Safari/537.36+Edg/98.0.1108.62 http://applications.ru.kworld.kpmg.com/LMS/Tracks/RM/ActingWithIntegrity/2021new/Module/index_lms.html 200 0 0 140 10.207.222.232",
                //        @"2022-03-05 00:01:26 10.207.32.142 GET /Services/Activity/Track/ScormService.asmx/SetData - 80 RU\ekaterinaklimova 10.207.32.141 Mozilla/5.0+(Windows+NT+10.0;+Win64;+x64)+AppleWebKit/537.36+(KHTML,+like+Gecko)+Chrome/98.0.4758.102+Safari/537.36+Edg/98.0.1108.62 http://applications.ru.kworld.kpmg.com/LMS/Tracks/RM/ActingWithIntegrity/2021new/Module/index_lms.html 200 0 0 140 10.207.222.232",
                //    },
                //    "2022-03-05 00:01:24\r\n2022-03-05 00:01:26\r\n"
                //    ,new Options{
                //        Skip = false,
                //        ExtractExpression = @"^(\d+-\d+-\d+ \d+:\d+:\d+).*",

                //    },new Regex(@".*")
                //};


                ////COUNT GET And POST 
                //yield return new object[] {new string[]{
                //        "#Software: Microsoft Internet Information Services 10.0",
                //        "#Version: 1.0",
                //        "#Date: 2022-03-05 00:00:11",
                //        "#Fields: date time s-ip cs-method cs-uri-stem cs-uri-query s-port cs-username c-ip cs(User-Agent) cs(Referer) sc-status sc-substatus sc-win32-status time-taken X-Forwarded-For",
                //        "2022-03-05 00:01:24 10.207.32.142\tPOST\t/Services/Activity/Track/ScormService.asmx/SetData - 80 RU\\ekaterinaklimova 10.207.32.141 Mozilla/5.0+(Windows+NT+10.0;+Win64;+x64)+AppleWebKit/537.36+(KHTML,+like+Gecko)+Chrome/98.0.4758.102+Safari/537.36+Edg/98.0.1108.62 http://applications.ru.kworld.kpmg.com/LMS/Tracks/RM/ActingWithIntegrity/2021new/Module/index_lms.html 200 0 0 140 10.207.222.232",
                //        "2022-03-05 00:01:26 10.207.32.142\tGET\t/Services/Activity/Track/ScormService.asmx/SetData - 80 RU\\ekaterinaklimova 10.207.32.141 Mozilla/5.0+(Windows+NT+10.0;+Win64;+x64)+AppleWebKit/537.36+(KHTML,+like+Gecko)+Chrome/98.0.4758.102+Safari/537.36+Edg/98.0.1108.62 http://applications.ru.kworld.kpmg.com/LMS/Tracks/RM/ActingWithIntegrity/2021new/Module/index_lms.html 200 0 0 140 10.207.222.232",
                //        "2022-03-05 00:01:28 10.207.32.142\tPOST\t/Services/Activity/Track/ScormService.asmx/SetData - 80 RU\\ekaterinaklimova 10.207.32.141 Mozilla/5.0+(Windows+NT+10.0;+Win64;+x64)+AppleWebKit/537.36+(KHTML,+like+Gecko)+Chrome/98.0.4758.102+Safari/537.36+Edg/98.0.1108.62 http://applications.ru.kworld.kpmg.com/LMS/Tracks/RM/ActingWithIntegrity/2021new/Module/index_lms.html 200 0 0 140 10.207.222.232",
                //    },
                //    "POST\t2\r\nGET\t1\r\n"
                //    ,new Options{
                //        Skip = false,
                //        ExtractExpression = ".*\t(POST|GET)\t.*",
                //        IsGrouping = true,

                //    },new Regex(@".*")
                //};


                //COUNT GET And POST 
                yield return new object[] {new string[]{
                        "#Software: Microsoft Internet Information Services 10.0",
                        "#Version: 1.0",
                        "#Date: 2022-03-05 00:00:11",
                        "#Fields: date time s-ip cs-method cs-uri-stem cs-uri-query s-port cs-username c-ip cs(User-Agent) cs(Referer) sc-status sc-substatus sc-win32-status time-taken X-Forwarded-For",
                        "2022-03-05 00:01:24 10.207.32.142 POST /Services/Activity/Track/ScormService.asmx/SetData - 80 RU\\ekaterinaklimova 10.207.32.141 Mozilla/5.0+(Windows+NT+10.0;+Win64;+x64)+AppleWebKit/537.36+(KHTML,+like+Gecko)+Chrome/98.0.4758.102+Safari/537.36+Edg/98.0.1108.62 http://applications.ru.kworld.kpmg.com/LMS/Tracks/RM/ActingWithIntegrity/2021new/Module/index_lms.html 200 0 0 140 10.207.222.232",
                        "2022-03-05 00:01:26 10.207.32.142 GET /Services/Activity/Track/ScormService.asmx/SetData - 80 RU\\ekaterinaklimova 10.207.32.141 Mozilla/5.0+(Windows+NT+10.0;+Win64;+x64)+AppleWebKit/537.36+(KHTML,+like+Gecko)+Chrome/98.0.4758.102+Safari/537.36+Edg/98.0.1108.62 http://applications.ru.kworld.kpmg.com/LMS/Tracks/RM/ActingWithIntegrity/2021new/Module/index_lms.html 200 0 0 140 10.207.222.232",
                        "2022-03-05 00:01:28 10.207.32.142 POST /Services/Activity/Track/ScormService.asmx/SetData - 80 RU\\ekaterinaklimova 10.207.32.141 Mozilla/5.0+(Windows+NT+10.0;+Win64;+x64)+AppleWebKit/537.36+(KHTML,+like+Gecko)+Chrome/98.0.4758.102+Safari/537.36+Edg/98.0.1108.62 http://applications.ru.kworld.kpmg.com/LMS/Tracks/RM/ActingWithIntegrity/2021new/Module/index_lms.html 200 0 0 140 10.207.222.232",
                    },
                    "POST\t2\r\nGET\t1\r\n"
                    ,new Options{
                        Skip = 4,
                        //ExtractExpression = ".*\t(POST|GET)\t.*",
                        IsGrouping = true,
                        GetColumns = "3",
                        IsBySplit = true,

                    },new Regex(@".*")
                };

            }
        }

        [DataTestMethod()]
        [DynamicData(nameof(TextsReal), DynamicDataSourceType.Property)]
        public void WriteLineTest_realCase_imitation_one_File(string[] list, string expected, Options options, Regex reg)
        {
            TextReader sr = new TextReaderTest(list);
            TextWriter sw = new TextWriterTest();
            Program.groups.Clear();
            Program.WriteLineStream("sourcePath", reg, options, sr, sw);

            if (options.IsGrouping)
            {
                Program.CollectStream(sw, options);
            }

            var result = sw.ToString();

            Assert.AreEqual(expected, result);
        }

        private static IEnumerable<object[]> FilteredByContainsSource
        {
            get
            {
                yield return new object[] { "123", new string[] { "1" }, null, true };
                yield return new object[] { "345", null, new string[] { "4" }, false };
                yield return new object[] { "678", null, new string[] { "5", "7" }, false };
                yield return new object[] { "901", null, new string[] { "5" }, true };
                yield return new object[] { "902", new string[] { "5" }, new string[] { "5" }, false };
                //yield return new object[] { "2021-12-05 23:59:30 10.207.32.142 POST /Services/Activity/Track/ScormService.asmx/SetData - 80 - 10.207.32.141 Mozilla/5.0+(Windows+NT+10.0;+Win64;+x64)+AppleWebKit/537.36+(KHTML,+like+Gecko)+Chrome/96.0.4664.55+Safari/537.36+Edg/96.0.1054.43 http://applications.ru.kworld.kpmg.com/LMS/Tracks/RM/ActingWithIntegrity/2021new/Module/index_lms.html 401 2 5 0 10.207.220.121", new string[] { "5" }, new string[] { "5" }, false };
            }
        }

        [DataTestMethod()]
        [DynamicData(nameof(FilteredByContainsSource), DynamicDataSourceType.Property)]
        public void FilteredByContainsTest(string line, string[] contains, string[] notContains, bool excepted)
        {
            var result = Program.FilteredByContains(line, contains, notContains);

            Assert.AreEqual(excepted, result);

        }
    }
}