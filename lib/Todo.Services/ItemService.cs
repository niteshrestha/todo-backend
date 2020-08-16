using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using Todo.Data.Model;

namespace Todo.Services
{
    public class ItemService : IItem
    {
        #region Fields
        private readonly IMongoCollection<Item> _items;
        #endregion

        #region Constructor
        public ItemService()
        {
            var client = new MongoClient(Environment.GetEnvironmentVariable("DATABASE_URL"));
            _items = client.GetDatabase("todo").GetCollection<Item>("Item");
        }
        #endregion

        #region Methods
        public string Create(Item item)
        {
            _items.InsertOne(item);
            return item.Id;
        }

        public void Delete(string id, string userId) =>
            _items.DeleteOne(item => item.Id == id && item.UserId == userId);

        public List<Item> Get(string userId, bool showDone)
        {
            if (showDone)
            {
                return _items.Find(item => item.UserId == userId).ToList();
            }
            return _items.Find(item => item.UserId == userId && item.IsDone == false).ToList();
        }
        public Item GetItem(string id, string userId) =>
            _items.Find(item => item.Id == id && item.UserId == userId).FirstOrDefault();

        public void ItemDone(string id, string userId, Item updatedItem) =>
            _items.ReplaceOne(item => item.Id == id && item.UserId == userId, updatedItem);
        #endregion
    }
}
