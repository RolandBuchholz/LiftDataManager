namespace LiftDataManager.Controls;

public sealed partial class OrderNumberControl : UserControl
{
    public OrderNumberControl()
    {
        InitializeComponent();
    }

    private string? orderYear;
    public string? OrderYear
    {
        get => orderYear;
        set
        {
            orderYear = value;
            SetOrderNumber();
        }
    }

    private string? orderMonth;
    public string? OrderMonth
    {
        get => orderMonth;
        set
        {
            orderMonth = value;
            SetOrderNumber();
        }
    }

    private string? orderId;
    public string? OrderId
    {
        get => orderId;
        set
        {
            orderId = value;
            SetOrderNumber();
        }
    }

    public string OrderNumber
    {
        get => (string)GetValue(OrderNumberProperty);
        set => SetValue(OrderNumberProperty, value); 
    }

    public static readonly DependencyProperty OrderNumberProperty =
        DependencyProperty.Register("OrderNumber", typeof(string), typeof(OrderNumberControl), new PropertyMetadata(string.Empty));

    private void SetOrderNumber()
    {
        string? fullOrderId;

        if (OrderId is null)
        {
            fullOrderId = "0000";
        }
        else
        {
            fullOrderId = OrderId;
        }

        while (fullOrderId.Length < 4)
        {
            fullOrderId = "0" + fullOrderId;
        }
        if (OrderYear is not null)
        {
            OrderNumber = OrderYear.Substring(2, 2) + "-" + OrderMonth + "-" + fullOrderId;
        }
    }
}
