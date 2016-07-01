using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Preferences;
using EstimoteSdk;

namespace ibeacon
{
    [Activity(Label = "Thread Nation Perks", MainLauncher = true, Icon = "@drawable/iconThread")]
    public class MainActivity : Activity, BeaconManager.IServiceReadyCallback
    {
        private static Region DefaultRegion = new Region("myregion", "B9407F30-F5F8-466E-AFF9-25556B57FE6D");
        public MainActivity()
        {
            Resource.UpdateIdValues();
        }
        private ISharedPreferences PreferenceManger;
        BeaconManager beaconManager;
        string scanId;
        private bool isScanning;
        public void OnServiceReady()
        {
            isScanning = true;
            
            beaconManager.StartMonitoring(DefaultRegion);

            
            beaconManager.SetRangingListener(new CustomListener());
            beaconManager.StartRanging(DefaultRegion);

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
            PreferenceManger = PreferenceManager.GetDefaultSharedPreferences(this);

            if (PreferenceManger.GetBoolean("TutorialFlag", true))
            {
                SetContentView(Resource.Layout.Tutorial);
                var bwwtextView = FindViewById<TextView>(Resource.Id.continueButton);
                bwwtextView.Click += (sender, args) =>
                {
                    PreferenceManger.Edit()
                                    .PutBoolean("TutorialFlag", false)
                                    .Apply();
                    BuildMainView();
                };
            }
            else
                BuildMainView();
        }

        private void BuildMainView()
        {
            SetContentView(Resource.Layout.Main);

            var bwwtextView = FindViewById<TextView>(Resource.Id.BdTextView);
            bwwtextView.Text = "Buffalo Wild Wings";
            bwwtextView.LongClick += SendSampleNotification;
            bwwtextView.Click += (sender, args) =>
            {
                StartActivity(new Intent(this, typeof(BuffaloActivity)));
            };


            var hiltonTextView = FindViewById<TextView>(Resource.Id.HiltonTextView);
            hiltonTextView.Text = "Hilton Garden Inn";
            hiltonTextView.Click += (sender, args) =>
            {
                StartActivity(new Intent(this, typeof(HiltonActivity)));
            };
            hiltonTextView.LongClick += SendSampleHiltonNotification;

            //// Create beacon manager
            beaconManager = new BeaconManager(this);

            ////Connect to beacon manager to start scanning
            beaconManager.Connect(this);
            beaconManager.EnteredRegion += BeaconManager_EnteredRegion;
            beaconManager.ExitedRegion += BeaconManager_ExitedRegion;
        }

        private void BeaconManager_ExitedRegion(object sender, BeaconManager.ExitedRegionEventArgs e)
        {

            if (e.Region.Major == 43790)
            {
                var textView = FindViewById<TextView>(Resource.Id.HiltonTextView);
                textView.Text = "Hilton Garden Inn";
            }

            if (e.Region.Major == 35710)
            {
                var textView = FindViewById<TextView>(Resource.Id.BdTextView);
                textView.Text = "Buffalo Wild Wings";
            }
        }
        private void BeaconManager_EnteredRegion(object sender, BeaconManager.EnteredRegionEventArgs e)
        {
            var hiltonBeacon = e.Beacons.FirstOrDefault(x => x.Major == 43790);
            if (hiltonBeacon != null)
            {
                var textView = FindViewById<TextView>(Resource.Id.HiltonTextView);
                textView.Text = "Hilton Garden Inn :" + Math.Round(Utils.ComputeAccuracy(hiltonBeacon), 2) + " m";
                BuildHiltonNotification();
            }

            var bwwBeacon = e.Beacons.FirstOrDefault(x => x.Major == 35710);
            if (bwwBeacon != null)
            {
                var textView = FindViewById<TextView>(Resource.Id.BdTextView);
                textView.Text = "Buffalo Wild Wings :" + Math.Round(Utils.ComputeAccuracy(bwwBeacon), 2) + " m";
                BuildBwwnotification();
            }

        }
        private void SendSampleNotification(object sender, EventArgs e)
        {
            BuildBwwnotification();
        }
        private void SendSampleHiltonNotification(object sender, EventArgs e)
        {
            BuildHiltonNotification();
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
                .SetSmallIcon(Resource.Drawable.iconThread)
                .SetVibrate(new long[] { 1000, 1000 })
                ;

            // Build the notification:
            var notification = builder.Build();

            // Get the notification manager:
            var notificationManager =
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

    public class CustomListener: BeaconManager.IRangingListener
    {
        public IList<Beacon> Beacons;
        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IntPtr Handle { get; }
        public void OnBeaconsDiscovered(Region region, IList<Beacon> beacons)
        {
            Beacons = beacons;
        }
    }
}

