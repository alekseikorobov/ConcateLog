using CommandLine;

namespace ConcatFiles
{
    internal class Options
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
        public bool DefoultPatern1 { get; set; }

        [Option('q', "Query", Required = false, HelpText = "патерн по которму происходит поиск файлов в заданной папке с помощью регулярного выражения")]
        public string Query { get; set; }

        [Option('t', "Template", Required = false, HelpText = "патерн по которму происходит поиск файлов в заданной папке с помощью стандартного шаблона")]
        public string Template { get; set; }

        [Option('k', "Skip", Required = false, HelpText = "указывает нужно ли пропустить первую срочку каждого файла (если есть заголовок)")]
        public bool Skip { get; set; }

        [Option('e', "extract", Required = false, HelpText = "Извлекает подстроку по регулярному выражению.")]
        public string ExtractExpression { get; set; }

        [Option('a', "addSubDirectory", Required = false, HelpText = "Если флаг есть, то будут происходить поиск по поддиректориям")]
        public bool AllDirectory { get; internal set; }
    }
}