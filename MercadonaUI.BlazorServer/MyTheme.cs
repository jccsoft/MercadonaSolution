using MudBlazor;

namespace MercadonaUI.BlazorServer;

public class MyTheme : MudBlazor.MudTheme
{
    public MyTheme()
    {
        Palette.Primary = Palette.Tertiary;
        Palette.AppbarBackground = new MudBlazor.Utilities.MudColor("#ffffff");

        //MudBlazor.Utilities.MudColor primary, appbarBackground;
        //primary = new MudBlazor.Utilities.MudColor("#BFF7DC"); //Palette.Tertiary;
        //appbarBackground = new MudBlazor.Utilities.MudColor("#3EB489");

        //Palette.Primary = primary;
        //Palette.AppbarBackground = appbarBackground;
    }
}
