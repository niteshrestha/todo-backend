using System.Collections.Generic;
using System.Net.Mime;
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
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Route("api/[controller]")]
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
        /// <summary>
        /// Get all todo items
        /// </summary>
        /// <param name="showDone"></param>
        /// <returns></returns>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<List<Item>> Get(bool showDone)
        {
            var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var items = _itemService.Get(userId, showDone);
            return items;
        }

        /// <summary>
        /// Get a specific todo item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:length(24)}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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

        /// <summary>
        /// Create a new todo item
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
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

        /// <summary>
        /// Check item is done
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("{id:length(24)}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult ItemDone(string id)
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

        /// <summary>
        /// Delete a todo item
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id:length(24)}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
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
