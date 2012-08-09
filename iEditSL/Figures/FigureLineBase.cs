using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using iEditSL.Utilities;
using iEditSL.Entities;

namespace iEditSL.Figures
{
    public class FigureLineBase : FigureBase
    {
        #region Fields
        // ドラッグ開始フラグ
        //private bool beganDrag = false;
        
        #endregion Fields

        # region Constructer

        public FigureLineBase()
        {

            this.MouseLeftButtonDown += new MouseButtonEventHandler(FigureLineBase_MouseLeftButtonDown);
            this.MouseMove += new MouseEventHandler(FigureLineBase_MouseMove);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(FigureLineBase_MouseLeftButtonUp);
        }
        
        # endregion Constructer

        #region Properties

        /// <summary>
        /// 端点(始点)を表す依存プロパティ
        /// </summary>
        public static readonly DependencyProperty PointFromProperty = DependencyProperty.Register(
            "PointFrom",
            typeof(Point),
            typeof(FigureLineBase),
            new PropertyMetadata(OnPointFromChanged));

        /// <summary>
        /// 端点(始点)
        /// </summary>
        public Point PointFrom
        {
            get { return (Point)GetValue(PointFromProperty); }
            set { SetValue(PointFromProperty, value); }
        }

        /// <summary>
        /// 端点(終点)を表す依存プロパティ
        /// </summary>
        public static readonly DependencyProperty PointToProperty = DependencyProperty.Register(
            "PointTo",
            typeof(Point),
            typeof(FigureLineBase),
            new PropertyMetadata(OnPointToChanged));

        /// <summary>
        /// 端点(終点)
        /// </summary>
        public Point PointTo
        {
            get { return (Point)GetValue(PointToProperty); }
            set { SetValue(PointToProperty, value); }
        }

        /// <summary>
        /// 矢印の方向性を表す依存プロパティ
        /// </summary>
        public static readonly DependencyProperty ArrowOrientationProperty = DependencyProperty.Register(
            "ArrowOrientation",
            typeof(ArrowOrientations),
            typeof(FigureLineBase),
            new PropertyMetadata(OnArrowOrientationChanged));

        /// <summary>
        /// 矢印の方向性
        /// </summary>
        public ArrowOrientations ArrowOrientation
        {
            get { return (ArrowOrientations)GetValue(ArrowOrientationProperty); }
            set { SetValue(ArrowOrientationProperty, value); }
        }
        
        #endregion Properties

        #region DependencyPropertyDelegates
        
        private static void OnPointFromChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as FigureLineBase).ChangePointFromProperty((Point)e.NewValue);
        }
        
        private static void OnPointToChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as FigureLineBase).ChangePointToProperty((Point)e.NewValue);
        }

        private static void OnArrowOrientationChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            (o as FigureLineBase).ChangeArrowOrientationProperty((ArrowOrientations)e.NewValue);
        }

        protected virtual void ChangePointFromProperty(Point point) { }

        protected virtual void ChangePointToProperty(Point point) { }

        protected virtual void ChangeArrowOrientationProperty(ArrowOrientations orientation) { }

        protected virtual void PointLinkAxis(Point point) { }
        
        protected virtual void DragLinkAxis(Point point) { }

        protected virtual void DropLinkAxis(Point point) { }
        
        #endregion DependencyPropertyDelegates

        #region Mouse Event Handlers

        void FigureLineBase_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ToolButtonState.Instance.FigureButtonState == FigureButtonStates.Line)
            {
                e.Handled = false;
                return;
            }
            
            Point point = e.GetPosition((UIElement)this.Parent);
            Point pointInner = e.GetPosition(this);
            this.BackupMousePoint(point);
            handleNumber = HitTestHandle(pointInner);

            CaptureMouse();
            if (Keyboard.Modifiers != ModifierKeys.Control && !TrackerVisible)
            {
                Document.Instance.UnSelectAllElements();
            }
            TrackerVisible = true;
            //beganDrag = true;
            if (Keyboard.Modifiers != ModifierKeys.Control)
            {
                Document.Instance.UpdateEdgeSelection();
            }
            PointLinkAxis(point);
            e.Handled = true;
        }
        
        void FigureLineBase_MouseMove(object sender, MouseEventArgs e)
        {
            var point = e.GetPosition(this);
            DragLinkAxis(point);
        }
        
        void FigureLineBase_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ReleaseMouseCapture();
            var point = e.GetPosition(this);
            DropLinkAxis(point);
            e.Handled = true;
            //beganDrag = false;
            handleNumber = 0;
        }
        
        #endregion Mouse Event Handlers

        #region Tracker

        public override int HandleCount
        {
            get
            {
                return 2;
            }
        }

        public override Point GetHandle(int handleNumber)
        {
            switch (handleNumber)
            {
                case 1:
                    return PointFrom;
                case 2:
                    return PointTo;
            }
            return PointFrom;
        }

        public override Cursor GetHandleCursor(int handleNumber)
        {
            return Cursors.Hand;
        }
        
        public override void ChangeTrackerControlVisiblity(bool visible) { }

        #endregion Tracker

    }
}
