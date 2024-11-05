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
        // stop animation if still running
        EffectTransform.BeginAnimation(ScaleTransform.ScaleXProperty, null);
        EffectTransform.BeginAnimation(ScaleTransform.ScaleYProperty, null);
    }

    public void MarkAsComplete()
    {
        NodeBackground.Fill = Brushes.Green;
        HasCompleted = true;
        Extra = "completed";
    }

    public void Reset()
    {
        HighlightEffect.Visibility = Visibility.Visible;
        NodeBackground.Fill = Brushes.Gray;
        HasCompleted = false;
        Extra = string.Empty;
    }

    public async void Start(Func<CancellationToken, Task<int>> longRunningTaskDelegate)
    {
        var tokenSource = new CancellationTokenSource();
        var ret = await longRunningTaskDelegate(tokenSource.Token); // 调用耗时的任务
        if (ret == 0) RaiseEvent(new RoutedEventArgs(NodeCompletedEvent));
    }

    #region 属性

    private static readonly RoutedEvent NodeCompletedEvent = EventManager.RegisterRoutedEvent(
        nameof(NodeCompleted),
        RoutingStrategy.Bubble,
        typeof(RoutedEventHandler),
        typeof(FlowNode)
    );

    public event RoutedEventHandler NodeCompleted
    {
        add => AddHandler(NodeCompletedEvent, value);
        remove => RemoveHandler(NodeCompletedEvent, value);
    }

    private static readonly DependencyProperty HasCompletedProperty = DependencyProperty.Register(
        nameof(HasCompleted),
        typeof(bool),
        typeof(FlowNode),
        new PropertyMetadata(false)
    );

    public bool HasCompleted
    {
        get => (bool)GetValue(HasCompletedProperty);
        set => SetValue(HasCompletedProperty, value);
    }

    private static readonly DependencyProperty NodeNameProperty = DependencyProperty.Register(
        nameof(NodeName),
        typeof(string),
        typeof(FlowNode),
        new PropertyMetadata(string.Empty)
    );

    public string NodeName
    {
        get => (string)GetValue(NodeNameProperty);
        set => SetValue(NodeNameProperty, value);
    }

    private static readonly DependencyProperty ExtraProperty = DependencyProperty.Register(
        nameof(Extra),
        typeof(string),
        typeof(FlowNode),
        new PropertyMetadata(string.Empty)
    );

    public string Extra
    {
        get => (string)GetValue(ExtraProperty);
        set => SetValue(ExtraProperty, value);
    }

    //定义Background属性
    public static readonly DependencyProperty CoreFillProperty = DependencyProperty.Register(
        nameof(CoreFill),
        typeof(Brush),
        typeof(FlowNode),
        new PropertyMetadata(Brushes.Gray)
    );

    //定义Background属性的包装器
    public Brush CoreFill
    {
        get => (Brush)GetValue(CoreFillProperty);
        set => SetValue(CoreFillProperty, value);
    }

    #endregion

    public void UpdateProgress(int countdown)
    {
        Extra = $"{countdown} seconds left";
    }
}