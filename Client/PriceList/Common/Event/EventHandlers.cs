namespace Common.Event
{
    public delegate void CountChangedEventHandler(object sender, DecimalValueChangedEventArgs e);

    public delegate void DoubleAnimationEventHandler(object sender, DoubleAnimationEventArgs e);

    public delegate void MinWidthEventHandler(object sender, MinWidthEventArgs e);

    public delegate void BeforeChangeEventHandler(object sender, BeforeChangeArgs e);
}
