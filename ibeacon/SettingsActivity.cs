using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace ibeacon
{
    [Activity(Label = "Thread Nation Perks", MainLauncher = false, Icon = "@drawable/icon")]
    public class SettingsActivity : Activity
    {
        public BluetoothAdapter DeviceBluetoothAdapter { get; }
        public ToggleButton BluetoothToggleButton { get; set; }
        public ToggleButton NotificationToggleButton { get; set; }

        private ISharedPreferences PreferenceManger { get; set; }

        public SettingsActivity()
        {
            DeviceBluetoothAdapter = BluetoothAdapter.DefaultAdapter;
        }
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.Settings);

            PreferenceManger = PreferenceManager.GetDefaultSharedPreferences(this);

            BluetoothToggleButton = FindViewById<ToggleButton>(Resource.Id.bluetoothToggleButton);
            BluetoothToggleButton.Click += BluetoothOnToggleClicked;

            NotificationToggleButton = FindViewById<ToggleButton>(Resource.Id.notificationToggleButton);
            NotificationToggleButton.Click += NotificationOnToggleClicked;

            if (DeviceBluetoothAdapter.IsEnabled)
                BluetoothToggleButton.Checked = true;

            if (PreferenceManger.GetBoolean("Notification", true))
                NotificationToggleButton.Checked = true;
        }
        public void BluetoothOnToggleClicked(object sender, EventArgs eventArgs)
        {
            if (BluetoothToggleButton.Checked)
                DeviceBluetoothAdapter.Enable();
            else
                DeviceBluetoothAdapter.Disable();
        }

        public void NotificationOnToggleClicked(object sender, EventArgs eventArgs)
        {
            var editor = PreferenceManger.Edit();
            editor.PutBoolean("Notification", NotificationToggleButton.Checked);
            editor.Apply();
        }
    }
}