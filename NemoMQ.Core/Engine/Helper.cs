using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NemoMQ.Core.Engine
{
    static class Helper
    {
        private static string _dataPath;

        public static void Init()
        {
            _dataPath = "Data";
            if (!Directory.Exists(_dataPath))
            {
                Directory.CreateDirectory(_dataPath);
            }
        }

        public static string GetDataPath()
        {
            return _dataPath;
        }
    }
}
