using System;
using System.Collections.Generic;
using KeePassLib;
using KeePassLib.Utility;

namespace KeePassSort.Utility
{
    internal class CompareZA : IComparer<PwEntry>
    {
        private readonly bool m_bCaseInsensitive;
        private readonly bool m_bCompareNaturally;
        private readonly string m_strFieldName;

        public CompareZA(string strFieldName, bool bCaseInsensitive, bool bCompareNaturally)
        {
            m_strFieldName = strFieldName ?? throw new ArgumentNullException(nameof(strFieldName));
            m_bCaseInsensitive = bCaseInsensitive;
            m_bCompareNaturally = bCompareNaturally;
        }


        public int Compare(PwEntry x, PwEntry y)
        {
            var str1 = x.Strings.ReadSafe(m_strFieldName);
            var str2 = y.Strings.ReadSafe(m_strFieldName);
            return m_bCompareNaturally
                ? StrUtil.CompareNaturally(str2, str1)
                : string.Compare(str2, str1, m_bCaseInsensitive);
        }
    }
}