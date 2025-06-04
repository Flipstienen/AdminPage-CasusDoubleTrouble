namespace KE03_INTDEV_SE_2_Base.Models
{
    public class OrderCreateViewModel
    {
        public DateTime OrderDate { get; set; }
        public int CustomerId { get; set; }
        public string DeliveryOption { get; set; }
        public bool Delivered { get; set; }
        public List<OrderPartViewModel> OrderParts { get; set; } = new List<OrderPartViewModel>();
    }

    public class OrderPartViewModel
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
