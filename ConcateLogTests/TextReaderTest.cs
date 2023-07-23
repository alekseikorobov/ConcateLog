namespace ConcatFiles.Tests
{
    public class TextReaderTest : System.IO.TextReader
    {
        int pisition = 0;
        public TextReaderTest(string[] list)
        {
            List = list;
        }

        public string[] List { get; }

        public override string ReadLine()
        {
            if (List.Length <= pisition) return null;

            return List[pisition++];
        }
    }
}