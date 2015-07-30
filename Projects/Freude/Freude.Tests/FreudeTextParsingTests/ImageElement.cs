namespace Freude.Tests.FreudeTextParsingTests
{
    public class ImageElement : IDocumentElement
    {
        public ImageElement(string imageUrl)
        {
            this.imageUrl = imageUrl;
        }

        public string ImageUrl
        {
            get { return imageUrl; }
        }

        private readonly string imageUrl;
    }
}