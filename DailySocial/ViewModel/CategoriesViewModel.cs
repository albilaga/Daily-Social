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

        #endregion

        private RootCategoriesModel _Model;

        public CategoriesViewModel(RootCategoriesModel model=null)
        {
            this._Model = model ?? new RootCategoriesModel();
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

        public List<CategoryModel> Categories
        {
            get
            {
                return _Model.Categories;
            }
            set
            {
                if(value!=_Model.Categories)
                {
                    _Model.Categories = value;
                    NotifyPropertyChanged("Categories");
                }
            }
        }

        public List<CategoryModel> TempCategories
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
                    NotifyPropertyChanged("TempCategories");
                }
            }
        }
    }
}