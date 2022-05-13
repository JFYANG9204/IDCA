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

        public static object? ShowDialog(string key, params string[] parameters)
        {
            if (_registerWindow.ContainsKey(key))
            {
                var typeInstance = Activator.CreateInstance((Type)_registerWindow[key]!);
                if (typeInstance is FolderBrowserDialog folderBrowserDialog)
                {
                    if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
                    {
                        return folderBrowserDialog.SelectedPath;
                    }
                }
                else if (typeInstance is OpenFileDialog openFileDialog)
                {
                    openFileDialog.Multiselect = false;
                    if (parameters.Length > 0)
                    {
                        openFileDialog.Filter = parameters[0];
                    }
                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        return openFileDialog.FileName;
                    }
                }
                else if (typeInstance is Window window)
                {
                    return window.ShowDialog();
                }
            }
            return null;
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
