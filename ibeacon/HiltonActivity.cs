using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Net;
using Android.OS;

namespace ibeacon
{
    [Activity(Label = "Thread Nation Perks", Icon = "@drawable/iconThread")]
    public class HiltonActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ActionBar.SetBackgroundDrawable(new ColorDrawable(new Color(0, 136, 187)));
            SetContentView(Resource.Layout.hilton);

            FindViewById(Resource.Id.HiltonMap).Click += (sender, arg) =>
            {
                Intent browserIntent = new Intent(Intent.Action, Uri.Parse("https://goo.gl/pNLcOU"));
                StartActivity(browserIntent);
            };
        }
    }
}