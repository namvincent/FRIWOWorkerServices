namespace FRIWOCenter.Data
{
    public class WipModel
    {
        public string orderNo { get; set; } = "";
        public string partNo { get; set; } = "";
        public int quantity { get; set; } = 0;

        public WipModel(string orderNo, string partNo, int quantity)
        {
            this.orderNo = orderNo;
            this.partNo = partNo;
            this.quantity = quantity;
        }
    }
}