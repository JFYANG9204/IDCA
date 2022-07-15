
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace IDCA.Client.View.Helper
{
    public static class HeaderedControlHelper
    {

        public static readonly DependencyProperty HeaderForegroundProperty =
            DependencyProperty.RegisterAttached(
                "HeaderForeground",
                typeof(Brush),
                typeof(HeaderedControlHelper),
                new UIPropertyMetadata(Brushes.Black));

        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static Brush GetHeaderForeground(UIElement element)
        {
            return (Brush)element.GetValue(HeaderForegroundProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static void SetHeaderForeground(UIElement element, Brush value)
        {
            element.SetValue(HeaderForegroundProperty, value);
        }


        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.RegisterAttached(
                "HeaderBackground",
                typeof(Brush),
                typeof(HeaderedControlHelper),
                new UIPropertyMetadata(Panel.BackgroundProperty.DefaultMetadata.DefaultValue));

        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static Brush GetHeaderBackground(UIElement element)
        {
            return (Brush)element.GetValue(HeaderBackgroundProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static void SetHeaderBackground(UIElement element, Brush value)
        {
            element.SetValue(HeaderBackgroundProperty, value);
        }


        public static readonly DependencyProperty HeaderFontFamilyProperty =
            DependencyProperty.RegisterAttached(
                "HeaderFontFamily",
                typeof(FontFamily),
                typeof(HeaderedControlHelper),
                new FrameworkPropertyMetadata(SystemFonts.MessageFontFamily, FrameworkPropertyMetadataOptions.Inherits));

        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static FontFamily GetHeaderFontFamily(UIElement element)
        {
            return (FontFamily)element.GetValue(HeaderFontFamilyProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static void SetHeaderFontFamily(UIElement element, FontFamily value)
        {
            element.SetValue(HeaderFontFamilyProperty, value);
        }


        public static readonly DependencyProperty HeaderFontSizeProperty =
            DependencyProperty.RegisterAttached(
                "HeaderFontSize",
                typeof(double),
                typeof(HeaderedControlHelper),
                new FrameworkPropertyMetadata(SystemFonts.MessageFontSize, FrameworkPropertyMetadataOptions.Inherits));

        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static double GetHeaderFontSize(UIElement element)
        {
            return (double)element.GetValue(HeaderFontSizeProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static void SetHeaderFontSize(UIElement element, double value)
        {
            element.SetValue(HeaderFontSizeProperty, value);
        }


        public static readonly DependencyProperty HeaderFontStretchProperty =
            DependencyProperty.RegisterAttached(
                "HeaderFontStretch",
                typeof(FontStretch),
                typeof(HeaderedControlHelper),
                new FrameworkPropertyMetadata(TextElement.FontStretchProperty.DefaultMetadata.DefaultValue, FrameworkPropertyMetadataOptions.Inherits));

        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static FontStretch GetHeaderFontStretch(UIElement element)
        {
            return (FontStretch)element.GetValue(HeaderFontStretchProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static void SetHeaderFontStretch(UIElement element, FontStretch value)
        {
            element.SetValue(HeaderFontStretchProperty, value);
        }



        public static readonly DependencyProperty HeaderFontWeightProperty =
            DependencyProperty.RegisterAttached(
                "HeaderFontWeight",
                typeof(FontWeight),
                typeof(HeaderedControlHelper),
                new FrameworkPropertyMetadata(SystemFonts.MessageFontWeight, FrameworkPropertyMetadataOptions.Inherits));


        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static FontWeight GetHeaderFontWeight(UIElement element)
        {
            return (FontWeight)element.GetValue(HeaderFontWeightProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static void SetHeaderFontWeight(UIElement element, FontWeight value)
        {
            element.SetValue(HeaderFontWeightProperty, value);
        }


        public static readonly DependencyProperty HeaderMarginProperty =
            DependencyProperty.RegisterAttached(
                "HeaderMargin",
                typeof(Thickness),
                typeof(HeaderedControlHelper),
                new FrameworkPropertyMetadata(new Thickness()));

        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static Thickness GetHeaderMargin(UIElement element)
        {
            return (Thickness)element.GetValue(HeaderMarginProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static void SetHeaderMargin(UIElement element, Thickness value)
        {
            element.SetValue(HeaderMarginProperty, value);
        }


        public static readonly DependencyProperty HeaderHorizontalContentAlignmentProperty =
            DependencyProperty.RegisterAttached(
                "HeaderHorizontalContentAlignment",
                typeof(HorizontalAlignment),
                typeof(HeaderedControlHelper),
                new FrameworkPropertyMetadata(HorizontalAlignment.Stretch));

        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static HorizontalAlignment GetHeaderHorizontalContentAlignment(UIElement element)
        {
            return (HorizontalAlignment)element.GetValue(HeaderHorizontalContentAlignmentProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static void SetHeaderHorizontalContentAlignment(UIElement element, HorizontalAlignment value)
        {
            element.SetValue(HeaderHorizontalContentAlignmentProperty, value);
        }


        public static readonly DependencyProperty HeaderVerticalContentAlignmentProperty =
            DependencyProperty.RegisterAttached(
                "HeaderVerticalContentAlignment",
                typeof(VerticalAlignment),
                typeof(HeaderedControlHelper),
                new FrameworkPropertyMetadata(VerticalAlignment.Stretch));

        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static VerticalAlignment GetHeaderVerticalContentAlignment(UIElement element)
        {
            return (VerticalAlignment)element.GetValue(HeaderVerticalContentAlignmentProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(HeaderedContentControl))]
        [AttachedPropertyBrowsableForType(typeof(TabControl))]
        public static void SetHeaderVerticalContentAlignment(UIElement element, VerticalAlignment value)
        {
            element.SetValue(HeaderVerticalContentAlignmentProperty, value);
        }

    }
}
