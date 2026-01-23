using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TACM.UI.Models
{
    public static class SessionTry
    {
        private static readonly Dictionary<string, object> _sessionData = new Dictionary<string, object>();

        public static void Set(string key, object value)
        {
            _sessionData[key] = value;
        }

        public static T Get<T>(string key)
        {
            if (_sessionData.ContainsKey(key))
            {
                return (T)_sessionData[key];
            }
            return default(T);
        }

        public static void Clear()
        {
            _sessionData.Clear();
        }
    }
}
