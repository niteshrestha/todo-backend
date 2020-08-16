using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Todo.Data.Model;
using Todo.Data.Request;
using Todo.Services;

namespace Todo.API.Controllers
{
    [Authorize]
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class ItemController : ControllerBase
    {
        #region Fields
        private readonly IItem _itemService;
        #endregion

        #region Constructor
        public ItemController(IItem itemService)
        {
            _itemService = itemService;
        }
        #endregion

        #region Methods
        [HttpGet]
        public ActionResult<List<Item>> Get()
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var items = _itemService.Get(userId);
            return items;
        }

        [HttpGet("{id:length(24)}", Name = "GetItem")]
        public ActionResult<Item> Get(string id)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var item = _itemService.GetItem(id, userId);

            if (item == null)
            {
                return NotFound();
            }
            return item;
        }

        [HttpPost]
        public ActionResult Create(ItemInput input)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var item = new Item
            {
                UserId = userId,
                Title = input.Title,
                Description = input.Description,
                IsDone = input.IsDone
            };

            item.Id = _itemService.Create(item);
            return CreatedAtRoute("GetItem", new { id = item.Id }, item);
        }

        [HttpPut("{id:length(24)}")]
        public IActionResult Update(string id)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var item = _itemService.GetItem(id, userId);

            if (item == null)
            {
                return NotFound();
            }

            item.IsDone = true;
            _itemService.ItemDone(id, userId, item);
            return NoContent();
        }

        [HttpDelete("{id:length(24)}")]
        public IActionResult Delete(string id)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var item = _itemService.GetItem(id, userId);
            if (item == null)
            {
                return NotFound();
            }

            _itemService.Delete(item.Id, userId);
            return NoContent();
        }
        #endregion
    }
}
