using System;

namespace OMetaSharp.Examples
{
    public class Factorial : OMeta<int>
    {
        public virtual bool Fact(OMetaStream<int> inputStream, out OMetaList<HostExpression> result, out OMetaStream <int> modifiedStream)
        {
            OMetaList<HostExpression> n = null;
            OMetaList<HostExpression> m = null;
            modifiedStream = inputStream;
            if(!MetaRules.Or(modifiedStream, out result, out modifiedStream,
            delegate(OMetaStream<int> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <int> modifiedStream2)
            {
                modifiedStream2 = inputStream2;
                if(!MetaRules.Apply(
                    delegate(OMetaStream<int> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <int> modifiedStream3)
                    {
                        modifiedStream3 = inputStream3;
                        if(!MetaRules.ApplyWithArgs(Exactly, modifiedStream3, out result3, out modifiedStream3, (0).AsHostExpressionList()))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        result3 = ( 1 ).AsHostExpressionList();
                        return MetaRules.Success();
                    }, modifiedStream2, out result2, out modifiedStream2))
                {
                    return MetaRules.Fail(out result2, out modifiedStream2);
                }
                return MetaRules.Success();
            }
            ,delegate(OMetaStream<int> inputStream2, out OMetaList<HostExpression> result2, out OMetaStream <int> modifiedStream2)
            {
                modifiedStream2 = inputStream2;
                if(!MetaRules.Apply(
                    delegate(OMetaStream<int> inputStream3, out OMetaList<HostExpression> result3, out OMetaStream <int> modifiedStream3)
                    {
                        modifiedStream3 = inputStream3;
                        if(!MetaRules.Apply(Anything, modifiedStream3, out result3, out modifiedStream3))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        n = result3;
                        if(!(n.As<int>() > 0))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        result3 = true.AsHostExpressionList();
                        if(!MetaRules.ApplyWithArgs(Fact, modifiedStream3, out result3, out modifiedStream3, (n.As<int>() - 1).AsHostExpressionList()))
                        {
                            return MetaRules.Fail(out result3, out modifiedStream3);
                        }
                        m = result3;
                        result3 = ( n.As<int>()*m.As<int>() ).AsHostExpressionList();
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
    }
}
