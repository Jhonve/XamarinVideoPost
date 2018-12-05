using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Views;
using Android.Util;
using static Android.Graphics.PorterDuff;

namespace DrawFunc
{
    public class MyDraw : SurfaceView, ISurfaceHolderCallback
    {
        private ISurfaceHolder myHolder;
        private Canvas myCanvas;
        private Paint myPaint;

        public MyDraw(Context context, IAttributeSet attrs) : base(context, attrs)
        {
            myHolder = this.Holder;
            // set surfaceView Translucent
            myHolder.SetFormat(Format.Translucent);
            myHolder.AddCallback(this);
            myPaint = new Paint();
            myPaint.SetARGB(100, 255, 0, 0);
            myPaint.StrokeWidth = 10;
        }

        public MyDraw(Context context) : base(context) { }
        public MyDraw(Context context, IAttributeSet attrs, int defStyle) : base(context, attrs, defStyle) { }

        public void SurfaceChanged(ISurfaceHolder holder, [GeneratedEnum] Format format, int width, int height) { }
        public void SurfaceCreated(ISurfaceHolder holder) { }
        public void SurfaceDestroyed(ISurfaceHolder holder) { }

        public void OnDraw(int[] lines, bool flag)
        {
            myCanvas = myHolder.LockCanvas();
            if (myCanvas != null)
            {
                myCanvas.DrawColor(Color.Transparent, Mode.Clear);
                if (flag)
                {
                    myCanvas.DrawLine(lines[0], lines[1], lines[2], lines[3], myPaint);
                    myCanvas.DrawLine(lines[2], lines[3], lines[4], lines[5], myPaint);
                    myCanvas.DrawLine(lines[4], lines[5], lines[6], lines[7], myPaint);
                    myCanvas.DrawLine(lines[6], lines[7], lines[0], lines[1], myPaint);
                }
                myHolder.UnlockCanvasAndPost(myCanvas);
            }
        }
    }
}