using System;

namespace OMetaSharp
{
    public class OMetaOptimizer : Parser<HostExpression>
    {
        public virtual bool OptimizeGrammar(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> u = null;
            OMetaList<HostExpression> ns = null;
            OMetaList<HostExpression> n = null;
            OMetaList<HostExpression> gtd = null;
            OMetaList<HostExpression> sn = null;
            OMetaList<HostExpression> btd = null;
            OMetaList<HostExpression> rs = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Form(
                        delegate(OMetaStream<HostExpression> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <HostExpression> modifiedStream3)
                        {
                            modifiedStream3 = inputStream3;
                            if(!MetaRules.Apply(
                                delegate(OMetaStream<HostExpression> inputStream4, out OMetaList<HostExpression> result4, out OMetaStream <HostExpression> modifiedStream4)
                                {
                                    modifiedStream4 = inputStream4;
                                    if(!MetaRules.ApplyWithArgs(Token, modifiedStream4, out result4, out modifiedStream4, ("Grammar").AsHostExpressionList()))
                                    {
                                        return MetaRules.Fail(out result4, out modifiedStream4);
                                    }
                                    if(!MetaRules.Apply(Anything, modifiedStream4, out result4, out modifiedStream4))
                                    {
                                        return MetaRules.Fail(out result4, out modifiedStream4);
                                    }
                                    u = result4;
                                    if(!MetaRules.Apply(Anything, modifiedStream4, out result4, out modifiedStream4))
                                    {
                                        return MetaRules.Fail(out result4, out modifiedStream4);
                                    }
                                    ns = result4;
                                    if(!MetaRules.Apply(Anything, modifiedStream4, out result4, out modifiedStream4))
                                    {
                                        return MetaRules.Fail(out result4, out modifiedStream4);
                                    }
                                    n = result4;
                                    if(!MetaRules.Apply(Anything, modifiedStream4, out result4, out modifiedStream4))
                                    {
                                        return MetaRules.Fail(out result4, out modifiedStream4);
                                    }
                                    gtd = result4;
                                    if(!MetaRules.Apply(Anything, modifiedStream4, out result4, out modifiedStream4))
                                    {
                                        return MetaRules.Fail(out result4, out modifiedStream4);
                                    }
                                    sn = result4;
                                    if(!MetaRules.Apply(Anything, modifiedStream4, out result4, out modifiedStream4))
                                    {
                                        return MetaRules.Fail(out result4, out modifiedStream4);
                                    }
                                    btd = result4;
                                    if(!MetaRules.Many(
                                        delegate(OMetaStream<HostExpression> inputStream5, out OMetaList<HostExpression> result5, out OMetaStream <HostExpression> modifiedStream5)
                                        {
                                            modifiedStream5 = inputStream5;
                                            if(!MetaRules.Apply(OptimizeRule, modifiedStream5, out result5, out modifiedStream5))
                                            {
                                                return MetaRules.Fail(out result5, out modifiedStream5);
                                            }
                                            return MetaRules.Success();
                                        }
                                    , modifiedStream4, out result4, out modifiedStream4))
                                    {
                                        return MetaRules.Fail(out result4, out modifiedStream4);
                                    }
                                    rs = result4;
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
                    result2 = ( Sugar.ConsWithFlatten("Grammar", u, ns, n, gtd, sn, btd, rs) ).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
        public virtual bool OptimizeRule(OMetaStream<HostExpression> inputStream, out OMetaList<HostExpression> result, out OMetaStream <HostExpression> modifiedStream)
        {
            OMetaList<HostExpression> r = null;
            modifiedStream = inputStream;
            if(!MetaRules.Apply(
                delegate(OMetaStream<HostExpression> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <HostExpression> modifiedStream2)
                {
                    modifiedStream2 = inputStream2;
                    if(!MetaRules.Apply(Anything, modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    r = result2;
                    if(!MetaRules.Many(
                        delegate(OMetaStream<HostExpression> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <HostExpression> modifiedStream3)
                        {
                            modifiedStream3 = inputStream3;
                            if(!MetaRules.ApplyWithArgs(Foreign, modifiedStream3, out result3, out modifiedStream3, (typeof(AndOrOptimization)).AsHostExpressionList(), ("Optimize").AsHostExpressionList(), (r).AsHostExpressionList()))
                            {
                                return MetaRules.Fail(out result3, out modifiedStream3);
                            }
                            r = result3;
                            return MetaRules.Success();
                        }
                    , modifiedStream2, out result2, out modifiedStream2))
                    {
                        return MetaRules.Fail(out result2, out modifiedStream2);
                    }
                    result2 = ( r ).AsHostExpressionList();
                    return MetaRules.Success();
                }, modifiedStream, out result, out modifiedStream))
            {
                return MetaRules.Fail(out result, out modifiedStream);
            }
            return MetaRules.Success();
        }
    }
}
