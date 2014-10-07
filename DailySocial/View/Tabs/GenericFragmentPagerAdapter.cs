using Android.App;
using Android.OS;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Util;
using Android.Views;

using System;
using System.Collections.Generic;

namespace DailySocial.View.Tabs
{
    public class GenericFragmentPagerAdapter : FragmentPagerAdapter
    {
        private List<Android.Support.V4.App.Fragment> _FragmentList = new List<Android.Support.V4.App.Fragment>();

        public GenericFragmentPagerAdapter(Android.Support.V4.App.FragmentManager fm)
            : base(fm)
        {
        }

        public override int Count
        {
            get { return _FragmentList.Count; }
        }

        public override Android.Support.V4.App.Fragment GetItem(int position)
        {
            return _FragmentList[position];
        }

        public void AddFragment(Android.Support.V4.App.Fragment fragment)
        {
            _FragmentList.Add(fragment);
        }

        public void AddFragmentView(Func<LayoutInflater, ViewGroup, Bundle, Android.Views.View> view)
        {
            _FragmentList.Add(new GenericViewPagerFragment(view));
        }
    }

    public static class ViewPagerExtensions
    {
        public static ActionBar.Tab GetViewPageTab(this ViewPager viewPager, ActionBar actionBar, string name)
        {
            var tab = actionBar.NewTab();
            tab.SetText(name);
            tab.TabSelected += (o, e) =>
            {
                Log.Info("ds", "tab = " + actionBar.SelectedNavigationIndex.ToString());
                viewPager.SetCurrentItem(actionBar.SelectedNavigationIndex, false);
            };
            return tab;
        }
    }
}