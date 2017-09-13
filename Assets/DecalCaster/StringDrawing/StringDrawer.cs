using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace StringImageMaker.StringDrawing
{
    /// <summary>
    /// 文字列を描画するクラス
    /// </summary>
    class StringDrawer
    {
        public StringDrawer(IMessageCreator messageCreator, FontManager fontManager)
        {
            messageCreator_ = messageCreator;
            fontManager_ = fontManager;
            sizeCalculator_ = new FontSizeCalculator();
        }

        /// <summary>
        /// 文字列を描画する
        /// 毎回異なる文字列を描画する
        /// </summary>
        /// <returns></returns>
        public Bitmap drawNext() {
            var curFont = fontManager_.nextFont();
            var curMessage = messageCreator_.nextString();

            SizeF sizef = sizeCalculator_.getSize(curFont, curMessage);
            Size imageSize = Size.Round(sizef);

            Bitmap canvas = new Bitmap(width: imageSize.Width, height: imageSize.Height, format: System.Drawing.Imaging.PixelFormat.Format32bppArgb); 
            Graphics g = Graphics.FromImage(canvas);
            g.Clear(Color.FromArgb(alpha:0, red:0, green:0, blue:0));
            SolidBrush brush = new SolidBrush(Color.FromArgb(255,155,155,155));
            g.DrawString(curMessage, curFont, brush, x: 0, y: 0, format:StringFormat.GenericTypographic);

            g.Dispose();

            return canvas;
        }

        IMessageCreator messageCreator_;
        FontManager fontManager_;
        FontSizeCalculator sizeCalculator_;
    }
}
