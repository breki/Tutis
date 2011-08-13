using System;

namespace OMetaSharp
{
    public class AndOrOptimization : NullOptimization
    {
        public override bool And(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            Rule<HostExpression> __baseRule__ = base.And;
            OMetaList<HostExpression> x = null;
            OMetaList<HostExpression> xs = null;
            modifiedStream = inputStream;
            if(!MetaRules.Or(modifiedStream, out result, out modifiedStream,
            delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
            {
                modifiedStream2 = inputStream2;
                if(!MetaRules.Apply(
                    delegate(OMetaStream<HostExpression> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <HostExpression> modifiedStream3)
                    {
                        modifiedStream3 = inputStream3;
                        if(!MetaRules.Apply(Trans, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        x = result3;
                        if(!MetaRules.Apply(End, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        if(!MetaRules.Apply(SetHelped, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        result3 = ( x ).AsHostExpressionList();
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
                if(!MetaRules.Apply(
                    delegate(OMetaStream<HostExpression> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <HostExpression> modifiedStream3)
                    {
                        modifiedStream3 = inputStream3;
                        if(!MetaRules.ApplyWithArgs(TransInside, modifiedStream3, out result3, out modifiedStream3, ("And").AsHostExpressionList()))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        xs = result3;
                        result3 = ( Sugar.HackedInnerConcat("And", xs) ).AsHostExpressionList();
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
        public override bool Or(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            Rule<HostExpression> __baseRule__ = base.Or;
            OMetaList<HostExpression> x = null;
            OMetaList<HostExpression> xs = null;
            modifiedStream = inputStream;
            if(!MetaRules.Or(modifiedStream, out result, out modifiedStream,
            delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
            {
                modifiedStream2 = inputStream2;
                if(!MetaRules.Apply(
                    delegate(OMetaStream<HostExpression> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <HostExpression> modifiedStream3)
                    {
                        modifiedStream3 = inputStream3;
                        if(!MetaRules.Apply(Trans, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        x = result3;
                        if(!MetaRules.Apply(End, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        if(!MetaRules.Apply(SetHelped, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        result3 = ( x ).AsHostExpressionList();
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
                if(!MetaRules.Apply(
                    delegate(OMetaStream<HostExpression> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <HostExpression> modifiedStream3)
                    {
                        modifiedStream3 = inputStream3;
                        if(!MetaRules.ApplyWithArgs(TransInside, modifiedStream3, out result3, out modifiedStream3, ("Or").AsHostExpressionList()))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        xs = result3;
                        result3 = ( Sugar.HackedInnerConcat("Or", xs) ).AsHostExpressionList();
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
        public virtual bool TransInside(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> t = null;
            OMetaList<HostExpression> xs = null;
            OMetaList<HostExpression> ys = null;
            OMetaList<HostExpression> x = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Apply(Anything, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    t = result2;
                    if(!MetaRules.Or(modifiedStream2, out result2, out modifiedStream2,
                    delegate(OMetaStream<HostExpression> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <HostExpression> modifiedStream3)
                    {
                        modifiedStream3 = inputStream3;
                        if(!MetaRules.Apply(
                            delegate(OMetaStream<HostExpression> inputStream4, out OMetaList<HostExpression> result4, out OMetaStream <HostExpression> modifiedStream4)
                            {
                                modifiedStream4 = inputStream4;
                                if(!MetaRules.Form(
                                    delegate(OMetaStream<HostExpression> inputStream5, out OMetaList<HostExpression> result5, out OMetaStream <HostExpression> modifiedStream5)
                                    {
                                        modifiedStream5 = inputStream5;
                                        if(!MetaRules.Apply(
                                            delegate(OMetaStream<HostExpression> inputStream6, out OMetaList<HostExpression> result6, out OMetaStream <HostExpression> modifiedStream6)
                                            {
                                                modifiedStream6 = inputStream6;
                                                if(!MetaRules.ApplyWithArgs(Exactly, modifiedStream6, out result6, out modifiedStream6, (t).AsHostExpressionList()))
                                                {
                                                    return MetaRules.Fail(out result6, out modifiedStream6);
                                                }
                                                if(!MetaRules.ApplyWithArgs(TransInside, modifiedStream6, out result6, out modifiedStream6, (t).AsHostExpressionList()))
                                                {
                                                    return MetaRules.Fail(out result6, out modifiedStream6);
                                                }
                                                xs = result6;
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
                                if(!MetaRules.ApplyWithArgs(TransInside, modifiedStream4, out result4, out modifiedStream4, (t).AsHostExpressionList()))
                                {
                                    return MetaRules.Fail(out result4, out modifiedStream4);
                                }
                                ys = result4;
                                if(!MetaRules.Apply(SetHelped, modifiedStream4, out result4, out modifiedStream4))
                                {
                                    return MetaRules.Fail(out result4, out modifiedStream4);
                                }
                                result4 = ( Sugar.ConsWithFlatten(xs, ys) ).AsHostExpressionList();
                                return MetaRules.Success();
                            }, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        return MetaRules.Success();
                    }
                    ,delegate(OMetaStream<HostExpression> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <HostExpression> modifiedStream3)
                    {
                        modifiedStream3 = inputStream3;
                        if(!MetaRules.Apply(
                            delegate(OMetaStream<HostExpression> inputStream4, out OMetaList<HostExpression> result4, out OMetaStream <HostExpression> modifiedStream4)
                            {
                                modifiedStream4 = inputStream4;
                                if(!MetaRules.Apply(Trans, modifiedStream4, out result4, out modifiedStream4))
                                {
                                    return MetaRules.Fail(out result4, out modifiedStream4);
                                }
                                x = result4;
                                if(!MetaRules.ApplyWithArgs(TransInside, modifiedStream4, out result4, out modifiedStream4, (t).AsHostExpressionList()))
                                {
                                    return MetaRules.Fail(out result4, out modifiedStream4);
                                }
                                xs = result4;
                                result4 = ( Sugar.ConsWithFlatten(x, xs) ).AsHostExpressionList();
                                return MetaRules.Success();
                            }, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        return MetaRules.Success();
                    }
                    ,delegate(OMetaStream<HostExpression> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <HostExpression> modifiedStream3)
                    {
                        modifiedStream3 = inputStream3;
                        result3 = ( Sugar.Cons() ).AsHostExpressionList();
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
    }
}
