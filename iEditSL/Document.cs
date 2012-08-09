using System.Collections.Generic;
using iEditSL.Entities;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Linq;
using System.IO;
using System.Text;
using System.Xml.Serialization;
using System;

namespace iEditSL
{
    /// <summary>
    /// XMLシリアライズ用のデータホルダー
    /// </summary>
    [System.Xml.Serialization.XmlRoot]
    public class iEditDocument
    {
        public List<Node> Nodes { get; set; }
        public List<Edge> Edges { get; set; }
    }
    
    /// <summary>
    /// Documentクラス
    /// </summary>
    public class Document : INotifyPropertyChanged
    {
        #region Fields
        
        private static Document instance = new Document();
        private List<Node> _root = new List<Node>();
        private List<IView> _views = new List<IView>();
        private int _number = 0;
        
        private ObservableCollection<Element> _elements = new ObservableCollection<Element>();

        private const double MIN_WIDTH = 600;
        private const double MIN_HEIGHT = 600;
        private double minWidth = MIN_WIDTH;
        private double minHeight = MIN_HEIGHT;
        private double zoomRatio = 1;
        private NodePropertyPage _nodePropertyPage;
        private EdgePropertyPage _edgePropertyPage;
        
        #endregion Fields

        #region Ctor

        private Document()
        {
            _elements.CollectionChanged += 
                new NotifyCollectionChangedEventHandler(elements_CollectionChanged);
            var node = new Node();
            node.Name = "主題";
            node.Width = 35;
            node.Height = 20;
            node.Left = 0;
            node.Top = 0;
            node.IsSelected = false;
            _root.Add(node);
        }

        #endregion Ctor

        #region Properties

        /// <summary>
        /// インスタンス取得
        /// </summary>
        public static Document Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// Root
        /// </summary>
        public List<Node> Root
        {
            get { return _root; }
            set { _root = value; }
        }
        
        /// <summary>
        /// 選択された要素を列挙するためのIEnumerableを得る
        /// </summary>
        private IEnumerable<Element> Selection
        {
            get
            {
                return (from e in _elements
                        where e.IsSelected
                        select e);
            }
        }

        /// <summary>
        /// ElementsからNodeの集合を取得する
        /// </summary>
        private IEnumerable<Node> Nodes
        {
            get
            {
                return (from e in _elements
                        where e is Node
                        select e as Node);
            }
        }

        /// <summary>
        /// ElementsからEdgeの集合を取得する
        /// </summary>
        private IEnumerable<Edge> Edges
        {
            get
            {
                return (from e in _elements
                        where e is Edge
                        select e as Edge);
            }
        }
        
        /// <summary>
        /// 全要素を包含する最小幅
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public double MinWidth
        { 
            get { return minWidth; }
            set
            {
                if ((double)value >= MIN_WIDTH * zoomRatio)
                {
                    minWidth = value;
                    OnPropertyChanged("MinWidth");
                }
            }
        }
        
        /// <summary>
        /// 全要素を包含する最小高
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public double MinHeight
        {
            get { return minHeight; }
            set
            {
                if ((double)value >= MIN_HEIGHT * zoomRatio)
                {
                    minHeight = value;
                    OnPropertyChanged("MinHeight");
                }
            }
        }

        /// <summary>
        /// ズーム表示の倍率を設定取得する
        /// </summary>
        [System.Xml.Serialization.XmlIgnore]
        public double ZoomRatio
        {
            get { return zoomRatio; }
            set
            {
                zoomRatio = value;
                CalcMinSize();
            }
        }

        /// <summary>
        /// 連番を取得する
        /// </summary>
        public int Number
        {
            get
            {
                _number++;
                return _number;
            }
        }

        #endregion Properties

        #region Functions

        /// <summary>
        /// Nodeのプロパティ設定画面を渡す
        /// </summary>
        /// <param name="page"></param>
        public void SetNodePropertyPage(NodePropertyPage page)
        {
            _nodePropertyPage = page;
        }

        /// <summary>
        /// Edgeのプロパティ設定画面を渡す
        /// </summary>
        /// <param name="page"></param>
        public void SetEdgePropertyPage(EdgePropertyPage page)
        {
            _edgePropertyPage = page;
        }

        /// <summary>
        /// Viewを追加する
        /// </summary>
        /// <param name="view">追加するView</param>
        public void AddView(IView view)
        {
            _views.Add(view);
            _elements.CollectionChanged += new NotifyCollectionChangedEventHandler(view.Update);
        }
        
        /// <summary>
        /// 要素を追加する
        /// </summary>
        /// <param name="element">追加する要素</param>
        public void Add(Element element)
        {
            _elements.Add(element);
        }

        /// <summary>
        /// すべての要素を包含する最小のサイズを再計算する
        /// 今はpublicにしておいて、移動イベントなどの終了時に呼ぶようにしている
        /// </summary>
        public void CalcMinSize()
        {
            MinWidth = MIN_WIDTH * zoomRatio;
            MinHeight = MIN_HEIGHT * zoomRatio;
            
            foreach (var node in Nodes)
            {
                double right = node.Left + node.Width;
                if (right * zoomRatio > MinWidth)
                {
                    MinWidth = right * zoomRatio;
                }
                double bottom = node.Top + node.Height;
                if (bottom * zoomRatio > MinHeight)
                {
                    MinHeight = bottom * zoomRatio;
                }
            }
        }

        /// <summary>
        /// すべての要素を非選択にする
        /// </summary>
        public void UnSelectAllElements()
        {
            _edgePropertyPage.Visibility = Visibility.Collapsed;
            foreach (var elem in _elements)
            {
                elem.IsSelected = false;
            }
        }
        
        /// <summary>
        /// 指定した矩形内にある要素を選択状態にする
        /// </summary>
        /// <param name="left">矩形の左辺</param>
        /// <param name="top">矩形の上辺</param>
        /// <param name="width">矩形の幅</param>
        /// <param name="height">矩形の高さ</param>
        public void SelectElementsInBound(double left, double top, double width, double height)
        {
            foreach (var element in _elements)
            {
                // TODO 判定メソッドをElementの仮想クラスとして持たせた方がよい
                element.IsSelected = element.Contained(left, top, width, height);
            }
        }

        /// <summary>
        /// 選択された要素を移動する
        /// </summary>
        /// <param name="dx">X方向の移動量</param>
        /// <param name="dy">Y方向の移動量</param>
        public void MoveElementsSelected(double dx, double dy)
        {
            foreach (var element in Selection)
            {
                element.MoveTo(dx, dy);
            }
        }

        /// <summary>
        /// NetViewでの選択状態とOutlineViewでの選択状態を同期させる
        /// 単一選択状態でのみ呼び出されることを前提としている
        /// </summary>
        public void UpdateSelection()
        {
            foreach (var item in Selection)
            {
                var node = item as Node;
                if (node != null)
                {
                    node.IsSelectedOnTree = true;
                    if (_nodePropertyPage != null)
                    {
                        _edgePropertyPage.Visibility = Visibility.Collapsed;
                        _nodePropertyPage.DataContext = node;
                        _nodePropertyPage.UpdateComboBox();
                    }
                    return;
                }
            }
        }

        /// <summary>
        /// エッジの選択状態を更新する
        /// </summary>
        public void UpdateEdgeSelection()
        {
            foreach (var edge in Edges)
            {
                if (edge.IsSelected)
                {
                    _edgePropertyPage.Visibility = Visibility.Visible;
                    _edgePropertyPage.DataContext = edge;
                    break;
                }
            }
        }

        /// <summary>
        /// 要素のHitTest
        /// </summary>
        /// <param name="point">テストするPoint</param>
        /// <returns>ヒットした要素</returns>
        // TODO:DocumentでHitTestせずにControlにクリックイベントを
        //  トンネリング(バブル?)させた方がいろいろな形状に対応できる
        //  BindされたFigureとElementの対応表が必要
        public Element HitTest(Point point)
        {
            foreach (var element in _elements)
            {
                if (element.HitTest(point))
                {
                    return element;
                }
            }
            return null;
        }

        /// <summary>
        /// Edgeの位置を再設定する
        /// </summary>
        public void ResetEdges()
        {
            foreach (var edge in Edges)
            {
                edge.Reset();
            }
        }

        /// <summary>
        /// Document をファイルに保存する
        /// </summary>
        /// <param name="fs">ファイルストリーム</param>
        public void Save(Stream fs)
        {
            var ser = new XmlSerializer(typeof(iEditDocument));
            var doc = new iEditDocument();
            doc.Nodes = Nodes.ToList(); //_root;
            doc.Edges = Edges.ToList();
            try
            {
                ser.Serialize(fs, doc);
                fs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("書き込みに失敗しました\r\n\r\n<エラー詳細>\r\n" + ex.ToString());
            }
        }

        /// <summary>
        /// ファイルを Document に読み出す
        /// </summary>
        /// <param name="reader"></param>
        public void Load(StreamReader reader)
        {
            var ser = new XmlSerializer(typeof(iEditDocument));
            var doc = new iEditDocument();
            try
            {
                doc = ser.Deserialize(reader) as iEditDocument;
                reader.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("読み出しに失敗しました\r\n\r\n<エラー詳細>\r\n" + ex.ToString());
                return;
            }
            //_root.Clear();
            //_root = doc.Nodes;
            var nodes = new List<Node>();
            nodes.Add(_root.Single());
            _root.Single().GetAllChildren(nodes);
            foreach (var node in doc.Nodes) // foreach (var node in nodes)
            {
                _elements.Add(node);
            }
            foreach (var edge in doc.Edges)
            {
                _elements.Add(edge);
            }
        }
        
        #endregion Functions

        #region Private Functions
        
        // 要素の集合に変更があったら再計算
        private void elements_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CalcMinSize();
        }

        #endregion Private Funcitons

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
