using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace LuckyProject.Lib.WinUi.Components.Controls
{
    public sealed partial class Separator : UserControl
    {
        public Separator()
        {
            InitializeComponent();
            Update(Orientation, LineSize);
        }

        #region DependancyProperties
        #region Orientation
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                nameof(Orientation),
                typeof(Orientation),
                typeof(Separator),
                new PropertyMetadata(Orientation.Horizontal, OnOrientationChanged));
        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        private static void OnOrientationChanged(
             DependencyObject d,
             DependencyPropertyChangedEventArgs e)
        {
            var self = (Separator)d;
            self.Update((Orientation)e.NewValue, self.LineSize);
        }
        #endregion

        #region LineSize
        public static readonly DependencyProperty LineSizeProperty =
        DependencyProperty.Register(
                nameof(LineSize),
                typeof(double),
                typeof(Separator),
                new PropertyMetadata(3.0, OnLineSizeChanged));
        public double LineSize
        {
            get => (double)GetValue(LineSizeProperty);
            set => SetValue(LineSizeProperty, value);
        }

        private static void OnLineSizeChanged(
             DependencyObject d,
             DependencyPropertyChangedEventArgs e)
        {
            var self = (Separator)d;
            self.Update(self.Orientation, (double)e.NewValue);
        }
        #endregion

        private void Update(Orientation orientation, double lineSize)
        {
            if (orientation == Orientation.Horizontal)
            {
                rect.VerticalAlignment = VerticalAlignment.Center;
                rect.HorizontalAlignment = HorizontalAlignment.Stretch;
                rect.Width = double.NaN;
                rect.Height = lineSize;
            }
            else
            {
                rect.VerticalAlignment = VerticalAlignment.Stretch;
                rect.HorizontalAlignment = HorizontalAlignment.Center;
                rect.Width = lineSize;
                rect.Height = double.NaN;
            }
        }
        #endregion
    }
}
