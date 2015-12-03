using System;
using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections;
using System.Net;
using System.IO;
using Android.Views.Animations;
using Android.Graphics.Drawables;
using Android.Graphics;
using System.Threading;
using System.Timers;

namespace App1
{
    [Activity(MainLauncher = true, Label = "Site Health Check", Icon = "@drawable/Logo")]
    public class MainActivity : Activity
    {
        public string machinename = "129.135.50.87";
        public string applcationanme = "NetDispatcher";
        public bool Notificationmsg = false;
        public int poolingtime = 1*60*1000;
        float[] values1 = new float[] { 300, 400, 100 };
        float[] values2 = new float[] { 500, 200, 400 };
        private System.Timers.Timer timerClock = new System.Timers.Timer();
        
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            this.RequestWindowFeature(WindowFeatures.NoTitle);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Main);
            Spinner spinner = FindViewById<Spinner>(Resource.Id.spinnerSite);
            Button webRequest = FindViewById<Button>(Resource.Id.webrequest);
            Button recycle = FindViewById<Button>(Resource.Id.recycle);
            Button dump = FindViewById<Button>(Resource.Id.dump);
            //ToggleButton toogleButton = FindViewById<ToggleButton>(Resource.Id.tbStartStop);
            ImageView btnsettings = FindViewById<ImageView>(Resource.Id.settingsBtn);
            string[] items = new string[2] { "NetDispatcher", "NetViewer" };
            //toogleButton.Toggle();
            try
            {
                ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, items);
                adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
                spinner.Adapter = adapter;
            }
            catch
            {
            }
            webRequest.Click += WebRequest_Click;
            //webRequest.Touch += WebRequest_Click;
            recycle.Click += Recycle_Click;
            //recycle.Touch += Recycle_Click;
            dump.Click += Dump_Click;
            btnsettings.Click += BtnSettings_Click;
            //dump.Touch += Dump_Click;
            StartAnimation();
            StartSettingsAnimation();
            DrawPieChart();
            //timerClock.Elapsed += new ElapsedEventHandler(WebRequest_Click);
            //timerClock.Interval = poolingtime;
            //timerClock.Enabled = true;
            //timerClock.Start();           

        }

        private void OnTimer(object sender, ElapsedEventArgs e)
        {
            m_statusChecker1();
        }

        public void m_statusChecker1()
        {
            PingServer1(@"http://{1}/IISHealthCheckApi/api/IISCheck/WebRequestCall?appName={0}");
        }

        private void StartSettingsAnimation()
        {
            ImageView imageView = FindViewById<ImageView>(Resource.Id.settingsBtn);
            AnimationDrawable animation = (AnimationDrawable)imageView.Drawable;
            animation.Start();
        }
        private void StartAnimation()
        {
            ImageView imageView = FindViewById<ImageView>(Resource.Id.animateImage);
            AnimationDrawable animation = (AnimationDrawable)imageView.Drawable;
            animation.Start();
        }

        private void AgainAnimationStart()
        {
            ImageView imageView = FindViewById<ImageView>(Resource.Id.animateImage);
            imageView.SetImageResource(Resource.Drawable.ImageAnimation);
            AnimationDrawable animation = (AnimationDrawable)imageView.Drawable;
            animation.Start();
        }
        private void BreakAnimationStart()
        {
            ImageView imageView = FindViewById<ImageView>(Resource.Id.animateImage);
            imageView.SetImageResource(Resource.Drawable.BreakAnimation);
            AnimationDrawable animation = (AnimationDrawable)imageView.Drawable;
            animation.Start();
        }
        private void Dump_Click(object sender, EventArgs e)
        {
            PingServer(@"http://{1}/IISHealthCheckApi/api/IISCheck/CreateDump?appName={0}");
        }

        private void Recycle_Click(object sender, EventArgs e)
        {
            PingServer(@"http://{1}/IISHealthCheckApi/api/IISCheck/RecycleSiteServer?appName={0}");
        }

        private void WebRequest_Click(object sender, EventArgs e)
        {
            PingServer1(@"http://{1}/IISHealthCheckApi/api/IISCheck/WebRequestCall?appName={0}");
        }

        private void ShowMessage(string title, string message)
        {
            AlertDialog alertDialog = new AlertDialog.Builder(this).Create();
            alertDialog.SetTitle(title);
            alertDialog.SetMessage(message);
            alertDialog.SetButton("Ok", HandllerNotingButton);
            alertDialog.Show();
        }
        void HandllerNotingButton(object sender, DialogClickEventArgs e)
        {

        }

        private void PingServer1(string inputUrl)
        {
            Spinner spinner1 = FindViewById<Spinner>(Resource.Id.spinnerSite);           
            string selectedSite =   (spinner1.SelectedItem != null) ?spinner1.SelectedItem.ToString():applcationanme;
            machinename = machinename == string.Empty ? "129.135.50.117" : machinename;
            string url = string.Format(inputUrl, selectedSite, machinename);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            try
            {
                string message = string.Empty;
                using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {

                    message = sr.ReadToEnd();
                    if (response != null && response.StatusCode == HttpStatusCode.OK)
                    {
                        if (!message.ToLower().Contains("site is running"))
                        {
                            ShowMessage("Status", message);
                            BreakAnimationStart();
                        }
                        else
                            AgainAnimationStart();
                    }
                    else
                    {
                        ShowMessage("Status", message);
                        BreakAnimationStart();
                    }
                     SendNotifications(message);
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Status", ex.Message);
                SendNotifications(ex.Message);
            }

        }

        private void PingServer(string inputUrl)
        {
            Spinner spinner = FindViewById<Spinner>(Resource.Id.spinnerSite);
            string selectedSite = spinner.SelectedItem.ToString();
            machinename = machinename == string.Empty ? "129.135.50.87" : machinename;
            string url = string.Format(inputUrl, selectedSite, machinename);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
            req.Method = "GET";
            try
            {
                string message = string.Empty;
                using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
                using (StreamReader sr = new StreamReader(response.GetResponseStream()))
                {

                    message = sr.ReadToEnd();
                    if (response != null && response.StatusCode == HttpStatusCode.OK)
                    {
                        ShowMessage("Status", message);
                    }
                    else
                    {
                        ShowMessage("Status", message);
                        // AnimationStop();
                    }
                    SendNotifications(message);
                }
            }
            catch (Exception ex)
            {
                ShowMessage("Status", ex.Message);
                SendNotifications(ex.Message);
            }

        }
        protected void Dialog()
        {
            Dialog dialog = new Dialog(this);
            dialog.RequestWindowFeature((int)WindowFeatures.NoTitle);

            dialog.SetContentView(Resource.Layout.settings);
            Button btnRetry = (Button)dialog.FindViewById(Resource.Id.btn1);
            Button btnCancel = (Button)dialog.FindViewById(Resource.Id.btn2);
            EditText editText = (EditText)dialog.FindViewById(Resource.Id.txt);
            EditText editTime = (EditText)dialog.FindViewById(Resource.Id.txtTime);
            dialog.Window.SetLayout(600, 600);
            editText.Text = machinename;
            editTime.Text = Convert.ToString(poolingtime);
            //string[] items = new string[2] { "NetDispatcher", "NetViewer" };
            //Spinner spinner = FindViewById<Spinner>(Resource.Id.spinnerSite1);

            //ArrayAdapter<string> adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerItem, items);
            //adapter.SetDropDownViewResource(Android.Resource.Layout.SimpleSpinnerDropDownItem);
            //spinner.Adapter = adapter;


            btnRetry.Click += (object sender, EventArgs e) =>
            {

                if (!string.IsNullOrEmpty(editText.Text))
                    machinename = editText.Text;
                poolingtime = Convert.ToInt32(editTime.Text);
                //AlertDialog.Builder builder = new AlertDialog.Builder(this);
                //builder.SetMessage(ipname);
                //builder.Create().Show();
                dialog.Dismiss();
            };
            btnCancel.Click += (object sender, EventArgs e) =>
            {
                dialog.Dismiss();
            };
            dialog.Show();
        }
        public void OnCreateDialog()
        {

            try
            {

                //AlertDialog.Builder builder = new AlertDialog.Builder(this);
                //builder.SetTitle("Custom Daialog");
                //builder.Create();
                //builder.Show();
                Dialog();
            }
            catch (Exception Ex)
            {

                AlertDialog.Builder builder = new AlertDialog.Builder(this);
                builder.SetMessage(Ex.Message);
                builder.Create().Show();

            }

        }

        private void BtnSettings_Click(object sender, EventArgs e)
        {
            // SetContentView(Resource.Layout.settings);
            OnCreateDialog();
        }

        private void SendNotifications(string msg)
        {
            DateTime now = DateTime.Now.ToLocalTime();
            // Instantiate the builder and set notification elements:
            Notification notification = new Notification.Builder(this)
                .SetContentTitle("Website Health Check Notification")
                .SetSmallIcon(Resource.Drawable.Notification)
                .SetDefaults(NotificationDefaults.Sound)
                .SetLargeIcon(BitmapFactory.DecodeResource(Resources, Resource.Drawable.Logo))
            .SetContentText(msg)
           .SetSubText(now.ToString())
            .Build();



            // Get the notification manager:
            NotificationManager notificationManager =
                GetSystemService(Context.NotificationService) as NotificationManager;

            // Publish the notification:
            const int notificationId = 0;
            // Build the notification:
           // Notification notification = builder.Notification;
            notificationManager.Notify(notificationId, notification);
        }

        private float[] CalculateData(float[] data)
        {
            float total = 0;
            for (int i = 0; i < data.Length; i++)
            {
                total += data[i];
            }
            for (int i = 0; i < data.Length; i++)
            {
                data[i] = 360 * (data[i] / total);
            }
            return data;
        }
        private void DrawPieChart()
        {
            LinearLayout graphLayoutMemory = FindViewById<LinearLayout>(Resource.Id.graphLayoutMemoryChild);
            LinearLayout graphLayoutCPU = FindViewById<LinearLayout>(Resource.Id.graphLayoutCPUChild);
            values1 = CalculateData(values1);
            values2 = CalculateData(values2);
            graphLayoutMemory.AddView(new MyGraphview(this, values1));
            graphLayoutCPU.AddView(new MyGraphview(this, values2));
        }
    }


    public class MyGraphview : View
    {
        private Paint paint = new Paint(PaintFlags.AntiAlias);
        private float[] value_degree;
        private Color[] Colors = new Color[] { new Color(170, 102, 204), new Color(153, 204, 0), new Color(255, 187, 51), Color.Gray, Color.Cyan, Color.Red };
        RectF rectf = new RectF(10, 10, 200, 200);
        int temp = 0;
        public MyGraphview(Context context, float[] values) : base(context)
        {
            value_degree = new float[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                value_degree[i] = values[i];
            }
        }
        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            for (int i = 0; i < value_degree.Length; i++)
            {//values2.length; i++) {
                if (i == 0)
                {
                    paint.Color = Colors[i];
                    canvas.DrawArc(rectf, 0, value_degree[i], true, paint);
                }
                else
                {
                    temp += (int)value_degree[i - 1];
                    paint.Color = Colors[i];
                    canvas.DrawArc(rectf, temp, value_degree[i], true, paint);
                }
            }
        }
    }



}

