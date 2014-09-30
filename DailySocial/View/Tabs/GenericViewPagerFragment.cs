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

namespace DailySocial.View.Tabs
{
    public class GenericViewPagerFragment: Android.Support.V4.App.Fragment   
    {
        private Func<LayoutInflater, ViewGroup, Bundle, Android.Views.View> _view;

        public GenericViewPagerFragment(Func<LayoutInflater, ViewGroup, Bundle, Android.Views.View> view)
        {
            _view = view;
        }

        public override Android.Views.View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            base.OnCreateView(inflater, container, savedInstanceState);
            return _view(inflater, container, savedInstanceState);
        }
    }
}