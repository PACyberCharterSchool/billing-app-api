using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

using Moq;
using NUnit.Framework;

using api.Controllers;
using api.Models;
using api.Tests.Util;

namespace api.Tests.Controllers
{
	[TestFixture]
	public class SchoolDistrictsControllerTests
	{
		private Mock<ISchoolDistrictRepository> _schoolDistricts;
		private ILogger<SchoolDistrictsController> _logger;

		private SchoolDistrictsController _uut;

		[SetUp]
		public void SetUp()
		{
			_schoolDistricts = new Mock<ISchoolDistrictRepository>();
			_logger = new TestLogger<SchoolDistrictsController>();

			_uut = new SchoolDistrictsController(_schoolDistricts.Object, _logger);
		}

		[Test]
		public async Task GetByIdReturnsSchoolDistrict()
		{
			var id = 3;
			var district = new SchoolDistrict { Id = 3 };
			_schoolDistricts.Setup(d => d.Get(id)).Returns(district);

			var result = await _uut.GetById(id);
			Assert.That(result, Is.TypeOf<ObjectResult>());

			var value = ((ObjectResult)result).Value;
			Assert.That(value, Is.TypeOf<SchoolDistrictsController.SchoolDistrictResponse>());

			var actual = ((SchoolDistrictsController.SchoolDistrictResponse)value).SchoolDistrict;
			Assert.That(actual, Is.EqualTo(district));
		}

		[Test]
		public async Task GetByIdReturnsNotFound()
		{
			var id = 3;
			_schoolDistricts.Setup(d => d.Get(id)).Returns((SchoolDistrict)null);

			var result = await _uut.GetById(id);
			Assert.That(result, Is.TypeOf<NotFoundResult>());
		}

		[Test]
		public async Task GetManyReturnsAll()
		{
			var districts = new[] {
				new SchoolDistrict{Id = 1},
				new SchoolDistrict{Id = 2},
				new SchoolDistrict{Id = 3},
			};
			_schoolDistricts.Setup(d => d.GetMany()).Returns(districts);

			var result = await _uut.GetMany();
			Assert.That(result, Is.TypeOf<ObjectResult>());

			var value = ((ObjectResult)result).Value;
			Assert.That(value, Is.TypeOf<SchoolDistrictsController.SchoolDistrictsResponse>());

			var actual = ((SchoolDistrictsController.SchoolDistrictsResponse)value).SchoolDistricts;
			Assert.That(actual, Is.EqualTo(districts));
		}

		[Test]
		public async Task GetManyReturnsEmptyListWhenNull()
		{
			_schoolDistricts.Setup(d => d.GetMany()).Returns((IList<SchoolDistrict>)null);

			var result = await _uut.GetMany();
			Assert.That(result, Is.TypeOf<ObjectResult>());

			var value = ((ObjectResult)result).Value;
			Assert.That(value, Is.TypeOf<SchoolDistrictsController.SchoolDistrictsResponse>());

			var actual = ((SchoolDistrictsController.SchoolDistrictsResponse)value).SchoolDistricts;
			Assert.That(actual, Is.Empty);
		}

		[Test]
		public async Task GetManyReturnsEmptyListWhenEmpty()
		{
			_schoolDistricts.Setup(d => d.GetMany()).Returns(new List<SchoolDistrict>());

			var result = await _uut.GetMany();
			Assert.That(result, Is.TypeOf<ObjectResult>());

			var value = ((ObjectResult)result).Value;
			Assert.That(value, Is.TypeOf<SchoolDistrictsController.SchoolDistrictsResponse>());

			var actual = ((SchoolDistrictsController.SchoolDistrictsResponse)value).SchoolDistricts;
			Assert.That(actual, Is.Empty);
		}
	}
}
