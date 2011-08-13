namespace GnuCashUtils.Web.Infrastucture
{
    public class HtmlWritingContext
    {
        public int Depth
        {
            get { return depth; }
            set { depth = value; }
        }

        private int depth;
    }
}