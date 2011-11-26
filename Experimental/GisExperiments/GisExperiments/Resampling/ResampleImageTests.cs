using System;
using System.Drawing;
using MbUnit.Framework;

namespace GisExperiments.Resampling
{
    public class ResampleImageTests
    {
        [Test]
        public void SuperSample()
        {
            SuperSampling algo = new SuperSampling();

            using (Bitmap source = (Bitmap)Bitmap.FromFile(@"D:\hg\Tutis\Experimental\GisExperiments\samples\sample.png"))
            using (Bitmap dest = algo.ReduceBitmapSize(source, 3))
                dest.Save("resampled.png");
        } 
    }
}