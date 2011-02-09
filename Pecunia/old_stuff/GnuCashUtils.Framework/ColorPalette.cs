using System;
using System.Collections.Generic;
using System.Text;

namespace GnuCashUtils.Framework
{
    public enum ColorPaletteType
    {
        LineColors,
        FillColors,
    }

    public class ColorPalette
    {
        public ColorPaletteType PaletteType
        {
            get { return paletteType; }
        }

        public ColorPalette (ColorPaletteType type)
        {
            this.paletteType = type;
            switch (paletteType)
            {
                case ColorPaletteType.FillColors:
                    defaultColors = new int[]
                        { 
                            0xFFBFBF,
                            0xBFFFBF,
                            0xFF8080,
                            0x80FF80,
                            0xFFF2BF,
                            0xD5BFFF,
                            0xFFE680,
                            0xAA80FF,
                            0xF2FFBF,
                            0xFFBFFF,
                            0xE6FF80,
                            0xFF80FF,
                            0xFFDFBF,
                            0xBFE4FF,
                            0xFFC080,
                            0x80C9FF,
                        };
                    break;

                case ColorPaletteType.LineColors:
                    defaultColors = new int[]
                    { 
                        0xBF6060,
                        0x60BF60,
                        0x8060BF,
                        0xBFAC60,
                        0xBF9060,
                        0x6096BF,
                        0xBF60BF,
                        0xACBF60,
                    };
                    break;

                default:
                    throw new NotSupportedException();

            }
        }

        public void AssignColor (string id, int colorRgb)
        {
            assignedColors[id] = colorRgb;
        }

        public int FindColorForObject (string objectType, string objectId)
        {
            if (assignedColors.ContainsKey (objectId))
                return assignedColors[objectId];

            if (false == usedColorsIndex.ContainsKey (objectType))
                usedColorsIndex[objectType] = -1;

            int usedColors = usedColorsIndex[objectType];
            usedColors = (usedColors + 1) % (defaultColors.Length);
            usedColorsIndex[objectType] = usedColors;

            if (usedColors < defaultColors.Length)
                AssignColor (objectId, defaultColors[usedColors]);
            else
                AssignColor (objectId, 0xffffff);

            return FindColorForObject (objectType, objectId);
        }


        private ColorPaletteType paletteType;
        private Dictionary<string, int> assignedColors = new Dictionary<string, int> ();
        private Dictionary<string, int> usedColorsIndex = new Dictionary<string, int> ();
        private int[] defaultColors;
    }
}
