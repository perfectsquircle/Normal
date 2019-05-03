using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Npgsql;
using Toadstool.UnitTests.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace Toadstool.UnitTests
{
    public class PerformanceTests
    {
        private readonly ITestOutputHelper _testOutput;
        public PerformanceTests(ITestOutputHelper testOutput)
        {
            _testOutput = testOutput;
        }

        [Fact]
        public async Task ItsFastish()
        {
            //Given
            Func<NpgsqlConnection> connectionCreator = () => new NpgsqlConnection("Host=localhost;Database=wide_world_importers_pg;Username=toadstool;Password=toadstool");
            var context = new DbContext(connectionCreator);
            var select = @"SELECT purchase_order_id, supplier_id, order_date, delivery_method_id, contact_person_id, expected_delivery_date, supplier_reference, is_order_finalized, comments, internal_comments, last_edited_by, last_edited_when
            FROM purchasing.purchase_orders;";

            //When
            const int testRuns = 100;
            long toadstoolAverage = 0;
            long adoAverage = 0;

            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                for (int i = 0; i < testRuns; i++)
                {
                    var results = await context
                        .Query(select)
                        .ToListAsync<PurchaseOrder>();

                    Assert.Equal(2074, results.Count);
                }
                stopwatch.Stop();
                toadstoolAverage = stopwatch.ElapsedMilliseconds / testRuns;
            }

            {
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                for (int i = 0; i < testRuns; i++)
                {
                    using (var connection = connectionCreator())
                    {
                        await connection.OpenAsync();
                        var command = connection.CreateCommand();
                        command.CommandText = select;

                        using (var reader = command.ExecuteReader())
                        {
                            var results = new List<PurchaseOrder>();
                            while (reader.Read())
                            {
                                results.Add(new PurchaseOrder
                                {
                                    PurchaseOrderId = (int)reader["purchase_order_id"],
                                    SupplierId = (int)reader["supplier_id"],
                                    OrderDate = (DateTime)reader["order_date"],
                                    DeliveryMethodId = (int)reader["delivery_method_id"],
                                    ContactPersonId = (int)reader["contact_person_id"],
                                    ExpectedDeliveryDate = (DateTime)(DBNull.Value == reader["expected_delivery_date"] ? null : reader["expected_delivery_date"]),
                                    SupplierReference = (string)(DBNull.Value == reader["supplier_reference"] ? null : reader["supplier_reference"]),
                                    IsOrderFinalized = (bool)reader["is_order_finalized"],
                                    Comments = (string)(DBNull.Value == reader["comments"] ? null : reader["comments"]),
                                    InternalComments = (string)(DBNull.Value == reader["internal_comments"] ? null : reader["internal_comments"]),
                                    LastEditedBy = (int)reader["last_edited_by"],
                                    LastEditedWhen = (DateTime)reader["last_edited_when"],
                                });
                            }
                            Assert.Equal(2074, results.Count);
                        }
                    }

                }
                stopwatch.Stop();
                adoAverage = stopwatch.ElapsedMilliseconds / testRuns;
            }

            _testOutput.WriteLine($"Toadstool average test results: {toadstoolAverage}ms");
            _testOutput.WriteLine($"ADO average test results: {adoAverage}ms");

            //Then
            Assert.InRange(toadstoolAverage, 0, 50); // Average is faster than 50ms
            Assert.True(toadstoolAverage < adoAverage * 4); // It's less than 4 times slower than straight ADO
        }
    }
}