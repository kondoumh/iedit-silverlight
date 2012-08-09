using System.Collections.Specialized;

namespace iEditSL
{
    /// <summary>
    /// Viewのインターフェース
    /// </summary>
    public interface IView
    {
        /// <summary>
        /// Documentの更新通知メソッド
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">変更のあったNodeのコレクション</param>
        void Update(object sender, NotifyCollectionChangedEventArgs e);
    }
}
