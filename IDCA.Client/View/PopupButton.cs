using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace IDCA.Client.View
{

    [TemplatePart(Name = PART_Popup, Type = typeof(Popup))]
    [TemplatePart(Name = PART_ShowPopupButton, Type = typeof(Button))]
    public class PopupButton : ToggleButton
    {

        public const string PART_Popup = "PART_Popup";
        public const string PART_ShowPopupButton = "PART_ShowPopupButton";

        static PopupButton()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PopupButton), new FrameworkPropertyMetadata(typeof(PopupButton)));
        }

        public static readonly DependencyProperty IsPopupOpenProperty = DependencyProperty
                .Register(nameof(IsPopupOpen),
                          typeof(bool),
                          typeof(PopupButton));
        /// <summary>
        /// 按钮的下拉框是否打开
        /// </summary>
        public bool IsPopupOpen
        {
            get => (bool)GetValue(IsPopupOpenProperty);
            set => SetValue(IsPopupOpenProperty, value);
        }

        public static readonly DependencyProperty PopupContentProperty = DependencyProperty
            .Register(nameof(PopupContent),
                      typeof(object),
                      typeof(PopupButton));

        public object PopupContent
        {
            get => GetValue(PopupContentProperty);
            set => SetValue(PopupContentProperty, value);
        }

        public static readonly DependencyProperty PopupTemplateProperty = DependencyProperty
            .Register(nameof(PopupTemplate),
                      typeof(DataTemplate),
                      typeof(PopupButton));

        public DataTemplate PopupTemplate
        {
            get => (DataTemplate)GetValue(PopupTemplateProperty);
            set => SetValue(PopupTemplateProperty, value);
        }

        protected override void OnClick()
        {
            base.OnClick();
            IsPopupOpen = IsChecked != null && (bool)IsChecked;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_popup != null)
            {
                _popup.Closed -= PopupClosed;
            }
            _popup = GetTemplateChild(PART_Popup) as Popup;
            if (_popup != null)
            {
                _popup.Closed += PopupClosed;
            }
        }

        private void PopupClosed(object? sender, System.EventArgs e)
        {
            IsPopupOpen = false;
            IsChecked = false;
        }

        Popup? _popup;
    }
}
