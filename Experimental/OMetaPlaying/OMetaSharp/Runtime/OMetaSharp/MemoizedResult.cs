namespace OMetaSharp
{
    /// <summary>
    /// Memoization is what keeps the packrat-like parser humming smoothly in
    /// the face of left recursion. 
    /// 
    /// This class helps store a previously computed result or indicates that
    /// the previous attempt was unsuccessful.
    /// </summary>    
    public class MemoizedResult<TInput>
    {        
        public OMetaList<HostExpression> Answer
        {
            get;
            set;
        }

        public OMetaStream<TInput> NextInputStream
        {
            get;
            set;
        }

        public bool HasBeenUsed
        {
            get;
            set;
        }
    }

    /// <summary>
    /// Indicates that memoization failed.
    /// </summary>
    /// <typeparam name="T">Type of the underlying OMeta stream.</typeparam>
    public class FailedMemoizedResult<TInput> : MemoizedResult<TInput>
    {
    }
}
