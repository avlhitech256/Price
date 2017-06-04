using System;
using System.Collections.Generic;

namespace LogService.Service
{
    public class ConsoleColorItem
    {
        #region Constructors

        public ConsoleColorItem(Dictionary<MessageLevel, ConsoleColor> colorMap)
        {
            ColorMap = colorMap;
        }
        public ConsoleColorItem()
            : this(ConsoleColor.White) { }
        public ConsoleColorItem(ConsoleColor highLevelColor)
            : this(highLevelColor, ConsoleColor.Gray) { }
        public ConsoleColorItem(ConsoleColor highLevelColor, ConsoleColor middleLevelColor) 
            : this(highLevelColor, middleLevelColor, ConsoleColor.DarkGray) { }
        public ConsoleColorItem(ConsoleColor highLevelColor, ConsoleColor middleLevelColor, ConsoleColor lowLevelColor)
        {
            ColorMap = new Dictionary<MessageLevel, ConsoleColor>();
            SetColor(highLevelColor, MessageLevel.High);
            SetColor(middleLevelColor, MessageLevel.Middle);
            SetColor(lowLevelColor, MessageLevel.Low);
        }

        #endregion

        #region Properties

        public Dictionary<MessageLevel, ConsoleColor> ColorMap { get; set; }

        public ConsoleColor HighLevelColor
        {
            get { return GetColor(MessageLevel.High); }
            set { SetColor(value, MessageLevel.High); }
        }

        public ConsoleColor MiddleLevelColor
        {
            get { return GetColor(MessageLevel.Middle); }
            set { SetColor(value, MessageLevel.Middle); }
        }

        public ConsoleColor LowLevelColor
        {
            get { return GetColor(MessageLevel.Low); }
            set { SetColor(value, MessageLevel.Low); }
        }


        #endregion

        #region Methods
        public ConsoleColor GetColor(MessageLevel level)
        {
            ConsoleColor color;

            if (!ColorMap.TryGetValue(level, out color))
            {
                color = ConsoleColor.Gray;
                SetColor(color, level);
            }

            return color;
        }

        public void SetColor(ConsoleColor color, MessageLevel level)
        {
            if (ColorMap.ContainsKey(level))
            {
                ColorMap[level] = color;
            }
            else
            {
                ColorMap.Add(level, color);
            }
        }

        #endregion
    }
}
