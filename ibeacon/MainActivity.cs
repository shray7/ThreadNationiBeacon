using System;
using System.Linq;
using Android;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Preferences;
using EstimoteSdk;

namespace ibeacon
{
    [Activity(Label = "Thread Nation Perks", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity, BeaconManager.IServiceReadyCallback
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
            //beaconManager.StartRanging(new Region("myregion", "B9407F30-F5F8-466E-AFF9-25556B57FE6D", 35710, 9555));
            beaconManager.StartMonitoring(new Region("myregion", "B9407F30-F5F8-466E-AFF9-25556B57FE6D"));

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

            var bwwtextView = FindViewById<TextView>(Resource.Id.BdTextView);
            bwwtextView.Click += (sender, args) =>
            {
                StartActivity(new Intent(this, typeof(BuffaloActivity)));
            };
            var hiltonTextView = FindViewById<TextView>(Resource.Id.HiltonTextView);
            hiltonTextView.Click += (sender, args) =>
            {
                StartActivity(new Intent(this, typeof(HiltonActivity)));
            };
            //// Create beacon manager
            beaconManager = new BeaconManager(this);

            ////Connect to beacon manager to start scanning
            beaconManager.Connect(this);


            var button = FindViewById<Button>(Resource.Id.SendSampleNotification);
            button.Click += SendSampleNotification;
            beaconManager.EnteredRegion += BeaconManager_EnteredRegion;

            //// Wearables will be triggered when nearables are found
            beaconManager.Ranging += (sender, e) =>
            {

                ActionBar.Subtitle = e.Beacons.Count.ToString();

            };
        }

        private void SendSampleNotification(object sender, EventArgs e)
        {
            BuildBwwnotification();
        }

        private void BeaconManager_EnteredRegion(object sender, BeaconManager.EnteredRegionEventArgs e)
        {
            ActionBar.Subtitle = e.Beacons.Count.ToString();
            var hiltonBeacon = e.Beacons.FirstOrDefault(x => x.Major == 43790);
            if (hiltonBeacon != null)
                BuildHiltonNotification();
            var bwwBeacon = e.Beacons.FirstOrDefault(x => x.Major == 35710);
            if(bwwBeacon != null)
                BuildBwwnotification();
        }

        private void BuildBwwnotification()
        {
            Buildnotification("Buffalo Wild Wings is close by !", "15% off discount on Mondays", 0);
        }

        private void BuildHiltonNotification()
        {
            Buildnotification("Hilton Garden In is close by !", "Stop by now", 1);
        }

        private void Buildnotification(string title, string text, int notificationId)
        {
            // Instantiate the builder and set notification elements:
            Notification.Builder builder = new Notification.Builder(this)
                .SetContentTitle(title)
                .SetContentText(text)
                .SetSmallIcon(Resource.Drawable.Icon)
                .SetVibrate(new long[] { 1000, 1000 })
                ;

            // Build the notification:
            Notification notification = builder.Build();

            // Get the notification manager:
            NotificationManager notificationManager =
                GetSystemService(NotificationService) as NotificationManager;

            // Publish the notification:
            var prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            if (notificationManager != null && prefs.GetBoolean("Notification", true)) notificationManager.Notify(notificationId, notification);
        }
        protected override void OnStop()
        {
            base.OnStop();
            if (!isScanning)
                return;

            isScanning = false;
            beaconManager.StopNearableDiscovery(scanId);
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            beaconManager.Disconnect();
        }
    }
}

