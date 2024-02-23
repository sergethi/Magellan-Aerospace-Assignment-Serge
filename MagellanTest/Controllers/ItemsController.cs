using Microsoft.AspNetCore.Mvc;
using Npgsql;
using NpgsqlTypes;

namespace MagellanTest.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly string connectionString = "Connection_String";

        [HttpPost]
        public ActionResult<int> CreateItem(Item item)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("INSERT INTO item (item_name, parent_item, cost, req_date) VALUES (@itemName, @parentItem, @cost, @reqDate) RETURNING id", conn))
                    {
                        cmd.Parameters.AddWithValue("@itemName", item.ItemName);
                        cmd.Parameters.AddWithValue("@parentItem", NpgsqlDbType.Integer, (object)item.ParentItem ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@cost", item.Cost);
                        cmd.Parameters.AddWithValue("@reqDate", item.ReqDate);

                        return (int)cmd.ExecuteScalar();
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("{id}")]
        public ActionResult<Item> GetItem(int id)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT * FROM item WHERE id = @id", conn))
                    {
                        cmd.Parameters.AddWithValue("@id", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                return new Item
                                {
                                    Id = (int)reader["id"],
                                    ItemName = reader["item_name"].ToString(),
                                    ParentItem = reader["parent_item"] as int?,
                                    Cost = (int)reader["cost"],
                                    ReqDate = (DateTime)reader["req_date"]
                                };
                            }
                            else
                            {
                                return NotFound();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("totalcost")]
        public ActionResult<int?> GetTotalCost(string itemName)
        {
            try
            {
                using (var conn = new NpgsqlConnection(connectionString))
                {
                    conn.Open();
                    using (var cmd = new NpgsqlCommand("SELECT Get_Total_Cost(@itemName)", conn))
                    {
                        cmd.Parameters.AddWithValue("@itemName", itemName);
                        var result = cmd.ExecuteScalar();
                        return result as int?;
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }



    }

    public class Item
        {
            public int Id { get; set; }
            public string ItemName { get; set; }
            public int? ParentItem { get; set; }
            public int Cost { get; set; }
            public DateTime ReqDate { get; set; }
        }
}
