using System.Collections.ObjectModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;

namespace ComputerGraphicsLab3Paint.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    public int Thickness {get; set;} = 1;
    public CanvasBitmap canvas { get; set;} = new CanvasBitmap(400, 400 );
    public MyColor clr {get; set;} = new MyColor(0,0,0);
    
}  
