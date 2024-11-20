using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace LuckyProject.Lib.WinUi.Components.Controls.Buttons
{
    public sealed partial class IconButton : Button
    {
        public IconButton()
        {
            InitializeComponent();
            UpdateOrientation(Orientation);
        }

        #region DependancyProperties
        #region Orientation
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                nameof(Orientation),
                typeof(Orientation),
                typeof(IconButton),
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
            var self = (IconButton)d;
            self.UpdateOrientation((Orientation)e.NewValue);
        }

        private void UpdateOrientation(Orientation orientation)
        {
            if (orientation == Orientation.Horizontal)
            {
                Grid.SetRow(fiIcon, 0);
                Grid.SetRow(tbCaption, 0);
                Grid.SetColumn(fiIcon, 0);
                Grid.SetColumn(tbCaption, 1);
            }
            else
            {
                Grid.SetRow(fiIcon, 0);
                Grid.SetRow(tbCaption, 1);
                Grid.SetColumn(fiIcon, 0);
                Grid.SetColumn(tbCaption, 0);
            }
        }
        #endregion

        #region Glyph
        public static readonly DependencyProperty GlyphProperty =
            DependencyProperty.Register(
                nameof(Glyph),
                typeof(string),
                typeof(IconButton),
                new PropertyMetadata(null));
        public string Glyph
        {
            get => (string)GetValue(GlyphProperty);
            set => SetValue(GlyphProperty, value);
        }
        #endregion

        #region IconSize
        public static readonly DependencyProperty IconSizeProperty =
            DependencyProperty.Register(
                nameof(IconSize),
                typeof(int),
                typeof(IconButton),
                new PropertyMetadata(16));
        public int IconSize
        {
            get => (int)GetValue(IconSizeProperty);
            set => SetValue(IconSizeProperty, value);
        }
        #endregion

        #region IconMargin
        public static readonly DependencyProperty IconMarginProperty =
            DependencyProperty.Register(
                nameof(IconMargin),
                typeof(Thickness),
                typeof(IconButton),
                new PropertyMetadata(new Thickness(4)));
        public Thickness IconMargin
        {
            get => (Thickness)GetValue(IconMarginProperty);
            set => SetValue(IconMarginProperty, value);
        }
        #endregion

        #region Caption
        public static readonly DependencyProperty CaptionProperty =
            DependencyProperty.Register(
                nameof(Caption),
                typeof(string),
                typeof(IconButton),
                new PropertyMetadata(null));
        public string Caption
        {
            get => (string)GetValue(CaptionProperty);
            set => SetValue(CaptionProperty, value);
        }
        #endregion

        #region CaptionStyle
        public static readonly DependencyProperty CaptionStyleProperty =
            DependencyProperty.Register(
                nameof(CaptionStyle),
                typeof(Style),
                typeof(IconButton),
                new PropertyMetadata(null));
        public Style CaptionStyle
        {
            get => (Style)GetValue(CaptionStyleProperty);
            set => SetValue(CaptionStyleProperty, value);
        }
        #endregion
        #endregion
    }
}
