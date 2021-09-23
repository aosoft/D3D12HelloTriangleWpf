using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using D3D12HelloTriangleSharp;

namespace D3D12HelloTriangleWpfD3DImage
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private D3D12HelloTriangleRenderTarget? _renderer;
        
        public MainWindow()
        {
            InitializeComponent();

            Loaded += (_, _) =>
            {
                _renderer = new D3D12HelloTriangleRenderTarget(800, 800);
                D3DImage.Lock();
                try
                {
                    D3DImage.SetBackBuffer(D3DResourceType.IDirect3DSurface9, _renderer.RenderTargetSurface.NativePointer);
                    _renderer.Render(1.0f);
                    D3DImage.AddDirtyRect(new Int32Rect(0, 0, 800, 800));
                }
                finally
                {
                    D3DImage.Unlock();
                }
            };
        }

        protected override void OnClosed(EventArgs e)
        {
            _renderer?.Dispose();
            base.OnClosed(e);
        }
    }
}
