using System;
using System.Drawing;
using System.Drawing.Imaging;
using MbUnit.Framework;

namespace App
{
    public class LayoutTests
    {
        [Test]
        public void Test2()
        {
            int controlPadding = 5;
            int maxWidth = 200;

            SimpleLayoutElement borderRight = new SimpleLayoutElement("borderRight");
            borderRight.X = maxWidth - controlPadding;
            borderRight.Width = controlPadding;
            borderRight.Visible = false;
            SimpleLayoutElement borderBottom = new SimpleLayoutElement("borderBottom");
            SimpleLayoutElement borderTop = new SimpleLayoutElement("borderTop");

            int longestLabelWidth = 20;

            SimpleLayoutElement label1 = new SimpleLayoutElement("label1", 10, 10);

            SimpleLayoutElement line1 = new SimpleLayoutElement("line1", 0, controlPadding);
            line1.Visible = false;
            
            SimpleLayoutElement label2 = new SimpleLayoutElement("label2", longestLabelWidth, 10);
            label2.Y = 20;
            label2.TopAnchor(line1);

            SimpleLayoutElement button = new SimpleLayoutElement("button", 10, 10);
            button.Y = 20;
            button
                .RightAnchor(borderRight)
                .TopAnchor(line1);

            SimpleLayoutElement rightGuide = new SimpleLayoutElement("rightGuide");
            rightGuide.Width = controlPadding;
            rightGuide.Visible = false;
            rightGuide.RightAnchor(button);

            SimpleLayoutElement leftGuide = new SimpleLayoutElement("leftGuide");
            leftGuide.Width = controlPadding;
            leftGuide.Visible = false;
            leftGuide.LeftNoFlyZone(label1, label2);

            SimpleLayoutElement textBox1 = new SimpleLayoutElement("textBox1", 50, 10);
            textBox1
                .MaximumWidth(70)
                .MinimumWidth (50)
                .LeftNoFlyZone(leftGuide)
                .RightNoFlyZone(rightGuide);

            line1.TopAnchor(label1, textBox1);

            SimpleLayoutElement textBox2 = new SimpleLayoutElement("textBox2", 75, 10);
            textBox2.Y = 20;
            textBox2
                .LeftNoFlyZone(leftGuide)
                .RightDock(rightGuide)
                .TopAnchor(line1);

            VisualSystem visualSystem = new VisualSystem();
            visualSystem.AddElement(label1);
            visualSystem.AddElement(textBox1);
            visualSystem.AddElement(label2);
            visualSystem.AddElement(textBox2);
            visualSystem.AddElement(button);
            visualSystem.AddElement(rightGuide);
            visualSystem.AddElement(leftGuide);
            visualSystem.AddElement(borderRight);
            visualSystem.AddElement(line1);

            ExperimentalLayoutMechanism layoutMechanism = new ExperimentalLayoutMechanism();
            layoutMechanism.PerformLayout(visualSystem);

            Assert.AreEqual(maxWidth - controlPadding, button.X + button.Width);
            Assert.AreEqual(10, button.Width);
            Assert.AreEqual(longestLabelWidth + controlPadding, textBox1.X);
            Assert.AreEqual(
                70, 
                textBox1.Width);

            RenderLayout(x
                =>
                             {
                                 x.Visualize(visualSystem);

                                 //x.StartingY = 110;
                                 //borderRight.X -= 90;
                                 //layoutMechanism.PerformLayout(visualSystem);
                                 //x.Visualize(visualSystem);

                                 //x.StartingY = 220;
                                 //borderRight.X += 200;
                                 //layoutMechanism.PerformLayout(visualSystem);
                                 //x.Visualize(visualSystem);
                             });
        }

        private static void RenderLayout(
            Action<GdiLayoutVisualizer> visualizerAction)
        {
            using (Bitmap bitmap = new Bitmap(1000, 600))
            {
                using (Graphics graphics = Graphics.FromImage(bitmap))
                {
                    GdiLayoutVisualizer visualizer = new GdiLayoutVisualizer(
                        graphics, 
                        3, 
                        10);
                    visualizerAction(visualizer);
                }

                bitmap.Save("layout.png", ImageFormat.Png);
            }
        }

        [FixtureSetUp]
        private void FixtureSetup()
        {
            log4net.Config.XmlConfigurator.Configure();
        }
    }
}