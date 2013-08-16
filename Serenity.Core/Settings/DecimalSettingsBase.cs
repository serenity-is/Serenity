using System;
using System.Text;
using Newtonsoft.Json;
using System.ComponentModel;

namespace Serenity.Data
{
    public abstract class DecimalSettingsBase
    {
        private string _DisplayFormat;
        private string _DisplayFormatNoSep;
        private string _EditorFormat;

        private int _MinDisplayDecimals;
        private int _MaxDisplayDecimals;
        private int _MaxEditorDecimals;

        protected DecimalSettingsBase()
        {
        }

        [JsonIgnore]
        public string DisplayFormatNoSep
        {
            get
            {
                if (_DisplayFormat == null)
                    _DisplayFormat = DisplayFormat;

                return _DisplayFormatNoSep;
            }
        }

        [JsonIgnore]
        public string EditorFormat
        {
            get
            {
                if (_DisplayFormat == null)
                    _DisplayFormat = DisplayFormat;

                return _EditorFormat;
            }
        }

        [JsonIgnore]
        public string DisplayFormat
        {
            get
            {
                if (_DisplayFormat == null)
                {
                    _DisplayFormat = FormatHelper.DecimalFormat(15, _MaxDisplayDecimals,
                        zeroDecimals: _MinDisplayDecimals, thousandSep: true, negatives: true);
                    _DisplayFormatNoSep = FormatHelper.DecimalFormat(15, _MaxDisplayDecimals,
                        zeroDecimals: _MinDisplayDecimals, thousandSep: false, negatives: true);
                    _EditorFormat = FormatHelper.DecimalFormat(15, _MaxDisplayDecimals,
                        zeroDecimals: 0, thousandSep: true, negatives: true);
                }
                return _DisplayFormat;
            }
        }

        [DisplayName("Kolonlar Minimum Ondalık")]
        public int MinDisplayDecimals
        {
            get
            {
                return _MinDisplayDecimals;
            }
            set
            {
                if (_MinDisplayDecimals != value)
                {
                    if (_MinDisplayDecimals < 0)
                        _MinDisplayDecimals = 0;

                    _MinDisplayDecimals = value;
                    _DisplayFormat = null;
                }
            }
        }

        [DisplayName("Kolonlar Maksimum Ondalık")]
        public int MaxDisplayDecimals
        {
            get
            {
                return _MaxDisplayDecimals;
            }
            set
            {
                if (_MaxDisplayDecimals != value)
                {
                    if (_MaxDisplayDecimals < 0)
                        _MinDisplayDecimals = 0;
                    _MaxDisplayDecimals = value;
                    _DisplayFormat = null;
                }
            }
        }

        [DisplayName("Girişte Maksimum Ondalık")]
        public int MaxEditorDecimals
        {
            get
            {
                return _MaxEditorDecimals;
            }
            set
            {
                if (_MaxEditorDecimals != value)
                {
                    if (_MaxEditorDecimals < 0)
                        _MaxEditorDecimals = 0;
                    _MaxEditorDecimals = value;
                }
            }
        }
    }
}