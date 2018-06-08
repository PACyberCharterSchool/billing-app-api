using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

using api.Dtos;
using models;

namespace api.Controllers
{
	[Route("api/[controller]")]
	public class StudentsController : Controller
	{
		private readonly IStudentRepository _students;
		private readonly ILogger _logger;

		public StudentsController(IStudentRepository students, ILogger<StudentsController> logger)
		{
			_students = students;
			_logger = logger;
		}

		public class GetManyArgs
		{
			[StudentField]
			public string Sort { get; set; }

			[EnumerationValidation(typeof(SortDirection))]
			public string Dir { get; set; }

			[Range(0, int.MaxValue)]
			public int Skip { get; set; }

			[Range(0, int.MaxValue)]
			public int Take { get; set; }

			public string Filter { get; set; }
		}

		public struct StudentsResponse
		{
			public IList<StudentDto> Students { get; set; }
		}

		[HttpGet]
		[Authorize(Policy = "STD+")]
		[ProducesResponseType(typeof(StudentsResponse), 200)]
		[ProducesResponseType(typeof(ErrorsResponse), 400)]
		public async Task<IActionResult> GetMany([FromQuery]GetManyArgs args)
		{
			if (!ModelState.IsValid)
				return new BadRequestObjectResult(new ErrorsResponse(ModelState));

			IList<Student> students = null;
			try
			{
				students = await Task.Run(() => _students.GetMany(
					sort: args.Sort,
					dir: string.IsNullOrWhiteSpace(args.Dir) ? null : SortDirection.FromString(args.Dir),
					skip: args.Skip,
					take: args.Take,
					filter: args.Filter
				));
			}
			catch (ArgumentException e)
			{
				return new BadRequestObjectResult(new ErrorResponse(e.Message));
			}

			if (students == null)
				students = new List<Student>();

			return new ObjectResult(new StudentsResponse
			{
				Students = students.Select(s => new StudentDto(s)).ToList(),
			});
		}

		public struct StudentResponse
		{
			public StudentDto Student { get; set; }
		}

		[HttpGet("{id}")]
		[Authorize(Policy = "STD+")]
		[ProducesResponseType(typeof(StudentResponse), 200)]
		public async Task<IActionResult> GetById(int id)
		{
			var student = await Task.Run(() => _students.Get(id));
			if (student == null)
				return NotFound();

			return new ObjectResult(new StudentResponse { Student = new StudentDto(student) });
		}
	}
}
