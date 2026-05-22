namespace BizKotei.Client.Services;

public class LayoutService
{
    public int screenWidth { get; set; }

    public bool IsMobile 
    {
        get
        {
            return (screenWidth < 960);
        }
    }

    public bool IsMenuSpreadDefault
    {
        get
        {
            return (screenWidth >= 1280);
        }
    }

    public event Action? OnChange;

    public void SetWidth(int width)
    {
        screenWidth = width;
        OnChange?.Invoke();
    }
}
