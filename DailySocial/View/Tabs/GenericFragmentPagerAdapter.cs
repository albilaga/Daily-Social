using Android.App;
using Android.Support.V4.App;
using Android.Support.V4.View;
using Android.Util;
using System.Collections.Generic;
using System.Globalization;

namespace DailySocial.View.Tabs
{
    public class GenericFragmentPagerAdapter : FragmentPagerAdapter
    {
        private readonly List<Android.Support.V4.App.Fragment> _FragmentList = new List<Android.Support.V4.App.Fragment>();

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
    }

    public static class ViewPagerExtensions
    {
        public static ActionBar.Tab GetViewPageTab(this ViewPager viewPager, ActionBar actionBar, string name)
        {
            var tab = actionBar.NewTab();
            tab.SetIcon(null);
            tab.SetText(name);
            tab.TabSelected += (o, e) =>
            {
                Log.Info("ds", "tab = " + actionBar.SelectedNavigationIndex.ToString(CultureInfo.InvariantCulture));
                viewPager.SetCurrentItem(actionBar.SelectedNavigationIndex, false);
            };
            return tab;
        }
    }
}