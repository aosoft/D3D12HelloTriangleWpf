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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
                _renderer = new D3D12HelloTriangle(WinFormsHost.Handle, (int)WinFormsHost.Width, (int)WinFormsHost.Height, false);
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
