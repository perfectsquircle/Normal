namespace Normal.UnitTests.Fixtures
{
    public class StockItem
    {
        public int StockItemID { get; set; }
        public string StockItemName { get; set; }
        public int SupplierId { get; set; }
        public int? ColorId { get; set; }
        public string Brand { get; set; }
        public string Size { get; set; }
        public double? RecommendedRetailPrice { get; set; }
        public bool IsChillerStock { get; set; }
        public byte[] Photo { get; set; }
    }
}