using DWPowerShell.Utility;
using DWPowerShell.Utility.ConsoleIO;
using StaticAbstraction;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DWGitsh.Extensions.Utility.Colors
{
    public class ColorGroupReader
    {
        private static ConsoleColors _colors = new ConsoleColors();

        private static Dictionary<string, ColorPair> _definedColors;
        private static string _defaultGroupName = null;
        private const string _defaultColorFileName = "defaultColors.csv";
        private const string _defaultCustomColorFileName = "customColors.csv";

        private string _defaultColorFilePath = null;
        private string _customColorFilePath = null;
        private DateTime _colorLastModified = DateTime.MinValue;
        private IStaticAbstraction _diskManager;
       

        static ColorGroupReader()
        {
            _definedColors = new Dictionary<string, ColorPair>(StringComparer.InvariantCultureIgnoreCase);
        }

        public ColorGroupReader()
        {
            Init(null, null, null);
        }

        public ColorGroupReader(string defaultColorFile, string customColorFile)
        {
            Init(null, defaultColorFile, customColorFile);
        }

        public ColorGroupReader(IStaticAbstraction diskManager)
        {
            Init(diskManager, null, null);
        }

        public ColorGroupReader(IStaticAbstraction diskManager, string defaultColorFile, string customColorFile)
        {
            Init(diskManager, defaultColorFile, customColorFile);
        }

        protected void Init(IStaticAbstraction diskManager, string defaultColorFile, string customColorFile)
        {
            _diskManager = diskManager ?? new StAbWrapper();

            var defColorFile = string.IsNullOrWhiteSpace(defaultColorFile) ? _defaultColorFileName : defaultColorFile;
            var custColorFile = string.IsNullOrWhiteSpace(customColorFile) ? _defaultCustomColorFileName : customColorFile;

            var path = _diskManager.NewFileInfo(_diskManager.Assembly.GetCallingAssembly().Location).DirectoryName;
            var testPath = DWPSUtils.IsFullPath(defColorFile) ? defColorFile : _diskManager.Path.Combine(path, defColorFile);
            if (_diskManager.File.Exists(testPath))
                _defaultColorFilePath = testPath;
            else
            {
                testPath = _diskManager.Path.Combine(path, "psscripts", defColorFile);
                if (_diskManager.File.Exists(testPath)) _defaultColorFilePath = testPath;
            }

            _customColorFilePath = DWPSUtils.IsFullPath(custColorFile) ? custColorFile : _diskManager.Path.Combine(path, custColorFile);

            RefreshColors();
        }

        public ColorPair GetColor(string groupName)
        {
            return GetColor(groupName, null, null);
        }

        public ColorPair GetColor(string groupName, ConsoleColor? foreGround, ConsoleColor? backGround)
        {
            RefreshColors();
            ColorPair result = new ColorPair();

            var group = string.IsNullOrWhiteSpace(groupName) ? _defaultGroupName : groupName;
            if (!_definedColors.ContainsKey(group)) group = _defaultGroupName;
            result = _definedColors[group];


            if (foreGround.HasValue) result.Foreground = foreGround.Value;
            if (backGround.HasValue) result.Background = backGround.Value;

            return result;
        }

        public ColorPair GetDefaultColor()
        {
            return _definedColors[_defaultGroupName];
        }

        public void RefreshColors()
        {
            if (IsNewer(_defaultColorFilePath) || IsNewer(_customColorFilePath))
            {
                _definedColors.Clear();
                ReadColorFile(_diskManager, _defaultColorFilePath);
                ReadColorFile(_diskManager, _customColorFilePath);
            }
        }

        protected bool IsNewer(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath) && _diskManager.File.Exists(filePath))
            {
                var info = _diskManager.NewFileInfo(filePath);
                if (info.LastWriteTime > _colorLastModified)
                {
                    _colorLastModified = info.LastWriteTime;
                    return true;
                }
            }

            return false;
        }


        private static void ReadColorFile(IStaticAbstraction diskManager, string fullPath)
        {
            if (string.IsNullOrWhiteSpace(fullPath) || !diskManager.File.Exists(fullPath)) return;

            var lines = diskManager.File.ReadAllLines(fullPath);

            var isFirstLine = true;

            foreach (var line in lines)
            {
                // ignore blanks and the header line
                if (string.IsNullOrWhiteSpace(line) 
                    || (isFirstLine && line.IndexOf("Group,", StringComparison.InvariantCultureIgnoreCase) >= 0)) continue;
                var segments = line.Split(',').Select(x => x.Trim()).ToArray();

                var groupName = segments[0];
                ConsoleColor? fg = null;
                ConsoleColor? bg = null;
                if (segments.Length > 1) fg = MatchColor(segments[1], true);
                if (segments.Length > 2) bg = MatchColor(segments[2], false);

                if (fg.HasValue || bg.HasValue)
                {
                    var color = new ColorPair(fg, bg);
                    _definedColors[groupName] = color;
                    if (_defaultGroupName == null) _defaultGroupName = groupName;
                }

                isFirstLine = false;
            }
        }


        private static ConsoleColor? MatchColor(string colorName, bool isForeground)
        {
            if (string.IsNullOrWhiteSpace(colorName) || colorName.ToLower() == "none") return null;

            var result = _colors[colorName];

            if (!result.HasValue)
            {
                if (_definedColors.ContainsKey(colorName))
                {
                    var defColor = _definedColors[colorName];
                    result = isForeground ? defColor.Foreground : defColor.Background;
                }
                else 
                    throw new ApplicationException($"Color '{colorName}' is not a valid ConsoleColor");
            }
            return result.Value;
        }
    }
}
