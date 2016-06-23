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

namespace ibeacon
{
    [Activity(Label = "Thread Nation Perks", MainLauncher = false, Icon = "@drawable/icon")]
    public class BuffaloActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.buffalo);



        }
    }
}