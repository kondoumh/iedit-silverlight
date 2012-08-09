using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using iEditSL.Entities;
using System.Windows.Data;
using iEditSL.Utilities;
using iEditSL.Figures;
using System.Collections.Specialized;

namespace iEditSL
{
    public class NetView : Canvas, IView
    {
        private Shape _rubberBand;
        private Line _previewLine;
        private Point _lastPoint;
        private Slider _zoomSlider;
        private TextBlock _zoomValue;
        private Edge _newEdge;
        
        public NetView()
        {
            this.MouseLeftButtonDown += new MouseButtonEventHandler(NetView_MouseLeftButtonDown);
            this.MouseMove += new MouseEventHandler(NetView_MouseMove);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(NetView_MouseLeftButtonUp);
            var root = Document.Instance.Root[0];
            if (root != null)
            {
                Document.Instance.Add(Document.Instance.Root[0]);
                bindNodeFigure(root);
            }
        }

        public void SetSliderControl(Slider slider)
        {
            _zoomSlider = slider;
            _zoomSlider.ValueChanged += new RoutedPropertyChangedEventHandler<double>(zoomSlider_ValueChanged);
        }
        
        public void SetZoomValueTextBlock(TextBlock textBlock)
        {
            _zoomValue = textBlock;
        }

        void zoomSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            var scale = e.NewValue / 10;
            if (_zoomValue != null)
            {
                _zoomValue.Text = ((int)(_zoomSlider.Value) * 10).ToString() + "%";
            }
            var st = new ScaleTransform();
            st.ScaleX = scale;
            st.ScaleY = scale;
            this.RenderTransform = st;
            Document.Instance.ZoomRatio = scale;
        }
        
        void NetView_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Document.Instance.UnSelectAllElements();
            Point point = e.GetPosition(this);
            _lastPoint = point;
            if (ToolButtonState.Instance.FigureButtonState == FigureButtonStates.Line)
            {
                var elementFrom = Document.Instance.HitTest(point);
                if (elementFrom == null)
                {
                    ToolButtonState.Instance.FigureButtonState = FigureButtonStates.Select;
                    return;
                }
                if (_previewLine == null)
                {
                    _previewLine = new Line();
                    _previewLine.Stroke = new SolidColorBrush(Colors.DarkGray);
                    _previewLine.StrokeThickness = 2;
                    var dc = new DoubleCollection();
                    dc.Add(4); dc.Add(1);
                    _previewLine.StrokeDashArray = dc;
                    _previewLine.Visibility = Visibility.Visible;
                    _previewLine.X1 = point.X;
                    _previewLine.Y1 = point.Y;
                    _previewLine.X2 = point.X;
                    _previewLine.Y2 = point.Y;
                    this.Children.Add(_previewLine);
                }
                _newEdge = new Edge();
                _newEdge.AssociationFrom = elementFrom;
            }
            else
            {
                if (_rubberBand == null)
                {
                    if (ToolButtonState.Instance.FigureButtonState == FigureButtonStates.Select ||
                        ToolButtonState.Instance.FigureButtonState == FigureButtonStates.Rectangle ||
                        ToolButtonState.Instance.FigureButtonState == FigureButtonStates.RoundedRect)
                    {
                        var rect = new Rectangle();
                        if (ToolButtonState.Instance.FigureButtonState == FigureButtonStates.RoundedRect)
                        {
                            rect.RadiusX = 15;
                            rect.RadiusY = 15;
                        }
                        _rubberBand = rect;
                    }
                    else
                    {
                        _rubberBand = new Ellipse();
                    }

                    _rubberBand.StrokeThickness = 2;
                    if (ToolButtonState.Instance.FigureButtonState == FigureButtonStates.Select)
                    {
                        _rubberBand.Stroke = new SolidColorBrush(Colors.DarkGray);
                        var dc = new DoubleCollection();
                        dc.Add(4); dc.Add(1);
                        _rubberBand.StrokeDashArray = dc;
                    }
                    else
                    {
                        _rubberBand.Stroke = new SolidColorBrush(Colors.Black);
                        _rubberBand.Opacity = 0.5;
                    }
                    _rubberBand.Visibility = Visibility.Visible;
                    _rubberBand.Width = 1;
                    _rubberBand.Height = 1;
                    Canvas.SetLeft(_rubberBand, point.X);
                    Canvas.SetTop(_rubberBand, point.Y);
                    this.Children.Add(_rubberBand);
                }
            }
            this.CaptureMouse();
        }

        void NetView_MouseMove(object sender, MouseEventArgs e)
        {
            Point point = e.GetPosition(this);
            if (ToolButtonState.Instance.FigureButtonState == FigureButtonStates.Line)
            {
                if (_previewLine == null) return;
                _previewLine.X2 = point.X;
                _previewLine.Y2 = point.Y;
            }
            else
            {
                if (_rubberBand == null) return;
                // 直前と現在のポインティング位置の差を計算
                double dx = point.X - _lastPoint.X;
                double dy = point.Y - _lastPoint.Y;

                //System.Diagnostics.Debug.WriteLine(point.X.ToString() + "," + point.Y.ToString());

                _rubberBand.Width = Math.Abs(dx);
                _rubberBand.Height = Math.Abs(dy);
                double left = Math.Min(_lastPoint.X, point.X);
                double top = Math.Min(_lastPoint.Y, point.Y);
                Canvas.SetLeft(_rubberBand, left);
                Canvas.SetTop(_rubberBand, top);
            }
        }

        void NetView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            Point point = e.GetPosition(this);
            if (ToolButtonState.Instance.FigureButtonState == FigureButtonStates.Line)
            {
                if (_previewLine != null)
                {
                    this.Children.Remove(_previewLine);
                    _previewLine = null;
                }
                var elementTo = Document.Instance.HitTest(point);
                if (_newEdge != null && elementTo != null)
                {
                    _newEdge.AssociationTo = elementTo;
                    addFigureLine();
                }
                else
                {
                    _newEdge = null;
                }
            }
            else
            {
                if (_rubberBand != null)
                {
                    this.Children.Remove(_rubberBand);
                    _rubberBand = null;
                }
            }
            this.ReleaseMouseCapture();

            if (ToolButtonState.Instance.FigureButtonState != FigureButtonStates.Select &&
                ToolButtonState.Instance.FigureButtonState != FigureButtonStates.Line)
            {
                addFigureRectangle(point);
            }
            else
            {
                double left = Math.Min(point.X, _lastPoint.X);
                double top = Math.Min(point.Y, _lastPoint.Y);
                double width = Math.Abs(point.X - _lastPoint.X);
                double height = Math.Abs(point.Y - _lastPoint.Y);
                Document.Instance.SelectElementsInBound(left, top, width, height);
            }
        }

        private void addFigureRectangle(Point point)
        {
            var node = new Node();
            node.Left = Math.Min(point.X, _lastPoint.X);
            node.Top = Math.Min(point.Y, _lastPoint.Y);
            node.Width = Math.Abs(point.X - _lastPoint.X);
            node.Height = Math.Abs(point.Y - _lastPoint.Y);
            node.Name = "新しいノード" + Document.Instance.Number.ToString();

            if (ToolButtonState.Instance.FigureButtonState == FigureButtonStates.Rectangle)
            {
                node.FigureType = FigureTypes.Rectangle;
            }
            else if (ToolButtonState.Instance.FigureButtonState == FigureButtonStates.RoundedRect)
            {
                node.FigureType = FigureTypes.RoundedRect;
            }
            else if (ToolButtonState.Instance.FigureButtonState == FigureButtonStates.Ellipse)
            {
                node.FigureType = FigureTypes.Ellipse;
            }

            Document.Instance.Add(node);
            node.IsSelected = true;
            
            ToolButtonState.Instance.FigureButtonState = FigureButtonStates.Select;
        }

        private void bindNodeFigure(Node node)
        {
            var figure = new FigureRectangle();
            figure.DataContext = node;
            BindingFunctions.BindProperty(figure, node, "FigureType",
                FigureRectangle.FigureTypeProperty, BindingMode.TwoWay);
            BindingFunctions.BindProperty(figure, node, "IsSelected",
                FigureBase.TrackerVisibleProperty, BindingMode.TwoWay);
            this.Children.Add(figure);
        }
        
        private void addFigureLine()
        {
            var line = new FigureLine();
            line.DataContext = _newEdge;
            BindingFunctions.BindProperty(line, _newEdge, "PointFrom",
                FigureLineBase.PointFromProperty, BindingMode.TwoWay);
            BindingFunctions.BindProperty(line, _newEdge, "PointTo",
                FigureLineBase.PointToProperty, BindingMode.TwoWay);
            BindingFunctions.BindProperty(line, _newEdge, "IsSelected",
                FigureBase.TrackerVisibleProperty, BindingMode.TwoWay);
            BindingFunctions.BindProperty(line, _newEdge, "ArrowOrientation",
                FigureLineBase.ArrowOrientationProperty, BindingMode.TwoWay);
            this.Children.Add(line);
            Document.Instance.Add(_newEdge);
            _newEdge.IsSelected = true;
            ToolButtonState.Instance.FigureButtonState = FigureButtonStates.Select;
            Document.Instance.UpdateEdgeSelection();
        }

        #region IView メンバ

        // TODO シリアル化対応 Linkが来たときの処理も書く
        public void Update(object sender, NotifyCollectionChangedEventArgs e)
        {
            var node = e.NewItems[0] as Node;
            if (node == null) return;
            var rnd = new Random();
            if (node.Width <= 1)
            {
                node.Left = rnd.Next(300);
                node.Top = rnd.Next(300);
                node.Width = 80;
                node.Height = 40;
            }
            bindNodeFigure(node);
            Document.Instance.UnSelectAllElements();
            node.IsSelected = true;
            Document.Instance.UpdateSelection();
        }

        #endregion
    }
}
