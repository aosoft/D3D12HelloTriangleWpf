using System;
using System.Windows;
using System.Windows.Media;
using D3D12HelloTriangleSharp;

namespace D3D12HelloTriangleWpfWinFormInterop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private D3D12HelloTriangleWindow? _renderer;
        private RenderThread? _renderThread;
        private float _ratio;

        public MainWindow()
        {
            InitializeComponent();
            _ratio = (float)Slider.Value;

            WinFormsHost.Loaded += (_, _) =>
            {
                _renderer = new D3D12HelloTriangleWindow(WinFormsPanel.Handle, WinFormsPanel.Width,
                    WinFormsPanel.Height);
                _renderer.Render(_ratio);

                _renderThread = new RenderThread(() => _renderer?.Render(_ratio));
            };
        }

        protected override void OnClosed(EventArgs e)
        {
            _renderThread?.Dispose();
            _renderer?.Dispose();
            base.OnClosed(e);
        }

        private void Slider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            _ratio = (float)Slider.Value;
        }
    }
}