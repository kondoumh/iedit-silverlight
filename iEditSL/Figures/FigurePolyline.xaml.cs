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

namespace iEditSL.Figures
{
    public partial class FigurePolyline : FigureLineBase
    {
        #region Fields
        
        // 移動対象の頂点
        private Point? _axis;

        #endregion Fields

        #region Ctor

        public FigurePolyline()
        {
            InitializeComponent();
        }

        #endregion Ctor

        #region Overrides

        protected override void ChangePointFromProperty(Point point)
        {
            var pointFrom = LinkPolyLine.Points.First();
            LinkPolyLine.Points.Remove(pointFrom);
            LinkPolyLine.Points.Insert(0, point);
        }

        protected override void ChangePointToProperty(Point point)
        {
            var pointTo = LinkPolyLine.Points.Last();
            LinkPolyLine.Points.Remove(pointTo);
            LinkPolyLine.Points.Add(point);
        }

        protected override void ChangeArrowOrientationProperty(iEditSL.Entities.ArrowOrientations orientation)
        {
        }

        protected override void PointLinkAxis(Point point)
        {
            _axis = hitTestAxis(point);
            if (_axis.HasValue)
            {
                // 頂点をドラッグ開始
            }
            else
            {
                // 頂点を追加(_axisに格納)
            }
        }
        
        protected override void DragLinkAxis(Point point)
        {
            _axis = point;
        }

        protected override void DropLinkAxis(Point point)
        {
        }

        #endregion Overrides

        private Point? hitTestAxis(Point point)
        {
            var margin = 2.5;
            foreach (var pt in LinkPolyLine.Points)
            {
                var r = new Rect(pt.X - margin, pt.Y - margin, margin * 2, margin * 2);
                if (r.Contains(point)) return pt;
            }
            return null;
        }
    }
}
