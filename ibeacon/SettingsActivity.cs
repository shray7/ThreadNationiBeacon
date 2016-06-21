using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ibeacon
{
    [Activity(Label = "ibeacon", MainLauncher = false, Icon = "@drawable/icon")]
    public class SettingsActivity : Activity
    {
        public SettingsActivity()
        {
            //var ba = BluetoothAdapter.DefaultAdapter;

        }
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Settings);

        }

    }
}