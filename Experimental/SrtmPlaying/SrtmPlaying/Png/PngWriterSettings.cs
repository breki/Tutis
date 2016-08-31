namespace Brejc.Imaging.Png
{
    public class PngWriterSettings
    {
        public bool UseDotNetZip { get; set; }
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