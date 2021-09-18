using System;
using System.Windows;
using D3D12HelloTriangleSharp;

namespace D3D12HelloTriangleWpfWinFormInterop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private D3D12HelloTriangle? _renderer;
        
        public MainWindow()
        {
            InitializeComponent();

            WinFormsHost.Loaded += (_, _) =>
            {
                _renderer = new D3D12HelloTriangle(WinFormsPanel.Handle, WinFormsPanel.Width, WinFormsPanel.Height, false);
                _renderer.OnRender();
            };
        }

        protected override void OnClosed(EventArgs e)
        {
            _renderer?.OnDestroy();
            _renderer?.Dispose();
            base.OnClosed(e);
        }
    }
}
