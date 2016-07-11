using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Net;
using Android.OS;
using Android.Text.Method;
using Android.Widget;

namespace ibeacon
{
    [Activity(Label = "Thread Nation Perks", MainLauncher = false, Icon = "@drawable/iconThread")]
    public class BuffaloActivity : Activity
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            ActionBar.SetBackgroundDrawable(new ColorDrawable(new Color(0, 136, 187)));
            SetContentView(Resource.Layout.buffalo);
            var bdTextView = FindViewById<TextView>(Resource.Id.BdTextView);
            bdTextView.MovementMethod = LinkMovementMethod.Instance;
            FindViewById(Resource.Id.BwwMap).Click += (sender, arg) =>
            {
                Intent browserIntent = new Intent(Intent.Action, Uri.Parse("https://goo.gl/NkVEao"));
                StartActivity(browserIntent);
            };
        }
    }
}