using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using iEditSL.Entities;

namespace iEditSL
{
    public partial class NodePropertyPage : UserControl
    {
        public NodePropertyPage()
        {
            InitializeComponent();
            comboBoxFigureType.SelectionChanged 
                += new SelectionChangedEventHandler(comboBoxFigureType_SelectionChanged);
            TBName.KeyUp += new KeyEventHandler(TBName_KeyUp);
        }

        void TBName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                comboBoxFigureType.Focus();
        }

        // Node.FigureTypeによるComboboxのバインディングの動作が怪しいのでコーディングで
        void comboBoxFigureType_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var node = DataContext as Node;
            if (node != null)
            {
                var selected = comboBoxFigureType.SelectedIndex;
                switch (selected)
                {
                    case 0:
                        node.FigureType = FigureTypes.Rectangle;
                        break;
                    case 1:
                        node.FigureType = FigureTypes.RoundedRect;
                        break;
                    case 2:
                        node.FigureType = FigureTypes.Ellipse;
                        break;
                }
            }
        }

        // Node.FigureTypeによるComboboxのバインディングの動作が怪しいのでコーディングで
        public void UpdateComboBox()
        {
            var node = DataContext as Node;
            if (node != null)
            {
                var figure = node.FigureType;
                switch (figure)
                {
                    case FigureTypes.Rectangle:
                        comboBoxFigureType.SelectedIndex = 0;
                        break;
                    case FigureTypes.RoundedRect:
                        comboBoxFigureType.SelectedIndex = 1;
                        break;
                    case FigureTypes.Ellipse:
                        comboBoxFigureType.SelectedIndex = 2;
                        break;
                }
            }
        }
    }
}
