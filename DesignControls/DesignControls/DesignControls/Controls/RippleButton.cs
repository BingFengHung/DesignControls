using SkiaSharp;
using SkiaSharp.Views.Forms;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace DesignControls.Controls
{
    // Ripple Effect
    // 1. 根據點擊的位置，將一個圓圈半徑放大
    // 2. 這個圓要是透明的，並且放大的效果要到達最遠端

    /// <summary>
    /// 方形按鈕
    /// </summary>
    class RippleButton : SKCanvasView
    {
        #region Fields

        SKPoint point = new SKPoint();  // 手指點到的地方
        float radius = 1;  // 圓圈效果的半徑
        bool isReleased;  // 手指是否有放開了

        #endregion

        public RippleButton()
        {
            EnableTouchEvents = true;
        }

        protected override void OnPaintSurface(SKPaintSurfaceEventArgs e)
        {
            base.OnPaintSurface(e);

            var info = e.Info;

            SKSurface surface = e.Surface;
            SKCanvas canvas = surface.Canvas;

            canvas.Clear();

            var height = e.Info.Height;

            // 點擊時出現的圓圈
            SKPaint circlePaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.White.WithAlpha(50),
            };

            // 按鈕的背景顏色
            SKPaint rectPaint = new SKPaint
            {
                Style = SKPaintStyle.Fill,
                Color = SKColors.Blue,
            };

            // 製作可以具有圓角的背景
            var background = new SKRoundRect(new SKRect(0, 0, info.Width, height), 5, 5);
            canvas.DrawRoundRect(background, rectPaint);

            canvas.DrawCircle(point, radius, circlePaint);
        }

        int old = 1;
        /// <summary>
        /// 當物件被碰到時候的事件
        /// </summary>
        /// <param name="e"></param>
        protected override void OnTouch(SKTouchEventArgs e)
        {
            switch (e.ActionType)
            {
                case SKTouchAction.Pressed:
                    {
                        isReleased = false;

                        // 尋求最遠邊
                        var slape = Math.Sqrt(Math.Pow(CanvasSize.Width, 2) + Math.Pow(CanvasSize.Height, 2));
                        point = e.Location;

                        var animate = new Animation(v =>
                        {
                            radius = (float)v;
                            this.InvalidateSurface();
                        }, old, slape, easing: Easing.Linear);

                        animate.Commit(this, "Ripple", length: 300, finished: (i, c) =>
                        {
                            // 如果觸碰被放開了就把效果移除
                            if (isReleased)
                                radius = 0;
                            animate = null;
                            this.InvalidateSurface();
                        });
                        break;
                    }
                case SKTouchAction.Released:
                    {
                        isReleased = true;
                        radius = 0;
                        this.InvalidateSurface();
                        break;
                    }
            }

            // 讓 OS 知道你想要一直接收 touch 事件
            e.Handled = true;
        }
    }
}
