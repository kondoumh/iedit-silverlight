using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;

namespace iEditSL
{
    public class OutlineViewItem : TreeViewItem
    {
        protected override DependencyObject GetContainerForItemOverride()
        {
            OutlineViewItem tvi = new OutlineViewItem();
            Binding expandedBinding = new Binding("IsShowSubnodes");
            expandedBinding.Mode = BindingMode.TwoWay;
            tvi.SetBinding(OutlineViewItem.IsExpandedProperty, expandedBinding);
            Binding selectedBinding = new Binding("IsSelectedOnTree");
            selectedBinding.Mode = BindingMode.TwoWay;
            tvi.SetBinding(OutlineViewItem.IsSelectedProperty, selectedBinding);
            return tvi;
        }
    }
}
