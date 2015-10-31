namespace Freude.Parsing
{
    public class WikiTokenizationSettings
    {
        public bool IsWholeLine
        {
            get { return isWholeLine; }
            set { isWholeLine = value; }
        }

        private bool isWholeLine;
    }
}