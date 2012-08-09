using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Windows;

namespace iEditSL.Entities
{
    /// <summary>
    /// Edgeの線種
    /// </summary>
    public enum LineTypes
    {
        Solid,    // 実線
        Dashdot   // 破線
    }

    /// <summary>
    /// グラフ構造の要素となる基底クラス
    /// </summary>
    public abstract class Element : INotifyPropertyChanged
    {
        #region Fields
        // 選択状態
        private bool isSelected = false;
        
        // 位置とサイズ(包含する最小の矩形)
        private string _name;
        private int _id;
        private double _left = 0;
        private double _top = 0;
        private double _width = 1;
        private double _height = 1; 
        
        private double _thickness = 1;
        private LineTypes _lineType;
        private string _lineColor;

        #endregion Fields

        #region Ctor
        
        /// <summary>
        /// コンストラクター Idを初期化
        /// </summary>
        protected Element()
        {
            _id = this.GetHashCode();
        }
        
        #endregion Ctor

        #region Properties

        /// <summary>
        /// ID
        /// </summary>
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }
        
        /// <summary>
        /// 名称
        /// </summary>
        public String Name
        {
            get { return _name; }
            set
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        
        /// <summary>
        /// 選択状態の取得・設定
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return isSelected;
            }
            set
            {
                isSelected = value;
                OnPropertyChanged("IsSelected");
            }
        }
        
        #region Position and Size

        /// <summary>
        /// 領域の左端
        /// </summary>
        public double Left
        {
            get
            {
                return _left;
            }
            set
            {
                if (value >= 0)
                {
                    _left = value;
                    OnPropertyChanged("Left");
                }
            }
        }

        /// <summary>
        /// 領域の上端
        /// </summary>
        public double Top
        {
            get
            {
                return _top;
            }
            set
            {
                if (value >= 0)
                {
                    _top = value;
                    OnPropertyChanged("Top");
                }
            }
        }

        /// <summary>
        /// 領域の幅
        /// </summary>
        public double Width
        {
            get
            {
                return _width;
            }
            set
            {
                if (value >= 1)
                {
                    _width = value;
                    OnPropertyChanged("Width");
                }
            }
        }

        /// <summary>
        /// 領域の高さ
        /// </summary>
        public double Height
        {
            get
            {
                return _height;
            }
            set
            {
                if (value >= 1)
                {
                    _height = value;
                    OnPropertyChanged("Height");
                }
            }
        }

        /// <summary>
        /// 領域の中心点
        /// </summary>
        public abstract Point CenterPoint { get; }
        
        #endregion Position and Size


        /// <summary>
        /// 線幅
        /// </summary>
        public double Thickness
        {
            get { return _thickness; }
            set
            {
                _thickness = value;
                OnPropertyChanged("Thickness");
            }
        }

        /// <summary>
        /// 線種
        /// </summary>
        public LineTypes LineType
        {
            get { return _lineType; }
            set
            {
                _lineType = value;
                OnPropertyChanged("LineType");
            }
        }

        /// <summary>
        /// 線色
        /// </summary>
        public string LineColor
        {
            get { return _lineColor; }
            set
            {
                _lineColor = value;
                OnPropertyChanged("LineColor");
            }
        }
        
        #endregion Properties

        #region Abstract Methods

        /// <summary>
        /// Elementの移動を行うための抽象メソッド
        /// </summary>
        /// <param name="dx">X軸方向の移動量</param>
        /// <param name="dy">Y軸方向の移動量</param>
        public abstract void MoveTo(double dx, double dy);

        /// <summary>
        /// Elementが指定の領域に含まれるか判定する抽象メソッド
        /// </summary>
        /// <param name="left">左端</param>
        /// <param name="top">上端</param>
        /// <param name="width">幅</param>
        /// <param name="height">高さ</param>
        /// <returns></returns>
        public abstract bool Contained(double left, double top, double width, double height);

        /// <summary>
        /// ヒットテスト
        /// </summary>
        /// <param name="point">Point</param>
        /// <returns></returns>
        public abstract bool HitTest(Point point);

        /// <summary>
        /// 線分と領域の交点を求める
        /// </summary>
        /// <param name="pointFrom">線分の開始点</param>
        /// <param name="pointTo">線分の終了点</param>
        /// <returns></returns>
        public abstract Point GetConnectionPoint(Point pointFrom);

        #endregion Abstract Methods

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;
        
        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
        
    }
}
