using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;
using Android.Media.Projection;
using Android.Util;
using Android.Hardware.Display;
using Android.Media;
using Android;
using Android.Support.V4.App;
using Xamarin.Forms;

namespace ScreenCapture.Droid
{
    [Activity(Label = "ScreenCapture", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        private static int REQUEST_EXTERNAL_STORAGE = 1;
        private static String[] PERMISSIONS_STORAGE = {
            Manifest.Permission.ReadExternalStorage,
            Manifest.Permission.WriteExternalStorage
        };

        private int ResultCode { get; set; }
        private Intent ResultData { get; set; }
        public int WidthPixels { get; private set; }
        public int HeightPixels { get; private set; }
        public int DensityDpi { get; private set; }

        private const string STATE_RESULT_CODE = "result_code";
        private const string STATE_RESULT_DATA = "result_data";
        private const int REQUEST_MEDIA_PROJECTION = 1;
        private static  int REQUEST_SCREENCAST = 59706;

        private MediaProjectionManager mediaProjectionManager;
        private MediaProjection mediaProjection;
        private SurfaceView surfaceView;
        private VirtualDisplay virtualDisplay;
        public MediaRecorder mMediaRecorder;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            var metrics = new DisplayMetrics();
            MessagingCenter.Subscribe<string,string>("OnButtonClicked", "ButtonClickEvent", (sender, arg) =>
            {
                StopRecording(true);
            });
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            mediaProjectionManager = (MediaProjectionManager)this.GetSystemService(Context.MediaProjectionService);
            mMediaRecorder = new MediaRecorder();
            StartActivityForResult(mediaProjectionManager.CreateScreenCaptureIntent(),
             REQUEST_SCREENCAST);
            if (savedInstanceState != null)
            {
                ResultCode = savedInstanceState.GetInt(STATE_RESULT_CODE);
                ResultData = (Intent)savedInstanceState.GetParcelable(STATE_RESULT_DATA);
            }
            metrics = new DisplayMetrics();
            LoadApplication(new App());
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            var metrics = new DisplayMetrics();
            this.WindowManager.DefaultDisplay.GetMetrics(metrics);
            DensityDpi = (int)metrics.DensityDpi;
            HeightPixels = (int)metrics.HeightPixels;
            WidthPixels = (int)metrics.WidthPixels;

            ResultCode = (int)resultCode;
            ResultData = data;
            SetUpMediaProjection();
            SetUpVirtualDisplay();
        }


        private void SetUpMediaProjection()
        {
            mediaProjection = mediaProjectionManager.GetMediaProjection(ResultCode, ResultData);
        }

        private void SetUpVirtualDisplay()
        {
            try
            {
                StartRecordingObject();

                virtualDisplay = mediaProjection.CreateVirtualDisplay("ScreenCapture",
                    WidthPixels, HeightPixels, DensityDpi,
                    (DisplayFlags)VirtualDisplayFlags.AutoMirror, mMediaRecorder.Surface, null, null);
                mMediaRecorder.Start();
            }
            catch(Exception ex)
            {

            }

        }

        private void StartRecordingObject()
        {
            try
            {
                ActivityCompat.RequestPermissions(
                    this,
                    PERMISSIONS_STORAGE,
                    REQUEST_EXTERNAL_STORAGE
                );

                string path = Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/test.mp4";
                mMediaRecorder.SetVideoSource(VideoSource.Surface);
                mMediaRecorder.SetOutputFormat(OutputFormat.Webm);
                //mMediaRecorder.SetVideoEncodingBitRate(512 * 1000);
                mMediaRecorder.SetVideoEncoder(VideoEncoder.Vp8);
                mMediaRecorder.SetVideoSize(1280, 720);
                //mMediaRecorder.SetVideoFrameRate(10);
                mMediaRecorder.SetOutputFile(path);
                mMediaRecorder.Prepare();
            }
            catch (Exception e)
            {

            }


        }

        public void StopRecording(bool Flag)
        {
            try
            {
                mMediaRecorder.Stop();

            }
            catch(Exception ex)
            {

            }

        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            mMediaRecorder.Release();
            mMediaRecorder.Dispose();
            mMediaRecorder = null;
            mediaProjection.Stop();
            mediaProjection = null;
            virtualDisplay.Release();
            virtualDisplay = null;
        }
    }
}