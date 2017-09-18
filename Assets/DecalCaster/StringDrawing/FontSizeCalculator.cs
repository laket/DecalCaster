using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace StringImageMaker.StringDrawing
{
    /// <summary>
    /// フォントの描画時のサイズを計測する
    /// </summary>
    class FontSizeCalculator : IDisposable
    {
        public FontSizeCalculator()
        {
            // MEMO: このBitmapのサイズ影響を調べる
            canvas_ = new Bitmap(width: 1024, height: 128);
            graphics_ = Graphics.FromImage(canvas_);
            //graphics_.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
        }

        public SizeF getSize(Font font, string message)
        {
            StringFormat format = new StringFormat(StringFormat.GenericTypographic);
            //StringFormat format = new StringFormat(StringFormat.GenericDefault);
            PointF origin = new PointF();

            var size = graphics_.MeasureString(message, font, origin: origin, stringFormat:format);

            return size;
        }

        public void Dispose()
        {
            if (graphics_ != null)
            {
                graphics_.Dispose();
            }
        }

        /// <summary>
        /// サイズ計測に用いるcanvas
        /// </summary>
        private Bitmap canvas_;

        /// <summary>
        /// サイズ計測に用いるGraphics
        /// </summary>
        private Graphics graphics_;

    }
}
