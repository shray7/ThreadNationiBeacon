using System;
using System.Collections.Generic;
using System.Linq;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Preferences;
using EstimoteSdk;
using Region = EstimoteSdk.Region;

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


            //beaconManager.SetRangingListener(new CustomListener());
            //beaconManager.StartRanging(DefaultRegion);

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

            ActionBar.SetBackgroundDrawable(new ColorDrawable(new Color(0, 136, 187)));
            PreferenceManger = PreferenceManager.GetDefaultSharedPreferences(this);

            if (!PreferenceManger.GetBoolean("TutorialFlag", true))
            {
                BuildMainView();
                return;
            }

            SetContentView(Resource.Layout.Tutorial);
            FindViewById<TextView>(Resource.Id.continueButton).Click += (sender, args) =>
            {
                PreferenceManger.Edit()
                                .PutBoolean("TutorialFlag", false)
                                .Apply();
                BuildMainView();
            };
        }

        private void BuildMainView()
        {
            SetContentView(Resource.Layout.Main);


            FindViewById<RelativeLayout>(Resource.Id.BwwContainer).Click += (sender, args) =>
            {
                StartActivity(new Intent(this, typeof (BuffaloActivity)));
            };
            FindViewById<RelativeLayout>(Resource.Id.HiltonContainer).Click += (sender, args) =>
            {
                StartActivity(new Intent(this, typeof(HiltonActivity)));
            };
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
        }


        private void BeaconManager_EnteredRegion(object sender, BeaconManager.EnteredRegionEventArgs e)
        {
            var hiltonBeacon = e.Beacons.FirstOrDefault(x => x.Major == 43790);
            if (hiltonBeacon != null)
            {
                var textView = FindViewById<TextView>(Resource.Id.HiltonTextView);
                textView.SetTypeface(null, TypefaceStyle.Bold);
                textView.Text = "Hilton Garden Inn : " + Math.Round(Utils.ComputeAccuracy(hiltonBeacon) * 3.3, 2) + " ft";
                BuildHiltonNotification();
            }

            var bwwBeacon = e.Beacons.FirstOrDefault(x => x.Major == 35710);
            if (bwwBeacon != null)
            {
                var textView = FindViewById<TextView>(Resource.Id.BdTextView);
                textView.SetTypeface(null, TypefaceStyle.Bold);
                textView.Text = "Buffalo Wild Wings : " + Math.Round(Utils.ComputeAccuracy(bwwBeacon) * 3.3, 2) + " ft";
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

            if (notificationId == 0)
            {
                Intent wingsNotificationIntent = new Intent(this, typeof(BuffaloActivity));
                PendingIntent pendingIntentBuffalo = PendingIntent.GetActivity(this, 0, wingsNotificationIntent, 0);
                builder.SetContentIntent(pendingIntentBuffalo);
                notification.SetLatestEventInfo(this, title, text, pendingIntentBuffalo);
            }
            if (notificationId == 1)
            {
                Intent hiltonNotificationIntent = new Intent(this, typeof(HiltonActivity));
                PendingIntent pendingIntentHilton = PendingIntent.GetActivity(this, 0, hiltonNotificationIntent, 0);
                builder.SetContentIntent(pendingIntentHilton);
                notification.SetLatestEventInfo(this, title, text, pendingIntentHilton);
            }
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

    public class CustomListener : BeaconManager.IRangingListener
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

