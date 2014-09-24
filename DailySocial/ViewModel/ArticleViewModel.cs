using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System.ComponentModel;
using DailySocial.Models;

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

        #endregion

        private RootArticleModel _Model;

        public ArticleViewModel(RootArticleModel model=null)
        {
            this._Model = model ?? new RootArticleModel();
        }

        public string Status
        {
            get
            {
                return _Model.Status;
            }
            set
            {
                if(value!=_Model.Status)
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
                if(value!=_Model.Post)
                {
                    _Model.Post = value;
                    NotifyPropertyChanged("Post");
                }
            }
        }

    }
}