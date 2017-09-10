using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StringImageMaker
{
    /// <summary>
    /// 生成する文字列を決めるインターフェイス
    /// </summary>
    interface IMessageCreator
    {
        /// <summary>
        /// 生成すべき次の文字列を返す
        /// 呼び出すたびに中身が変わる
        /// </summary>
        /// <returns></returns>
        string nextString();
    }
}
