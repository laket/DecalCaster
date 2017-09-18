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
            random_ = new Random();

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

            //int alpha = random_.Next(220, 255);
            int alpha = random_.Next(110, 200);
            int red = random_.Next(255);
            int green = random_.Next(255);
            int blue = random_.Next(255);

            //SolidBrush brush = new SolidBrush(Color.FromArgb(255,155,155,155));
            SolidBrush brush = new SolidBrush(Color.FromArgb(alpha, red, green, blue));            
            g.DrawString(curMessage, curFont, brush, x: 0, y: 0, format:StringFormat.GenericTypographic);

            g.Dispose();

            return canvas;
        }

        IMessageCreator messageCreator_;
        FontManager fontManager_;
        FontSizeCalculator sizeCalculator_;
        Random random_;
    }
}
