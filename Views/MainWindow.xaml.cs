using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using FlowDiagram.Controls;

namespace FlowDiagram.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        InitializeFlowNodes();
    }

    private void InitializeFlowNodes()
    {
        for (var i = 1; i <= 5; i++)
        {
            var flowNode = new FlowNode
            {
                DataContext = new { NodeName = $"Step {i}" }
            };
            flowNode.NodeCompleted += FlowNode_NodeCompleted;
            FlowPanel.Children.Add(flowNode);

            if (i >= 5) continue;
            var lineContainer = new Grid
            {
                VerticalAlignment = VerticalAlignment.Center,
                Background = Brushes.Gold
            };
            var connector = new Line
            {
                Stroke = Brushes.Gray,
                StrokeThickness = 2,
                X1 = 0,
                Y1 = 0,
                X2 = 50,
                Y2 = 0,
                VerticalAlignment = VerticalAlignment.Top
            };
            lineContainer.Children.Add(connector);
            FlowPanel.Children.Add(lineContainer);
        }

        // Start highlight effect on the first node
        (FlowPanel.Children[0] as FlowNode)?.StartHighlight();
    }

    private void FlowNode_NodeCompleted(object sender, RoutedEventArgs e)
    {
        var currentNode = sender as FlowNode;
        var currentIndex = FlowPanel.Children.IndexOf(currentNode);
        if (currentNode == null) return;

        if (currentIndex < FlowPanel.Children.Count - 1)
        {
            currentNode.MarkAsComplete();

            if (currentIndex < FlowPanel.Children.Count - 2) // connector node's index
            {
                var connector = FlowPanel.Children[currentIndex + 1] as Line;
                connector.Stroke = Brushes.Green;
                connector.StrokeThickness = 2;
            }

            var nextNode = FlowPanel.Children[currentIndex + 2] as FlowNode;
            nextNode.StartHighlight();
        }

        currentNode.StopHighlight();
    }
}