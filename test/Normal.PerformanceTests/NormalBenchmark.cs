using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using BenchmarkDotNet.Attributes;
using Dapper;
using Npgsql;
using Normal.PerformanceTests.Fixtures;

namespace Normal.PerformanceTests
{
    [CoreJob]
    [RankColumn]
    public class NormalBenchmark
    {
        CreateConnection _connectionCreator;
        IDbContext _context;
        string _select;

        public NormalBenchmark()
        {
            _connectionCreator = () => new NpgsqlConnection("Host=localhost;Database=wide_world_importers_pg;Username=normal;Password=normal");
            _context = new DbContext(_connectionCreator);
            _select = @"SELECT purchase_order_id, supplier_id, order_date, delivery_method_id, contact_person_id, expected_delivery_date, supplier_reference, is_order_finalized, comments, internal_comments, last_edited_by, last_edited_when
            FROM purchasing.purchase_orders;";
        }

        [Benchmark]
        public async Task<IList<PurchaseOrder>> GetPurchaseOrdersNormal()
        {
            return await _context
                .CreateCommand(_select)
                .ToListAsync<PurchaseOrder>();
        }

        [Benchmark]
        public async Task<IList<PurchaseOrder>> GetPurchaseOrdersAdo()
        {
            using (var connection = _connectionCreator())
            {
                await (connection as DbConnection).OpenAsync();
                var command = connection.CreateCommand();
                command.CommandText = _select;

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
                    return results;
                }
            }
        }

        [Benchmark]
        public async Task<IList<PurchaseOrder>> GetPurchaseOrdersDapper()
        {
            using (var connection = _connectionCreator())
            {
                return (await connection.QueryAsync<PurchaseOrder>(_select)).ToList();
            }
        }
    }
}