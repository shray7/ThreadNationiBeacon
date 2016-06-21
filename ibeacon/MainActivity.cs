using System;
using Android;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using EstimoteSdk;

namespace ibeacon
{
    [Activity(Label = "ibeacon", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
        //, BeaconManager.IServiceReadyCallback
    {

        public MainActivity()
        {
            Resource.UpdateIdValues();
        }
        int count = 1;
        BeaconManager beaconManager;
        string scanId;
        private bool isScanning;
        public void OnServiceReady()
        {
            isScanning = true;
            beaconManager.StartRanging(new Region("myregion", "B9407F30-F5F8-466E-AFF9-25556B57FE6D", 35710,9555));
        }
        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.settingsMenu, menu);
            return base.OnCreateOptionsMenu(menu);
        }
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            switch (item.ItemId)
            {
                case Resource.Id.action_settings:
                    StartActivity(new Intent(this, typeof(SettingsActivity)));
                    return true;
                
            }
            return base.OnOptionsItemSelected(item);
        }
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);

            var textView = FindViewById<TextView>(Resource.Id.BdTextView);
            textView.Click += (sender, args) =>
            {
                StartActivity(new Intent(this, typeof (BuffaloActivity)));
            };

            //// Create beacon manager
            //beaconManager = new BeaconManager(this);

            ////Connect to beacon manager to start scanning
            //beaconManager.Connect(this);

            //// Wearables will be triggered when nearables are found
            //beaconManager.Ranging += (sender, e) =>
            //{
            //    ActionBar.Subtitle = "Found beacons: " + e.Beacons.Count;
            //};

            // Instantiate the builder and set notification elements:
            Notification.Builder builder = new Notification.Builder(this)
                .SetContentTitle("Sample Notification")
                .SetContentText("Hello World! This is my first notification!")
                .SetSmallIcon(Resource.Drawable.Icon);

            // Build the notification:
            Notification notification = builder.Build();

            // Get the notification manager:
            NotificationManager notificationManager =
                GetSystemService(NotificationService) as NotificationManager;

            // Publish the notification:
            const int notificationId = 0;
            if (notificationManager != null) notificationManager.Notify(notificationId, notification);
        }
        //protected override void OnStop()
        //{
        //    base.OnStop();
        //    if (!isScanning)
        //        return;

        //    isScanning = false;
        //    beaconManager.StopNearableDiscovery(scanId);
        //}


        //protected override void OnDestroy()
        //{
        //    base.OnDestroy();
        //    beaconManager.Disconnect();
        //}
    }
}

