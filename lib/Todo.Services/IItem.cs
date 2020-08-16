using System.Collections.Generic;
using Todo.Data.Model;

namespace Todo.Services
{
    public interface IItem
    {
        Item GetItem(string id, string userId);
        List<Item> Get(string userId);

        string Create(Item item);
        void Delete(string id, string userId);
        void ItemDone(string id, string userId, Item updatedItem);
    }
}
