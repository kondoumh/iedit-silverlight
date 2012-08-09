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
    public partial class EdgePropertyPage : UserControl
    {
        public EdgePropertyPage()
        {
            InitializeComponent();
            TBName.KeyUp += new KeyEventHandler(TBName_KeyUp);
        }

        void TBName_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                ComboOrientation.Focus();
            }
        }
    }
}
