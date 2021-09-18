﻿using System;
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
        private D3D12HelloTriangle? _renderer;
        private float _ratio;
        
        public MainWindow()
        {
            InitializeComponent();
            _ratio = (float)Slider.Value;

            WinFormsHost.Loaded += (_, _) =>
            {
                _renderer = new D3D12HelloTriangle(WinFormsPanel.Handle, WinFormsPanel.Width, WinFormsPanel.Height);
                _renderer.Render(_ratio);
            };

            CompositionTarget.Rendering += CompositionTarget_OnRendering;
        }

        protected override void OnClosed(EventArgs e)
        {
            CompositionTarget.Rendering -= CompositionTarget_OnRendering;
            _renderer?.Dispose();
            base.OnClosed(e);
        }

        private void CompositionTarget_OnRendering(object? sender, EventArgs e)
        {
            if (_renderer != null)
            {
                float value = (float)Slider.Value;
                if (_ratio != value)
                {
                    _ratio = value;
                    _renderer.Render(value);
                }
            }
        }
    }
}
