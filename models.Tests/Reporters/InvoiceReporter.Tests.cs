using Microsoft.EntityFrameworkCore;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json;
using NUnit.Framework;

using models.Reporters;

namespace models.Tests.Reporters
{
	[TestFixture]
	public class InvoiceReporterTests
	{
		private PacBillContext _context;
		private InvoiceReporter _uut;

		private readonly SqliteConnection _conn = new SqliteConnection("Data Source=:memory:");
		public PacBillContext NewContext()
		{
			var ctx = new PacBillContext(new DbContextOptionsBuilder<PacBillContext>().UseSqlite(_conn).Options);
			ctx.Database.Migrate();
			return ctx;
		}

		[SetUp]
		public void SetUp()
		{
			_conn.Open();
			_context = NewContext();

			_uut = new InvoiceReporter(_context);
		}

		[TearDown]
		public void TearDown()
		{
			_context.Database.EnsureDeleted();
			_conn.Close();
		}

		[Test]
		[Ignore("Not now")]
		public void GenerateReportGeneratesReport()
		{
			var aun = 123456789;
			var schoolYear = "2017-2018";

			var schoolDistrict = new SchoolDistrict
			{
				Aun = aun,
				Name = "Some SD",
				Rate = 10000m,
				SpecialEducationRate = 20000m,
			};

			// var statuses = new[] {
			// 	new CommittedStudentStatusRecord {
			// 		StudentId = "123456",
			// 		StudentPaSecuredId = 1234567890,
			// 		SchoolDistrictId = aun,
			// 		StudentFirstName = "Alice",
			// 		StudentMiddleInitial = "B",
			// 		StudentLastName = "Charlie",
			// 		StudentStreet1 = "Somewhere",
			// 		StudentCity = "Over The Rainbow",
			// 		StudentState = "PA",
			// 		StudentZipCode = "15000",
			// 		StudentGradeLevel = "K",
			// 		StudentDateOfBirth = new DateTime(2012, 7, 1),
			// 		StudentEnrollmentDate = new DateTime(2017, 7, 1),
			// 		StudentIsSpecialEducation = false,
			// 	},
			// 	new CommittedStudentStatusRecord {
			// 		StudentId = "234567",
			// 		StudentPaSecuredId = 2345678901,
			// 		SchoolDistrictId = aun,
			// 		StudentFirstName = "Bob",
			// 		StudentLastName = "Doug",
			// 		StudentStreet1 = "Not",
			// 		StudentStreet2 = "There",
			// 		StudentCity = "Anywhere",
			// 		StudentState = "PA",
			// 		StudentZipCode = "15000",
			// 		StudentGradeLevel = "12",
			// 		StudentDateOfBirth = new DateTime(1999, 7, 1),
			// 		StudentEnrollmentDate = new DateTime(2017, 7, 1),
			// 		StudentWithdrawalDate = new DateTime(2017, 8, 1),
			// 		StudentCurrentIep = new DateTime(2017, 7, 1),
			// 		StudentFormerIep = new DateTime(2016, 8, 1),
			// 		StudentIsSpecialEducation = true,
			// 	},
			// };

			var payments = new[] {
				new Payment {
					Type = PaymentType.Check,
					ExternalId = "09876",
					Amount = 100m,
					Date = new DateTime(2017, 8, 10),
					SchoolYear = schoolYear,
					SchoolDistrict = schoolDistrict,
				},
				new Payment {
					Type = PaymentType.UniPay,
					Amount = 200m,
					Date = new DateTime(2018, 1, 10),
					SchoolYear = schoolYear,
					SchoolDistrict = schoolDistrict,
				}
			};

			var refunds = new[] {
				new Refund {
					Amount = 150m,
					SchoolYear = schoolYear,
					SchoolDistrict = schoolDistrict,
					Date = new DateTime(2017, 10, 23),
				},
				new Refund {
					Amount = 200m,
					SchoolYear = schoolYear,
					SchoolDistrict = schoolDistrict,
					Date = new DateTime(2018, 1, 13),
				}
			};

			using (var ctx = NewContext())
			{
				ctx.Add(schoolDistrict);
				// ctx.AddRange(statuses);
				ctx.AddRange(payments);
				ctx.AddRange(refunds);

				ctx.SaveChanges();
			}

			var time = new DateTime(2018, 2, 1);
			var config = new InvoiceReporter.Config
			{
				InvoiceNumber = "1234567890",
				SchoolYear = schoolYear,
				AsOf = time,
				Prepared = time.AddDays(5),
				ToSchoolDistrict = time.AddDays(10),
				ToPDE = time.AddDays(20),
				SchoolDistrictAun = aun,
			};
			var actual = _uut.GenerateReport(config);

			Console.WriteLine($"actual: {JsonConvert.SerializeObject(actual, Formatting.Indented)}");

			// basic header information
			Assert.That(actual.Number, Is.EqualTo(config.InvoiceNumber));
			Assert.That(actual.SchoolYear, Is.EqualTo(config.SchoolYear));
			Assert.That(actual.FirstYear, Is.EqualTo(2017));
			Assert.That(actual.SecondYear, Is.EqualTo(2018));
			Assert.That(actual.AsOf, Is.EqualTo(config.AsOf));
			Assert.That(actual.Prepared, Is.EqualTo(config.Prepared));
			Assert.That(actual.ToSchoolDistrict, Is.EqualTo(config.ToSchoolDistrict));
			Assert.That(actual.ToPDE, Is.EqualTo(config.ToPDE));

			// school district
			{
				Assert.That(actual.SchoolDistrict, Is.Not.Null);
				var district = actual.SchoolDistrict;
				Assert.That(district.Id, Is.EqualTo(schoolDistrict.Id));
				Assert.That(district.Aun, Is.EqualTo(schoolDistrict.Aun));
				Assert.That(district.Name, Is.EqualTo(schoolDistrict.Name));
				Assert.That(district.RegularRate, Is.EqualTo(schoolDistrict.Rate));
				Assert.That(district.SpecialRate, Is.EqualTo(schoolDistrict.SpecialEducationRate));
			}

			// regular enrollments
			{
				Assert.That(actual.RegularEnrollments, Is.Not.Null);
				var enrollments = actual.RegularEnrollments;
				// Assert.That(enrollments.July, Is.EqualTo(1));
				// Assert.That(enrollments.August, Is.EqualTo(1));
				// Assert.That(enrollments.September, Is.EqualTo(1));
				// Assert.That(enrollments.October, Is.EqualTo(1));
				// Assert.That(enrollments.November, Is.EqualTo(1));
				// Assert.That(enrollments.December, Is.EqualTo(1));
				// Assert.That(enrollments.January, Is.EqualTo(1));
				// Assert.That(enrollments.February, Is.EqualTo(1));
				// // 0 after AsOf date
				// Assert.That(enrollments.March, Is.EqualTo(0));
				// Assert.That(enrollments.April, Is.EqualTo(0));
				// Assert.That(enrollments.May, Is.EqualTo(0));
				// Assert.That(enrollments.June, Is.EqualTo(0));
			}

			// special enrollments
			{
				Assert.That(actual.SpecialEnrollments, Is.Not.Null);
				var enrollments = actual.SpecialEnrollments;
				// Assert.That(enrollments.July, Is.EqualTo(1));
				// Assert.That(enrollments.August, Is.EqualTo(1));
				// Assert.That(enrollments.September, Is.EqualTo(0));
				// Assert.That(enrollments.October, Is.EqualTo(0));
				// Assert.That(enrollments.November, Is.EqualTo(0));
				// Assert.That(enrollments.December, Is.EqualTo(0));
				// Assert.That(enrollments.January, Is.EqualTo(0));
				// Assert.That(enrollments.February, Is.EqualTo(0));
				// Assert.That(enrollments.March, Is.EqualTo(0));
				// Assert.That(enrollments.April, Is.EqualTo(0));
				// Assert.That(enrollments.May, Is.EqualTo(0));
				// Assert.That(enrollments.June, Is.EqualTo(0));
			}

			// transactions
			{
				Assert.That(actual.Transactions, Is.Not.Null);
				var transactions = actual.Transactions;

				// empty months
				foreach (var month in new[] {
					"July",
					// "August",
					"September",
					// "October",
					"November",
					"December",
					// "January",
					"February",
					"March",
					"April",
					"May",
					"June",
				})
				{
					var property = typeof(InvoiceTransactions).GetProperty(month);
					var transaction = property.GetValue(transactions) as InvoiceTransaction;

					Assert.That(transaction, Is.Not.Null);
					Assert.That(transaction.Payment, Is.Null);
					Assert.That(transaction.Refund, Is.Null);
				}

				// August
				{
					Assert.That(transactions.August, Is.Not.Null);
					var month = transactions.August;
					Assert.That(month.Payment, Is.Not.Null);
					var payment = month.Payment;
					Assert.That(payment.Type, Is.EqualTo(payments[0].Type.Value));
					Assert.That(payment.CheckAmount, Is.EqualTo(payments[0].Amount));
					Assert.That(payment.CheckNumber, Is.EqualTo(payments[0].ExternalId));
					Assert.That(payment.Date, Is.EqualTo(payments[0].Date));
					Assert.That(month.Refund, Is.Null);
				}

				// October
				{
					Assert.That(transactions.October, Is.Not.Null);
					var month = transactions.October;
					Assert.That(month.Payment, Is.Null);
					Assert.That(month.Refund, Is.EqualTo(refunds[0].Amount));
				}

				// January
				{
					Assert.That(transactions.January, Is.Not.Null);
					var month = transactions.January;
					Assert.That(month.Payment, Is.Not.Null);
					var payment = month.Payment;
					Assert.That(payment.Type, Is.EqualTo(payments[1].Type.Value));
					Assert.That(payment.UniPayAmount, Is.EqualTo(payments[1].Amount));
					Assert.That(payment.CheckNumber, Is.EqualTo(payments[1].ExternalId));
					Assert.That(payment.Date, Is.EqualTo(payments[1].Date));
					Assert.That(month.Refund, Is.EqualTo(refunds[1].Amount));
				}
			}

			// students
			{
				Assert.That(actual.Students, Is.Not.Null);
				var students = actual.Students;
				Assert.That(students, Has.Count.EqualTo(2));

				for (var i = 0; i < students.Count; i++)
				{
					var student = students[i];
					// var status = statuses[i];
					// Assert.That(student.PASecuredID, Is.EqualTo(status.StudentPaSecuredId));
					// Assert.That(student.FirstName, Is.EqualTo(status.StudentFirstName));
					// Assert.That(student.MiddleInitial, Is.EqualTo(status.StudentMiddleInitial));
					// Assert.That(student.LastName, Is.EqualTo(status.StudentLastName));
					// if (string.IsNullOrEmpty(student.MiddleInitial))
					// 	Assert.That(student.FullName,
					// 		Is.EqualTo($"{status.StudentLastName}, {status.StudentFirstName}"));
					// else
					// 	Assert.That(student.FullName,
					// 		Is.EqualTo($"{status.StudentLastName}, {status.StudentFirstName} {status.StudentMiddleInitial}"));
					// Assert.That(student.Street1, Is.EqualTo(status.StudentStreet1));
					// Assert.That(student.Street2, Is.EqualTo(status.StudentStreet2));
					// if (string.IsNullOrEmpty(student.Street2))
					// 	Assert.That(student.Address1, Is.EqualTo($"{status.StudentStreet1}"));
					// else
					// 	Assert.That(student.Address1, Is.EqualTo($"{status.StudentStreet1} {status.StudentStreet2}"));
					// Assert.That(student.City, Is.EqualTo(status.StudentCity));
					// Assert.That(student.State, Is.EqualTo(status.StudentState));
					// Assert.That(student.ZipCode, Is.EqualTo(status.StudentZipCode));
					// Assert.That(student.Address2,
					// 	Is.EqualTo($"{status.StudentCity}, {status.StudentState} {status.StudentZipCode}"));
					// Assert.That(student.DateOfBirth, Is.EqualTo(status.StudentDateOfBirth));
					// Assert.That(student.Grade, Is.EqualTo(status.StudentGradeLevel));
					// Assert.That(student.FirstDay, Is.EqualTo(status.StudentEnrollmentDate));
					// Assert.That(student.LastDay, Is.EqualTo(status.StudentWithdrawalDate));
					// Assert.That(student.IsSpecialEducation, Is.EqualTo(status.StudentIsSpecialEducation));
					// Assert.That(student.CurrentIep, Is.EqualTo(status.StudentCurrentIep));
					// Assert.That(student.FormerIep, Is.EqualTo(status.StudentFormerIep));
				}
			}
		}

		[Test]
		[Ignore("Not now")]
		public void GenerateReportUsesAlternateRates()
		{
			var schoolDistrict = new SchoolDistrict
			{
				Aun = 1234567890,
				Name = "Some SD",
				Rate = 1m,
				AlternateRate = 10000m,
				SpecialEducationRate = 5m,
				AlternateSpecialEducationRate = 50000m,
			};

			using (var ctx = NewContext())
			{
				ctx.Add(schoolDistrict);
				ctx.SaveChanges();
			}

			var time = new DateTime(2018, 2, 1);
			var config = new InvoiceReporter.Config
			{
				InvoiceNumber = "1234567890",
				SchoolYear = "2017-2018",
				AsOf = time,
				Prepared = time.AddDays(5),
				ToSchoolDistrict = time.AddDays(10),
				ToPDE = time.AddDays(20),
				SchoolDistrictAun = schoolDistrict.Aun,
			};
			var actual = _uut.GenerateReport(config);

			Console.WriteLine($"actual: {JsonConvert.SerializeObject(actual, Formatting.Indented)}");

			Assert.That(actual.SchoolDistrict, Is.Not.Null);
			var district = actual.SchoolDistrict;
			Assert.That(district.RegularRate, Is.EqualTo(schoolDistrict.AlternateRate));
			Assert.That(district.SpecialRate, Is.EqualTo(schoolDistrict.AlternateSpecialEducationRate));
		}

		[Test]
		[Ignore("Not now")]
		public void GenerateReportDoesNotIncludeOptOuts()
		{
			var schoolDistrict = new SchoolDistrict
			{
				Aun = 1234567890,
				Name = "Some SD",
				Rate = 10000m,
				SpecialEducationRate = 20000m,
			};

			// var statuses = new[] {
			// 	new CommittedStudentStatusRecord
			// 	{
			// 		StudentId = "123456",
			// 		StudentPaSecuredId = 1234567890,
			// 		SchoolDistrictId = schoolDistrict.Aun,
			// 		StudentFirstName = "Alice",
			// 		StudentMiddleInitial = "B",
			// 		StudentLastName = "Charlie",
			// 		StudentStreet1 = "Somewhere",
			// 		StudentCity = "Over The Rainbow",
			// 		StudentState = "PA",
			// 		StudentZipCode = "15000",
			// 		StudentGradeLevel = "K",
			// 		StudentDateOfBirth = new DateTime(2012, 7, 1),
			// 		StudentEnrollmentDate = new DateTime(2017, 6, 1),
			// 		StudentWithdrawalDate = new DateTime(2017, 6, 1),
			// 		StudentIsSpecialEducation = false,
			// 	}
			// };

			using (var ctx = NewContext())
			{
				ctx.Add(schoolDistrict);
				// ctx.AddRange(statuses);
				ctx.SaveChanges();
			}

			var time = new DateTime(2018, 2, 1);
			var config = new InvoiceReporter.Config
			{
				InvoiceNumber = "1234567890",
				SchoolYear = "2017-2018",
				AsOf = time,
				Prepared = time.AddDays(5),
				ToSchoolDistrict = time.AddDays(10),
				ToPDE = time.AddDays(20),
				SchoolDistrictAun = schoolDistrict.Aun,
			};
			var actual = _uut.GenerateReport(config);

			Console.WriteLine($"actual: {JsonConvert.SerializeObject(actual, Formatting.Indented)}");

			Assert.That(actual.RegularEnrollments, Is.Not.Null);
			var enrollments = actual.RegularEnrollments;
			foreach (var property in enrollments.GetType().GetProperties())
			{
				var value = property.GetValue(enrollments);
				Assert.That(value, Is.EqualTo(0));
			}

			Assert.That(actual.Students, Has.Count.EqualTo(0));
		}

		[Test]
		[Ignore("Not now")]
		public void GenerateReportOrdersStudentByName()
		{
			var schoolDistrict = new SchoolDistrict
			{
				Aun = 1234567890,
				Name = "Some SD",
				Rate = 10000m,
				SpecialEducationRate = 20000m,
			};

			// var statuses = new[] {
			// 	new CommittedStudentStatusRecord { // fourth
			// 		StudentPaSecuredId = 1234567890,
			// 		SchoolDistrictId = schoolDistrict.Aun,
			// 		StudentFirstName = "B",
			// 		StudentMiddleInitial = "C",
			// 		StudentLastName = "D",
			// 	},
			// 	new CommittedStudentStatusRecord { // first
			// 		StudentPaSecuredId = 2345678901,
			// 		SchoolDistrictId = schoolDistrict.Aun,
			// 		StudentFirstName = "B",
			// 		StudentMiddleInitial = "C",
			// 		StudentLastName = "A",
			// 	},
			// 	new CommittedStudentStatusRecord { // third
			// 		StudentPaSecuredId = 3456789012,
			// 		SchoolDistrictId = schoolDistrict.Aun,
			// 		StudentFirstName = "B",
			// 		StudentMiddleInitial = "A",
			// 		StudentLastName = "D"
			// 	},
			// 	new CommittedStudentStatusRecord { // second
			// 		StudentPaSecuredId = 4567890123,
			// 		SchoolDistrictId = schoolDistrict.Aun,
			// 		StudentFirstName = "A",
			// 		StudentMiddleInitial = "C",
			// 		StudentLastName = "D",
			// 	},
			// };

			using (var ctx = NewContext())
			{
				ctx.Add(schoolDistrict);
				// ctx.AddRange(statuses);
				ctx.SaveChanges();
			}

			var time = new DateTime(2018, 2, 1);
			var config = new InvoiceReporter.Config
			{
				InvoiceNumber = "1234567890",
				SchoolYear = "2017-2018",
				AsOf = time,
				Prepared = time.AddDays(5),
				ToSchoolDistrict = time.AddDays(10),
				ToPDE = time.AddDays(20),
				SchoolDistrictAun = schoolDistrict.Aun,
			};
			var actual = _uut.GenerateReport(config);

			Console.WriteLine($"actual: {JsonConvert.SerializeObject(actual, Formatting.Indented)}");

			Assert.That(actual.Students, Is.Not.Null);
			Assert.That(actual.Students, Has.Count.EqualTo(4));
			var students = actual.Students;
			// Assert.That(students[0].PASecuredID, Is.EqualTo(statuses[1].StudentPaSecuredId));
			// Assert.That(students[1].PASecuredID, Is.EqualTo(statuses[3].StudentPaSecuredId));
			// Assert.That(students[2].PASecuredID, Is.EqualTo(statuses[2].StudentPaSecuredId));
			// Assert.That(students[3].PASecuredID, Is.EqualTo(statuses[0].StudentPaSecuredId));
		}
	}
}
