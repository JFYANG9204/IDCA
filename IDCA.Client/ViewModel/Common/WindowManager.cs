using System;
using System.Collections;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;

namespace IDCA.Client.ViewModel.Common
{
    public static class WindowManager
    {
        readonly static Hashtable _registerWindow = new()
        {
            { "OpenFileDialog", typeof(OpenFileDialog) },
            { "FolderBrowserDialog", typeof(FolderBrowserDialog) },
        };

        public static void Register<T>(string key)
        {
            if (!_registerWindow.ContainsKey(key))
            {
                _registerWindow.Add(key, typeof(T));
            }
        }

        public static void Register(string key, Type type)
        {
            if (!_registerWindow.ContainsKey(key))
            {
                _registerWindow.Add(key, type);
            }
        }

        public static void Remove(string key)
        {
            if (_registerWindow.ContainsKey(key))
            {
                _registerWindow.Remove(key);
            }
        }

        public static void Warn(string message)
        {
            System.Windows.MessageBox.Show(message);
        }

        public static void Show(string key)
        {
            if (_registerWindow.ContainsKey(key))
            {
                var typeInstance = Activator.CreateInstance((Type)_registerWindow[key]!);
                if (typeInstance is Window window)
                {
                    window.Show();
                }
            }
        }

        public static void ShowAxisSettingDialog()
        {
            if (_registerWindow.ContainsKey("SpecAxisSettingDialog"))
            {
                var typeInstance = Activator.CreateInstance((Type)_registerWindow["SpecAxisSettingDialog"]!);
                if (typeInstance is Window window)
                {
                    window.ShowDialog();
                }
            }
        }

        public static string? ShowFolderBrowserDialog()
        {
            var browser = new FolderBrowserDialog();
            if (browser.ShowDialog() == DialogResult.OK)
            {
                return browser.SelectedPath;
            }
            return null;
        }

        public static string? ShowOpenFileDialog(string filter)
        {
            var dialog = new OpenFileDialog() 
            { 
                Multiselect = false,
                Filter = filter 
            };
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.FileName;
            }
            return null;
        }

        public static void ShowWindow(string key, object? viewModel = null)
        {
            if (_registerWindow.ContainsKey(key))
            {
                var typeInstance = Activator.CreateInstance((Type)_registerWindow[key]!);
                if (typeInstance is Window window)
                {
                    if (viewModel != null)
                    {
                        window.DataContext = viewModel;
                    }
                    window.ShowDialog();
                }
            }
        }

        public static T? FindVisualParent<T>(DependencyObject? obj) where T : class
        {
            while (obj != null)
            {
                if (obj is T)
                {
                    return obj as T;
                }
                obj = VisualTreeHelper.GetParent(obj);
            }
            return null;
        }

        public static void CloseWindow(object? sender)
        {
            FindVisualParent<Window>(sender as DependencyObject)?.Close();
        }

    }
}
