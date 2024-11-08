﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FlowDiagram.Controls;
using MaterialDesignThemes.Wpf;
using Microsoft.Win32;

namespace FlowDiagram.Views;

public partial class MainWindow
{
    public MainWindow()
    {
        InitializeComponent();

        // 加载主题
        UpdateTheme();
        // 监听系统主题变化
        SystemEvents.UserPreferenceChanged += (_, args) =>
        {
            // 当事件是由于主题变化引起的
            if (args.Category == UserPreferenceCategory.General)
                // 这里你可以写代码来处理主题变化，例如，重新加载样式或者资源
                UpdateTheme();
        };

        InitializeFlowNodes();
    }

    private void InitializeFlowNodes()
    {
        for (var i = 1; i <= 5; i++)
        {
            var flowNode = new FlowNode
            {
                NodeName = $"Step {i}"
            };
            flowNode.NodeCompleted += FlowNode_NodeCompleted;
            FlowPanel.Children.Add(flowNode);

            if (i >= 5) continue;
            var lineContainer = new Grid
            {
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(-10, 0, -10, 0)
            };
            var connector = new Line
            {
                Stroke = Brushes.Gray,
                StrokeThickness = 2,
                X1 = 0,
                Y1 = 0,
                X2 = 50,
                Y2 = 0,
                VerticalAlignment = VerticalAlignment.Center
            };
            lineContainer.Children.Add(connector);
            FlowPanel.Children.Add(lineContainer);
        }

        // Start highlight effect on the first node
        (FlowPanel.Children[0] as FlowNode)?.StartHighlight();
    }

    private void FlowNode_NodeCompleted(object sender, RoutedEventArgs e)
    {
        if (sender is not FlowNode currentNode) return;
        currentNode.MarkAsComplete();

        var currentIndex = FlowPanel.Children.IndexOf(currentNode);
        if (currentIndex < FlowPanel.Children.Count - 1)
        {
            if (currentIndex < FlowPanel.Children.Count - 2) // connector node's index
            {
                if (FlowPanel.Children[currentIndex + 1] is Grid grid && grid.Children[0] is Line connector)
                {
                    connector.Stroke = Brushes.Green;
                    connector.StrokeThickness = 2;
                }
            }

            if (FlowPanel.Children[currentIndex + 2] is FlowNode nextNode)
            {
                nextNode.StartHighlight();
                StartNode(nextNode);
            }
        }

        currentNode.StopHighlight();
    }

    private void ButtonTest_OnClick(object sender, RoutedEventArgs e)
    {
        if (FlowPanel.Children[0] is not FlowNode currentFlowNode) return;
        StartNode(currentFlowNode);
    }

    private void StartNode(FlowNode node)
    {
        node.Start(async token =>
        {
            await Task.Run(async () =>
            {
                var countdown = 5;
                while (countdown >= 0)
                {
                    // Update UI
                    Dispatcher.Invoke(() => { node.UpdateProgress(countdown); });

                    // Simulate long running task
                    await Task.Delay(TimeSpan.FromSeconds(1), token);

                    countdown--;
                }
            }, token);
            return 0;
        });
    }

    private static void UpdateTheme()
    {
        var paletteHelper = new PaletteHelper();
        var theme = paletteHelper.GetTheme();
        // 检查当前主题并应用
        switch (Theme.GetSystemTheme())
        {
            case BaseTheme.Light:
                theme.SetBaseTheme(BaseTheme.Light);
                break;
            case BaseTheme.Dark:
                theme.SetBaseTheme(BaseTheme.Dark);
                break;
        }

        paletteHelper.SetTheme(theme);
    }

    private void ButtonReset_OnClick(object sender, RoutedEventArgs e)
    {
        foreach (var child in FlowPanel.Children)
            switch (child)
            {
                case FlowNode flowNode:
                    flowNode.Reset();
                    flowNode.StopHighlight();
                    break;
                case Grid grid when grid.Children[0] is Line connector:
                    connector.Stroke = Brushes.Gray;
                    connector.StrokeThickness = 2;
                    break;
            }

        // Start highlight effect on the first node
        if (FlowPanel.Children[0] is FlowNode firstNode) firstNode.StartHighlight();
    }
}