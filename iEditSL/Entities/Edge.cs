using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace iEditSL.Entities
{
    /// <summary>
    /// 矢印の方向性
    /// </summary>
    public enum ArrowOrientations
    {
        None,
        OneWay,   
        TwoWay
    }

    /// <summary>
    /// 矢印の種類
    /// </summary>
    public enum ArrowTypes
    {
        Normal,
        Dependency,
        Aggrigation,
        Composition
    }
    
    public class Edge : Element
    {
        #region Fields
        
        private Point _pointFrom = new Point();
        private Point _pointTo = new Point();
        private Element _associationFrom;
        private Element _associationTo;
        private ArrowOrientations _arrowOrientation;

        #endregion Fields

        #region Ctor

        public Edge()
        {
        }
        
        #endregion Ctor

        #region Properties

        /// <summary>
        /// 端点(From)
        /// </summary>
        public Point PointFrom
        {
            get
            {
                return _pointFrom;
            }
            set
            {
                _pointFrom = value;
                calcSize();
                OnPropertyChanged("PointFrom");
            }
        }

        /// <summary>
        /// 端点(To)
        /// </summary>
        public Point PointTo
        {
            get
            {
                return _pointTo;
            }
            set
            {
                _pointTo = value;
                calcSize();
                OnPropertyChanged("PointTo");
            }
        }

        /// <summary>
        /// 矢印方向
        /// </summary>
        public ArrowOrientations ArrowOrientation
        {
            get { return _arrowOrientation; }
            set
            {
                _arrowOrientation = value;
                OnPropertyChanged("ArrowOrientation");
            }
        }

        [System.Xml.Serialization.XmlIgnore]
        public override Point CenterPoint
        {
            get { return new Point((_pointFrom.X + _pointTo.X) / 2, (_pointFrom.Y + _pointTo.Y) / 2); }
        }

        /// <summary>
        /// 関連端(From)
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public Element AssociationFrom
        {
            get { return _associationFrom; }
            set
            {
                _associationFrom = value;
                resetPointFromTo();
                OnPropertyChanged("AssociationFrom");
            }
        }

        /// <summary>
        /// 関連端(To)
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public Element AssociationTo
        {
            get { return _associationTo; }
            set
            {
                _associationTo = value;
                resetPointFromTo();
                OnPropertyChanged("AssociationTo");
            }
        }

        void _associationEnd_Moved(object sender, EventArgs e)
        {
            resetPointFromTo();
        }

        #endregion Properties

        #region Overrides

        public override void MoveTo(double dx, double dy)
        {
            PointFrom = new Point(PointFrom.X + dx, PointFrom.Y + dy);
            PointTo = new Point(PointTo.X + dx, PointTo.Y + dy);
        }

        public override bool Contained(double left, double top, double width, double height)
        {
            double l = Math.Min(PointFrom.X, PointTo.X);
            double t = Math.Min(PointFrom.Y, PointTo.Y);
            double r = Math.Max(PointFrom.X, PointTo.X);
            double b = Math.Max(PointFrom.Y, PointTo.Y);
            return l >= left && t >= top && r <= left + width && b <= top + height;
        }

        public override bool HitTest(Point point)
        {
            /* LineGeometoryにStrokeContainsメソッドがない(WPFにはある)ので
             * 点と線の距離を計算(線を曲げる機能つけると線分毎にやる) */
            var dx = _pointTo.X - _pointFrom.X;
            var dy = _pointTo.Y - _pointFrom.Y;
            var l2 = Math.Pow(dx, 2) + Math.Pow(dy, 2);
            var rx = point.X - _pointFrom.X;
            var ry = point.Y - _pointFrom.Y;
            var w = rx * dx + ry * dy;
            if (l2 < w)
            {
                rx = _pointTo.X - point.X;
                ry = _pointTo.Y - point.Y;
                w = rx * dx + ry * dy;
            }
            double dist = -1;
            if (w < 0)
            {
                dist = Math.Sqrt(Math.Pow(rx, 2) + Math.Pow(ry, 2));
            }
            else
            {
                dist = Math.Abs(rx * dy - ry * dx) / Math.Sqrt(l2);
            }
            return dist <= 4.0;
        }
        
        #endregion Overrides

        #region Functions

        public void Reset()
        {
            resetPointFromTo();
        }
        
        private void calcSize()
        {
            Width = Math.Max(_pointFrom.X, _pointTo.X) + 15;
            Height = Math.Max(_pointFrom.Y, _pointTo.Y) + 15;
        }

        private void resetPointFromTo()
        {
            if (_associationFrom == null || _associationTo == null) return;
            var fromCenter = _associationFrom.CenterPoint;
            var toCenter = _associationTo.CenterPoint;
            var center = new Point();
            center.X = (fromCenter.X + toCenter.X) / 2;
            center.Y = (fromCenter.Y + toCenter.Y) / 2;
            PointFrom = _associationFrom.GetConnectionPoint(center);
            PointTo = _associationTo.GetConnectionPoint(center);
        }
        
        #endregion Functions

        public override Point GetConnectionPoint(Point pointFrom)
        {
            return CenterPoint;
        }
    }
}
