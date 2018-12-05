using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using Android.Graphics;
using Java.IO;

using System;

namespace Android_test5
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, ISurfaceHolderCallback, Android.Hardware.Camera.IPreviewCallback
    {
        // set the image's size to be posted
        public int image_test_width = 540;
        public int image_test_height = 960;
        Android.Hardware.Camera camera;
        public ISurfaceHolder surfaceholder;
        public int frame_count = 0;
        public int[] lines = new int[8];
        public bool draw_flag = false;

        DrawFunc.MyDraw imageview;

        TextView _textview;

        public static Bitmap ZoomImage(Bitmap bgimage, double newWidth, double newHeight)
        {
            float width = bgimage.Width;
            float height = bgimage.Height;

            Matrix matrix = new Matrix();

            float scaleWidth = ((float)newWidth) / width;
            float scaleHeight = ((float)newHeight) / height;

            matrix.PostScale(scaleWidth, scaleHeight);
            Bitmap bitmap = Bitmap.CreateBitmap(bgimage, 0, 0, (int)width, (int)height, matrix, true);
            return bitmap;
        }

        public byte[] YUVRotate(byte[] data, int imageWidth, int imageHeight)
        {
            byte[] yuv = new byte[imageWidth * imageHeight * 3 / 2];
            // Rotate the Y luma
            int i = 0;
            for (int x = 0; x < imageWidth; x++)
            {
                for (int y = imageHeight - 1; y >= 0; y--)
                {
                    yuv[i] = data[y * imageWidth + x];
                    i++;
                }
            }
            // Rotate the U and V color components
            i = imageWidth * imageHeight * 3 / 2 - 1;
            for (int x = imageWidth - 1; x > 0; x = x - 2)
            {
                for (int y = 0; y < imageHeight / 2; y++)
                {
                    yuv[i] = data[(imageWidth * imageHeight) + (y * imageWidth) + x];
                    i--;
                    yuv[i] = data[(imageWidth * imageHeight) + (y * imageWidth) + (x - 1)];
                    i--;
                }
            }
            return yuv;
        }

        public void OnPreviewFrame(byte[] data, Android.Hardware.Camera camera)
        {
            // throw new NotImplementedException();
            // process data from camera(preiew-image on surfaceView) per frame
            frame_count++;
            if (frame_count == 4)
            {
                frame_count = 0;
                try
                {
                    int previewWidth = camera.GetParameters().PreviewSize.Width;
                    int previewHeight = camera.GetParameters().PreviewSize.Height;

                    byte[] rotate_data = YUVRotate(data, previewWidth, previewHeight);

                    //after rotate height became width, and width became height
                    YuvImage image = new YuvImage(rotate_data, ImageFormat.Nv21, previewHeight, previewWidth, null);

                    if (image != null)
                    {
                        System.IO.MemoryStream stream = new System.IO.MemoryStream();
                        image.CompressToJpeg(new Rect(0, 0, previewHeight, previewWidth), 100, stream);

                        byte[] image_data = stream.ToArray();
                        Bitmap bitmap_temp = BitmapFactory.DecodeByteArray(image_data, 0, image_data.Length);
                        Bitmap bitmap_result = ZoomImage(bitmap_temp, image_test_width, image_test_height);
                        System.IO.MemoryStream stream_ = new System.IO.MemoryStream();
                        bitmap_result.Compress(Bitmap.CompressFormat.Jpeg, 100, stream_);
                        byte[] image_data_ = stream_.ToArray();

                        image = null;
                        image_data = null;
                        bitmap_temp = null;
                        bitmap_result = null;
                        stream = null;
                        stream_ = null;
                        GC.Collect();
                        string result = PostFunc.MyPost.PostFunc(Convert.ToBase64String(image_data_));
                        string[] result_split = result.Split(new char[] { ',' });
                        _textview.Text = result_split[0] + " " + result_split[1] + "%";
                        if (result_split[0] == "Match!")
                        {
                            draw_flag = true;
                            for (int i = 0; i < 8; i++)
                            {
                                lines[i] = int.Parse(result_split[i + 2]);
                                if (i % 2 == 0)
                                {
                                    lines[i] = lines[i] * imageview.Width / image_test_width;
                                }
                                else
                                {
                                    lines[i] = lines[i] * imageview.Height / image_test_height;
                                }
                            }
                            imageview.OnDraw(lines, draw_flag);
                        }
                        else
                        {
                            draw_flag = false;
                            imageview.OnDraw(lines, draw_flag);
                        }
                    }
                }
                catch (IOException) { }
            }
        }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height)
        {
            // throw new NotImplementedException();
            // change camera's parameters
        }

        public void SurfaceCreated(ISurfaceHolder holder)
        {
            // throw new NotImplementedException();
            // open camera's parameters
            camera = Android.Hardware.Camera.Open();
            camera.SetDisplayOrientation(90);
            Android.Hardware.Camera.Parameters p = camera.GetParameters();
            p.PictureFormat = ImageFormatType.Jpeg;
            camera.SetParameters(p);
            camera.SetPreviewCallback(this);
            camera.SetPreviewDisplay(holder);
            camera.StartPreview();
        }

        public void SurfaceDestroyed(ISurfaceHolder holder)
        {
            // throw new NotImplementedException();
            holder.RemoveCallback(this);
            camera.SetPreviewCallback(null);
            camera.StopPreview();
            camera.Release();
            camera = null;
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            _textview = FindViewById<TextView>(Resource.Id.resultView);

            SurfaceView surface = FindViewById<SurfaceView>(Resource.Id.surfaceView);
            surfaceholder = surface.Holder;
            surfaceholder.AddCallback(this);
            // surfaceholder.SetType(SurfaceType.PushBuffers);
            // surfaceholder.SetFixedSize(100, 100);

            imageview = FindViewById<DrawFunc.MyDraw>(Resource.Id.drawView);
            // set surfaceView Translucent
            imageview.SetZOrderMediaOverlay(true);
        }
    }
}