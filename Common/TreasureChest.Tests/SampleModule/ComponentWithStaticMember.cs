namespace TreasureChest.Tests.SampleModule
{
    public class ComponentWithStaticMember
    {
        public static int Value
        {
            get { return value; }
            set { ComponentWithStaticMember.value = value; }
        }

        private static int value;
    }
}