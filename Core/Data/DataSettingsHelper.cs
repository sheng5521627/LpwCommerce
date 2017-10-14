﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Data
{
    public partial class DataSettingsHelper
    {
        private static bool? _databaseIsInstalled;

        public static bool DataBaseIsInstalled()
        {
            if (!_databaseIsInstalled.HasValue)
            {
                var manager = new DataSettingManager();
                var settings = manager.LoadSettings();
                _databaseIsInstalled = settings != null && !string.IsNullOrEmpty(settings.DataConnectionString);
            }
            return _databaseIsInstalled.Value;
        }

        public static void ResetCache()
        {
            _databaseIsInstalled = null;
        }
    }
}
