using System.Collections.Generic;

namespace Brejc.Imaging.Png
{
    public class PngImageAnalysisInfo
    {
        public int ColorsCount { get { return colorsUsed.Count; } }
        public bool IsTransparencyUsed { get; set; }
        public bool IsMoreThan256Colors { get { return colorsUsed.Count > 256; } }

        public void AddUsedColor (int color)
        {
            colorsUsed.Add(color);
        }

        private HashSet<int> colorsUsed = new HashSet<int>();
    }
}