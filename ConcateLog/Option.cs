using CommandLine;

namespace ConcatFiles
{
    public class Options
    {
        [Option('s', "Source", Required = false, HelpText = "шаблон по которому ищутся источники файлов")]
        public string Source { get; set; }
        [Option('f', "File", Required = false, HelpText = "чтение из файла списка путей для объединения")]
        public string File { get; set; }

        [Option('r', "Result", Required = false, HelpText = "файл в который записываются результаты")]
        public string Result { get; set; }

        [Option('p', "Pattern", Required = false, HelpText = "патерн по которму происходит склейка в одну строку")]
        public string Pattern { get; set; }

        [Option('1', "def1", Required = false, HelpText = " патерн по умолчанию 1 - \"^\\d+-\\d+-\\d+ \\d+:\\d+:\\d+.\\d+.*\" (Использовать только полное название параметра)")]
        public bool DefaultPatern1 { get; set; }

        [Option('q', "Query", Required = false, HelpText = "патерн по которму происходит поиск файлов в заданной папке с помощью регулярного выражения")]
        public string Query { get; set; }

        [Option('t', "Template", Required = false, HelpText = "патерн по которму происходит поиск файлов в заданной папке с помощью стандартного шаблона")]
        public string Template { get; set; }

        [Option('k', "skip", Required = false, HelpText = "указывает сколько строк нужно пропустить каждого файла (если есть заголовок)")]
        public int Skip { get; set; }

        [Option("take", Required = false, HelpText = "сколько строк прочитать N")]
        public int? Take { get; set; }

        [Option('e', "extract", Required = false, HelpText = "Извлекает подстроку по регулярному выражению.")]
        public string ExtractExpression { get; set; }

        [Option('a', "addSubDirectory", Required = false, HelpText = "Если флаг есть, то будут происходить поиск по поддиректориям")]
        public bool AllDirectory { get; set; }

        [Option('d', "isDeleteFileBeforeRun", Required = false, HelpText = "Удаляет результатирующей файл, если есть, перед запуском")]
        public bool IsDeleteFileBeforeRun { get; set; }

        [Option('w', "writePath", Required = false, HelpText = "если флаг есть, то записывается полный путь файла из которого найдено совпадение")]
        public bool IsWritePath { get; set; }
        
        
        [Option('g', "group", Required = false, HelpText = "сгруппировать строки по уникальности и добавляет последним полем количество, внимание! этот флаг увеличивает расход пямяти, так как помещает итоговый результат в пямять")]
        public bool IsGrouping { get; set; }

        [Option("split", Required = false, HelpText = "разбивать строку на табуляцию")]
        public bool IsBySplit { get; set; }

        [Option("cols", Required = false, HelpText = "выбирает колонки можно использовать через зяпятую 0,2")]
        public string GetColumns { get; set; }

        [Option("test", Required = false, HelpText = "выводит на экран, не записывает в файл (только указанных N записей)")]
        public int? TestRow { get; set; }


        [Option("contains", Required = false, HelpText = "фильтр по содержанию строки запятая (,) разделитель фильтров, восклицательный знак (!) отрицание ")]
        public string Contains { get; set; }

        [Option("tolow", Required = false, HelpText = "преобразовать встре строки в нижний регистр")]
        public bool ToLower { get; set; }     
    }
}