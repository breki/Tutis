using System.Collections.Generic;

namespace SrtmPlaying.Png
{
    public class PngImageAnalysisInfo
    {
        public int ColorsCount { get { return colorsUsed.Count; } }
        public bool IsTransparencyUsed { get; set; }
        public bool IsMoreThan256Colors { get { return colorsUsed.Count > 256; } }
        public int PixelSize { get; set; }

        public void AddUsedColor (int color)
        {
            colorsUsed.Add(color);
        }

        private readonly HashSet<int> colorsUsed = new HashSet<int>();
    }
}