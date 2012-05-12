using System;
using System.Windows.Forms;
using SharpDX;
using SharpDX.DXGI;
using SharpDX.Direct2D1;
using Factory = SharpDX.Direct2D1.Factory;

namespace Direct2DPlaying
{
    public partial class Form1 : Form
    {
        public Form1 ()
        {
            InitializeComponent ();

            SetStyle (ControlStyles.ResizeRedraw, true);
        }

        protected override void OnLoad (EventArgs e)
        {
            base.OnLoad (e);

            d2DFactory = new Factory ();
            Initialize();
        }

        protected override void OnPaint (PaintEventArgs e)
        {
            base.OnPaint (e);

            renderTarget.BeginDraw ();
            renderTarget.Clear (currentFillBrush.Color);

            using (PathGeometry geometry = new PathGeometry (d2DFactory))
            {
                using (GeometrySink sink = geometry.Open ())
                {
                    sink.BeginFigure (new DrawingPointF(10, 10), FigureBegin.Hollow);
                    sink.AddLine(new DrawingPointF(ClientSize.Width - 10, 10));
                    sink.AddLine (new DrawingPointF (ClientSize.Width - 10, ClientSize.Height - 10));
                    sink.AddLine(new DrawingPointF(10, ClientSize.Height - 10));
                    sink.AddLine(new DrawingPointF(10, 10));
                    sink.EndFigure(FigureEnd.Open);
                    sink.Close ();
                }

                renderTarget.DrawGeometry (geometry, currentLineBrush, 10);
            }

            renderTarget.EndDraw();
        }

        protected override void OnResize (EventArgs e)
        {
            base.OnResize (e);
            renderTarget.Resize(new DrawingSize(ClientSize.Width, ClientSize.Height));

            //Destroy();
            //Initialize();
        }

        private void Initialize()
        {
            HwndRenderTargetProperties hwndRenderTargetProperties = new HwndRenderTargetProperties ();
            hwndRenderTargetProperties.Hwnd = Handle;
            hwndRenderTargetProperties.PixelSize = ClientSize;
            hwndRenderTargetProperties.PresentOptions = PresentOptions.None;

            // Straight alpha. The color components of the pixel represent the color intensity prior to alpha blending
            // Premultiplied alpha. The color components of the pixel represent the color intensity multiplied by the alpha value. This format is more efficient to render than straight alpha, because the term from the alpha-blending formula is pre-computed. However, this format is not appropriate for storing in an image file.
            PixelFormat pixelFormat = new PixelFormat (Format.Unknown, AlphaMode.Premultiplied);
            RenderTargetProperties renderTargetProperties = new RenderTargetProperties (pixelFormat);
            //{
            //    Type = RenderTargetType.Hardware,
            //    PixelFormat = pixelFormat,
            //    //Usage = RenderTargetUsage.None, 
            //    //PixelFormat = new PixelFormat(Format.)
            //};

            renderTarget = new WindowRenderTarget (
                d2DFactory,
                renderTargetProperties,
                hwndRenderTargetProperties);
            renderTarget.AntialiasMode = AntialiasMode.PerPrimitive;
            renderTarget.TextAntialiasMode = TextAntialiasMode.Cleartype;

            currentFillBrush = new SolidColorBrush (renderTarget, new Color4 (0, 1, 1, 1));
            currentLineBrush = new SolidColorBrush (renderTarget, new Color4 (1, 0, 1, 1));
        }

        private void Destroy()
        {
            currentFillBrush.Dispose ();
            currentLineBrush.Dispose ();
            renderTarget.Dispose ();
        }

        private WindowRenderTarget renderTarget;
        private Factory d2DFactory;
        private SolidColorBrush currentFillBrush;
        private Brush currentLineBrush;
    }
}
