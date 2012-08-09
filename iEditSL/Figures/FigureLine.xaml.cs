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
    public partial class FigureLine : FigureLineBase
    {
        public FigureLine()
        {
            InitializeComponent();
        }

        protected override void ChangePointFromProperty(Point point)
        {
            HitTestLine.X1 = point.X;
            HitTestLine.Y1 = point.Y;
            LinkLine.X1 = point.X;
            LinkLine.Y1 = point.Y;
            Canvas.SetLeft(tr1, point.X - 2.5);
            Canvas.SetTop(tr1, point.Y - 2.5);
            resetArrowFrom();
            resetArrowTo();
            resetNameLabel();
        }

        protected override void ChangePointToProperty(Point point)
        {
            HitTestLine.X2 = point.X;
            HitTestLine.Y2 = point.Y;
            LinkLine.X2 = point.X;
            LinkLine.Y2 = point.Y;
            Canvas.SetLeft(tr2, point.X - 2.5);
            Canvas.SetTop(tr2, point.Y - 2.5);
            resetArrowFrom();
            resetArrowTo();
            resetNameLabel();
        }

        protected override void ChangeArrowOrientationProperty(iEditSL.Entities.ArrowOrientations orientation)
        {
            switch (orientation)
            {
                case ArrowOrientations.None:
                    ArrowFrom.Visibility = Visibility.Collapsed;
                    ArrowTo.Visibility = Visibility.Collapsed;
                    break;
                case ArrowOrientations.OneWay:
                    ArrowFrom.Visibility = Visibility.Collapsed;
                    ArrowTo.Visibility = Visibility.Visible;
                    break;
                case ArrowOrientations.TwoWay:
                    ArrowFrom.Visibility = Visibility.Visible;
                    ArrowTo.Visibility = Visibility.Visible;
                    break;
            }
        }
        
        public override void ChangeTrackerControlVisiblity(bool visible)
        {
            Tracker.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void resetNameLabel()
        {
            Canvas.SetLeft(NameLabel, (LinkLine.X1 + LinkLine.X2) / 2);
            Canvas.SetTop(NameLabel, (LinkLine.Y1 + LinkLine.Y2) / 2);
        }
        
        private void resetArrowTo()
        {
            var points = new PointCollection();
            var x = LinkLine.X2;
            var y = LinkLine.Y2;
            points.Add(new Point(x, y));
            points.Add(new Point(x - 15, y - 5));
            points.Add(new Point(x - 15, y + 5));
            ArrowTo.Points = points;
            var transform = new RotateTransform();
            transform.CenterX = x;
            transform.CenterY = y;
            var dx = LinkLine.X2 - LinkLine.X1;
            var dy = LinkLine.Y2 - LinkLine.Y1;
            transform.Angle = Math.Atan2(dy, dx) * 180 / Math.PI;
            ArrowTo.RenderTransform = transform;
        }

        private void resetArrowFrom()
        {
            var points = new PointCollection();
            var x = LinkLine.X1;
            var y = LinkLine.Y1;
            points.Add(new Point(x, y));
            points.Add(new Point(x + 15, y - 5));
            points.Add(new Point(x + 15, y + 5));
            ArrowFrom.Points = points;
            var transform = new RotateTransform();
            transform.CenterX = x;
            transform.CenterY = y;
            var dx = LinkLine.X2 - LinkLine.X1;
            var dy = LinkLine.Y2 - LinkLine.Y1;
            transform.Angle = Math.Atan2(dy, dx) * 180 / Math.PI;
            ArrowFrom.RenderTransform = transform;
        }
    }
}
