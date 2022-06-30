using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace IDCA.Client.View
{

    [TemplatePart(Name = PART_Popup, Type = typeof(Popup))]
    [TemplatePart(Name = PART_ShowPopupButton, Type = typeof(Button))]
    public class PopupButton : ContentControl
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

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (_button != null)
            {
                _button.Click -= ButtonClicked;
            }
            _button = GetTemplateChild(PART_ShowPopupButton) as Button;
            if (_button != null)
            {
                _button.Click += ButtonClicked;
            }

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
            if (IsPopupOpen)
            {
                IsPopupOpen = false;
            }
        }

        private void ButtonClicked(object? sender, RoutedEventArgs e)
        {
            
            IsPopupOpen = !IsPopupOpen;
        }

        Button? _button;
        Popup? _popup;
    }
}
