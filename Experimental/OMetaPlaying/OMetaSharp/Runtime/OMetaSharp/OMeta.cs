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
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace OMetaSharp
{
    /// <summary>
    /// Extends the core OMeta functionality by adding in some prebuilt rules.
    /// </summary>
    /// <typeparam name="T">Type of the underlying OMeta stream.</typeparam>
    /// <remarks>
    /// All of the rules are virtual so that they can be overridden by derived classes.
    /// </remarks>
    public class OMeta<TInput> : OMetaBase<TInput>
    {
        private Dictionary<Type, Dictionary<string, MethodInfo>> m_MethodCache;
        private Dictionary<Type, object> m_ForeignGrammarCache;
        private Type m_SelfType;

        [DebuggerStepThrough]
        public OMeta()
        {
            m_SelfType = this.GetType();
        }

        public virtual bool Exactly(OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream)
        {
            // Input stream has arguments on the argument stack.
            // Remember that everything is immutable, so "popping" the argument stack will
            // create a new stream.

            OMetaList<HostExpression> argument;
            if (!MetaRules.Apply(Anything, inputStream, out argument, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            
            var argumentAsList = argument.HeadFirstItem.ToList<TInput>();
            result = null;

            while (argumentAsList != OMetaList<TInput>.Nil)
            {
                var currentItem = argumentAsList.Head;
                
                if (!MetaRules.Apply(Anything, modifiedStream, out result, out modifiedStream))
                {
                    return MetaRules.Fail(out result, out modifiedStream);
                }

                if (!currentItem.Equals(result))
                {
                    return MetaRules.Fail(out result, out modifiedStream);
                }

                argumentAsList = argumentAsList.Tail;
            }
            
            return MetaRules.Success();
        }

        public virtual bool Character(OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream)
        {            
            if (!Anything(inputStream, out result, out modifiedStream))
            {                
                return false;
            }

            return SingleItemMatches(result, e => e.Is<char>());            
        }

        public virtual bool Lower(OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream)
        {
            return IsCharacterType(inputStream, 
                                   out result, 
                                   out modifiedStream, 
                                   (char c) => Char.IsLower(c));
        }

        public virtual bool Upper(OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream)
        {
            return IsCharacterType(inputStream, 
                                   out result, 
                                   out modifiedStream, 
                                   c => Char.IsUpper(c));
        }

        public virtual bool Letter(OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream)
        {
            return IsCharacterType(inputStream, 
                                   out result, 
                                   out modifiedStream, 
                                   c => (Char.IsLower(c) || Char.IsUpper(c)));
        }
    
        public virtual bool LetterOrDigit(OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream)
        {
            return MetaRules.Or(inputStream, out result, out modifiedStream, 
                       Letter, Digit);
        }
    
        public virtual bool FirstAndRest(OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream)
        {
            OMetaList<HostExpression> first;
            OMetaList<HostExpression> rest;

            if (!MetaRules.Apply(Anything, inputStream, out first, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }

            if (!MetaRules.Apply(Anything, modifiedStream, out rest, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
                                    
            Rule<TInput> firstRule;
            OMetaList<HostExpression> firstResult;

            if (!TryGetMethod(first, out firstRule)
                ||
                !MetaRules.Apply(firstRule, modifiedStream, out firstResult, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }

            Rule<TInput> restRule;

            if (!TryGetMethod(rest, out restRule))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }

            if (!MetaRules.Many(
                    delegate(OMetaStream<TInput> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream<TInput> modifiedStream2)
                    {
                        if (!MetaRules.Apply(restRule, inputStream2, out result2, out modifiedStream2))
                        {
                            return MetaRules.Fail(out result2, out modifiedStream2);
                        }

                        return MetaRules.Success();
                    },
                    modifiedStream,
                    out result,
                    out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }

            result = new OMetaList<HostExpression>(firstResult, result);

            // HACK
            string compressed;
            if (result.TryHostExpressionCompressionOnString(out compressed))
            {
                result = compressed.AsHostExpressionList();
            }

            return MetaRules.Success();
        }

        public virtual bool NotLast(OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream)
        {
            OMetaList<HostExpression> rule;
            OMetaList<HostExpression> r;

            if (!MetaRules.Apply(Anything, inputStream, out rule, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }

            Rule<TInput> actualRule;

            if (!TryGetMethod(rule, out actualRule))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }

            if (!MetaRules.Apply(actualRule, modifiedStream, out r, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }

            if (!MetaRules.Lookahead(
                delegate(OMetaStream<TInput> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream<TInput> modifiedStream2)
                {
                    return MetaRules.Apply(actualRule, inputStream2, out result2, out modifiedStream2);
                },
                modifiedStream,
                out result,
                out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }

            result = r;
            return MetaRules.Success();            
        }
          
        public virtual bool Digit(OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream)
        {
            if (!Character(inputStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }

            return SingleItemMatches(
                result,
                e => e.Satisfies<char>(c => ((c >= '0') && (c <= '9'))));
        }

        protected virtual bool SingleItemMatches(OMetaList<HostExpression> result, Predicate<HostExpression> test)
        {
            if (!result.IsSingleItem)
            {
                return false;
            }

            HostExpression h = (HostExpression)result;

            return test(h);
        }

        public virtual bool Number(OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream)
        {
            if (!Anything(inputStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }

            return SingleItemMatches(
                result,
                e =>
                    (e.Is<int>()
                     ||
                     e.Is<long>()
                     ||
                     e.Satisfies<char>(c => char.IsNumber(c))));
        }

        public virtual bool String(OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream)
        {
            OMetaList<HostExpression> r;
            if (!MetaRules.Apply(Anything, inputStream, out r, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }

            result = r;

            return SingleItemMatches(result,
                                     e=> e.Is<char>() || e.Is<string>());            
        }

        public virtual bool Space(OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream)
        {
            if (!Character(inputStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }

            return MatchesPredicate(Char.IsWhiteSpace(result.HeadFirstItem.As<char>()));            
        }

        public virtual bool Spaces(OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream)
        {
            return MetaRules.Many(Space, inputStream, out result, out modifiedStream);
        }

        /// <summary>
        /// Grabs arguments off the stack and ensures that the next input items are equal to that.
        /// </summary>        
        public virtual bool Seq(OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream)
        {
            OMetaList<HostExpression> arguments;
            if (!MetaRules.Apply(Anything, inputStream, out arguments, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }            

            // REVIEW: What's the best result? This is sort of a placeholder.
            result = arguments;

            // SMELL: This is slightly different than OMeta/JS in that we allow multiple arguments            
            foreach (OMetaList<HostExpression> currentArgument in arguments)
            {
                // The "Exactly" production is what converts the host expression to a list of input
                OMetaList<HostExpression> throwAwayResult;
                if (!MetaRules.ApplyWithArgs(Exactly, modifiedStream, out throwAwayResult, out modifiedStream, currentArgument))
                {
                    return MetaRules.Fail(out result, out modifiedStream);
                }
            }

            return MetaRules.Success();
        }

        [DebuggerStepThrough]
        protected virtual OMetaList<HostExpression> GenericMatch(OMetaStream<TInput> inputStream, Rule<TInput> rule, OMetaList<HostExpression>[] arguments, out OMetaStream<TInput> modifiedStream)
        {
            OMetaList<HostExpression> result;
        
            if (arguments == null)
            {
                if (!MetaRules.Apply(rule, inputStream, out result, out modifiedStream))
                {
                    throw new OMetaException((m_LastFailStackTrace != null) ? m_LastFailStackTrace.ToString() : "Failure and no stack trace exists");
                }
            }
            else
            {
                if (!MetaRules.ApplyWithArgs(rule, inputStream, out result, out modifiedStream, arguments))
                {
                    throw new OMetaException((m_LastFailStackTrace != null) ? m_LastFailStackTrace.ToString() : "Failure and no stack trace exists");
                }
            }

            return result;
        }

        [DebuggerStepThrough]
        protected virtual OMetaList<HostExpression> GenericMatch(OMetaStream<TInput> inputStream, Rule<TInput> rule, OMetaList<HostExpression>[] arguments)
        {
            OMetaStream<TInput> modifiedStream;
            return GenericMatch(inputStream, rule, arguments, out modifiedStream);
        }

        [DebuggerStepThrough]
        public virtual OMetaList<HostExpression> Match(OMetaStream<TInput> inputStream, Rule<TInput> rule)
        {
            return GenericMatch(inputStream, rule, null);
        }

        [DebuggerStepThrough]
        public virtual OMetaList<HostExpression> Match(OMetaStream<TInput> inputStream, Rule<TInput> rule, out OMetaStream<TInput> modifiedStream)
        {
            return GenericMatch(inputStream, rule, null, out modifiedStream);
        }

        [DebuggerStepThrough]
        public virtual OMetaList<HostExpression> Match(OMetaStream<TInput> inputStream, Rule<TInput> rule, OMetaList<HostExpression>[] arguments)
        {
            return GenericMatch(inputStream, rule, arguments);
        }

        public virtual TResult Match<TResult>(OMetaStream<TInput> inputStream, Rule<TInput> rule)
        {
            return Match(inputStream, rule).As<TResult>();
        }
        
        public virtual TResult Match<TResult>(OMetaStream<TInput> inputStream, Rule<TInput> rule, OMetaList<HostExpression> argument)
        {
            return Match<TResult>(inputStream, rule, new OMetaList<HostExpression>[] { argument });
        }

        public virtual TResult Match<TResult>(OMetaStream<TInput> inputStream, Rule<TInput> rule, OMetaList<HostExpression>[] arguments)
        {
            return Match(inputStream, rule, arguments).As<TResult>();
        }

        public virtual bool Apply(OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream)
        {
            OMetaList<HostExpression> rule;
            if (!MetaRules.Apply(Anything, inputStream, out rule, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }

            Rule<TInput> actualRule;

            if (!TryGetMethod(rule, out actualRule))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }

            return MetaRules.Apply(actualRule, modifiedStream, out result, out modifiedStream);
        }

        public virtual bool Empty(OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream)
        {
            result = OMetaList<HostExpression>.Nil;
            modifiedStream = inputStream;
            return MetaRules.Success();
        }
                
        // HACK: This is different than OMeta/JS to make things more typesafe.. might need to revisit
        public virtual bool Foreign(OMeta<TInput> foreignGrammar, Func<OMeta<TInput>, Rule<TInput>> foreignRuleFetcher, OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream)
        {
            OMetaList<HostExpression> ans;
            Debug.Assert(foreignGrammar != null);

            Rule<TInput> foreignRule = foreignRuleFetcher(foreignGrammar);

            var proxyStream = new OMetaProxyStream<TInput>(inputStream);

            modifiedStream = proxyStream.TargetStream;

            if (!foreignGrammar.MetaRules.Apply(foreignRule, proxyStream, out ans, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            
            result = ans;
            return MetaRules.Success();
        }

        // This is the more traditional approach that I'm slowly converging to.
        public virtual bool Foreign(OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream)
        {
            OMetaList<HostExpression> grammar;
            if (!MetaRules.Apply(Anything, inputStream, out grammar, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }

            Type typedGrammar = grammar.As<Type>();

            if (m_ForeignGrammarCache == null)
            {
                m_ForeignGrammarCache = new Dictionary<Type, object>();
            }

            object grammarInstance = null;

            if (typedGrammar == m_SelfType)
            {
                grammarInstance = this;
            }
            else
            {
                if (!m_ForeignGrammarCache.TryGetValue(typedGrammar, out grammarInstance))
                {
                    grammarInstance = Activator.CreateInstance(typedGrammar);
                    m_ForeignGrammarCache[typedGrammar] = grammarInstance;
                }
            }

            OMetaList<HostExpression> rule;
            if (!MetaRules.Apply(Anything, modifiedStream, out rule, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }

            var ruleName = rule.As<string>();

            Rule<TInput> foreignRule;

            if (!TryGetMethod(typedGrammar, ruleName, out foreignRule))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }

            var proxyStream = new OMetaProxyStream<TInput>(modifiedStream);

            OMeta<TInput> foreignGrammar = grammarInstance as OMeta<TInput>;

            OMetaList<HostExpression> ans;

            if (!foreignGrammar.MetaRules.Apply(foreignRule, proxyStream, out ans, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            
            result = ans;
            return MetaRules.Success();
        }


        protected virtual bool MatchesPredicate(bool predicate)
        {
            return predicate;
        }

        protected bool TryGetMethod(Type type, string methodName, out Rule<TInput> ruleResult)
        {
            if (m_MethodCache == null)
            {
                m_MethodCache = new Dictionary<Type, Dictionary<string, MethodInfo>>();
            }

            Dictionary<string, MethodInfo> methodInfoDict = null;

            if (!m_MethodCache.TryGetValue(type, out methodInfoDict))
            {
                methodInfoDict = new Dictionary<string, MethodInfo>(StringComparer.Ordinal);
                m_MethodCache[type] = methodInfoDict;
            }

            // HOTSPOT
            MethodInfo methodInfo;

            if (!methodInfoDict.TryGetValue(methodName, out methodInfo))
            {
                methodInfo =
                type.GetMethod(
                    methodName,
                    new Type[] 
                    { 
                        typeof(OMetaStream<TInput>),
                        typeof(OMetaList<HostExpression>).MakeByRefType(),
                        typeof(OMetaStream<TInput>).MakeByRefType()
                    });

                methodInfoDict[methodName] = methodInfo;
            }

            if (methodInfo == null)
            {
                ruleResult = null;
                return false;
            }
#line hidden
            ruleResult = delegate(OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream)
            {
                result = null;
                modifiedStream = null;
                object[] methodParams = new object[] { inputStream, result, modifiedStream };

                object grammarInstance = null;

                if (type == m_SelfType)
                {
                    // Optimize the common case.
                    grammarInstance = this;
                }
                else
                {
                    if (!m_ForeignGrammarCache.TryGetValue(type, out grammarInstance))
                    {
                        grammarInstance = Activator.CreateInstance(type);
                        m_ForeignGrammarCache[type] = grammarInstance;
                    }
                }

                bool invokeResult = (bool)methodInfo.Invoke(grammarInstance, methodParams);
                result = methodParams[1] as OMetaList<HostExpression>;
                modifiedStream = (OMetaStream<TInput>)methodParams[2];
                return invokeResult;
            };
#line hidden

            return ruleResult != null;
        }

        protected bool HasMethod(OMetaList<HostExpression> methodNameList)
        {
            Rule<TInput> ruleResult;
            return TryGetMethod(methodNameList, out ruleResult);
        }

        protected bool TryGetMethod(OMetaList<HostExpression> methodNameList, out Rule<TInput> ruleResult)
        {
            return TryGetMethod(m_SelfType, methodNameList.As<string>(), out ruleResult);            
        }

        private bool IsCharacterType(OMetaStream<TInput> inputStream,
                                     out OMetaList<HostExpression> result,
                                     out OMetaStream<TInput> modifiedStream,
                                     Predicate<char> predicate)
        {

            if (!MetaRules.Apply(Character, inputStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }

            if (!MatchesPredicate(predicate(result.As<char>())))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }

            return MetaRules.Success();
        }
    }

    /// <summary>
    /// By default, we'll assume that we want to operate on a stream of characters.
    /// </summary>
    public class OMeta : OMeta<char>
    {
    }
}
