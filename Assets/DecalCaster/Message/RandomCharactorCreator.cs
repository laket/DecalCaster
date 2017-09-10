using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringImageMaker
{
    /// <summary>
    /// 文字ごとにランダムに選択し、文字列を生成するクラス
    /// </summary>
    class RandomCharactorCreator : IMessageCreator
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="charcters">生成される候補の文字</param>
        /// <param name="minLen">数字の桁数の下限(含まれる)</param>
        /// <param name="maxLen">数字の桁数の上限(含まれる)</param>
        public RandomCharactorCreator(char[] characters, int minLen, int maxLen)
        {
            minLen_ = minLen;
            maxLen_ = maxLen;
            random_ = new Random();
            chars_ = new char[characters.Length];
            Array.Copy(characters, chars_, characters.Length);
        }

        string IMessageCreator.nextString()
        {
            int curLen = random_.Next(minLen_, maxLen_ + 1);
            char[] outputs = new char[curLen];

            for (int i = 0; i < curLen; i++)
            {
                int pos = random_.Next(chars_.Length);
                char next = chars_[pos];
                outputs[i] = next;
            }

            return new string(outputs);
        }

        /// <summary>
        /// 数値を母集団としてランダムな文字列を返すCreatorを作成する
        /// </summary>
        /// <param name="minLen_"></param>
        /// <param name="maxLen_"></param>
        /// <returns></returns>
        public static RandomCharactorCreator makeNumericCreator(int minLen_, int maxLen_)
        {
            char[] CANDIDATES = new char[]{ '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };

            return new RandomCharactorCreator(CANDIDATES, minLen_, maxLen_);
        }

    private int minLen_;
        private int maxLen_;
        private Random random_;

        /// <summary>
        /// 生成される文字
        /// </summary>
        private char[] chars_;

    }
}
