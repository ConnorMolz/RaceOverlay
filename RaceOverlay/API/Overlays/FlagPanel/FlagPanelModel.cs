using RaceOverlay.Data;

namespace RaceOverlay.API.Overlays.FlagPanel;

public class FlagPanelModel
{
    public string flag { get; set; }

    public FlagPanelModel()
    {
        IrsdkFlags flagState = MainWindow.IRacingData.LocalDriver.CurrentIrsdkFlags;
        switch (flagState)
        {
            case IrsdkFlags.Disqualify:
                flag = "dsq";
                break;
            case IrsdkFlags.Checkered:
                flag = "checkered";
                break;
            case IrsdkFlags.Black | IrsdkFlags.Furled:
                flag = "black";
                break;
            case IrsdkFlags.Debris:
                flag = "debris";
                break;
            case IrsdkFlags.Repair:
                flag = "repair";
                break;
            case IrsdkFlags.Red:
                flag = "red";
                break;
            case IrsdkFlags.Blue:
                flag = "blue";
                break;
            case IrsdkFlags.Caution | IrsdkFlags.Yellow | IrsdkFlags.CautionWaving | IrsdkFlags.YellowWaving:
                flag = "yellow";
                break;
            case IrsdkFlags.White:
                flag = "white";
                break;
            case IrsdkFlags.Green:
                flag = "green";
                break;
            default:
                flag = "none";
                break;
        }
    }
}