using System;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using iEditSL.Entities;
using System.Windows.Input;
using System.Collections.Specialized;

namespace iEditSL
{
    public class OutlineView : TreeView, IView
    {
        private bool _addLocal = false;
        
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


        public void ButtonAddSubNode_Click(object sender, RoutedEventArgs e)
        {
            var selectedNode = SelectedValue as Node;
            if (selectedNode != null)
            {
                var node = new Node();
                node.Name = "新しいノード" + Document.Instance.Number.ToString();
                node.Parent = selectedNode;
                selectedNode.SubNodes.Add(node);
                selectedNode.IsShowSubnodes = true;
                node.IsSelectedOnTree = true;
                _addLocal = true;
                Document.Instance.Add(node);
            }
        }

        public void ButtonAddSiblingNode_Click(object sender, RoutedEventArgs e)
        {
            var selectedNode = SelectedValue as Node;
            if (selectedNode != null)
            {
                var parent = selectedNode.Parent;
                if (parent != null)
                {
                    var node = new Node();
                    node.Name = "新しいノード" + Document.Instance.Number.ToString();
                    node.Parent = parent;
                    parent.SubNodes.Add(node);
                    parent.IsShowSubnodes = true;
                    node.IsSelectedOnTree = true;
                    _addLocal = true;
                    Document.Instance.Add(node);
                }
            }
        }

        public void ButtonLevelUp_Click(object sender, RoutedEventArgs e)
        {
            var selectedNode = SelectedValue as Node;
            if (selectedNode != null)
            {
                var parent = selectedNode.Parent;
                if (parent != null)
                {
                    var grandParent = parent.Parent;
                    if (grandParent != null)
                    {
                        parent.SubNodes.Remove(selectedNode);
                        selectedNode.Parent = grandParent;
                        grandParent.SubNodes.Add(selectedNode);
                        selectedNode.IsSelectedOnTree = true;
                    }
                }
            }
        }

        public void ButtonLevelDown_Click(object sender, RoutedEventArgs e)
        {
            var selectedNode = SelectedValue as Node;
            if (selectedNode != null)
            {
                var parent = selectedNode.Parent;
                if (parent != null)
                {
                    var index = parent.SubNodes.IndexOf(selectedNode);
                    if (index >= 1)
                    {
                        var prevSibling = parent.SubNodes[index - 1];
                        parent.SubNodes.Remove(selectedNode);
                        prevSibling.SubNodes.Add(selectedNode);
                        selectedNode.Parent = prevSibling;
                        prevSibling.IsShowSubnodes = true;
                        selectedNode.IsSelectedOnTree = true;
                    }
                }
            }
        }

        public void ButtonPositionUp_Click(object sender, RoutedEventArgs e)
        {
            var selectedNode = SelectedValue as Node;
            if (selectedNode != null)
            {
                var parent = selectedNode.Parent;
                if (parent != null)
                {
                    var index = parent.SubNodes.IndexOf(selectedNode);
                    if (index >= 1)
                    {
                        parent.SubNodes.Remove(selectedNode);
                        parent.SubNodes.Insert(index - 1, selectedNode);
                        selectedNode.IsSelectedOnTree = true;
                    }
                }
            }
        }

        public void ButtonPositionDown_Click(object sender, RoutedEventArgs e)
        {
            var selectedNode = SelectedValue as Node;
            if (selectedNode != null)
            {
                var parent = selectedNode.Parent;
                if (parent != null)
                {
                    var index = parent.SubNodes.IndexOf(selectedNode);
                    if (index < parent.SubNodes.Count - 1)
                    {
                        parent.SubNodes.Remove(selectedNode);
                        parent.SubNodes.Insert(index + 1, selectedNode);
                        selectedNode.IsSelectedOnTree = true;
                    }
                }
            }
        }

        public void UpdateSelection()
        {
            var node = SelectedValue as Node;
            if (node != null)
            {
                Document.Instance.UnSelectAllElements();
                node.IsSelected = true;
            }
        }

        
        #region IView メンバ

        // TODO Serial化対応 NodeのParent ID を見て IDで検索してHitしたやつのしたにぶら下げる
        public void Update(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_addLocal)
            {
                _addLocal = false;
                return;
            }
            var node = e.NewItems[0] as Node;
            if (node == null) return;
            var selectedNode = SelectedValue as Node;
            if (selectedNode != null)
            {
                var parent = selectedNode.Parent;
                if (parent == null)
                {
                    parent = Document.Instance.Root[0];
                }
                node.Parent = parent;
                parent.SubNodes.Add(node);
                parent.IsShowSubnodes = true;
            }
            else
            {
                var root = Document.Instance.Root[0];
                node.Parent = root;
                root.SubNodes.Add(node);
                root.IsShowSubnodes = true;
            }
            node.IsSelectedOnTree = true;
        }

        #endregion
    }
}
