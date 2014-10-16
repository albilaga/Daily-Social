using DailySocial.Models;
using System;
using System.ComponentModel;

namespace DailySocial.ViewModel
{
    public class ArticleViewModel : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged implementation

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion INotifyPropertyChanged implementation

        private RootArticleModel _Model;

        public ArticleViewModel(RootArticleModel model = null)
        {
            _Model = model ?? new RootArticleModel();
        }

        public string Status
        {
            get
            {
                return _Model.Status;
            }
            set
            {
                if (value != _Model.Status)
                {
                    _Model.Status = value;
                    NotifyPropertyChanged("Status");
                }
            }
        }

        public PostModel Post
        {
            get
            {
                return _Model.Post;
            }
            set
            {
                if (value != _Model.Post)
                {
                    _Model.Post = value;
                    NotifyPropertyChanged("Post");
                }
            }
        }
    }
}