
using IDCA.Client.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace IDCA.Client.View
{
    public class TableSettingContentPresenter : Control
    {

        static TableSettingContentPresenter()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TableSettingContentPresenter), new FrameworkPropertyMetadata(typeof(TableSettingContentPresenter)));
        }

        public TableSettingContentPresenter()
        {
        }

        readonly static Dictionary<Type, object> _registerCache = new Dictionary<Type, object>();

        public static object? Registered(string fullTypeName)
        {
            foreach (var item in _registerCache.Keys)
            {
                if (fullTypeName.Equals(item.FullName))
                {
                    return _registerCache[item];
                }
            }
            return null;
        }

        public readonly static DependencyProperty ContentProperty = DependencyProperty.Register(
            nameof(Content),
            typeof(object),
            typeof(TableSettingContentPresenter));

        public object Content
        {
            get { return GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }

        public readonly static DependencyProperty ViewModelProperty = DependencyProperty.Register(
            nameof(ViewModel),
            typeof(object),
            typeof(TableSettingContentPresenter),
            new FrameworkPropertyMetadata(OnViewModelPropertyChanged));

        public object ViewModel
        {
            get { return GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        static void OnViewModelPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
        {
            object? view = null;
            if (args.NewValue.GetType().GetCustomAttributes(typeof(ViewModelMarkAttribute), false)?.First() is ViewModelMarkAttribute attribute)
            {
                var exist = Registered(attribute.FullTypeName);
                if (exist != null)
                {
                    view = exist;
                }
                else
                {
                    Assembly assembly = typeof(TableSettingContentPresenter).Assembly;
                    var type = assembly.GetType(attribute.FullTypeName);
                    if (type != null && (view = Activator.CreateInstance(type)) != null)
                    {
                        _registerCache.Add(type, view);
                    }
                }
                if (view is FrameworkElement element)
                {
                    element.DataContext = args.NewValue;
                    d.SetValue(ContentProperty, element);
                }
            }
        }

    }
}
