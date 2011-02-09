/*
  Copyright (c) 2008 Jeff Moser <jeff@moserware.com>
 
  Portions derived from JavaScript code that is:
  Copyright (c) 2007, 2008 Alessandro Warth <awarth@cs.ucla.edu>
  
  Permission is hereby granted, free of charge, to any person
  obtaining a copy of this software and associated documentation
  files (the "Software"), to deal in the Software without
  restriction, including without limitation the rights to use,
  copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the
  Software is furnished to do so, subject to the following
  conditions:

  The above copyright notice and this permission notice shall be
  included in all copies or substantial portions of the Software.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
  EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
  OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
  NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
  HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
  WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
  FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
  OTHER DEALINGS IN THE SOFTWARE.
*/

namespace OMetaSharp
{
    /// <summary>
    /// A grammar that is useful in being the basis of most derived grammars. It contains
    /// rules that are common across many languages.
    /// </summary>
    /// <typeparam name="T">Type of the underlying OMeta stream.</typeparam>
    /// <remarks>
    /// This class was derived from Alessandro's OMeta/JS version that is at:
    /// http://jarrett.cs.ucla.edu/ometa-js/parser.js
    /// </remarks>
    public class Parser<TInput> : OMeta<TInput>
    {
        public virtual bool ListOf(OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream)
        {
            OMetaList<HostExpression> rule;
            OMetaList<HostExpression> delim;

            if (!MetaRules.Apply(Anything, inputStream, out rule, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }

            if (!MetaRules.Apply(Anything, modifiedStream, out delim, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }

            return MetaRules.Or(modifiedStream, out result, out modifiedStream,
                delegate(OMetaStream<TInput> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream<TInput> modifiedStream2)
                {
                    OMetaList<HostExpression> r;
                    Rule<TInput> actualRule;

                    if (!TryGetMethod(rule, out actualRule))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }

                    if (!MetaRules.Apply(actualRule, inputStream2, out r, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }


                    if (!MetaRules.Many(
                        delegate(OMetaStream<TInput> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream<TInput> modifiedStream3)
                        {
                            if (!MetaRules.ApplyWithArgs(Token, inputStream3, out result3, out modifiedStream3, delim))
                            {
                                return MetaRules.Fail(out result3, out modifiedStream3);
                            }

                            return MetaRules.Apply(actualRule, modifiedStream3, out result3, out modifiedStream3);
                        },
                        modifiedStream2,
                        out result2,
                        out modifiedStream2,
                        r))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }

                    return MetaRules.Success();
                },
                delegate(OMetaStream<TInput> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream<TInput> modifiedStream2)
                {
                    result2 = OMetaList<HostExpression>.Nil;
                    modifiedStream2 = inputStream2;
                    return MetaRules.Success();
                });    
        }

        public virtual bool Token(OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream)
        {
            OMetaList<HostExpression> cs;

            if(!MetaRules.Apply(Anything, inputStream, out cs, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }            
            
            if (!MetaRules.Apply(Spaces, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }

            if (!MetaRules.ApplyWithArgs(Seq, modifiedStream, out result, out modifiedStream, cs))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }

            return MetaRules.Success();
        }        
    }

    public class Parser : Parser<char>
    {
    }
}
