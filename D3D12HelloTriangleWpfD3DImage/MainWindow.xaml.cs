using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Animation;
using System.Windows.Media.Media3D;
using D3D12HelloTriangleSharp;

namespace D3D12HelloTriangleWpfD3DImage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private static readonly int RenderTargetWidth = 800;
        private static readonly int RenderTargetHeight = 800;
        
        private D3D12HelloTriangleRenderTarget? _renderer;
        
        public MainWindow()
        {
            InitializeComponent();

            Loaded += (_, _) =>
            {
                _renderer = new D3D12HelloTriangleRenderTarget(RenderTargetWidth, RenderTargetHeight);
                Render(1.0f);
            };
            
            var anim = new DoubleAnimation
            {
                BeginTime = TimeSpan.Zero,
                Duration = new Duration(TimeSpan.FromSeconds(10)),
                From = 0.0,
                To = 360.0,
                RepeatBehavior = RepeatBehavior.Forever
            };
            
            BaseRotate.BeginAnimation(AxisAngleRotation3D.AngleProperty, anim);
        }

        protected override void OnClosed(EventArgs e)
        {
            _renderer?.Dispose();
            base.OnClosed(e);
        }

        private void Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Render((float)Slider.Value);
        }

        private void Render(float ratio)
        {
            if (_renderer == null)
            {
                return;
            }

            var d3dimage = D3DImage;
            d3dimage.Lock();
            try
            {
                d3dimage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, _renderer.RenderTargetSurface.NativePointer);
                _renderer.Render(ratio);
                d3dimage.AddDirtyRect(new Int32Rect(0, 0, RenderTargetWidth, RenderTargetHeight));
            }
            finally
            {
                d3dimage.Unlock();
            }
            
        }
    }
}
