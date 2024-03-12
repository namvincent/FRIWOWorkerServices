using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FRIWOLocalAPI.Models;
using System.IO.Ports;
using System.Collections.Generic;
using System.Linq;
using FRIWOLocalAPI.SerialPorts;
using Microsoft.AspNetCore.Components;
using RouteAttribute = Microsoft.AspNetCore.Mvc.RouteAttribute;

namespace FRIWOLocalAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SerialPortsController : ControllerBase
    {
        private SerialService _serialService;
        private readonly SerialPortContext _context;

        public SerialPortsController(SerialPortContext context, SerialService serialService)
        {
            _context = context;
            _serialService = serialService;
        }

        // GET: api/SerialPorts
        [HttpGet("GetSerialPorts")]
        public async Task<ActionResult<IEnumerable<SerialPortItem>>> GetSerialPorts()
        {
            var rs = await _serialService.GetSerialPortList();

            if (rs != null)
            {
                return Ok(rs);
            }
            else
            {
                return BadRequest();
            }

        }

        //// GET: api/TodoItems/5
        //[HttpGet("{id}")]
        //public async Task<ActionResult<TodoItemDTO>> GetTodoItem(long id)
        //{
        //    var todoItem = await _context.TodoItems.FindAsync(id);

        //    if (todoItem == null)
        //    {
        //        return NotFound();
        //    }

        //    return ItemToDTO(todoItem);
        //}

        //// PUT: api/TodoItems/5
        //// To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        //[HttpPut("{id}")]
        //public async Task<IActionResult> UpdateTodoItem(long id, TodoItemDTO todoItemDTO)
        //{
        //    if (id != todoItemDTO.Id)
        //    {
        //        return BadRequest();
        //    }

        //    var todoItem = await _context.TodoItems.FindAsync(id);
        //    if (todoItem == null)
        //    {
        //        return NotFound();
        //    }

        //    todoItem.Name = todoItemDTO.Name;
        //    todoItem.IsComplete = todoItemDTO.IsComplete;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException) when (!TodoItemExists(id))
        //    {
        //        return NotFound();
        //    }

        //    return NoContent();
        //}

        // POST: api/SerialPorts/4
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("{pane}")]
        public async Task<ActionResult<List<Unit>>> Start(int pane)
        {
            List<Unit> l_unit_testing = new();

            for (int i = 0; i < pane; i++)
            {
                l_unit_testing.Add(new Unit { IsTested = false, Result = false, Data = "" });
                _context.Units.Add(l_unit_testing[i]);
                await _context.SaveChangesAsync();
            }

            Console.WriteLine($"count{_context.Units.Count()}");
            return _context.Units.ToList();
        }

        [HttpGet("Test")]
        public async Task<ActionResult<List<Unit>>> GetUnits()
        {
            List<Unit> rs;
            rs = await _context.Units.ToListAsync();
            var result = await _serialService.SendData(rs);
            Console.WriteLine($"count{_context.Units.Count()}");
            return result;
        }

        [HttpPost("COMP/{comp}")]
        public async Task<ActionResult<string>> Start(string comp)
        {
            _serialService.PortName = comp;
            await Task.CompletedTask;
            return Ok(comp);
        }

        // GET: api/SerialPorts
        [HttpGet("GetUnits")]
        public async Task<ActionResult<List<Unit>>> Test()
        {
            List<Unit> rs;
            rs = await _context.Units.ToListAsync();
            Console.WriteLine($"count{_context.Units.Count()}");
            return Ok(rs);
        }

        // DELETE: api/SerialPorts/Delete
        [HttpDelete("Delete")]
        public async Task Delete()
        {
            _context.Units.RemoveRange(_context.Units.ToList());

            await _context.SaveChangesAsync();
            Console.WriteLine($"count{_context.Units.Count()}");
            await Task.CompletedTask;
        }

        //private bool TodoItemExists(long id)
        //{
        //    return _context.TodoItems.Any(e => e.Id == id);
        //}

        //private static TodoItemDTO ItemToDTO(TodoItem todoItem) =>
        //    new TodoItemDTO
        //    {
        //        Id = todoItem.Id,
        //        Name = todoItem.Name,
        //        IsComplete = todoItem.IsComplete
        //    };
    }
}
