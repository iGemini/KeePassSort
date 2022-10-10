using System;
using System.Collections.Generic;
using KeePassLib;
using KeePassLib.Utility;

namespace KeePassSort.Utility
{
    internal class CompareAscending : IComparer<PwEntry>
    {
        private readonly bool _caseInsensitive;
        private readonly bool _compareNaturally;
        private readonly string _fieldName;

        public CompareAscending(string fieldName, bool caseInsensitive, bool compareNaturally)
        {
            _fieldName = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
            _caseInsensitive = caseInsensitive;
            _compareNaturally = compareNaturally;
        }


        public int Compare(PwEntry x, PwEntry y)
        {
            var str1 = x.Strings.ReadSafe(_fieldName);
            var str2 = y.Strings.ReadSafe(_fieldName);

            if (str1 == str2)
            {
                str1 = x.Strings.ReadSafe("UserName");
                str2 = y.Strings.ReadSafe("UserName");
            }

            return _compareNaturally
                ? StrUtil.CompareNaturally(str1, str2)
                : string.Compare(str1, str2, _caseInsensitive);
        }
    }
}
