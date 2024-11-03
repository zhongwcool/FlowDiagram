using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace FlowDiagram.Controls;

public partial class FlowNode
{
    public FlowNode()
    {
        InitializeComponent();
    }

    private void CompleteButton_Click(object sender, RoutedEventArgs e)
    {
        RaiseEvent(new RoutedEventArgs(NodeCompletedEvent));
    }

    public void StartHighlight()
    {
        HighlightEffect.Visibility = Visibility.Visible;

        var sb = new Storyboard();
        var scaleAnim = new DoubleAnimation(1, 1.2, new Duration(TimeSpan.FromSeconds(0.5)))
        {
            AutoReverse = true,
            RepeatBehavior = RepeatBehavior.Forever
        };

        Storyboard.SetTarget(scaleAnim, EffectTransform);
        Storyboard.SetTargetProperty(scaleAnim, new PropertyPath("ScaleX"));
        sb.Children.Add(scaleAnim);

        Storyboard.SetTargetProperty(scaleAnim, new PropertyPath("ScaleY"));
        sb.Children.Add(scaleAnim);

        sb.Begin();
    }

    public void StopHighlight()
    {
        HighlightEffect.Visibility = Visibility.Collapsed;
        // Add logic to stop animation.
    }

    public void MarkAsComplete()
    {
        NodeBackground.Fill = Brushes.Green;
    }

    #region 属性

    private static readonly RoutedEvent NodeCompletedEvent =
        EventManager.RegisterRoutedEvent(nameof(NodeCompleted), RoutingStrategy.Bubble, typeof(RoutedEventHandler),
            typeof(FlowNode));

    public event RoutedEventHandler NodeCompleted
    {
        add => AddHandler(NodeCompletedEvent, value);
        remove => RemoveHandler(NodeCompletedEvent, value);
    }

    #endregion
}