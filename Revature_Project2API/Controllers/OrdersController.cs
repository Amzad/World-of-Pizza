﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Revature_Project2API.Data;
using Entities.Models;
using Microsoft.AspNetCore.Authorization;

namespace Revature_Project2API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Orders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            return await _context.Orders.ToListAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            return order;
        }

        // PUT: api/Orders/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrder(int id, Order order)
        {

            try
            {
                order.CustomerID = id;
                _context.Orders.Add(order);
                _context.Entry(order).State = EntityState.Added;
                await _context.SaveChangesAsync();
                return Ok();
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
        }
        //// PUT: api/Orders/5
        [HttpPut("Checkout/{id}")]
        public async Task<IActionResult> CheckOutOrder(int id, Order order)
        {

            try
            {
                order.CustomerID = id;
                _context.Orders.Add(order);
                _context.Entry(order).State = EntityState.Added;
                await _context.SaveChangesAsync();
                Order returnOrder = _context.Orders.LastOrDefault(c => c.CustomerID == order.CustomerID);
                return CreatedAtAction("GetOrder", new { id = order.OrderID }, returnOrder);
                //return Ok(order);
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

        }
        //POST: api/Orders
        [HttpPost]
        public async Task<ActionResult<Order>> PostOrder(Order order)
        {
            order.OrderDateTime = DateTime.Now;
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            Order returnOrder = _context.Orders.LastOrDefault(c => c.CustomerID == order.CustomerID);
            return CreatedAtAction("GetOrder", new { id = order.OrderID }, returnOrder);
            //return Ok();
            //return CreatedAtAction("GetOrder", new { id = order.OrderID }, order);
        }

        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Order>> DeleteOrder(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return order;
        }

        private bool OrderExists(int id)
        {
            return _context.Orders.Any(e => e.OrderID == id);
        }
    }
}
