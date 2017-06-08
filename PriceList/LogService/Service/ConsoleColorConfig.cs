using System;
using System.Collections.Generic;
using Common.Domain;

namespace LogService.Service
{
    public class ConsoleColorConfig
    {
        #region Constructors

        public ConsoleColorConfig() : this(new Dictionary<MessageType, ConsoleColorItem>()) { }
        public ConsoleColorConfig(Dictionary<MessageType, ConsoleColorItem> colorMap)
        {
            ColorMap = colorMap;
            PopulateColorMap();
        }

        #endregion#

        #region Properties

        public Dictionary<MessageType, ConsoleColorItem> ColorMap { get; set; }

        public ConsoleColor HighLevelErrorColor
        {
            get { return GetColor(MessageType.Error, MessageLevel.High); }
            set { SetColor(value, MessageType.Error, MessageLevel.High); }
        }

        public ConsoleColor MiddleLevelErrorColor
        {
            get { return GetColor(MessageType.Error, MessageLevel.Middle); }
            set { SetColor(value, MessageType.Error, MessageLevel.Middle); }
        }

        public ConsoleColor LowLevelErrorColor
        {
            get { return GetColor(MessageType.Error, MessageLevel.Low); }
            set { SetColor(value, MessageType.Error, MessageLevel.Low); }
        }

        public ConsoleColor HighLevelWarningColor
        {
            get { return GetColor(MessageType.Warning, MessageLevel.High); }
            set { SetColor(value, MessageType.Warning, MessageLevel.High); }
        }

        public ConsoleColor MiddleLevelWarningColor
        {
            get { return GetColor(MessageType.Warning, MessageLevel.Middle); }
            set { SetColor(value, MessageType.Warning, MessageLevel.Middle); }
        }

        public ConsoleColor LowLevelWarningColor
        {
            get { return GetColor(MessageType.Warning, MessageLevel.Low); }
            set { SetColor(value, MessageType.Warning, MessageLevel.Low); }
        }

        public ConsoleColor HighLevelInfoColor
        {
            get { return GetColor(MessageType.Info, MessageLevel.High); }
            set { SetColor(value, MessageType.Info, MessageLevel.High); }
        }

        public ConsoleColor MiddleLevelInfoColor
        {
            get { return GetColor(MessageType.Info, MessageLevel.Middle); }
            set { SetColor(value, MessageType.Info, MessageLevel.Middle); }
        }

        public ConsoleColor LowLevelInfoColor
        {
            get { return GetColor(MessageType.Info, MessageLevel.Low); }
            set { SetColor(value, MessageType.Info, MessageLevel.Low); }
        }

        #endregion#

        #region Methods

        private void PopulateColorMap()
        {
            ConsoleColorItem item;

            if (!ColorMap.TryGetValue(MessageType.Error, out item))
            {
                item = new ConsoleColorItem(ConsoleColor.Red, ConsoleColor.Yellow, ConsoleColor.Blue);
                ColorMap.Add(MessageType.Error, item);
            }

            if (!ColorMap.TryGetValue(MessageType.Warning, out item))
            {
                item = new ConsoleColorItem(ConsoleColor.DarkRed, ConsoleColor.DarkYellow, ConsoleColor.DarkBlue);
                ColorMap.Add(MessageType.Warning, item);
            }

            if (!ColorMap.TryGetValue(MessageType.Info, out item))
            {
                item = new ConsoleColorItem(ConsoleColor.White, ConsoleColor.Gray, ConsoleColor.DarkGray);
                ColorMap.Add(MessageType.Info, item);
            }
        }

        public ConsoleColor GetColor(MessageType type, MessageLevel level)
        {
            ConsoleColorItem colorItem;

            if (!ColorMap.TryGetValue(MessageType.Error, out colorItem))
            {
                colorItem = new ConsoleColorItem();
                ColorMap.Add(type, colorItem);
            }

            ConsoleColor color = colorItem.GetColor(level);

            return color;
        }

        public void SetColor(ConsoleColor color, MessageType type, MessageLevel level)
        {
            ConsoleColorItem colorItem;

            if (!ColorMap.TryGetValue(type, out colorItem))
            {
                colorItem = new ConsoleColorItem();
                ColorMap.Add(type, colorItem);
            }

            colorItem.SetColor(color, level);
        }

        #endregion#
    }
}
