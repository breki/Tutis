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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace OMetaSharp
{
    /// <summary>
    /// A rule in the grammar.
    /// </summary>
    public delegate bool Rule<TInput>(OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream);
    
    /// <summary>
    /// This is the core of the implementation. It implements all of the necessary "meta rules"
    /// to get the implementation to work.
    /// </summary>    
    /// <remarks>
    /// Most of this code was translated from Alessandro Warth's OMeta/JS version
    /// that is available at:
    /// 
    /// http://jarrett.cs.ucla.edu/ometa-js/ometa-base.js
    /// </remarks>
    public class OMetaBase<TInput> : IMetaRuleEvaluator<TInput>
    {        
#line hidden
        private readonly Dictionary<string, object> m_VariableDictionary = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);
        private readonly IMetaRuleEvaluator<TInput> m_SelfAsMetaRuleEvaluator;
        public static bool EnableDebugTracing = false;

        // Helps with debugging:        
        protected StackTrace m_LastFailStackTrace = null;
#line default

        [DebuggerStepThrough]
        protected OMetaBase()
        {
            m_SelfAsMetaRuleEvaluator = this as IMetaRuleEvaluator<TInput>;
        }

        // Convenience methods for storing variables while parsing.

        // HACK: It's only returning a variable because the parser semantic action is set as a result. 
        //       Need to fix that.
        [DebuggerStepThrough]
        public virtual TVariable Set<TVariable>(string key, TVariable value)
        {
            // HOTSPOT
            m_VariableDictionary[key] = value;
            return value;
        }
                
        public virtual TVariable Get<TVariable>(string key)
        {
            return (TVariable)m_VariableDictionary[key];
        }
                
        public virtual TVariable Get<TVariable>(string key, TVariable defaultValue)
        {
            object val;
            if (m_VariableDictionary.TryGetValue(key, out val))
            {
                return (TVariable)val;
            }

            return defaultValue;
        }

        protected IMetaRuleEvaluator<TInput> MetaRules
        {
            // Since this getter is called so many times and this method itself isn't too interesting,
            // don't bother going into it.
            [DebuggerStepThrough]
            get
            {
                // HOTSPOT     
                return m_SelfAsMetaRuleEvaluator;
            }
        }
                
        // Note that the result of Anything is a TInput on purpose                                    
        [DebuggerStepThrough]
        public virtual bool Anything(OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream)
        {
            // HOTSPOT

            // Arguments take precendence over actual stream-y stuff.
            if (inputStream.HasArguments)
            {
                modifiedStream = inputStream.PopArgument(out result);
                return true;
            }

            if (inputStream.IsEnd)
            {
                result = null;
                modifiedStream = null;
                return false;
            }
                        
            // HACK
            // SMELL: Should this be special cased?
            if (typeof(TInput).Equals(typeof(HostExpression)))
            {
                result = (OMetaList<HostExpression>)((object)inputStream.Head);
            }
            else
            {
                result = HostExpression.From(inputStream.Head);
            }

            modifiedStream = inputStream.TailStream;
            return MetaRules.Success();
        }

        [DebuggerStepThrough]
        protected virtual bool End(OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream)
        {
            return MetaRules.Not(Anything, inputStream, out result, out modifiedStream);
        }
        
        #region IMetaRuleEvaluator<TInput> Members

        [DebuggerStepThrough]
        bool IMetaRuleEvaluator<TInput>.Success()
        {
            if (EnableDebugTracing)
            {
                m_LastFailStackTrace = null;
            }
            return true;
        }

        [DebuggerStepThrough]
        bool IMetaRuleEvaluator<TInput>.Fail(out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream)
        {
            if (EnableDebugTracing)
            {
                var st = new StackTrace(1, true);
                if (m_LastFailStackTrace == null)
                {
                    m_LastFailStackTrace = st;
                }
            }
            result = null;
            modifiedStream = null;
            return false;
        }

        [DebuggerStepThrough]
        bool IMetaRuleEvaluator<TInput>.Or(OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream, params Rule<TInput>[] rules)
        {
            // HOTSPOT
            // Really small optimization of not using foreach simply because this
            // is called so many times
            for (int ix = 0; ix < rules.Length; ix++)
            {
                Rule<TInput> currentRule = rules[ix];

                if (currentRule(inputStream, out result, out modifiedStream))
                {
                    return MetaRules.Success();
                }
            }

            return MetaRules.Fail(out result, out modifiedStream);
        }

        [DebuggerStepThrough]
        bool IMetaRuleEvaluator<TInput>.Apply(Rule<TInput> rule, OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream)
        {
            // HOTSPOT
            MemoizedResult<TInput> memoizedValue;

            if (!inputStream.TryGetMemoizedResult(rule, out memoizedValue))
            {
                MemoizedResult<TInput> failer = new FailedMemoizedResult<TInput>();
                inputStream.SetMemoizedResult(rule, failer);

                if (!rule(inputStream, out result, out modifiedStream))
                {
                    return MetaRules.Fail(out result, out modifiedStream);
                }

                memoizedValue = new MemoizedResult<TInput>();
                inputStream.SetMemoizedResult(rule, memoizedValue);

                memoizedValue.Answer = result;
                memoizedValue.NextInputStream = modifiedStream;

                if (failer.HasBeenUsed)
                {
                    OMetaStream<TInput> sentinel = modifiedStream;

                    while (true)
                    {
                        OMetaList<HostExpression> nestedAnswer;

                        OMetaStream<TInput> nestedModifiedStream;

                        if (!rule(inputStream, out nestedAnswer, out nestedModifiedStream))
                        {
                            break;
                        }

                        if (nestedModifiedStream == sentinel)
                        {
                            break;
                        }

                        memoizedValue.Answer = nestedAnswer;
                        memoizedValue.NextInputStream = nestedModifiedStream;
                    }
                }
            }
            else if (memoizedValue is FailedMemoizedResult<TInput>)
            {
                memoizedValue.HasBeenUsed = true;
                return MetaRules.Fail(out result, out modifiedStream);
            }

            modifiedStream = memoizedValue.NextInputStream;
            result = memoizedValue.Answer;
            return MetaRules.Success();
        }

        [DebuggerStepThrough]
        bool IMetaRuleEvaluator<TInput>.Not(Rule<TInput> rule, OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream)
        {
            if (rule(inputStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }

            result = OMetaList<HostExpression>.Nil;
            modifiedStream = inputStream;
            return MetaRules.Success();        
        }

        bool IMetaRuleEvaluator<TInput>.Lookahead(Rule<TInput> rule, OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream)
        {
            if (!MetaRules.Apply(rule, inputStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }

            // don't touch the input
            modifiedStream = inputStream;
            return MetaRules.Success();
        }

        bool IMetaRuleEvaluator<TInput>.SuperApplyWithArgs(OMeta<TInput> superClass,
                                                           Rule<TInput> rule,
                                                           OMetaStream<TInput> inputStream,
                                                           out OMetaList<HostExpression> result,
                                                           out OMetaStream<TInput> modifiedStream)
        {
            // REVIEW: Should this even be needed given that rules can refer to their base class?
            //for (int ixArg = arguments.Count - 1; ixArg >= 0; ixArg--)
            //{
            //    inputStream = new OMetaStream<TInner>(new OMetaList<TInner>(arguments[ixArg], inputStream.AsList()));
            //}

            return superClass.MetaRules.Apply(rule, inputStream, out result, out modifiedStream);
        }

        
        bool IMetaRuleEvaluator<TInput>.Form(Rule<TInput> actionRule,
                                               OMetaStream<TInput> inputStream,
                                               out OMetaList<HostExpression> result,
                                               out OMetaStream<TInput> modifiedStream
                                               )
#line hidden
        {        
            OMetaList<HostExpression> v;

            if (!MetaRules.Apply(Anything, inputStream, out v, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }

            var currentItemAsList = v as OMetaList<TInput>;

            if (currentItemAsList == null)
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }

            OMetaStream<TInput> input = new OMetaStream<TInput>(currentItemAsList);

            OMetaStream<TInput> throwAwayStream;
            OMetaList<HostExpression> throwAwayResult;
            OMetaList<HostExpression> r;
#line default

            if (!MetaRules.Apply(actionRule, input, out r, out throwAwayStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
                        
            result = v;

            // Make sure that the rule consumed all the inner input

            if (!MetaRules.Apply(End, throwAwayStream, out throwAwayResult, out throwAwayStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }

            return MetaRules.Success();
        }

        [DebuggerStepThrough]
        bool IMetaRuleEvaluator<TInput>.Many(Rule<TInput> rule, OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream)
        {
            return MetaRules.Many(rule, inputStream, out result, out modifiedStream, null);
        }

        [DebuggerStepThrough]
        bool IMetaRuleEvaluator<TInput>.Many(Rule<TInput> rule, OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream, OMetaList<HostExpression> argument)
        {               
            OMetaStream<TInput> currentStream = inputStream;

            var allResults = new List<OMetaList<HostExpression>>();

            if ((argument != null) && (argument != OMetaList<HostExpression>.Nil))
            {
                allResults.Add(argument);
            }

            while (true)
            {
                OMetaList<HostExpression> currentResult;
                OMetaStream<TInput> currentOut;

                if (!rule(currentStream, out currentResult, out currentOut))
                {
                    break;
                }

                allResults.Add(currentResult);                                
                currentStream = currentOut;
            }

            result = OMetaList<HostExpression>.ConcatLists(allResults.ToArray());

            modifiedStream = currentStream;

            return MetaRules.Success();
        }

        [DebuggerStepThrough]
        bool IMetaRuleEvaluator<TInput>.Many1(Rule<TInput> rule, OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream)
        {
            if (!MetaRules.Apply(rule, inputStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }

            if (!MetaRules.Many(rule, modifiedStream, out result, out modifiedStream, result))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            
            return MetaRules.Success();
        }

        [DebuggerStepThrough]
        bool IMetaRuleEvaluator<TInput>.ApplyWithArgs(Rule<TInput> rule, OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream, params OMetaList<HostExpression>[] arguments)
        {
            // HACK
            Debug.Assert(arguments.Length > 0);
            OMetaList<HostExpression> actualArgs;

            if (arguments.Length == 1)
            {
                actualArgs = (arguments[0].Count > 1) ? new OMetaList<HostExpression>(arguments[0]) : arguments[0];
            }
            else
            {
                actualArgs = OMetaList<HostExpression>.ConcatLists(arguments);
            }

            OMetaStream<TInput> streamWithArguments = inputStream.PushArguments(actualArgs);
            return MetaRules.Apply(rule, streamWithArguments, out result, out modifiedStream);
        }
            
        #endregion
    }
}
