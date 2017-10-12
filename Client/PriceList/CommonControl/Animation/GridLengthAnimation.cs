using System;
using System.Windows;
using System.Windows.Media.Animation;

namespace CommonControl.Animation
{
    /// <summary>
    /// Animates a grid length value just like the DoubleAnimation animates a double value
    /// </summary>
    public class GridLengthAnimation : AnimationTimeline
    {
        #region Members

        private bool isCompleted;
        AnimationClock clock;

        #endregion

        #region Properties

        /// <summary>
        /// Marks the animation as completed
        /// </summary>
        public bool IsCompleted
        {
            get
            {
                return isCompleted;
            }
            set
            {
                isCompleted = value;
            }
        }

        /// <summary>
        /// Returns the type of object to animate
        /// </summary>
        public override Type TargetPropertyType => typeof(GridLength);

        /// <summary>
        /// Sets the reverse value for the second animation
        /// </summary>
        public double ReverseValue
        {
            get
            {
                return (double?) GetValue(ReverseValueProperty) ?? 0;
            }
            set
            {
                SetValue(ReverseValueProperty, value);
            }
        }

        /// <summary>
        /// CLR Wrapper for the From depenendency property
        /// </summary>
        public GridLength From
        {
            get
            {
                return (GridLength?) GetValue(FromProperty) 
                    ?? new GridLength(0, From.IsStar ? GridUnitType.Star : GridUnitType.Pixel);
            }
            set
            {
                SetValue(FromProperty, value);
            }
        }

        /// <summary>
        /// CLR Wrapper for the To property
        /// </summary>
        public GridLength To
        {
            get
            {
                return (GridLength?)GetValue(ToProperty) 
                    ?? new GridLength(0, From.IsStar ? GridUnitType.Star : GridUnitType.Pixel);
            }
            set
            {
                SetValue(ToProperty, value);
            }
        }

        #endregion

        #region Dependency Properties

        /// <summary>
        /// Dependency property. Sets the reverse value for the second animation
        /// </summary>
        public static readonly DependencyProperty ReverseValueProperty =
            DependencyProperty.Register("ReverseValue", typeof(double),
                                        typeof(GridLengthAnimation), new UIPropertyMetadata(0.0));

        /// <summary>
        /// Dependency property for the From property
        /// </summary>
        public static readonly DependencyProperty FromProperty =
            DependencyProperty.Register("From", typeof(GridLength), typeof(GridLengthAnimation));

        /// <summary>
        /// Dependency property for the To property
        /// </summary>
        public static readonly DependencyProperty ToProperty =
            DependencyProperty.Register("To", typeof(GridLength), typeof(GridLengthAnimation));

        #endregion

        #region Methods

        /// <summary>
        /// Creates an instance of the animation object
        /// </summary>
        /// <returns>Returns the instance of the GridLengthAnimation</returns>
        protected override Freezable CreateInstanceCore()
        {
            return new GridLengthAnimation();
        }

        /// <summary>
        /// registers to the completed event of the animation clock
        /// </summary>
        /// <param name="animationClock">the animation clock to notify completion status</param>
        void VerifyAnimationCompletedStatus(AnimationClock animationClock)
        {
            if (clock == null)
            {
                clock = animationClock;
                clock.Completed += delegate { isCompleted = true; };
            }
        }

        /// <summary>
        /// Animates the grid let set
        /// </summary>
        /// <param name="defaultOriginValue">The original value to animate</param>
        /// <param name="defaultDestinationValue">The final value</param>
        /// <param name="animationClock">The animation clock (timer)</param>
        /// <returns>Returns the new grid length to set</returns>
        public override object GetCurrentValue(object defaultOriginValue,
                                               object defaultDestinationValue, 
                                               AnimationClock animationClock)
        {
            object currentValue = null;

            //check the animation clock event
            VerifyAnimationCompletedStatus(animationClock);

            //check if the animation was completed
            if (isCompleted)
            {
                currentValue = (GridLength) defaultDestinationValue;
            }
            else
            {
                //if not then create the value to animate
                double fromVal = From.Value;
                double toVal = To.Value;

                //check if the value is already collapsed
                if (Math.Abs(((GridLength)defaultOriginValue).Value - toVal) < Double.Epsilon)
                {
                    fromVal = toVal;
                    toVal = ReverseValue;
                }
                //check to see if this is the last tick of the animation clock.
                else if (Math.Abs((animationClock.CurrentProgress ?? 0) - 1.0) < Double.Epsilon)
                {
                    currentValue = To;
                }

                if (currentValue == null)
                {
                    if (fromVal > toVal)
                    {
                        currentValue =
                            new GridLength((1 - (animationClock.CurrentProgress ?? 0)) * (fromVal - toVal) + toVal,
                                           From.IsStar ? GridUnitType.Star : GridUnitType.Pixel);
                    }
                    else
                    {
                        currentValue = 
                            new GridLength((animationClock.CurrentProgress ?? 0) * (toVal - fromVal) + fromVal,
                                           From.IsStar ? GridUnitType.Star : GridUnitType.Pixel);
                    }
                }
            }

            return currentValue;
        }

        #endregion
    }
}
