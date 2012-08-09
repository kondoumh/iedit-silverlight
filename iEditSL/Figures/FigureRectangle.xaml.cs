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

namespace iEditSL.Figures
{
    public partial class FigureRectangle : FigureRectangleBase
    {

        #region Ctors
        
        public FigureRectangle()
        {
            InitializeComponent();
            this.SizeChanged += new SizeChangedEventHandler(FigureRectangle_SizeChanged);
        }

        #endregion Ctors

        #region DependencyProperties
        public static DependencyProperty FigureTypeProperty = DependencyProperty.Register(
            "FigureType",
            typeof(FigureTypes),
            typeof(FigureRectangle),
            new PropertyMetadata(OnFigureTypeChanged));

        public FigureTypes FigureType
        {
            get { return (FigureTypes)GetValue(FigureTypeProperty); }
            set { SetValue(FigureTypeProperty, value); }
        }

        private static void OnFigureTypeChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as FigureRectangle).changeFigureType((FigureTypes)e.NewValue);
        }

        private void changeFigureType(FigureTypes type)
        {
            switch (type)
            {
                case FigureTypes.Rectangle:
                    ShapeRectangle.Visibility = Visibility.Visible;
                    ShapeRoundedRect.Visibility = Visibility.Collapsed;
                    ShapeEllipse.Visibility = Visibility.Collapsed;
                    break;
                case FigureTypes.RoundedRect:
                    ShapeRectangle.Visibility = Visibility.Collapsed;
                    ShapeRoundedRect.Visibility = Visibility.Visible;
                    ShapeEllipse.Visibility = Visibility.Collapsed;
                    break;
                case FigureTypes.Ellipse:
                    ShapeRectangle.Visibility = Visibility.Collapsed;
                    ShapeRoundedRect.Visibility = Visibility.Collapsed;
                    ShapeEllipse.Visibility = Visibility.Visible;
                    break;
            }
        }

        #endregion DependencyProperties

        #region Overrides

        public override void ChangeTrackerControlVisiblity(bool visible)
        {
            Tracker.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        #endregion Overrides

        #region Event Handlers

        // リサイズしたときにTrackerの位置を追従させる
        void FigureRectangle_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Canvas.SetLeft(tr1, -2.5);
            Canvas.SetTop(tr1, -2.5);
            Canvas.SetLeft(tr2, this.ActualWidth / 2 - 2.5);
            Canvas.SetTop(tr2, -2.5);
            Canvas.SetLeft(tr3, this.ActualWidth - 2.5);
            Canvas.SetTop(tr3, -2.5);
            Canvas.SetLeft(tr4, this.ActualWidth - 2.5);
            Canvas.SetTop(tr4, this.ActualHeight / 2 - 2.5);
            Canvas.SetLeft(tr5, this.ActualWidth - 2.5);
            Canvas.SetTop(tr5, this.ActualHeight - 2.5);
            Canvas.SetLeft(tr6, this.ActualWidth / 2 - 2.5);
            Canvas.SetTop(tr6, this.ActualHeight - 2.5);
            Canvas.SetLeft(tr7, -2.5);
            Canvas.SetTop(tr7, this.ActualHeight - 2.5);
            Canvas.SetLeft(tr8, -2.5);
            Canvas.SetTop(tr8, this.ActualHeight / 2 - 2.5);
        }
        
        #endregion Event Handlers
    }
}
