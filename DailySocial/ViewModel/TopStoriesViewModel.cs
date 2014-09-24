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
    public class TopStoriesViewModel : INotifyPropertyChanged
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

        private RootTopStoriesModel _Model;

        public TopStoriesViewModel(RootTopStoriesModel model=null)
        {
            this._Model = model ?? new RootTopStoriesModel();
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

        public List<PostModel> Posts
        {
            get
            {
                return _Model.Posts;
            }
            set
            {
                if(value!=_Model.Posts)
                {
                    _Model.Posts = value;
                    NotifyPropertyChanged("Posts");
                }
            }
        }
    }
}