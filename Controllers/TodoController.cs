using Microsoft.AspNetCore.Mvc;
using MeuTodo.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using MeuTodo.Data;
using System.Data;
using Microsoft.EntityFrameworkCore;
using MeuTodo.ViewModels;
using System;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace MeuTodo.Controllers
{
	[ApiController]
	[Route("v1")]
	public class TodoController : ControllerBase
	{
		[HttpGet]
		[Route("todos")]
		public async Task<IActionResult> GetAsync(
			[FromServices] AppDbContext context)
		{
			var todos = await context
				.Todos
				.AsNoTracking()
				.ToListAsync();
			return Ok(todos);
		}

		[HttpGet]
		[Route("todos/{id}")]
		public async Task<IActionResult> GetByIdAsync(
			[FromServices] AppDbContext context, [FromRoute] int id)
		{
			var todo = await context
				.Todos
				.AsNoTracking()
				.FirstOrDefaultAsync(x => x.Id == id);
			return todo == null ? NotFound() : Ok(todo);
		}

		[HttpPost]
		[Route("todos")]
		public async Task<IActionResult> PostAsync(
			[FromServices] AppDbContext context,
			[FromBody] CreateTodoViewModel model)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}
			var todo = new Todo
			{
				Date = DateTime.Now,
				Done = false,
				Title = model.Title
			};
			try
			{
				await context.Todos.AddAsync(todo);
				await context.SaveChangesAsync();
				return Created($"v1/todos/{todo.Id}", todo);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return BadRequest();
			}
		}

		[HttpPut]
		[Route("todos/{id}")]
		public async Task<IActionResult> PutAsync(
			[FromServices] AppDbContext context,
			[FromBody] CreateTodoViewModel model,
			[FromRoute] int id)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest();
			}
			var todo = await context.Todos.FirstOrDefaultAsync(x => x.Id == id);

			if (todo == null)
			{
				return NotFound();
			}

			try
			{
				todo.Title = model.Title;
				context.Todos.Update(todo);
				await context.SaveChangesAsync();
				return Ok(todo);
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return BadRequest();
			}
		}

		[HttpDelete]
		[Route("todos/{id}")]
		public async Task<IActionResult> DeleteAsync(
			[FromServices] AppDbContext context,
			[FromRoute] int id)
		{
			var todo = await context.Todos.FirstOrDefaultAsync(x => x.Id == id);
			try
			{
				context.Todos.Remove(todo);
				await context.SaveChangesAsync();
				return Ok("Deletado com sucesso!");
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return BadRequest();
			}
		}

	}
}

