namespace Common.ViewModel
{
    public delegate void HideWindow();

    public interface IChildWindowViewModel
    {
        HideWindow HideWindow { get; set; }
    }
}
