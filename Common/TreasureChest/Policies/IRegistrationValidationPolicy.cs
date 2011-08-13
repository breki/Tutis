namespace TreasureChest.Policies
{
    public interface IRegistrationValidationPolicy : IMultipleInstancePolicy
    {
        void Validate(ServiceRegistration registration);
    }
}