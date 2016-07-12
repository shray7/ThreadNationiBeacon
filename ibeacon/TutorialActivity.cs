using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;

namespace ibeacon
{
    [Activity(Label = "Tutorial", Icon = "@drawable/iconThread")]
    public class TutorialActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            ActionBar.SetBackgroundDrawable(new ColorDrawable(new Color(0, 136, 187)));
            SetContentView(Resource.Layout.Tutorial);

            FindViewById(Resource.Id.continueButton).Click += (sender, arg) =>
            {
                Finish();
            };
        }
    }
}