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
        HighlightEffect.Visibility = Visibility.Hidden;
        // Add logic to stop animation.
    }

    public void MarkAsComplete()
    {
        NodeBackground.Fill = Brushes.Green;
        HasCompleted = true;
        Extra = "completed";
    }

    public async void Start(Func<CancellationToken, Task<int>> longRunningTaskDelegate)
    {
        var tokenSource = new CancellationTokenSource();
        var ret = await longRunningTaskDelegate(tokenSource.Token); // 调用耗时的任务
        if (ret == 0) RaiseEvent(new RoutedEventArgs(NodeCompletedEvent));
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

    private static readonly DependencyProperty HasCompletedProperty =
        DependencyProperty.Register(nameof(HasCompleted), typeof(bool), typeof(FlowNode),
            new PropertyMetadata(false));

    public bool HasCompleted
    {
        get => (bool)GetValue(HasCompletedProperty);
        set => SetValue(HasCompletedProperty, value);
    }

    private static readonly DependencyProperty NodeNameProperty =
        DependencyProperty.Register(
            nameof(NodeName),
            typeof(string),
            typeof(FlowNode),
            new PropertyMetadata(string.Empty));

    public string NodeName
    {
        get => (string)GetValue(NodeNameProperty);
        set => SetValue(NodeNameProperty, value);
    }

    private static readonly DependencyProperty ExtraProperty =
        DependencyProperty.Register(
            nameof(Extra),
            typeof(string),
            typeof(FlowNode),
            new PropertyMetadata(string.Empty));

    public string Extra
    {
        get => (string)GetValue(ExtraProperty);
        set => SetValue(ExtraProperty, value);
    }

    #endregion

    public void UpdateProgress(int countdown)
    {
        Extra = $"{countdown} seconds left";
    }
}