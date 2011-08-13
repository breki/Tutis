using System.IO;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;

namespace LucenePlaying
{
    public class SourceCodeCharTokenizer : Tokenizer
    {
        public SourceCodeCharTokenizer(TextReader reader) : base(reader)
        {
            offset = 0;
            bufferIndex = 0;
            dataLen = 0;
            ioBuffer = new char[0x1000];
            offsetAtt = (OffsetAttribute)AddAttribute(typeof(OffsetAttribute));
            termAtt = (TermAttribute)AddAttribute(typeof(TermAttribute));
        }

        public override void End()
        {
            int finalOffset = CorrectOffset(this.offset);
            offsetAtt.SetOffset(finalOffset, finalOffset);
        }

        public override bool IncrementToken()
        {
            ClearAttributes();

            int length = 0;
            int start = bufferIndex;
            char[] buffer = termAtt.TermBuffer();

            while (true)
            {
                if (bufferIndex >= dataLen)
                {
                    offset += dataLen;
                    dataLen = input.Read(ioBuffer, 0, ioBuffer.Length);
                    if (dataLen <= 0)
                    {
                        dataLen = 0;
                        if (length <= 0)
                            return false;

                        break;
                    }

                    bufferIndex = 0;
                }

                char c = ioBuffer[bufferIndex++];
                if (IsTokenChar(c))
                {
                    if (length == 0)
                        start = (offset + bufferIndex) - 1;
                    else if (length == buffer.Length)
                        buffer = termAtt.ResizeTermBuffer(1 + length);
                    buffer[length++] = c;

                    if (length == 0xff)
                        break;
                }
                else if (length > 0)
                    break;
            }

            termAtt.SetTermLength(length);
            offsetAtt.SetOffset(CorrectOffset(start), CorrectOffset(start + length));

            return true;
        }

        private bool IsTokenChar(char c)
        {
            return char.IsLetter(c) || char.IsNumber(c) || c == '_';
        }

        public override void Reset(TextReader input)
        {
            base.Reset(input);
            bufferIndex = 0;
            offset = 0;
            dataLen = 0;
        }

        private int bufferIndex;
        private int dataLen;
        private const int IO_BUFFER_SIZE = 0x1000;
        private char[] ioBuffer;
        private const int MAX_WORD_LEN = 0xff;
        private int offset;
        private OffsetAttribute offsetAtt;
        private TermAttribute termAtt;
    }
}