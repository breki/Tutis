using System;
using System.Diagnostics;
using Hjg.Pngcs;
using Hjg.Pngcs.Chunks;
using MbUnit.Framework;

namespace GisExperiments.PngOptimization
{
    public class PngCsTests
    {
        [Test]
        public void Test()
        {
            PngReader pngr = FileHelper.CreatePngReader (@"../../../samples/topo.png");

            PngChunkPLTE plte = pngr.GetMetadata ().GetPLTE ();
            PngChunkTRNS trns = pngr.GetMetadata ().GetTRNS (); // transparency metadata, can be null
            bool alpha = trns != null;
            ImageInfo im2 = new ImageInfo (pngr.ImgInfo.Cols, pngr.ImgInfo.Rows, 8, alpha);

            string outputFile = "output_pngcs.png";
            PngWriter pngw = FileHelper.CreatePngWriter (outputFile, im2, true);
            pngw.CopyChunksFirst (pngr, ChunkCopyBehaviour.COPY_ALL_SAFE);
            int[] buf = null;
            for (int row = 0; row < pngr.ImgInfo.Rows; row++)
            {
                ImageLine line = pngr.ReadRowInt (row);
                buf = ImageLineHelper.Palette2rgb (line, plte, trns, buf);
                pngw.WriteRowInt (buf, row);
            }
            pngw.CopyChunksLast (pngr, ChunkCopyBehaviour.COPY_ALL_SAFE);
            pngr.End ();
            pngw.End ();
        } 
    }
}