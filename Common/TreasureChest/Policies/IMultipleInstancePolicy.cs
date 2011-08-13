using System.Diagnostics.CodeAnalysis;

namespace TreasureChest.Policies
{
    [SuppressMessage ("Microsoft.Design", "CA1040:AvoidEmptyInterfaces")]
    public interface IMultipleInstancePolicy : IPolicy
    {
    }
}