using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace iEditSL.Utilities
{
    public class BindingFunctions
    {
        public static void BindProperty(Control control, object source, string path,
            DependencyProperty property, BindingMode mode)
        {
            var binding = new Binding(path);
            binding.Source = source;
            binding.Mode = mode;
            control.SetBinding(property, binding);
        }

        public static void BindProperty(FrameworkElement element, object source, 
            string path, DependencyProperty property, BindingMode mode)
        {
            var binding = new Binding(path);
            binding.Source = source;
            binding.Mode = mode;
            element.SetBinding(property, binding);
        }
    }
}
