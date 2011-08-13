namespace TreasureChest.Policies
{
    public abstract class GlobalChestPolicyBase : IGlobalChestPolicy
    {
        public virtual void Initialize(IChestMaster chest)
        {
            this.chest = chest;
        }

        public void AssignLogger(ILogger logger)
        {
            this.logger = logger;
        }

        protected IChestMaster Chest
        {
            get { return chest; }
        }

        protected ILogger Logger
        {
            get { return logger; }
        }

        private IChestMaster chest;
        private ILogger logger;
    }
}