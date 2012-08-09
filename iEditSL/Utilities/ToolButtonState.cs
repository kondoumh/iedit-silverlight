using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;

namespace iEditSL.Utilities
{
    public enum FigureButtonStates
    {
        Select,
        Rectangle,
        RoundedRect,
        Ellipse,
        Line,
        Polyline
    }
    
    public class ToolButtonState : INotifyPropertyChanged
    {
        private static ToolButtonState instance = new ToolButtonState();
        private ToolButtonState() { }

        public static ToolButtonState Instance
        {
            get { return instance; }
        }

        private FigureButtonStates _figureButtonState;
        public FigureButtonStates FigureButtonState
        {
            get { return _figureButtonState; }
            set
            {
                _figureButtonState = value;
                OnPropertyChanged("FigureButtonState");
            }
        }

        #region INotifyPropertyChanged メンバ

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
