using System.Collections.Generic;

namespace SrtmPlaying.Png
{
    public class PngImageAnalysisInfo
    {
        public PngImageAnalysisInfo(
            int imageWidth,
            int imageHeight,
            int sourcePixelSize,
            int clipX, 
            int clipY, 
            int clipWidth, 
            int clipHeight)
        {
            ImageWidth = imageWidth;
            ImageHeight = imageHeight;
            SourcePixelSize = sourcePixelSize;
            this.clipX = clipX;
            this.clipY = clipY;
            this.clipWidth = clipWidth;
            this.clipHeight = clipHeight;
        }

        public int ClipX
        {
            get { return clipX; }
        }

        public int ClipY
        {
            get { return clipY; }
        }

        public int ClipWidth
        {
            get { return clipWidth; }
        }

        public int ClipHeight
        {
            get { return clipHeight; }
        }

        public int ColorsCount { get { return colorsUsed.Count; } }

        public PngFilterType FilterType { get; set; } = PngFilterType.Sub;
        public int ImageWidth { get; private set; }
        public int ImageHeight { get; private set; }
        public bool IsTransparencyUsed { get; set; }
        public bool IsMoreThan256Colors { get { return colorsUsed.Count > 256; } }
        public int SourcePixelSize { get; set; }
        public int DestinationPixelSize { get; set; }

        public void AddUsedColor (int color)
        {
            colorsUsed.Add(color);
        }

        private readonly int clipX;
        private readonly int clipY;
        private readonly int clipWidth;
        private readonly int clipHeight;
        private readonly HashSet<int> colorsUsed = new HashSet<int>();
    }
}