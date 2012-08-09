using System;
using System.Windows;
using System.Windows.Controls;
using iEditSL.Utilities;
using System.Windows.Data;
using System.Net.NetworkInformation;
using System.Windows.Media;
using System.IO;
using System.Net;

namespace iEditSL
{
    public partial class Page : UserControl
    {

        #region Fields
        
        private OOBStats _Stats = new OOBStats();
        SaveFileDialog _AsyncSaveDialog = new SaveFileDialog();

        #endregion Fields

        #region init
        
        public Page()
        {
            InitializeComponent();

            // DocumentとViewを接続
            Document.Instance.AddView(outlineView);
            Document.Instance.AddView(netView);

            // Documentとプロパティページを接続
            Document.Instance.SetNodePropertyPage(nodePropertyPage);
            Document.Instance.SetEdgePropertyPage(edgePropertyPage);
            
            // OutlineViewにRootNodeをバインドする
            outlineView.ItemsSource = Document.Instance.Root;
            
            // ToolButton.Click
            ButtonOpenFile.Click += new RoutedEventHandler(ButtonOpenFile_Click);
            ButtonSaveFile.Click += new RoutedEventHandler(ButtonSaveFile_Click);
            ButtonSaveToPng.Click += new RoutedEventHandler(ButtonSaveToPng_Click);
            ButtonAddSubNode.Click += new RoutedEventHandler(outlineView.ButtonAddSubNode_Click);
            ButtonAddSiblingNode.Click += new RoutedEventHandler(outlineView.ButtonAddSiblingNode_Click);
            ButtonLevelUp.Click += new RoutedEventHandler(outlineView.ButtonLevelUp_Click);
            ButtonLevelDown.Click += new RoutedEventHandler(outlineView.ButtonLevelDown_Click);
            ButtonPositionUp.Click += new RoutedEventHandler(outlineView.ButtonPositionUp_Click);
            ButtonPositionDown.Click += new RoutedEventHandler(outlineView.ButtonPositionDown_Click);
            ButtonSwichFullScreen.Click +=new RoutedEventHandler(ButtonSwichFullScreen_Click);
            ButtonDetach.Click += new RoutedEventHandler(ButtonDetach_Click);
            
            // NetViewのサイズとドキュメントサイズをバインド
            // TOOD:ScrollViewerにマウスホイールサポートを
            bindDocumentSize();
            // NetViewのツールボタンの状態をバインド
            ToolBar.DataContext = ToolButtonState.Instance;
            ToolButtonState.Instance.FigureButtonState = FigureButtonStates.Select;
            
            // NetViewにSliderコントロールを渡す
            netView.SetSliderControl(NetViewZoomer);
            netView.SetZoomValueTextBlock(ZoomValue);
            
            // リサイズとフルスクリーンイベント
            Application.Current.Host.Content.Resized += new EventHandler(DisplaySizeInformation);
            Application.Current.Host.Content.FullScreenChanged +=
                new EventHandler(DisplaySizeInformation);

            // インストール・ネットワーク関係イベント
            NetworkChange.NetworkAddressChanged +=
                new NetworkAddressChangedEventHandler(NetworkChange_NetworkAddressChanged);
            Application.Current.InstallStateChanged += new EventHandler(Current_InstallStateChanged);
            SetOutOfBrowserStats();
            StatusBar.DataContext = _Stats;
        }

        // DocumentとNetViewのサイズをバインド
        private void bindDocumentSize()
        {
            BindingFunctions.BindProperty(
                netView, Document.Instance,
                "MinWidth", Canvas.WidthProperty,
                BindingMode.TwoWay);

            BindingFunctions.BindProperty(
                netView, Document.Instance,
                "MinHeight", Canvas.HeightProperty,
                BindingMode.TwoWay);
        }
        
        #endregion init

        #region ToolButton Click Handlers

        void ButtonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            var openDialog = new OpenFileDialog();
            openDialog.Filter = "iEditSL File|*.iedsl|All Files|*.*";

            bool? open = openDialog.ShowDialog();
            if (open.HasValue && open.Value)
            {
                netView.Children.Clear();
                using (var reader = openDialog.File.OpenText())
                {
                    Document.Instance.Load(reader);
                }
            }
            //outlineView.ItemsSource = Document.Instance.Root;
        }

        void ButtonSaveFile_Click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog();
            saveDialog.DefaultExt = ".iedsl";
            saveDialog.Filter = "iEditSL File|*.iedsl|All Files|*.*";

            bool? open = saveDialog.ShowDialog();

            if (open.HasValue && open.Value)
            {
                using (var fs = saveDialog.OpenFile())
                {
                    Document.Instance.Save(fs);
                }
            }
        }

        void ButtonSaveToPng_Click(object sender, RoutedEventArgs e)
        {
            _AsyncSaveDialog.DefaultExt = ".jpg";
            _AsyncSaveDialog.Filter = "JPG File|*.jpg|All Files|*.*";

            bool? open = _AsyncSaveDialog.ShowDialog();

            if (open.HasValue && open.Value)
            {
                Uri uri = new Uri("http://silverlight.net/Themes/silverlight/images/logo.jpg");
                var webClient = new WebClient();
                webClient.OpenReadAsync(uri);
                webClient.OpenReadCompleted += new OpenReadCompletedEventHandler(webClient_OpenReadCompleted);
            }
        }

        private void ButtonSwichFullScreen_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Host.Content.IsFullScreen = !Application.Current.Host.Content.IsFullScreen;
        }

        private void ButtonDetach_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Application.Current.Install(); // take the app out of browser
            }
            catch (InvalidOperationException opex)
            {
                var mes = opex.Message;
                MessageBox.Show("iEdit SL はすでにインストールされています。");
            }
            catch (Exception ex)
            {
                MessageBox.Show("iEdit SL をインストールすることができませんでした。" + Environment.NewLine +
                  ex.Message);
            }
        }
       
        #endregion ToolButton Click Handlers

        #region Other Event Handlers
        
        /// <summary>
        /// ウィンドウリサイズイベント
        /// </summary>
        void DisplaySizeInformation(object sender, EventArgs e)
        {
            this.Width = Application.Current.Host.Content.ActualWidth;
            this.Height = Application.Current.Host.Content.ActualHeight;
        }

        /// <summary>
        /// アウトラインビューでの選択変更通知イベント
        /// </summary>
        private void OutlineView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            edgePropertyPage.Visibility = Visibility.Collapsed;
            nodePropertyPage.DataContext = outlineView.SelectedValue;
            nodePropertyPage.UpdateComboBox();
            outlineView.UpdateSelection();
        }

        /// <summary>
        /// アウトオブブラウザーインストール状態の変化
        /// </summary>
        void Current_InstallStateChanged(object sender, EventArgs e)
        {
            SetNetworkInstallState();
        }

        /// <summary>
        /// ネットワーク接続状態変化の通知イベント
        /// </summary>
        void NetworkChange_NetworkAddressChanged(object sender, EventArgs e)
        {
            _Stats.IsNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
            SetNetworkIndicator();
        }
        
        /// <summary>
        /// 非同期ダウンロード完了通知イベント
        /// </summary>
        void webClient_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            if (!e.Cancelled)
            {
                using (Stream fs = _AsyncSaveDialog.OpenFile())
                {
                    int length = Convert.ToInt32(e.Result.Length);
                    byte[] byteResult = new byte[length];
                    e.Result.Read(byteResult, 0, length);
                    fs.Write(byteResult, 0, byteResult.Length);
                    fs.Close();
                }
            }
            MessageBox.Show("今のところSilverlightのロゴ(Jpeg)を非同期にダウンロードするだけです。");
        }

        #endregion Other Event Handlers

        #region Out of Browser Helpers

        private void SetOutOfBrowserStats()
        {
            SetNetworkInstallState();
            _Stats.IsNetworkAvailable = NetworkInterface.GetIsNetworkAvailable();
            SetNetworkIndicator();
            SetOfflineStatus();
        }

        // 本当はデータバインドでやりたいがとりあえず
        private void SetNetworkIndicator()
        {
            var color = new Color();
            if (_Stats.IsNetworkAvailable)
            {
                color.R = 0; color.G = 255; color.B = 127; color.A = 255;
            }
            else
            {
                color.R = 127; color.G = 127; color.B = 127; color.A = 255;
            }
            NetworkIndicator.Fill = new SolidColorBrush(color);
        }

        private void SetOfflineStatus()
        {
            if (Application.Current.IsRunningOutOfBrowser)
                _Stats.RunningModeMessage = "デスクトップ";
            else
                _Stats.RunningModeMessage = "ブラウザー内";
        }

        /// <summary>
        /// Web サービスを利用する時ネットワーク接続状態をチェックする。
        /// </summary>
        /// <returns>接続状態の時 true を返す</returns>
        private bool CheckNetworkStatus()
        {
            if (NetworkInterface.GetIsNetworkAvailable())
                return true;

            MessageBox.Show("ネットワーク接続が検出できませんでした。");
            return false;
        }
        
        private void SetNetworkInstallState()
        {
            InstallState installState = Application.Current.InstallState;
            switch (installState)
            {
                case InstallState.NotInstalled:
                    _Stats.InstallStateMessage = "オンライン";
                    break;
                case InstallState.Installing:
                    _Stats.InstallStateMessage = "インストール中";
                    break;
                case InstallState.Installed:
                    _Stats.InstallStateMessage = "インストール済";
                    break;
                case InstallState.InstallFailed:
                    _Stats.InstallStateMessage = "インストール失敗";
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        #endregion Out of Browser Helpers
    }
}
