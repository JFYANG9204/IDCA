using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace IDCA.Client.View.Helper
{
    public class UIHelper
    {

        public static DependencyObject? GetParentObject(DependencyObject child)
        {
            if (child is null)
            {
                return null;
            }

            if (child is ContentElement contentElement)
            {
                DependencyObject parent = ContentOperations.GetParent(contentElement);
                if (parent != null)
                {
                    return parent;
                }

                return contentElement is FrameworkContentElement frameworkContentElement ? frameworkContentElement.Parent : null;
            }

            return VisualTreeHelper.GetParent(child);

        }

        public static T? TryFindParent<T>(DependencyObject child) where T : DependencyObject
        {
            var parentObject = GetParentObject(child);

            if (parentObject == null)
            {
                return null;
            }

            if (parentObject is T parent)
            {
                return parent;
            }
            else
            {
                return TryFindParent<T>(parentObject);
            }
        }

        public static void UpdateBindingSources(DependencyObject obj, params DependencyProperty[] properties)
        {
            foreach (var property in properties)
            {
                var be = BindingOperations.GetBindingExpression(obj, property);
                if (be != null)
                {
                    be.UpdateSource();
                }
            }

            int count = VisualTreeHelper.GetChildrenCount(obj);
            for (int i = 0; i < count; i++)
            {
                var childObject = VisualTreeHelper.GetChild(obj, i);
                UpdateBindingSources(childObject, properties);
            }
        }

        public static T? TryFindFromPoint<T>(UIElement reference, Point point) where T : DependencyObject
        {
            if (reference.InputHitTest(point) is not DependencyObject element)
            {
                return null;
            }
            else
            {
                return element is T ele ? ele : TryFindParent<T>(element);
            }
        }


    }
}
