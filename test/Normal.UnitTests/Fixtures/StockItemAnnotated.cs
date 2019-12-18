namespace Normal.UnitTests.Fixtures
{
    [Table("warehouse.stock_items")]
    public class StockItemAnnotated
    {
        [PrimaryKey]
        [Column("stock_item_id")]
        public int StockItemID { get; set; }

        [Column("stock_item_name")]
        public string StockItemName { get; set; }

        [Column("supplier_id")]
        public int SupplierId { get; set; }

        [Column("color_id")]
        public int? ColorId { get; set; }

        public string Brand { get; set; }

        public string Size { get; set; }

        [Column("recommended_retail_price")]

        public double? RecommendedRetailPrice { get; set; }

        [Column("is_chiller_stock")]
        public bool IsChillerStock { get; set; }

        public byte[] Photo { get; set; }
    }
}