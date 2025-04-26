
using System;
using System.Collections.Generic;
using System.Drawing;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using ComputerGraphicsLab3Paint.ViewModels;

namespace ComputerGraphicsLab3Paint.Views;

public partial class MainWindow : Window
{
    private  System.Drawing.Point? initialClick = null;
    public int shape = 0;
    public int action = 0;
    public List<Point> points = new List<Point>();
    
    public MainWindow()
    {
        InitializeComponent();
    }
    
    private void OnRedrawClick(object? sender, RoutedEventArgs e)
    {
        var vm = DataContext as MainWindowViewModel;
        vm.canvas.Redraw();
        ImageCanvas.Source = null;
        ImageCanvas.Source = vm.canvas;
    }

    private void Image_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        var vm = DataContext as MainWindowViewModel;
        var p = e.GetPosition(ImageCanvas);
        var position  = new Point((int)p.X, (int)p.Y);
        if(action == 1){// DELETE SHAPE
            var pair = vm.canvas.whichShape(position);
            if(pair.index == -1 || pair.shape == -1) return;
            if(vm.canvas.DeleteShape(pair.index, pair.shape)){
                // TRIGER RERENDER
                ImageCanvas.Source = null;
                ImageCanvas.Source = vm.canvas;
            }
        }
        else if(action == 2){// CHANGE THICKNESS
            var pair = vm.canvas.whichShape(position);
            if((pair.shape == 0 || pair.shape == 2) && pair.index != -1 && vm.canvas.ChangeThickness(pair.index, pair.shape, vm.Thickness)){
                // TRIGER RERENDER
                ImageCanvas.Source = null;
                ImageCanvas.Source = vm.canvas;
            }
        }
        else if(action == 3){// CHANGE COLOR
            var pair = vm.canvas.whichShape(position);
            if(pair.index == -1 || pair.shape == -1) return;
            if(vm.canvas.ChangeColor(pair.index, pair.shape, vm.clr)){
                // TRIGER RERENDER
                ImageCanvas.Source = null;
                ImageCanvas.Source = vm.canvas;
            }
        }
        else if ((action == 0 || action == 4 || action == 5) && initialClick == null)
        {
            initialClick = position;
            if(shape == 2 && action == 0){
                points.Add(new Point((int)initialClick.Value.X, (int)initialClick.Value.Y));
            }
        }
        else if ((action == 5 || action == 4) && initialClick != null){// MOVE SHAPE // STRECH SHAPE
            var pair = vm.canvas.whichShape(new Point(initialClick.Value.X, initialClick.Value.Y));
            if(pair.index == -1 || pair.shape == -1) return;
            if((action == 4 && vm.canvas.MoveShape(pair.index, pair.shape, new Point((int)initialClick.Value.X, (int)initialClick.Value.Y), position)) || 
                (action == 5 && vm.canvas.StrechShape(pair.index, pair.shape, new Point((int)initialClick.Value.X, (int)initialClick.Value.Y), position))){
                // TRIGER RERENDER
                ImageCanvas.Source = null;
                ImageCanvas.Source = vm.canvas;
            
            initialClick = null;
            }
        }
        else if (action == 0 && initialClick != null)
        {
            if(shape == 0){
                vm.canvas.AddLine(new Point((int)initialClick.Value.X, (int)initialClick.Value.Y), position, vm.Thickness, vm.clr);
                initialClick = null;
            } else if(shape == 1){
                vm.canvas.AddCircle(new Point((int)initialClick.Value.X, (int)initialClick.Value.Y), position, vm.clr);
                initialClick = null;
            } else if(shape == 2){
                if(Math.Abs(initialClick.Value.X - position.X) < 15 && Math.Abs(initialClick.Value.Y - position.Y) < 15){
                    vm.canvas.AddPolygon(points, vm.Thickness, vm.clr);
                    points = new List<Point>();
                    initialClick = null;
                } else {
                    points.Add(position);
                }
            } 

            // TRIGER RERENDER
            ImageCanvas.Source = null;
            ImageCanvas.Source = vm.canvas;
        }
    }
    private async void OnOpenClick(object? sender, RoutedEventArgs e)
    {
        var vm = DataContext as MainWindowViewModel;

        var dialog = new OpenFileDialog();
        dialog.Title = "Open Canvas";
        dialog.Filters.Add(new FileDialogFilter() { Name = "JSON Files", Extensions = { "json" } });

        string[]? result = await dialog.ShowAsync(window); // `window` is your parent window

        if (result != null && result.Length > 0)
        {
            string filePath = result[0];
            var cdata = CanvasStorage.Load(filePath);
            var canvasBitmap = new CanvasBitmap(cdata);
            vm.canvas = canvasBitmap;
        }

        ImageCanvas.Source = null;
        ImageCanvas.Source = vm.canvas;
    }
    private async void OnSaveClick(object? sender, RoutedEventArgs e)
    {
        var vm = DataContext as MainWindowViewModel;

        var dialog = new SaveFileDialog();
        dialog.Title = "Save Canvas";
        dialog.Filters.Add(new FileDialogFilter() { Name = "JSON Files", Extensions = { "json" } });
        dialog.DefaultExtension = "json";

        string? result = await dialog.ShowAsync(window); // `window` is your parent window

        if (result != null)
        {
            string filePath = result;
            var cd = new CanvasData(vm.canvas);
            CanvasStorage.Save(cd, filePath);
        }
    }
    private void OnClearAllClick(object? sender, RoutedEventArgs e)
    {
        var vm = DataContext as MainWindowViewModel;
        vm.canvas.ClearAll();

        ImageCanvas.Source = null;
        ImageCanvas.Source = vm.canvas;
    }   
    private void OnStrechShapeClick(object? sender, RoutedEventArgs e)
    {
        action = 5;
    } 
    private void OnMoveShapeClick(object? sender, RoutedEventArgs e)
    {
        action = 4;
    }
    private void OnChangeColorClick(object? sender, RoutedEventArgs e)
    {
        action = 3;
    }
    private void OnChangeThicknessClick(object? sender, RoutedEventArgs e)
    {
        action = 2;
    }
    private void OnDleteClick(object? sender, RoutedEventArgs e)
    {
        action = 1;
    }
    private void OnSetPolygonClick(object? sender, RoutedEventArgs e)
    {
        shape = 2;
        action = 0;
    }
    private void OnSetLineClick(object? sender, RoutedEventArgs e)
    {
        shape = 0;
        action = 0;
    }
    private void OnSetCircleClick(object? sender, RoutedEventArgs e)
    {
        shape = 1;
        action = 0;
    }
        // private void OnPointerPressed(object? sender, PointerPressedEventArgs e)
        // {
        //     var rightClick = e.GetCurrentPoint(this).Properties.IsRightButtonPressed;
        //     var pos = e.GetPosition(this);

        //     if (rightClick){
        //         for (int i = 0; i < _handles.Count; i++)
        //         {
        //             var handle = _handles[i];
        //             var handlePos = new Point((double)handle.GetValue(Canvas.LeftProperty) + 5, (double)handle.GetValue(Canvas.TopProperty) + 5);
                    
        //             if (Math.Abs(pos.X - handlePos.X) < 10 && Math.Abs(pos.Y - handlePos.Y) < 10 && i > 0 && i < 255)
        //             {   
        //                 Children.Remove(_handles[i]);
        //                 _handles.RemoveAt(i);
        //                 _polyline.Points.RemoveAt(i);
        //                 break;
        //             }
        //         }
        //     }
        //     else{
        //         for (int i = 0; i < _handles.Count; i++)
        //         {
        //             var handle = _handles[i];
        //             var handlePos = new Point((double)handle.GetValue(Canvas.LeftProperty) + 5, (double)handle.GetValue(Canvas.TopProperty) + 5);
                    
        //             if (Math.Abs(pos.X - handlePos.X) < 10 && Math.Abs(pos.Y - handlePos.Y) < 10)
        //             {
        //                 _draggingIndex = i;
        //                 break;
        //             }
        //         }
        //     }
        // }
        // private void OnPointerDoubleTap(object? sender, TappedEventArgs e)
        // {
        //     var pos = e.GetPosition(this);

        //     int insertIndex = 0;
        //     while (insertIndex < _polyline.Points.Count && _polyline.Points[insertIndex].X < pos.X)
        //     {
        //         insertIndex++;
        //     }
        //     if(insertIndex>=255 || insertIndex < 0) return;
        //     _polyline.Points.Insert(insertIndex, new Point(pos.X, pos.Y));

        //     var handle = new Ellipse
        //         {
        //             Width = 10,
        //             Height = 10,
        //             Fill = Brushes.Red,
        //             [Canvas.LeftProperty] = pos.X - 5,
        //             [Canvas.TopProperty] = pos.Y - 5
        //         };
        //     _handles.Insert(insertIndex, handle);
        //     Children.Add(handle);
        // }
        // private void OnPointerMoved(object? sender, PointerEventArgs e)
        // {
        //     if (_draggingIndex == -1) return;

        //     var pos = e.GetPosition(this);

        //     // Update handle position
        //     if(_draggingIndex != 0 && _draggingIndex != _handles.Count - 1){
        //         _handles[_draggingIndex].SetValue(Canvas.LeftProperty, pos.X - 5);
        //     }
        //     _handles[_draggingIndex].SetValue(Canvas.TopProperty, pos.Y - 5);

        //     // Update polyline
        //     var points = new Points(_polyline.Points);
        //     if(_draggingIndex != 0 && _draggingIndex != _handles.Count - 1){
        //         points[_draggingIndex] = pos;
        //     }
        //     else {
        //         points[_draggingIndex] = new Point( points[_draggingIndex].X, pos.Y);
        //     }
        //     _polyline.Points = points;
        // }

        // private void OnPointerReleased(object? sender, PointerReleasedEventArgs e)
        // {
        //     _draggingIndex = -1;
        // }
}