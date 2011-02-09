using System.Collections.Generic;

namespace OMetaSharp
{
    /// <summary>
    /// The "meta rules" are the rules that are required to implement the OMeta
    /// implementation itself. Alessandro's JavaScript implementation prefixed
    /// all of them with an underscore. I decided to factor them into their own
    /// interface to make them comply better with .NET naming guidelines.
    /// 
    /// Additionally, by factoring them out to an interface, the actual OMeta 
    /// implementation can implement them explicitly and thus make it harder
    /// for grammars to accidentally invoke them.
    /// </summary>    
    public interface IMetaRuleEvaluator<TInput>
    {
        bool Or(OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream, params Rule<TInput>[] rules);
        bool Success();
        bool Fail(out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream);
        bool Apply(Rule<TInput> rule, OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream);
        bool Not(Rule<TInput> rule, OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream);
        bool Lookahead(Rule<TInput> rule, OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream);
        bool SuperApplyWithArgs(OMeta<TInput> superClass, Rule<TInput> rule, OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream);
        bool Form(Rule<TInput> actionRule, OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream);
        bool Many(Rule<TInput> rule, OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream);
        bool Many(Rule<TInput> rule, OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream, OMetaList<HostExpression> argument);
        bool Many1(Rule<TInput> rule, OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream);        
        bool ApplyWithArgs(Rule<TInput> rule, OMetaStream<TInput> inputStream, out OMetaList<HostExpression> result, out OMetaStream<TInput> modifiedStream, params OMetaList<HostExpression>[] arguments);
    }
}
