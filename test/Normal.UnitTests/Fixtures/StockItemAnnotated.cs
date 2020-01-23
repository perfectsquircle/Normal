namespace Normal.UnitTests.Fixtures
{
    [Table("warehouse.stock_items")]
    public class StockItemAnnotated
    {
        [PrimaryKey]
        [Column("stock_item_id")] // serial NOT NULL,
        public int StockItemID { get; set; }

        [Column("stock_item_name")] // varchar(200) NOT NULL,
        public string StockItemName { get; set; }

        [Column("supplier_id")] // int4 NOT NULL,
        public int SupplierId { get; set; }

        [Column("color_id")] // int4 NULL,
        public int ColorId { get; set; }

        [Column("unit_package_id")] // int4 NOT NULL,
        public int UnitPackageId { get; set; }

        [Column("outer_package_id")] // int4 NOT NULL,
        public int OuterPackageId { get; set; }

        [Column("brand")] // varchar(100) NULL,
        public string Brand { get; set; }

        [Column("size")] // varchar(40) NULL,
        public string Size { get; set; }

        [Column("lead_time_days")] // int4 NOT NULL,
        public int LeadTimeDays { get; set; }

        [Column("quantity_per_outer")] // int4 NOT NULL,
        public int QuantityPerOuter { get; set; }

        [Column("is_chiller_stock")] // bool NOT NULL,
        public bool IsChillerStock { get; set; }

        [Column("barcode")] // varchar(100) NULL,
        public string Barcode { get; set; }

        [Column("tax_rate")] // numeric(18,3) NOT NULL,
        public decimal TaxRate { get; set; }

        [Column("unit_price")] // numeric(18,2) NOT NULL,
        public decimal UnitPrice { get; set; }

        [Column("recommended_retail_price")] // numeric(18,2) NULL,
        public decimal RecommendedRetailPrice { get; set; }

        [Column("typical_weight_per_unit")] // numeric(18,3) NOT NULL,
        public decimal TypicalWeightPerUnit { get; set; }

        [Column("marketing_comments")] // text NULL,
        public string MarketingComments { get; set; }

        [Column("internal_comments")] // text NULL,
        public string InternalComments { get; set; }

        [Column("photo")] // bytea NULL,
        public byte[] Photo { get; set; }

        [Column("custom_fields")] // text NULL,
        public string CustomFields { get; set; }

        [Column("tags")] // text NULL,
        public string Tags { get; set; }

        [Column("search_details")] // text NOT NULL,
        public string SearchDetails { get; set; }

        [Column("last_edited_by")] // int4 NOT NULL,
        public int LastEditedBy { get; set; }
    }
}