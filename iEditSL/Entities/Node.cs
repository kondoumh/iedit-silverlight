using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;

namespace iEditSL.Entities
{
    public enum FigureTypes
    {
        Rectangle,
        RoundedRect,
        Ellipse
    }
    
    public class Node : Element
    {
        #region Fields

        // SubNodes
        private ObservableCollection<Node> _subNodes = new ObservableCollection<Node>();

        // Parent
        private Node _parent;
        
        private FigureTypes _figureType;

        private bool _isShowSubnodes;

        private bool _isSelectedOnTree;

        #endregion Fields

        #region Ctor

        public Node()
        {
        }

        #endregion Ctor

        #region Properties

        /// <summary>
        /// 図形属性
        /// </summary>
        public FigureTypes FigureType
        {
            get { return _figureType; }
            set
            {
                _figureType = value;
                OnPropertyChanged("FigureType");
            }
        }

        /// <summary>
        /// サブノードのコレクションを取得する
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public ObservableCollection<Node> SubNodes
        {
            get { return _subNodes; }
            set { _subNodes = (ObservableCollection<Node>)value; }
        }

        /// <summary>
        /// 親ノード
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public Node Parent
        {
            get { return _parent; }
            set
            {
                _parent = value;
                OnPropertyChanged("Parent");
            }
        }

        /// <summary>
        /// サブノードを表示するかどうか
        /// </summary>
        public bool IsShowSubnodes
        {
            get { return _isShowSubnodes; }
            set
            {
                _isShowSubnodes = value;
                OnPropertyChanged("IsShowSubnodes");
            }
        }

        /// <summary>
        /// TreeViewでの選択状態
        /// </summary>
        public bool IsSelectedOnTree
        {
            get { return _isSelectedOnTree; }
            set
            {
                _isSelectedOnTree = value;
                OnPropertyChanged("IsSelectedOnTree");
            }
        }
        
        #endregion Properties

        #region Overrides

        public override void MoveTo(double dx, double dy)
        {
            Left += dx;
            Top += dy;
        }

        public override bool Contained(double left, double top, double width, double height)
        {
            return Left >= left && Top >= top && Left + Width <= left + width && Top + Height <= top + height;
        }

        public override bool HitTest(Point point)
        {
            return (
                point.X >= Left &&
                point.X <= Left + Width &&
                point.Y >= Top &&
                point.Y <= Top + Height);
        }
        
        public override Point CenterPoint
        {
            get
            {
                return new Point((Left + Left + Width) / 2, (Top + Top + Height) / 2);
            }
        }
        
        public override Point GetConnectionPoint(Point pointFrom)
        {
            //return getClossPointByEquation(pointFrom, CenterPoint);
            return getClossPointByHitTest(pointFrom, CenterPoint);
        }

        #endregion Overrides

        #region Functions

        public void GetAllChildren(List<Node> nodes)
        {
            foreach (var node in SubNodes)
            {
                nodes.Add(node);
                if (node.SubNodes.Count > 0)
                {
                    node.GetAllChildren(nodes);
                }
            }
        }

        #endregion Functions

        #region ClossPoint Calculation

        private Point getClossPointByHitTest(Point pointFrom, Point pointTo)
        {
            var length = Math.Sqrt(
                Math.Pow(pointFrom.X - pointTo.X, 2) + Math.Pow(pointFrom.Y - pointTo.Y, 2));
            double step = 1 / length;
            var rtnPoint = pointFrom;
            for (double t = step; t < 1.0; t += step)
            {
                rtnPoint.X = pointFrom.X + (pointTo.X - pointFrom.X) * t;
                rtnPoint.Y = pointFrom.Y + (pointTo.Y - pointFrom.Y) * t;
                if (HitTest(rtnPoint)) break;
            }
            return rtnPoint;
        }
        
        private Point getClossPointByEquation(Point pointFrom, Point pointTo)
        {
            Point?[] points = new Point?[5];
            points[0] = CenterPoint;
            points[1] = calcClossPoint(
                pointFrom,
                pointTo,
                new Point(Left, Top),
                new Point(Left + Width, Top));
            points[2] = calcClossPoint(
                pointFrom,
                pointTo,
                new Point(Left + Width, Top),
                new Point(Left + Width, Top + Height));
            points[3] = calcClossPoint(
                pointFrom,
                pointTo,
                new Point(Left + Width, Top + Height),
                new Point(Left, Top + Height));
            points[4] = calcClossPoint(
                pointFrom,
                pointTo,
                new Point(Left, Top + Height),
                new Point(Left, Top));

            double[] diffs = new double[5];
            diffs[0] = double.MaxValue;
            diffs[1] = points[1].HasValue ?
                Math.Abs(pointFrom.X - points[1].Value.X) + Math.Abs(pointFrom.Y - points[1].Value.Y) :
                double.MaxValue;

            diffs[2] = points[2].HasValue ?
                Math.Abs(pointFrom.X - points[2].Value.X) + Math.Abs(pointFrom.Y - points[2].Value.Y) :
                double.MaxValue;

            diffs[3] = points[3].HasValue ?
                Math.Abs(pointFrom.X - points[3].Value.X) + Math.Abs(pointFrom.Y - points[3].Value.Y) :
                double.MaxValue;
            diffs[4] = points[4].HasValue ?
                Math.Abs(pointFrom.X - points[4].Value.X) + Math.Abs(pointFrom.Y - points[4].Value.Y) :
                double.MaxValue;

            Point rtnPoint = CenterPoint;
            for (int i = 1; i < 5; i++)
            {
                if (points[i].HasValue)
                {
                    if (diffs[i] <= diffs[i - 1])
                    {
                        rtnPoint = points[i].Value;
                    }
                }
            }

            return rtnPoint;
        }
        
        private Point? calcClossPoint(Point pointFrom1, Point pointTo1, Point pointFrom2, Point pointTo2)
        {
            double x1 = pointFrom1.X;
            double y1 = pointFrom1.Y;
            double x2 = pointTo1.X;
            double y2 = pointTo1.Y;
            double x3 = pointFrom2.X;
            double y3 = pointFrom2.Y;
            double x4 = pointTo2.X;
            double y4 = pointTo2.Y;

            double a, b, c, d, e, f, x, y;

            if (x1 == x2)
            {
                a = 1; b = 0; c = x1;
            }
            else
            {
                a = (y2 - y1) / (x2 - x1);
                b = -1;
                c = ((y2 - y1) / (x2 - x1)) * x1 - y1;
            }

            if (x3 == x4)
            {
                d = 1; e = 0; f = x3;
            }
            else
            {
                d = (y4 - y3) / (x4 - x3);
                e = -1;
                f = ((y4 - y3) / (x4 - x3)) * x3 - y3;
            }
            if (a * e == b * d)
            {
                if (x1 * y1 + x2 * y2 + x3 * y3 != x1 * y2 + x2 * y3 + x3 * y1)
                {
                    return null;
                }
                x = (x1 + x2 + x3 + x4) / 4;
                y = (y1 + y2 + y3 + y4) / 4;
            }
            else
            {
                x = (c * e - b * f) / (a * e - b * d);
                y = (c * d - a * f) / (b * d - a * e);
            }

            if (x1 <= x2)
            {
                if (x1 > x || x > x2) return null;
            }
            else
            {
                if (x2 > x || x > x1) return null;
            }

            if (x3 <= x4)
            {
                if (x3 > x || x > x4) return null;
            }
            else
            {
                if (x4 > x || x > x3) return null;
            }

            if (y1 <= y2)
            {
                if (y1 > y || y > y2) return null;
            }
            else
            {
                if (y2 > y || y > y1) return null;
            }

            if (y3 <= y4)
            {
                if (y3 > y || y > y4) return null;
            }
            else
            {
                if (y4 > y || y > y3) return null;
            }
            return new Point(x, y);
        }
        
        #endregion ClossPoint Calculation
    }
}
