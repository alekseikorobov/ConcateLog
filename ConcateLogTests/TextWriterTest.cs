using System.Text;
using System.IO;

namespace ConcatFiles.Tests
{
    public class TextWriterTest : TextWriter
    {
        public override Encoding Encoding => Encoding.Default;

        StringBuilder sb = new StringBuilder();

        public override void Write(string value)
        {
            sb.Append(value);
        }

        public override string ToString()
        {
            return sb.ToString();
        }


    }
}