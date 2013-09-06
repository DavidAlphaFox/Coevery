﻿using Coevery.Entities.Settings;

namespace Coevery.Fields.Settings {
    public class PhoneFieldSettings : FieldSettings {
        public string DefaultValue { get; set; }

        public PhoneFieldSettings()
        {
            DefaultValue = null;
        }
    }
}
