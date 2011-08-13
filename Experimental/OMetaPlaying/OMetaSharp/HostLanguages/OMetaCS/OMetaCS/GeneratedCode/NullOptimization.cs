using System;

namespace OMetaSharp
{
    public class NullOptimization : OMeta<HostExpression>
    {
        public virtual bool SetHelped(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            modifiedStream = inputStream;
            result = (Set("DidSomething", true)).AsHostExpressionList();
            return MetaRules.Success();
        }
        public virtual bool SetNotHelped(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            modifiedStream = inputStream;
            result = (Set("DidSomething", false)).AsHostExpressionList();
            return MetaRules.Success();
        }
        public virtual bool Helped(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            modifiedStream = inputStream;
            if(!(Get<bool>("DidSomething", false)))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            result = true.AsHostExpressionList();
            return MetaRules.Success();
        }
        public virtual bool Trans(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> t = null;
            OMetaList<HostExpression> ans = null;
            modifiedStream = inputStream;
            if(!MetaRules.Or(modifiedStream, out result, out modifiedStream,
            delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
            {
                modifiedStream2 = inputStream2;
                if(!MetaRules.Apply(
                    delegate(OMetaStream<HostExpression> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <HostExpression> modifiedStream3)
                    {
                        modifiedStream3 = inputStream3;
                        if(!MetaRules.Form(
                            delegate(OMetaStream<HostExpression> inputStream4, out OMetaList<HostExpression> result4, out OMetaStream <HostExpression> modifiedStream4)
                            {
                                modifiedStream4 = inputStream4;
                                if(!MetaRules.Apply(
                                    delegate(OMetaStream<HostExpression> inputStream5, out OMetaList<HostExpression> result5, out OMetaStream <HostExpression> modifiedStream5)
                                    {
                                        modifiedStream5 = inputStream5;
                                        if(!MetaRules.Apply(Anything, modifiedStream5, out result5, out modifiedStream5))
                                        {
                                            return MetaRules.Fail(out result5, out modifiedStream5);
                                        }
                                        t = result5;
                                        if(!(HasMethod(t)))
                                        {
                                            return MetaRules.Fail(out result5, out modifiedStream5);
                                        }
                                        result5 = true.AsHostExpressionList();
                                        if(!MetaRules.ApplyWithArgs(Apply, modifiedStream5, out result5, out modifiedStream5, (t).AsHostExpressionList()))
                                        {
                                            return MetaRules.Fail(out result5, out modifiedStream5);
                                        }
                                        ans = result5;
                                        return MetaRules.Success();
                                    }, modifiedStream4, out result4, out modifiedStream4))
                                {
                                    return MetaRules.Fail(out result4, out modifiedStream4);
                                }
                                return MetaRules.Success();
                            }
                        , modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        result3 = ( ans ).AsHostExpressionList();
                        return MetaRules.Success();
                    }, modifiedStream2, out result2, out modifiedStream2))
                {
                    return MetaRules.Fail(out result2, out modifiedStream2);
                }
                return MetaRules.Success();
            }
            ,delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
            {
                modifiedStream2 = inputStream2;
                if(!MetaRules.Apply(Anything, modifiedStream2, out result2, out modifiedStream2))
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
        public virtual bool Optimize(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> x = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Apply(SetNotHelped, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    if(!MetaRules.Apply(Trans, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    x = result2;
                    if(!MetaRules.Apply(Helped, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    result2 = ( x ).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool Or(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> xs = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Many(
                        delegate(OMetaStream<HostExpression> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <HostExpression> modifiedStream3)
                        {
                            modifiedStream3 = inputStream3;
                            if(!MetaRules.Apply(Trans, modifiedStream3, out result3, out modifiedStream3))
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
                    result2 = ( Sugar.HackedInnerConcat("Or", xs) ).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool And(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> xs = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Many(
                        delegate(OMetaStream<HostExpression> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <HostExpression> modifiedStream3)
                        {
                            modifiedStream3 = inputStream3;
                            if(!MetaRules.Apply(Trans, modifiedStream3, out result3, out modifiedStream3))
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
                    result2 = ( Sugar.HackedInnerConcat("And", xs) ).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool Many(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> x = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Apply(Trans, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    x = result2;
                    result2 = (Sugar.Cons("Many",x)).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool Many1(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> x = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Apply(Trans, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    x = result2;
                    result2 = (Sugar.Cons("Many1",x)).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool Set(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> n = null;
            OMetaList<HostExpression> v = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Apply(Anything, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    n = result2;
                    if(!MetaRules.Apply(Trans, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    v = result2;
                    result2 = (Sugar.Cons("Set",n,v)).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool Not(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> x = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Apply(Trans, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    x = result2;
                    result2 = (Sugar.Cons("Not",x)).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool Lookahead(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> x = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Apply(Trans, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    x = result2;
                    result2 = (Sugar.Cons("Lookahead",x)).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool Form(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> x = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Apply(Trans, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    x = result2;
                    result2 = (Sugar.Cons("Form",x)).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool Rule(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> name = null;
            OMetaList<HostExpression> over = null;
            OMetaList<HostExpression> ls = null;
            OMetaList<HostExpression> body = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Apply(Anything, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    name = result2;
                    if(!MetaRules.Apply(Anything, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    over = result2;
                    if(!MetaRules.Apply(Anything, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    ls = result2;
                    if(!MetaRules.Apply(Trans, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    body = result2;
                    result2 = (Sugar.Cons("Rule",name,over,ls,body)).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
    }
}
