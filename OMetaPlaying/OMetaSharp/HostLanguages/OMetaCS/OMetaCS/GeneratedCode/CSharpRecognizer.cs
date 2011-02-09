using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace OMetaSharp
{
    public class CSharpRecognizer : Parser
    {
        public virtual bool ParenExpr(OMetaStream<char> inputStream, out OMetaList<HostExpression> result, out OMetaStream <char> modifiedStream)
        {
            modifiedStream = inputStream;
            if(!MetaRules.ApplyWithArgs(EverythingInside, modifiedStream, out result, out modifiedStream, ("(").AsHostExpressionList(), (")").AsHostExpressionList()))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool Block(OMetaStream<char> inputStream, out OMetaList<HostExpression> result, out OMetaStream <char> modifiedStream)
        {
            modifiedStream = inputStream;
            if(!MetaRules.Or(modifiedStream, out result, out modifiedStream,
            delegate(OMetaStream<char> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <char> modifiedStream2)
            {
                modifiedStream2 = inputStream2;
                if(!MetaRules.Apply(SugaryList, modifiedStream2, out result2, out modifiedStream2))
                {
                    return MetaRules.Fail(out result2, out modifiedStream2);
                }
                return MetaRules.Success();
            }
            ,delegate(OMetaStream<char> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <char> modifiedStream2)
            {
                modifiedStream2 = inputStream2;
                if(!MetaRules.ApplyWithArgs(EverythingInside, modifiedStream2, out result2, out modifiedStream2, ("{").AsHostExpressionList(), ("}").AsHostExpressionList()))
                {
                    return MetaRules.Fail(out result2, out modifiedStream2);
                }
                return MetaRules.Success();
            }
            ))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool SugaryPiece(OMetaStream<char> inputStream, out OMetaList<HostExpression> result, out OMetaStream <char> modifiedStream)
        {
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<char> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <char> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Apply(Spaces, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    if(!MetaRules.Or(modifiedStream2, out result2, out modifiedStream2,
                    delegate(OMetaStream<char> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <char> modifiedStream3)
                    {
                        modifiedStream3 = inputStream3;
                        if(!MetaRules.Apply(IgnoredChunk, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        return MetaRules.Success();
                    }
                    ,delegate(OMetaStream<char> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <char> modifiedStream3)
                    {
                        modifiedStream3 = inputStream3;
                        if(!MetaRules.Apply(BasicName, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        return MetaRules.Success();
                    }
                    ))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool SugaryList(OMetaStream<char> inputStream, out OMetaList<HostExpression> result, out OMetaStream <char> modifiedStream)
        {
            OMetaList<HostExpression> first = null;
            OMetaList<HostExpression> rest = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<char> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <char> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.ApplyWithArgs(Token, modifiedStream2, out result2, out modifiedStream2, ("{").AsHostExpressionList()))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    if(!MetaRules.Apply(SugaryPiece, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    first = result2;
                    if(!MetaRules.Many1(
                        delegate(OMetaStream<char> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <char> modifiedStream3)
                        {
                            modifiedStream3 = inputStream3;
                            if(!MetaRules.Apply(
                                delegate(OMetaStream<char> inputStream4, out OMetaList<HostExpression> result4, out OMetaStream <char> modifiedStream4)
                                {
                                    modifiedStream4 = inputStream4;
                                    if(!MetaRules.ApplyWithArgs(Token, modifiedStream4, out result4, out modifiedStream4, (",").AsHostExpressionList()))
                                    {
                                        return MetaRules.Fail(out result4, out modifiedStream4);
                                    }
                                    if(!MetaRules.Apply(SugaryPiece, modifiedStream4, out result4, out modifiedStream4))
                                    {
                                        return MetaRules.Fail(out result4, out modifiedStream4);
                                    }
                                    return MetaRules.Success();
                                }, modifiedStream3, out result3, out modifiedStream3))
                            {
                                return MetaRules.Fail(out result3, out modifiedStream3);
                            }
                            return MetaRules.Success();
                        }
                    , modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    rest = result2;
                    if(!MetaRules.ApplyWithArgs(Token, modifiedStream2, out result2, out modifiedStream2, ("}").AsHostExpressionList()))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    result2 = ( "Sugar.Cons(" + Sugar.Join(",", first, rest) + ")" ).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool BlockWithSubst(OMetaStream<char> inputStream, out OMetaList<HostExpression> result, out OMetaStream <char> modifiedStream)
        {
            OMetaList<HostExpression> v = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<char> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <char> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Apply(Anything, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    v = result2;
                    result2 = ( Set("HostVars", v.As<VariableSet>()) ).AsHostExpressionList();
                    if(!MetaRules.Apply(Block, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool Expr(OMetaStream<char> inputStream, out OMetaList<HostExpression> result, out OMetaStream <char> modifiedStream)
        {
            modifiedStream = inputStream;
            if(!MetaRules.Or(modifiedStream, out result, out modifiedStream,
            delegate(OMetaStream<char> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <char> modifiedStream2)
            {
                modifiedStream2 = inputStream2;
                if(!MetaRules.Apply(ParenExpr, modifiedStream2, out result2, out modifiedStream2))
                {
                    return MetaRules.Fail(out result2, out modifiedStream2);
                }
                return MetaRules.Success();
            }
            ,delegate(OMetaStream<char> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <char> modifiedStream2)
            {
                modifiedStream2 = inputStream2;
                if(!MetaRules.Apply(
                    delegate(OMetaStream<char> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <char> modifiedStream3)
                    {
                        modifiedStream3 = inputStream3;
                        if(!MetaRules.Apply(Spaces, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        if(!MetaRules.Or(modifiedStream3, out result3, out modifiedStream3,
                        delegate(OMetaStream<char> inputStream4, out OMetaList<HostExpression> result4, out OMetaStream <char> modifiedStream4)
                        {
                            modifiedStream4 = inputStream4;
                            if(!MetaRules.Apply(StringLiteral, modifiedStream4, out result4, out modifiedStream4))
                            {
                                return MetaRules.Fail(out result4, out modifiedStream4);
                            }
                            return MetaRules.Success();
                        }
                        ,delegate(OMetaStream<char> inputStream4, out OMetaList<HostExpression> result4, out OMetaStream <char> modifiedStream4)
                        {
                            modifiedStream4 = inputStream4;
                            if(!MetaRules.Apply(FunctionCall, modifiedStream4, out result4, out modifiedStream4))
                            {
                                return MetaRules.Fail(out result4, out modifiedStream4);
                            }
                            return MetaRules.Success();
                        }
                        ,delegate(OMetaStream<char> inputStream4, out OMetaList<HostExpression> result4, out OMetaStream <char> modifiedStream4)
                        {
                            modifiedStream4 = inputStream4;
                            if(!MetaRules.Apply(BasicName, modifiedStream4, out result4, out modifiedStream4))
                            {
                                return MetaRules.Fail(out result4, out modifiedStream4);
                            }
                            return MetaRules.Success();
                        }
                        ))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        return MetaRules.Success();
                    }, modifiedStream2, out result2, out modifiedStream2))
                {
                    return MetaRules.Fail(out result2, out modifiedStream2);
                }
                return MetaRules.Success();
            }
            ))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool ExprWithSubst(OMetaStream<char> inputStream, out OMetaList<HostExpression> result, out OMetaStream <char> modifiedStream)
        {
            OMetaList<HostExpression> v = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<char> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <char> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Apply(Anything, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    v = result2;
                    result2 = ( Set("HostVars", v.As<VariableSet>()) ).AsHostExpressionList();
                    if(!MetaRules.Apply(Expr, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool EverythingInside(OMetaStream<char> inputStream, out OMetaList<HostExpression> result, out OMetaStream <char> modifiedStream)
        {
            OMetaList<HostExpression> x = null;
            OMetaList<HostExpression> y = null;
            OMetaList<HostExpression> xs = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<char> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <char> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Apply(Anything, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    x = result2;
                    if(!MetaRules.Apply(Anything, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    y = result2;
                    if(!MetaRules.ApplyWithArgs(Token, modifiedStream2, out result2, out modifiedStream2, (x).AsHostExpressionList()))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    if(!MetaRules.Many(
                        delegate(OMetaStream<char> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <char> modifiedStream3)
                        {
                            modifiedStream3 = inputStream3;
                            if(!MetaRules.Apply(
                                delegate(OMetaStream<char> inputStream4, out OMetaList<HostExpression> result4, out OMetaStream <char> modifiedStream4)
                                {
                                    modifiedStream4 = inputStream4;
                                    if(!MetaRules.Not(
                                        delegate(OMetaStream<char> inputStream5, out OMetaList<HostExpression> result5, out OMetaStream <char> modifiedStream5)
                                        {
                                            modifiedStream5 = inputStream5;
                                            if(!MetaRules.ApplyWithArgs(Seq, modifiedStream5, out result5, out modifiedStream5, (y).AsHostExpressionList()))
                                            {
                                                return MetaRules.Fail(out result5, out modifiedStream5);
                                            }
                                            return MetaRules.Success();
                                        }
                                    , modifiedStream4, out result4, out modifiedStream4))
                                    {
                                        return MetaRules.Fail(out result4, out modifiedStream4);
                                    }
                                    if(!MetaRules.Or(modifiedStream4, out result4, out modifiedStream4,
                                    delegate(OMetaStream<char> inputStream5, out OMetaList<HostExpression> result5, out OMetaStream <char> modifiedStream5)
                                    {
                                        modifiedStream5 = inputStream5;
                                        if(!MetaRules.Apply(IgnoredChunk, modifiedStream5, out result5, out modifiedStream5))
                                        {
                                            return MetaRules.Fail(out result5, out modifiedStream5);
                                        }
                                        return MetaRules.Success();
                                    }
                                    ,delegate(OMetaStream<char> inputStream5, out OMetaList<HostExpression> result5, out OMetaStream <char> modifiedStream5)
                                    {
                                        modifiedStream5 = inputStream5;
                                        if(!MetaRules.ApplyWithArgs(EverythingMetaInside, modifiedStream5, out result5, out modifiedStream5, (x).AsHostExpressionList(), (y).AsHostExpressionList()))
                                        {
                                            return MetaRules.Fail(out result5, out modifiedStream5);
                                        }
                                        return MetaRules.Success();
                                    }
                                    ))
                                    {
                                        return MetaRules.Fail(out result4, out modifiedStream4);
                                    }
                                    return MetaRules.Success();
                                }, modifiedStream3, out result3, out modifiedStream3))
                            {
                                return MetaRules.Fail(out result3, out modifiedStream3);
                            }
                            return MetaRules.Success();
                        }
                    , modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    xs = result2;
                    if(!MetaRules.ApplyWithArgs(Seq, modifiedStream2, out result2, out modifiedStream2, (y).AsHostExpressionList()))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    result2 = ( Sugar.Implode(xs) ).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool EverythingMetaInside(OMetaStream<char> inputStream, out OMetaList<HostExpression> result, out OMetaStream <char> modifiedStream)
        {
            OMetaList<HostExpression> x = null;
            OMetaList<HostExpression> y = null;
            OMetaList<HostExpression> xs = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<char> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <char> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Apply(Anything, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    x = result2;
                    if(!MetaRules.Apply(Anything, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    y = result2;
                    if(!MetaRules.Or(modifiedStream2, out result2, out modifiedStream2,
                    delegate(OMetaStream<char> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <char> modifiedStream3)
                    {
                        modifiedStream3 = inputStream3;
                        if(!MetaRules.Apply(
                            delegate(OMetaStream<char> inputStream4, out OMetaList<HostExpression> result4, out OMetaStream <char> modifiedStream4)
                            {
                                modifiedStream4 = inputStream4;
                                if(!MetaRules.ApplyWithArgs(EverythingInside, modifiedStream4, out result4, out modifiedStream4, (x).AsHostExpressionList(), (y).AsHostExpressionList()))
                                {
                                    return MetaRules.Fail(out result4, out modifiedStream4);
                                }
                                xs = result4;
                                result4 = ( Sugar.Implode(x, xs, y) ).AsHostExpressionList();
                                return MetaRules.Success();
                            }, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        return MetaRules.Success();
                    }
                    ,delegate(OMetaStream<char> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <char> modifiedStream3)
                    {
                        modifiedStream3 = inputStream3;
                        if(!MetaRules.Apply(
                            delegate(OMetaStream<char> inputStream4, out OMetaList<HostExpression> result4, out OMetaStream <char> modifiedStream4)
                            {
                                modifiedStream4 = inputStream4;
                                if(!MetaRules.Many1(
                                    delegate(OMetaStream<char> inputStream5, out OMetaList<HostExpression> result5, out OMetaStream <char> modifiedStream5)
                                    {
                                        modifiedStream5 = inputStream5;
                                        if(!MetaRules.Apply(
                                            delegate(OMetaStream<char> inputStream6, out OMetaList<HostExpression> result6, out OMetaStream <char> modifiedStream6)
                                            {
                                                modifiedStream6 = inputStream6;
                                                if(!MetaRules.Not(
                                                    delegate(OMetaStream<char> inputStream7, out OMetaList<HostExpression> result7, out OMetaStream <char> modifiedStream7)
                                                    {
                                                        modifiedStream7 = inputStream7;
                                                        if(!MetaRules.Apply(IgnoredChunk, modifiedStream7, out result7, out modifiedStream7))
                                                        {
                                                            return MetaRules.Fail(out result7, out modifiedStream7);
                                                        }
                                                        return MetaRules.Success();
                                                    }
                                                , modifiedStream6, out result6, out modifiedStream6))
                                                {
                                                    return MetaRules.Fail(out result6, out modifiedStream6);
                                                }
                                                if(!MetaRules.Not(
                                                    delegate(OMetaStream<char> inputStream7, out OMetaList<HostExpression> result7, out OMetaStream <char> modifiedStream7)
                                                    {
                                                        modifiedStream7 = inputStream7;
                                                        if(!MetaRules.ApplyWithArgs(Seq, modifiedStream7, out result7, out modifiedStream7, (x).AsHostExpressionList()))
                                                        {
                                                            return MetaRules.Fail(out result7, out modifiedStream7);
                                                        }
                                                        return MetaRules.Success();
                                                    }
                                                , modifiedStream6, out result6, out modifiedStream6))
                                                {
                                                    return MetaRules.Fail(out result6, out modifiedStream6);
                                                }
                                                if(!MetaRules.Not(
                                                    delegate(OMetaStream<char> inputStream7, out OMetaList<HostExpression> result7, out OMetaStream <char> modifiedStream7)
                                                    {
                                                        modifiedStream7 = inputStream7;
                                                        if(!MetaRules.ApplyWithArgs(Seq, modifiedStream7, out result7, out modifiedStream7, (y).AsHostExpressionList()))
                                                        {
                                                            return MetaRules.Fail(out result7, out modifiedStream7);
                                                        }
                                                        return MetaRules.Success();
                                                    }
                                                , modifiedStream6, out result6, out modifiedStream6))
                                                {
                                                    return MetaRules.Fail(out result6, out modifiedStream6);
                                                }
                                                if(!MetaRules.Apply(Anything, modifiedStream6, out result6, out modifiedStream6))
                                                {
                                                    return MetaRules.Fail(out result6, out modifiedStream6);
                                                }
                                                return MetaRules.Success();
                                            }, modifiedStream5, out result5, out modifiedStream5))
                                        {
                                            return MetaRules.Fail(out result5, out modifiedStream5);
                                        }
                                        return MetaRules.Success();
                                    }
                                , modifiedStream4, out result4, out modifiedStream4))
                                {
                                    return MetaRules.Fail(out result4, out modifiedStream4);
                                }
                                xs = result4;
                                result4 = (
                                													  Sugar.Statement(() =>{
                                																var inputText = Sugar.Implode(xs).As<string>();
                                                                                                var hv = Get<VariableSet>("HostVars", null);
                                                                                                if((hv == null) || (hv.Count == 0))
                                                                                                {
                                                                                                    return inputText;
                                                                                                }
                                                                                                var varArray = hv.ToArray();
                                                                                                var re = @"\b(?<varName>" + string.Join("|", varArray) + @")\b(?!\.As\<)";
                                                                                                var replaceResult = varArray.Length > 0 ?
                                                                                                       Regex.Replace(inputText, re, currentMatch =>{
                                                                                                           var varName = currentMatch.Groups["varName"].Value;
                                                                                                           var varType = hv.GetType(varName);
                                                                                                           var currentReplaceResult = currentMatch.Groups["varName"].Value;
                                                                                                           if (!string.IsNullOrEmpty(varType)){
                                                                                                               currentReplaceResult += ".As<" + hv.GetType(varName) + ">()";
                                                                                                           }
                                                                                                           return currentReplaceResult;
                                                                                                       }) : inputText;
                                                                                                return replaceResult;																										   
                                														})
                                												   ).AsHostExpressionList();
                                return MetaRules.Success();
                            }, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        return MetaRules.Success();
                    }
                    ))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool IgnoredChunk(OMetaStream<char> inputStream, out OMetaList<HostExpression> result, out OMetaStream <char> modifiedStream)
        {
            modifiedStream = inputStream;
            if(!MetaRules.Or(modifiedStream, out result, out modifiedStream,
            delegate(OMetaStream<char> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <char> modifiedStream2)
            {
                modifiedStream2 = inputStream2;
                if(!MetaRules.Apply(RegularString, modifiedStream2, out result2, out modifiedStream2))
                {
                    return MetaRules.Fail(out result2, out modifiedStream2);
                }
                return MetaRules.Success();
            }
            ,delegate(OMetaStream<char> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <char> modifiedStream2)
            {
                modifiedStream2 = inputStream2;
                if(!MetaRules.Apply(VerbatimString, modifiedStream2, out result2, out modifiedStream2))
                {
                    return MetaRules.Fail(out result2, out modifiedStream2);
                }
                return MetaRules.Success();
            }
            ,delegate(OMetaStream<char> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <char> modifiedStream2)
            {
                modifiedStream2 = inputStream2;
                if(!MetaRules.Apply(LiteralChar, modifiedStream2, out result2, out modifiedStream2))
                {
                    return MetaRules.Fail(out result2, out modifiedStream2);
                }
                return MetaRules.Success();
            }
            ,delegate(OMetaStream<char> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <char> modifiedStream2)
            {
                modifiedStream2 = inputStream2;
                if(!MetaRules.Apply(Comment, modifiedStream2, out result2, out modifiedStream2))
                {
                    return MetaRules.Fail(out result2, out modifiedStream2);
                }
                return MetaRules.Success();
            }
            ))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool EscapedChar(OMetaStream<char> inputStream, out OMetaList<HostExpression> result, out OMetaStream <char> modifiedStream)
        {
            OMetaList<HostExpression> c = null;
            modifiedStream = inputStream;
            if(!MetaRules.Or(modifiedStream, out result, out modifiedStream,
            delegate(OMetaStream<char> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <char> modifiedStream2)
            {
                modifiedStream2 = inputStream2;
                if(!MetaRules.Apply(
                    delegate(OMetaStream<char> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <char> modifiedStream3)
                    {
                        modifiedStream3 = inputStream3;
                        if(!MetaRules.ApplyWithArgs(Exactly, modifiedStream3, out result3, out modifiedStream3, ("\\").AsHostExpressionList()))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        if(!MetaRules.Apply(Character, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        c = result3;
                        result3 = ( Sugar.Implode("\\", c) ).AsHostExpressionList();
                        return MetaRules.Success();
                    }, modifiedStream2, out result2, out modifiedStream2))
                {
                    return MetaRules.Fail(out result2, out modifiedStream2);
                }
                return MetaRules.Success();
            }
            ,delegate(OMetaStream<char> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <char> modifiedStream2)
            {
                modifiedStream2 = inputStream2;
                if(!MetaRules.Apply(
                    delegate(OMetaStream<char> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <char> modifiedStream3)
                    {
                        modifiedStream3 = inputStream3;
                        if(!MetaRules.Apply(Character, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        c = result3;
                        result3 = ( c ).AsHostExpressionList();
                        return MetaRules.Success();
                    }, modifiedStream2, out result2, out modifiedStream2))
                {
                    return MetaRules.Fail(out result2, out modifiedStream2);
                }
                return MetaRules.Success();
            }
            ))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool LiteralChar(OMetaStream<char> inputStream, out OMetaList<HostExpression> result, out OMetaStream <char> modifiedStream)
        {
            OMetaList<HostExpression> x = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<char> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <char> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.ApplyWithArgs(Exactly, modifiedStream2, out result2, out modifiedStream2, ("'").AsHostExpressionList()))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    if(!MetaRules.Apply(EscapedChar, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    x = result2;
                    if(!MetaRules.ApplyWithArgs(Exactly, modifiedStream2, out result2, out modifiedStream2, ("'").AsHostExpressionList()))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    result2 = ( "'" + x.As<string>() + "'" ).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool VerbatimStringChar(OMetaStream<char> inputStream, out OMetaList<HostExpression> result, out OMetaStream <char> modifiedStream)
        {
            OMetaList<HostExpression> c = null;
            OMetaList<HostExpression> x = null;
            modifiedStream = inputStream;
            if(!MetaRules.Or(modifiedStream, out result, out modifiedStream,
            delegate(OMetaStream<char> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <char> modifiedStream2)
            {
                modifiedStream2 = inputStream2;
                if(!MetaRules.Apply(
                    delegate(OMetaStream<char> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <char> modifiedStream3)
                    {
                        modifiedStream3 = inputStream3;
                        if(!MetaRules.Not(
                            delegate(OMetaStream<char> inputStream4, out OMetaList<HostExpression> result4, out OMetaStream <char> modifiedStream4)
                            {
                                modifiedStream4 = inputStream4;
                                if(!MetaRules.ApplyWithArgs(Exactly, modifiedStream4, out result4, out modifiedStream4, ("\"\"").AsHostExpressionList()))
                                {
                                    return MetaRules.Fail(out result4, out modifiedStream4);
                                }
                                return MetaRules.Success();
                            }
                        , modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        if(!MetaRules.Not(
                            delegate(OMetaStream<char> inputStream4, out OMetaList<HostExpression> result4, out OMetaStream <char> modifiedStream4)
                            {
                                modifiedStream4 = inputStream4;
                                if(!MetaRules.ApplyWithArgs(Exactly, modifiedStream4, out result4, out modifiedStream4, ("\"").AsHostExpressionList()))
                                {
                                    return MetaRules.Fail(out result4, out modifiedStream4);
                                }
                                return MetaRules.Success();
                            }
                        , modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        if(!MetaRules.Apply(Character, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        c = result3;
                        result3 = ( c ).AsHostExpressionList();
                        return MetaRules.Success();
                    }, modifiedStream2, out result2, out modifiedStream2))
                {
                    return MetaRules.Fail(out result2, out modifiedStream2);
                }
                return MetaRules.Success();
            }
            ,delegate(OMetaStream<char> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <char> modifiedStream2)
            {
                modifiedStream2 = inputStream2;
                if(!MetaRules.Apply(
                    delegate(OMetaStream<char> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <char> modifiedStream3)
                    {
                        modifiedStream3 = inputStream3;
                        if(!MetaRules.ApplyWithArgs(Exactly, modifiedStream3, out result3, out modifiedStream3, ("\"\"").AsHostExpressionList()))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        x = result3;
                        result3 = ( "\"" ).AsHostExpressionList();
                        return MetaRules.Success();
                    }, modifiedStream2, out result2, out modifiedStream2))
                {
                    return MetaRules.Fail(out result2, out modifiedStream2);
                }
                return MetaRules.Success();
            }
            ))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool RegularString(OMetaStream<char> inputStream, out OMetaList<HostExpression> result, out OMetaStream <char> modifiedStream)
        {
            OMetaList<HostExpression> xs = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<char> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <char> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.ApplyWithArgs(Exactly, modifiedStream2, out result2, out modifiedStream2, ("\"").AsHostExpressionList()))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    if(!MetaRules.Many(
                        delegate(OMetaStream<char> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <char> modifiedStream3)
                        {
                            modifiedStream3 = inputStream3;
                            if(!MetaRules.Apply(
                                delegate(OMetaStream<char> inputStream4, out OMetaList<HostExpression> result4, out OMetaStream <char> modifiedStream4)
                                {
                                    modifiedStream4 = inputStream4;
                                    if(!MetaRules.Not(
                                        delegate(OMetaStream<char> inputStream5, out OMetaList<HostExpression> result5, out OMetaStream <char> modifiedStream5)
                                        {
                                            modifiedStream5 = inputStream5;
                                            if(!MetaRules.ApplyWithArgs(Exactly, modifiedStream5, out result5, out modifiedStream5, ("\"").AsHostExpressionList()))
                                            {
                                                return MetaRules.Fail(out result5, out modifiedStream5);
                                            }
                                            return MetaRules.Success();
                                        }
                                    , modifiedStream4, out result4, out modifiedStream4))
                                    {
                                        return MetaRules.Fail(out result4, out modifiedStream4);
                                    }
                                    if(!MetaRules.Apply(EscapedChar, modifiedStream4, out result4, out modifiedStream4))
                                    {
                                        return MetaRules.Fail(out result4, out modifiedStream4);
                                    }
                                    return MetaRules.Success();
                                }, modifiedStream3, out result3, out modifiedStream3))
                            {
                                return MetaRules.Fail(out result3, out modifiedStream3);
                            }
                            return MetaRules.Success();
                        }
                    , modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    xs = result2;
                    if(!MetaRules.ApplyWithArgs(Exactly, modifiedStream2, out result2, out modifiedStream2, ("\"").AsHostExpressionList()))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    result2 = ( "\"" + xs.ToLiteralString() + "\"").AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool VerbatimString(OMetaStream<char> inputStream, out OMetaList<HostExpression> result, out OMetaStream <char> modifiedStream)
        {
            OMetaList<HostExpression> xs = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<char> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <char> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.ApplyWithArgs(Exactly, modifiedStream2, out result2, out modifiedStream2, ("@").AsHostExpressionList()))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    if(!MetaRules.ApplyWithArgs(Exactly, modifiedStream2, out result2, out modifiedStream2, ("\"").AsHostExpressionList()))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    if(!MetaRules.Many(
                        delegate(OMetaStream<char> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <char> modifiedStream3)
                        {
                            modifiedStream3 = inputStream3;
                            if(!MetaRules.Apply(VerbatimStringChar, modifiedStream3, out result3, out modifiedStream3))
                            {
                                return MetaRules.Fail(out result3, out modifiedStream3);
                            }
                            return MetaRules.Success();
                        }
                    , modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    xs = result2;
                    if(!MetaRules.ApplyWithArgs(Exactly, modifiedStream2, out result2, out modifiedStream2, ("\"").AsHostExpressionList()))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    result2 = ( "@\"" + xs.ToLiteralString().Replace("\"", "\"\"") + "\"" ).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool StringLiteral(OMetaStream<char> inputStream, out OMetaList<HostExpression> result, out OMetaStream <char> modifiedStream)
        {
            modifiedStream = inputStream;
            if(!MetaRules.Or(modifiedStream, out result, out modifiedStream,
            delegate(OMetaStream<char> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <char> modifiedStream2)
            {
                modifiedStream2 = inputStream2;
                if(!MetaRules.Apply(RegularString, modifiedStream2, out result2, out modifiedStream2))
                {
                    return MetaRules.Fail(out result2, out modifiedStream2);
                }
                return MetaRules.Success();
            }
            ,delegate(OMetaStream<char> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <char> modifiedStream2)
            {
                modifiedStream2 = inputStream2;
                if(!MetaRules.Apply(VerbatimString, modifiedStream2, out result2, out modifiedStream2))
                {
                    return MetaRules.Fail(out result2, out modifiedStream2);
                }
                return MetaRules.Success();
            }
            ))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool BasicName(OMetaStream<char> inputStream, out OMetaList<HostExpression> result, out OMetaStream <char> modifiedStream)
        {
            OMetaList<HostExpression> lod = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<char> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <char> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Many1(
                        delegate(OMetaStream<char> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <char> modifiedStream3)
                        {
                            modifiedStream3 = inputStream3;
                            if(!MetaRules.Apply(LetterOrDigit, modifiedStream3, out result3, out modifiedStream3))
                            {
                                return MetaRules.Fail(out result3, out modifiedStream3);
                            }
                            return MetaRules.Success();
                        }
                    , modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    lod = result2;
                    result2 = ( lod.As<string>() ).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool FunctionCall(OMetaStream<char> inputStream, out OMetaList<HostExpression> result, out OMetaStream <char> modifiedStream)
        {
            OMetaList<HostExpression> n = null;
            OMetaList<HostExpression> pe = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<char> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <char> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Apply(BasicName, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    n = result2;
                    if(!MetaRules.Apply(ParenExpr, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    pe = result2;
                    result2 = ( n.As<string>() + "(" + pe.As<string>() + ")" ).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool Comment(OMetaStream<char> inputStream, out OMetaList<HostExpression> result, out OMetaStream <char> modifiedStream)
        {
            modifiedStream = inputStream;
            if(!MetaRules.Or(modifiedStream, out result, out modifiedStream,
            delegate(OMetaStream<char> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <char> modifiedStream2)
            {
                modifiedStream2 = inputStream2;
                if(!MetaRules.ApplyWithArgs(FromTo, modifiedStream2, out result2, out modifiedStream2, ("/*").AsHostExpressionList(), ("*/").AsHostExpressionList()))
                {
                    return MetaRules.Fail(out result2, out modifiedStream2);
                }
                return MetaRules.Success();
            }
            ,delegate(OMetaStream<char> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <char> modifiedStream2)
            {
                modifiedStream2 = inputStream2;
                if(!MetaRules.ApplyWithArgs(FromTo, modifiedStream2, out result2, out modifiedStream2, ("//").AsHostExpressionList(), ("\n").AsHostExpressionList()))
                {
                    return MetaRules.Fail(out result2, out modifiedStream2);
                }
                return MetaRules.Success();
            }
            ))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool FromTo(OMetaStream<char> inputStream, out OMetaList<HostExpression> result, out OMetaStream <char> modifiedStream)
        {
            OMetaList<HostExpression> x = null;
            OMetaList<HostExpression> y = null;
            OMetaList<HostExpression> a = null;
            OMetaList<HostExpression> bs = null;
            OMetaList<HostExpression> c = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<char> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <char> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Apply(Anything, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    x = result2;
                    if(!MetaRules.Apply(Anything, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    y = result2;
                    if(!MetaRules.ApplyWithArgs(Seq, modifiedStream2, out result2, out modifiedStream2, (x).AsHostExpressionList()))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    a = result2;
                    if(!MetaRules.Many(
                        delegate(OMetaStream<char> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <char> modifiedStream3)
                        {
                            modifiedStream3 = inputStream3;
                            if(!MetaRules.Apply(
                                delegate(OMetaStream<char> inputStream4, out OMetaList<HostExpression> result4, out OMetaStream <char> modifiedStream4)
                                {
                                    modifiedStream4 = inputStream4;
                                    if(!MetaRules.Not(
                                        delegate(OMetaStream<char> inputStream5, out OMetaList<HostExpression> result5, out OMetaStream <char> modifiedStream5)
                                        {
                                            modifiedStream5 = inputStream5;
                                            if(!MetaRules.ApplyWithArgs(Seq, modifiedStream5, out result5, out modifiedStream5, (y).AsHostExpressionList()))
                                            {
                                                return MetaRules.Fail(out result5, out modifiedStream5);
                                            }
                                            return MetaRules.Success();
                                        }
                                    , modifiedStream4, out result4, out modifiedStream4))
                                    {
                                        return MetaRules.Fail(out result4, out modifiedStream4);
                                    }
                                    if(!MetaRules.Apply(Character, modifiedStream4, out result4, out modifiedStream4))
                                    {
                                        return MetaRules.Fail(out result4, out modifiedStream4);
                                    }
                                    return MetaRules.Success();
                                }, modifiedStream3, out result3, out modifiedStream3))
                            {
                                return MetaRules.Fail(out result3, out modifiedStream3);
                            }
                            return MetaRules.Success();
                        }
                    , modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    bs = result2;
                    if(!MetaRules.ApplyWithArgs(Seq, modifiedStream2, out result2, out modifiedStream2, (y).AsHostExpressionList()))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    c = result2;
                    result2 = ( a.As<string>() + bs.As<string>() + c.As<string>() ).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
    }
}
