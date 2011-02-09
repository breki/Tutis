using System.Collections.Generic;

namespace LucenePlaying
{
    public class CamelCaseWordBreaker
    {
        public IList<CamelCaseTokenPart> BreakIntoWords(CamelCaseTokenPart token)
        {
            this.token = token;
            words.Clear();
            i = 0;
            partStartIndex = 0;
            bool isLastLowercase = false;
            while(i <= token.Text.Length)
            {
                if (i == token.Text.Length)
                {
                    AddNewPartIfPossible();
                    break;
                }

                char c = token.Text[i];

                if (i == partStartIndex)
                {
                    i++;

                    if (c == '_')
                    {
                        isLastLowercase = false;
                        partStartIndex++;
                    }
                    else
                        isLastLowercase = char.IsLower(c);
                    continue;
                }

                if (char.IsUpper(c) && isLastLowercase)
                    AddNewPartIfPossible();
                else if (c == '_')
                {
                    AddNewPartIfPossible();
                    i++;
                    partStartIndex++;
                }
                else
                {
                    i++;
                    isLastLowercase = char.IsLower(c);                    
                }
            }

            return words;
        }

        private void AddNewPartIfPossible()
        {
            CamelCaseTokenPart part = token.CreatePart(partStartIndex, i);

            if (part != null)
                words.Add(part);

            partStartIndex = i;
        }

        private int partStartIndex;
        private int i;
        private List<CamelCaseTokenPart> words = new List<CamelCaseTokenPart>();
        private CamelCaseTokenPart token;
    }
}