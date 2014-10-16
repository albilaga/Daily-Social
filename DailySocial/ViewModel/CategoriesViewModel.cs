using DailySocial.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DailySocial.ViewModel
{
    public class CategoriesViewModel : INotifyPropertyChanged
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

        private RootCategoriesModel _Model;

        public CategoriesViewModel(RootCategoriesModel model = null)
        {
            _Model = model ?? new RootCategoriesModel();
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

        public List<CategoryModel> Categories
        {
            get
            {
                return _Model.Categories;
            }
            set
            {
                if (value != _Model.Categories)
                {
                    _Model.Categories = value;
                    NotifyPropertyChanged("Categories");
                }
            }
        }
    }
}