using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace IDCA.Mvvm
{
    public class NotificationObject : INotifyPropertyChanged
    {
        protected PropertyChangedEventHandler? PropertyChangedHandler => this.PropertyChanged;

        public event PropertyChangedEventHandler? PropertyChanged;

        public void VerifyPropertyName(string propertyName)
        {
            TypeInfo typeInfo = GetType().GetTypeInfo();
            if (string.IsNullOrEmpty(propertyName) || typeInfo.GetDeclaredProperty(propertyName) is not null)
            {
                return;
            }

            bool flag = false;
            while (typeInfo.BaseType != null && typeInfo.BaseType != typeof(object))
            {
                typeInfo = typeInfo.BaseType.GetTypeInfo();
                if (typeInfo.GetDeclaredProperty(propertyName) != null)
                {
                    flag = true;
                    break;
                }
            }

            if (!flag)
            {
                throw new ArgumentException("Property not found", propertyName);
            }
        }

        public virtual void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public virtual void RaisePropertyChanged<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression != null)
            {
                string propertyName = GetPropertyName(propertyExpression);
                if (!string.IsNullOrEmpty(propertyName))
                {
                    RaisePropertyChanged(propertyName);
                }
            }
        }

        protected static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            {
                throw new ArgumentException(null, nameof(propertyExpression));
            }

            return ((((propertyExpression.Body as MemberExpression) ?? 
                    throw new ArgumentException("Invalid argument", nameof(propertyExpression))).Member as PropertyInfo) ?? 
                    throw new ArgumentException("Argument is not a property", nameof(propertyExpression))).Name;
        }

        protected bool Set<T>(Expression<Func<T>> propertyExpression, ref T field, T newValue)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }
            field = newValue;
            RaisePropertyChanged(propertyExpression);
            return true;
        }

        protected bool Set<T>(string? propertyName, ref T field, T newValue)
        {
            if (EqualityComparer<T>.Default.Equals(field, newValue))
            {
                return false;
            }
            field = newValue;
            RaisePropertyChanged(propertyName);
            return true;
        }

        protected bool Set<T>(ref T field, T newValue, [CallerMemberName] string? propertyName = null)
        {
            return Set(propertyName, ref field, newValue);
        }
    }
}
