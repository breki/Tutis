using System;

namespace Freude.DocModel
{
    public class ImageElement : IDocumentElement
    {
        public ImageElement(Uri imageUrl)
        {
            this.imageUrl = imageUrl;
        }

        public Uri ImageUrl
        {
            get { return imageUrl; }
        }

        private readonly Uri imageUrl;
    }
}