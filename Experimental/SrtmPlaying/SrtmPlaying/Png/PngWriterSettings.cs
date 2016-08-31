namespace SrtmPlaying.Png
{
    public class PngWriterSettings
    {
        public int CompressionLevel
        {
            get { return compressionLevel; }
            set { compressionLevel = value; }
        }

        public PngTransparency Transparency
        {
            get { return transparency; }
            set { transparency = value; }
        }

        private int compressionLevel = 5;
        private PngTransparency transparency = PngTransparency.AutoDetect;
    }
}