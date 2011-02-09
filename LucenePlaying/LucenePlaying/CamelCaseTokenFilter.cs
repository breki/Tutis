using System;
using System.Collections.Generic;
using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;

namespace LucenePlaying
{
    public class CamelCaseTokenFilter : TokenFilter
    {
        public CamelCaseTokenFilter(TokenStream input) : base(input)
        {
            offsetAtt = (OffsetAttribute)AddAttribute(typeof(OffsetAttribute));
            termAtt = (TermAttribute)AddAttribute(typeof(TermAttribute));
        }

        public override bool IncrementToken()
        {
            ClearAttributes();

            if (wordCounter >= words.Count)
            {
                TermAttribute innerTermAttribute;
                string termText;
                while (true)
                {
                    if (false == input.IncrementToken())
                        return false;

                    innerTermAttribute = (TermAttribute)input.GetAttribute(typeof(TermAttribute));
                    termText = innerTermAttribute.Term();

                    char startingChar = termText[0];
                    if (!char.IsNumber(startingChar))
                        break;
                }

                OffsetAttribute innerOffsetAtt = (OffsetAttribute)input.GetAttribute(typeof(OffsetAttribute));
                int offset = innerOffsetAtt.StartOffset();

                words = wordBreaker.BreakIntoWords(
                    new CamelCaseTokenPart(termText, offset));
                wordCounter = 0;

                termAtt.SetTermBuffer(
                    innerTermAttribute.TermBuffer(), 
                    0,
                    innerTermAttribute.TermLength());
                offsetAtt.SetOffset(innerOffsetAtt.StartOffset(), innerOffsetAtt.EndOffset());

                // if the token was broken into a single word, we don't need it
                if (words.Count == 1)
                    wordCounter++;
            }
            else
            {
                CamelCaseTokenPart word = words[wordCounter++];
                termAtt.SetTermBuffer(word.Text);
                offsetAtt.SetOffset(word.Offset, word.Offset + word.Text.Length);
            }

            return true;
        }

        private CamelCaseWordBreaker wordBreaker = new CamelCaseWordBreaker();
        private IList<CamelCaseTokenPart> words = new List<CamelCaseTokenPart>();
        private int wordCounter;
        private OffsetAttribute offsetAtt;
        private TermAttribute termAtt;
    }
}