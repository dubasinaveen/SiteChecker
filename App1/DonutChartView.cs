using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace App1
{
    public class DonutChartView : View
    {
        private Color[] c = new Color[]{ new Color(170, 102, 204), new Color(153, 204, 0), new Color(255, 187, 51), Color.Gray, Color.Cyan, Color.Red };
        private int valuesLength = 0;
        private RectF rectF;
        private Paint slicePaint;
        private Path path;
        private float[] finalValues;

        public DonutChartView(Context context, float[] values):base(context)
        {
            valuesLength = values.Length;
            finalValues = values;
            slicePaint = new Paint();
            slicePaint.AntiAlias = true;
            slicePaint.Dither = true;
            slicePaint.SetStyle(Paint.Style.Fill);
            path = new Path();
        }
        protected override void OnDraw(Canvas canvas)
        {
            base.OnDraw(canvas);
            if (finalValues != null)
            {
                int startTop = 0;
                int startLeft = 0;
                int endBottom = 240;
                int endRight = endBottom;// This makes an equal square.
                rectF = new RectF(startLeft, startTop, endRight, endBottom);

                float[] scaledValues = Scale();
                float sliceStartPoint = 0;
                path.AddCircle(rectF.CenterX(), rectF.CenterY(), 90, Path.Direction.Cw);
                canvas.ClipPath(path, Region.Op.Difference);

                for (int i = 0; i < valuesLength; i++)
                {
                    slicePaint.Color = c[i];
                    path.Reset();
                    path.AddArc(rectF, sliceStartPoint, scaledValues[i]);
                    path.LineTo(rectF.CenterX(), rectF.CenterY());
                    canvas.DrawPath(path, slicePaint);
                    sliceStartPoint += scaledValues[i];//This updates the starting point of next slice.
                }
            }
        }
        private float[] Scale()
        {
            float[] scaledValues = new float[this.finalValues.Length];
            float total = GetTotal(); //Total all values supplied to the chart
            for (int i = 0; i < this.finalValues.Length; i++)
            {
                scaledValues[i] = (this.finalValues[i] / total) * 360; //Scale each value
            }
            return scaledValues;
        }

        private float GetTotal()
        {
            float total = 0;
            foreach (float val in this.finalValues)
                total += val;
            return total;
        }
    }
}