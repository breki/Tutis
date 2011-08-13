using System.Diagnostics.CodeAnalysis;

namespace TreasureChest
{
    public interface ILeaseReturning
    {
        /// <summary>
        /// Returns an object instance to the chest.
        /// </summary>
        /// <param name="instance">Instance to be returned.</param>
        [SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Return")]
        void Return(object instance);
    }
}