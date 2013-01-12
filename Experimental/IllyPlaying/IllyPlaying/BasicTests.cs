using System.Collections.Generic;
using System.Diagnostics;
using Illustrator;
using NUnit.Framework;

namespace IllyPlaying
{
    public class BasicTests
    {
        [Test]
        public void OpenAI()
        {
            Application app = new Application();
            foreach (Document document1 in app.Documents)
                document1.Close(AiSaveOptions.aiDoNotSaveChanges);

            Document document = app.Open (
                @"D:\MyStuff\Dropbox\Public\Pigment\H_Maribor_130102_180245.pdf", AiDocumentColorSpace.aiDocumentRGBColor, null);

            Layer zeroLayer = document.ActiveLayer;
            zeroLayer.Preview = false;

            Debug.WriteLine("GraphItems: {0}", zeroLayer.GraphItems.Count);
            Debug.WriteLine ("PathItems: {0}", zeroLayer.PathItems.Count);
            Debug.WriteLine ("GroupItems: {0}", zeroLayer.GroupItems.Count);
            Debug.WriteLine ("CompoundPathItems: {0}", zeroLayer.CompoundPathItems.Count);
            Debug.WriteLine ("NonNativeItems: {0}", zeroLayer.NonNativeItems.Count);
            Debug.WriteLine ("PageItems: {0}", zeroLayer.PageItems.Count);
            Debug.WriteLine ("PlacedItems: {0}", zeroLayer.PlacedItems.Count);
            Debug.WriteLine ("RasterItems: {0}", zeroLayer.RasterItems.Count);
            Debug.WriteLine ("SymbolItems: {0}", zeroLayer.SymbolItems.Count);
            Debug.WriteLine ("TextFrames: {0}", zeroLayer.TextFrames.Count);

            Layer newLayer = document.Layers.Add();
            newLayer.Name = "text layer";

            List<TextFrame> textFrames = new List<TextFrame>();
            foreach (TextFrame item in zeroLayer.TextFrames)
                textFrames.Add(item);

            foreach (TextFrame item in textFrames)
            {
                item.Move(newLayer, AiElementPlacement.aiPlaceAtEnd);
                //Debug.WriteLine("Moved text '{0}'", item.Name, null);
            }

            Layer groupLayer = document.Layers.Add();
            groupLayer.Name = "group layer";

            List<GroupItem> groupItems = new List<GroupItem> ();
            foreach (GroupItem item in zeroLayer.GroupItems)
                groupItems.Add (item);

            foreach (GroupItem item in groupItems)
                item.Move (groupLayer, AiElementPlacement.aiPlaceAtEnd);

            Debug.WriteLine ("zero.GroupItems: {0}", zeroLayer.GroupItems.Count);
            Debug.WriteLine ("zero.TextFrames: {0}", zeroLayer.TextFrames.Count);
            Debug.WriteLine ("newLayer.GroupItems: {0}", newLayer.GroupItems.Count);
            Debug.WriteLine ("newLayer.TextFrames: {0}", newLayer.TextFrames.Count);
            Debug.WriteLine ("groupLayer.GroupItems: {0}", groupLayer.GroupItems.Count);
            Debug.WriteLine ("groupLayer.TextFrames: {0}", groupLayer.TextFrames.Count);

            //foreach (PathItem pageItem in zeroLayer.PathItems)
            //    pageItem.Move(newLayer, AiElementPlacement.aiPlaceAtEnd);

            zeroLayer.Preview = true;
            newLayer.Preview = true;
            groupLayer.Preview = true;

            IllustratorSaveOptions options = new IllustratorSaveOptions();
            options.FlattenOutput = AiOutputFlattening.aiPreserveAppearance;
            options.FontSubsetThreshold = 100;
            options.PDFCompatible = true;
            options.Compatibility = AiCompatibility.aiIllustrator12;
            options.Compressed = true;
            document.SaveAs (@"D:\hg\Tutis\Experimental\IllyPlaying\IllyPlaying\bin\Debug\test.ai", options);

            //Thread.Sleep(5000);
            //app.Quit();
        }         

        //     doc.Export(@"F:/AI_Prog/2009Calendar.png",Illustrator.AiExportType.aiPNG24, null);  
        // doc.Close(Illustrator.AiSaveOptions.aiDoNotSaveChanges); 
    }
}