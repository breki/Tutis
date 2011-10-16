using System;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

namespace Mono
{
    [Activity(Label = "Mono", MainLauncher = true, Icon = "@drawable/icon")]
    public class Activity1 : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            TextView tv = new TextView(this);
            tv.Text = "test";
            SetContentView(tv);
        }
    }
}

