using System;
using System.Windows;
using System.Windows.Media.Animation;
using static System.Double;


namespace WpfApplication3
{
    /// <summary>
    /// Animates a double value 
    /// </summary>
    public class ExpanderDoubleAnimation : DoubleAnimationBase
    {
        #region Properties

        /// <summary>
        /// CLR Wrapper for the From depenendency property
        /// </summary>
        public double? From
        {
            get
            {
                return (double?) GetValue(FromProperty);
            }
            set
            {
                SetValue(FromProperty, value);
            }
        }

        /// <summary>
        /// CLR Wrapper for the To property
        /// </summary>
        public double? To
        {
            get
            {
                return (double?) GetValue(ToProperty);
            }
            set
            {
                SetValue(ToProperty, value);
            }
        }

        /// <summary>
        /// Sets the reverse value for the second animation
        /// </summary>
        public double? ReverseValue
        {
            get
            {
                return (double?) GetValue(ReverseValueProperty);
            }
            set
            {
                SetValue(ReverseValueProperty, value);
            }
        }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Dependency property for the From property
        /// </summary>
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(double?), typeof(ExpanderDoubleAnimation));

        /// <summary>
        /// Dependency property for the To property
        /// </summary>
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(double?), typeof(ExpanderDoubleAnimation));

        /// <summary>
        /// Sets the reverse value for the second animation
        /// </summary>
        public static readonly DependencyProperty ReverseValueProperty =
            DependencyProperty.Register("ReverseValue", typeof(double?),
                                        typeof(ExpanderDoubleAnimation), new UIPropertyMetadata(0.0));

        #endregion

        #region Methods

        /// <summary>
        /// Creates an instance of the animation
        /// </summary>
        /// <returns></returns>
        protected override Freezable CreateInstanceCore()
        {
            return new ExpanderDoubleAnimation();
        }

        /// <summary>
        /// Animates the double value
        /// </summary>
        /// <param name="defaultOriginValue">The original value to animate</param>
        /// <param name="defaultDestinationValue">The final value</param>
        /// <param name="animationClock">The animation clock (timer)</param>
        /// <returns>Returns the new double to set</returns>
        protected override double GetCurrentValueCore(double defaultOriginValue,
                                                      double defaultDestinationValue,
                                                      AnimationClock animationClock)
        {
            double fromVal = From ?? 0;
            double toVal = To ?? 0;

            if (Math.Abs(defaultOriginValue - toVal) < Epsilon)
            {
                fromVal = toVal;
                toVal = ReverseValue ?? 0;
            }

            double currentValueCore;

            if (fromVal > toVal)
            {
                currentValueCore = (1 - (animationClock.CurrentProgress ?? 0)) * (fromVal - toVal) + toVal;
            }
            else
            {
                currentValueCore = ((animationClock.CurrentProgress ?? 0) * (toVal - fromVal) + fromVal);
            }

            return currentValueCore;
        }

        #endregion#
    }
}
