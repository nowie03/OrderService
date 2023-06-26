using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using orderService.Context;
using orderService.Models;
using OrderService.MessageBroker;
using OrderService.Models;

namespace OrderService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ServiceContext _context;
        private readonly IMessageBrokerClient _rabbitMQclient;

        public OrdersController(ServiceContext context, IServiceProvider serviceProvider)
        {
            _context = context;
            _rabbitMQclient = serviceProvider.GetRequiredService<IMessageBrokerClient>();
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            if (_context.Orders == null)
            {
                return NotFound();
            }
            return await _context.Orders.AsNoTracking().ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            if (_context.Orders == null)
            {
                return NotFound();
            }
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // PUT: api/Orders/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {
            if (id != order.Id)
            {
                return BadRequest();
            }

            _context.Entry(order).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Orders
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            if (_context.Orders == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Orders'  is null.");
            }

            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                return CreatedAtAction("GetOrder", new { id = order.Id }, order);

            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        [HttpPost]
        [Route("checkout")]
        public async Task<ActionResult<bool>> Checkout(int orderId)
        {
            if (_context.Orders == null)
                return NoContent();

            Order? order = await _context.Orders.FindAsync(orderId);

            if (order == null)
                return BadRequest($"oder with order id {orderId} could not be found");

            try
            {
                Message<Order> message = new Message<Order>(Constants.EventTypes.PAYMENT_INITIATED, order);

                _rabbitMQclient.SendMessage(message, Constants.EventTypes.PAYMENT_INITIATED);

                return Ok(true);

            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }

        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrder(int id)
        {
            if (_context.Orders == null)
            {
                return NotFound();
            }
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            try
            {

                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return Problem(ex.Message);
            }
        }

        private bool OrderExists(int id)
        {
            return (_context.Orders?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
