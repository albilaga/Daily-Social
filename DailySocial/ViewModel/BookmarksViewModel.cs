using DailySocial.Models;

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DailySocial.ViewModel
{
    public class BookmarksViewModel : INotifyPropertyChanged
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

        private List<PostModel> _Models;

        public BookmarksViewModel(List<PostModel> model = null)
        {
            _Models = model ?? new List<PostModel>();
        }

        public List<PostModel> Bookmarks
        {
            get
            {
                return _Models;
            }
            set
            {
                if (value != _Models)
                {
                    _Models = value;
                    NotifyPropertyChanged("Bookmarks");
                }
            }
        }
    }
}