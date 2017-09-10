using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace StringImageMaker
{
    /// <summary>
    /// ランダムに次に使うフォントを返す
    /// （必要かはわからないが）毎回Fontのインスタンスを作成せずに、一度に作ることでコストを下げる
    /// </summary>
    class FontManager : IDisposable
    {
        /// <summary>
        /// コンストラクタ
        /// MEMO: フォントサイズに多様性が不要か検討する (サイズ多様性は別プログラムで担保する)
        /// </summary>
        /// <param name="fontFamilies">フォントのファミリー名のリスト</param>
        /// <param name="minSize">フォントの最小サイズ</param>
        /// <param name="maxSize">フォントの最大サイズ</param>
        public FontManager(string[] fontFamilies, int minSize, int maxSize) {
            // フォントサイズを適当に間引く
            const int MAX_NUM_FONT_SIZE = 5;
            int[] sizes;

            if (maxSize - minSize + 1 > MAX_NUM_FONT_SIZE)
            {
                sizes = new int[MAX_NUM_FONT_SIZE];

                for (int i = 0; i < MAX_NUM_FONT_SIZE; i++)
                {
                    sizes[i] = minSize + (int)((float)(maxSize - minSize) / (MAX_NUM_FONT_SIZE - 1)) * i;
                }
            }
            else
            {
                sizes = new int[maxSize - minSize + 1];
                for (int i = 0; i < sizes.Length; i++)
                {
                    sizes[i] = minSize + i;
                }
            }

            fonts_ = new List<Font>();
            foreach(var fontFamily in fontFamilies)
            {
                foreach(var size in sizes)
                {
                    var curFont = new Font(fontFamily, size);
                    fonts_.Add(curFont);
                }
            }
            rand_ = new Random();
        }

        /// <summary>
        /// 次に用いるフォントを返す。呼ぶたびにRandomに変化する
        /// </summary>
        /// <returns></returns>
        public Font nextFont()
        {
            int nextIdx = rand_.Next(fonts_.Count);
            return fonts_[nextIdx];
        }

        void IDisposable.Dispose()
        {
            if(fonts_ != null)
            {
                foreach(var font in fonts_)
                {
                    font.Dispose();
                }
            }
        }

        List<Font> fonts_;
        Random rand_;
    }

}
