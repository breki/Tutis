namespace TreasureChest.Policies
{
    public interface IRegistrationsConflictValidationPolicy : IMultipleInstancePolicy
    {
        void Validate(
            ServiceRegistration registration1,
            ServiceRegistration registration2);
    }
}