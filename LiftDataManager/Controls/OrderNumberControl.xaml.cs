using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace LiftDataManager.Controls
{
    public sealed partial class OrderNumberControl : UserControl
    {
        public OrderNumberControl()
        {
            this.InitializeComponent();
        }

        private string _OrderYear;
        public string OrderYear
        {
            get { return _OrderYear; }
            set
            {
                _OrderYear = value;
                SetOrderNumber();
            }
        }

        private string _OrderMonth;
        public string OrderMonth
        {
            get { return _OrderMonth; }
            set
            {
                _OrderMonth = value;
                SetOrderNumber();
            }
        }

        private string _OrderId;
        public string OrderId
        {
            get { return _OrderId; }
            set
            {
                _OrderId = value;
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
            string fullOrderId;

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

            OrderNumber = OrderYear.Substring(2, 2) + "-" + OrderMonth + "-" + fullOrderId;
        }
    }
}
