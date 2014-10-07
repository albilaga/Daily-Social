using Android.OS;
using Android.Views;

using System;

namespace DailySocial.View.Tabs
{
    public class GenericViewPagerFragment : Android.Support.V4.App.Fragment
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