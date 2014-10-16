using DailySocial.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;

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

        #endregion INotifyPropertyChanged implementation

        private RootTopStoriesModel _Model;

        public TopStoriesViewModel(RootTopStoriesModel model = null)
        {
            _Model = model ?? new RootTopStoriesModel();
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

        public List<PostModel> Posts
        {
            get
            {
                return _Model.Posts;
            }
            set
            {
                if (value != _Model.Posts)
                {
                    _Model.Posts = value;
                    NotifyPropertyChanged("Posts");
                }
            }
        }

        private List<PostModel> _TempPosts;

        public List<PostModel> TempPosts
        {
            get
            {
                return _TempPosts;
            }
            set
            {
                if (value != _TempPosts)
                {
                    _TempPosts = value;
                    NotifyPropertyChanged("TempPosts");
                }
            }
        }
    }
}