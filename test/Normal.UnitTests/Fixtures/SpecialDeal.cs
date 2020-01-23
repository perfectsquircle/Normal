namespace Normal.UnitTests.Fixtures
{
    [Table("sales.special_deals")]
    public class SpecialDeal
    {
        [PrimaryKey]
        [Column("special_deal_id")] // serial NOT NULL,
        public int SpecialDealId { get; set; }
    }
}