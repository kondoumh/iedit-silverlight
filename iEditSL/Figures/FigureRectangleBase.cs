using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using iEditSL.Utilities;

namespace iEditSL.Figures
{
    /// <summary>
    /// 矩形コントロールの基底クラス
    /// </summary>
    public class FigureRectangleBase : FigureBase
    {
        #region Fields

        // リサイズ開始時に矩形のオリジナル情報を対比するためのメンバー
        private double leftOrg, topOrg, rightOrg, bottomOrg, widthOrg, heightOrg;
        // ドラッグ開始フラグ
        private bool beganDrag = false;

        #endregion Fields

        #region Ctors

        public FigureRectangleBase()
        {
            this.MouseLeftButtonDown += new MouseButtonEventHandler(FigureRectangleBase_MouseLeftButtonDown);
            this.MouseMove += new MouseEventHandler(FigureRectangleBase_MouseMove);
            this.MouseLeftButtonUp += new MouseButtonEventHandler(FigureRectangleBase_MouseLeftButtonUp);
        }
        
        #endregion Ctors
        
        #region Mouse Event Handlers

        void FigureRectangleBase_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (ToolButtonState.Instance.FigureButtonState == FigureButtonStates.Line)
            {
                e.Handled = false;
                return;
            }
            
            Point point = e.GetPosition((UIElement)this.Parent);
            this.BackupMousePoint(point);
            Point ptInBound = e.GetPosition(this);

            Point pointInner = e.GetPosition(this);
            handleNumber = HitTestHandle(pointInner);

            topOrg = Canvas.GetTop(this);
            leftOrg = Canvas.GetLeft(this);
            widthOrg = this.Width;
            heightOrg = this.Height;
            rightOrg = leftOrg + widthOrg;
            bottomOrg = topOrg + heightOrg;

            CaptureMouse();
            beganDrag = true;
            if (Keyboard.Modifiers != ModifierKeys.Control && !TrackerVisible)
            {
                Document.Instance.UnSelectAllElements();
            }

            e.Handled = true;
            TrackerVisible = true;
            if (Keyboard.Modifiers != ModifierKeys.Control)
            {
                Document.Instance.UpdateSelection();
            }
        }
        
        void FigureRectangleBase_MouseMove(object sender, MouseEventArgs e)
        {
            
            Point pointInner = e.GetPosition(this);
            SetCurrentCursor(pointInner);

            if (!beganDrag) return;
            
            Point point = e.GetPosition((UIElement)this.Parent);
            if (handleNumber != 0)
            {
                ResizeControl(e, handleNumber);
            }
            else
            {
                double left = point.X - offSetX;
                double top = point.Y - offSetY;
                Canvas.SetLeft(this, left >= 0 ? left : 0);
                Canvas.SetTop(this, top >= 0 ? top : 0);
                double dx = point.X - prevPoint.X;
                double dy = point.Y - prevPoint.Y;
                Document.Instance.MoveElementsSelected(dx, dy);
                prevPoint = point;
            }
            Document.Instance.ResetEdges();
        }

        void FigureRectangleBase_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            handleNumber = 0;
            if (beganDrag)
            {
                beganDrag = false;
                e.Handled = true;
                Document.Instance.CalcMinSize();
            }
        }
        
        #endregion Mouse Event Handlers


        #region Overrides

        /// <summary>
        /// ハンドルの数を取得するプロパティ
        /// </summary>
        public override int HandleCount
        {
            get
            {
                return 8;
            }
        }

        /// <summary>
        /// 1を起点するハンドル番号のハンドルの座標を取得する
        /// 矩形の左上を基点とする座標が取得される
        /// </summary>
        public override Point GetHandle(int handleNumber)
        {
            double left = 0;
            double top = 0;
            double right = left + this.Width;
            double bottom = top + this.Height;
            double x = left;
            double y = top;
            double xCenter = (right + left) / 2;
            double yCenter = (bottom + top) / 2;

            switch (handleNumber)
            {
                case 1:
                    x = left;
                    y = top;
                    break;
                case 2:
                    x = xCenter;
                    y = top;
                    break;
                case 3:
                    x = right;
                    y = top;
                    break;
                case 4:
                    x = right;
                    y = yCenter;
                    break;
                case 5:
                    x = right;
                    y = bottom;
                    break;
                case 6:
                    x = xCenter;
                    y = bottom;
                    break;
                case 7:
                    x = left;
                    y = bottom;
                    break;
                case 8:
                    x = left;
                    y = yCenter;
                    break;
            }
            return new Point(x, y);
        }

        /// <summary>
        /// ハンドルに対応したカーソルを取得する
        /// </summary>
        public override Cursor GetHandleCursor(int handleNumber)
        {
            switch (handleNumber)
            {
                case 1:
                    return Cursors.Hand;
                case 2:
                    return Cursors.SizeNS;
                case 3:
                    return Cursors.Hand;
                case 4:
                    return Cursors.SizeWE;
                case 5:
                    return Cursors.Hand;
                case 6:
                    return Cursors.SizeNS;
                case 7:
                    return Cursors.Hand;
                case 8:
                    return Cursors.SizeWE;
                default:
                    return Cursors.Arrow;
            }
        }

        #endregion Overrides

        #region Resize Method

        /// <summary>
        /// ドラッグ中のトラッカハンドルの位置によりコントロールをリサイズする
        /// </summary>
        protected void ResizeControl(MouseEventArgs e, int HandleNumber)
        {
            if (handleNumber == 0) return;

            Point point = e.GetPosition((UIElement)this.Parent);
            double left = Canvas.GetLeft(this);
            double top = Canvas.GetTop(this);
            double width = this.Width;
            double height = this.Height;

            switch (handleNumber)
            {
                case 1:
                    left = Math.Min(point.X, rightOrg);
                    top = Math.Min(point.Y, bottomOrg);
                    width = Math.Abs(point.X - rightOrg);
                    height = Math.Abs(point.Y - bottomOrg);
                    break;
                case 2:
                    top = Math.Min(point.Y, bottomOrg);
                    height = Math.Abs(point.Y - bottomOrg);
                    break;
                case 3:
                    left = Math.Min(point.X, leftOrg);
                    top = Math.Min(point.Y, bottomOrg);
                    width = Math.Abs(point.X - leftOrg);
                    height = Math.Abs(point.Y - bottomOrg);
                    break;
                case 4:
                    left = Math.Min(leftOrg, point.X);
                    width = Math.Abs(leftOrg - point.X);
                    break;
                case 5:
                    left = Math.Min(point.X, leftOrg);
                    top = Math.Min(point.Y, topOrg);
                    width = Math.Abs(point.X - leftOrg);
                    height = Math.Abs(point.Y - topOrg);
                    break;
                case 6:
                    top = Math.Min(point.Y, topOrg);
                    height = Math.Abs(point.Y - topOrg);
                    break;
                case 7:
                    left = Math.Min(point.X, rightOrg);
                    top = Math.Min(point.Y, topOrg);
                    width = Math.Abs(point.X - rightOrg);
                    height = Math.Abs(point.Y - topOrg);
                    break;
                case 8:
                    left = Math.Min(point.X, rightOrg);
                    width = Math.Abs(point.X - rightOrg);
                    break;
            }
            Canvas.SetLeft(this, left >= 0 ? left : 0);
            Canvas.SetTop(this, top >= 0 ? top : 0);
            this.Width = width;
            this.Height = height;
        }
        
        #endregion Resize Method

        public override void ChangeTrackerControlVisiblity(bool visible) { }

    }
}
