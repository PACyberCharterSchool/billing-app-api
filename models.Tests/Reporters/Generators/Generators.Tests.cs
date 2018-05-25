using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using NUnit.Framework;

using models.Reporters.Generators;
using static models.Reporters.Generators.Generator;

namespace models.Tests.Reporters.Generators
{
	[TestFixture]
	public class GeneratorsTests
	{
		[Test]
		public void InputGeneratorReturnsFromInput()
		{
			var value = 1;
			var input = new
			{
				a = value,
			};

			var actual = Input(i => i.a)(input: input);
			Assert.That(actual, Is.EqualTo(value));
		}

		[Test]
		public void ObjectGeneratorSetsProperty()
		{
			var value = 1;
			var input = new
			{
				a = value,
			};

			var prop = "A";
			var actual = Object(
				(prop, Input(i => i.a))
			)(input: input);

			Assert.That(actual, Contains.Key(prop));
			Assert.That(actual[prop], Is.EqualTo(value));
		}

		[Test]
		public void ReferenceGeneratorReturnsFromState()
		{
			var value = 1;
			var state = new State
			{
				{"a", new State
				{
					{"b", value},
				}},
			};

			var actual = Reference(s => s["a"]["b"])(state: state);

			Assert.That(actual, Is.EqualTo(value));
		}

		[Test]
		public void ReferenceGeneratorCanReferenceSibling()
		{
			var key = "a";
			var reference = "b";
			var value = 1;

			var actual = Object(
				(key, Constant(value)),
				(reference, Reference(s => s[key]))
			)();

			Assert.That(actual[reference], Is.EqualTo(value));
		}

		[Test]
		public void ReferenceGeneratorCanReferenceGrandSibling()
		{
			var key = "a";
			var reference = "b";
			var value = 1;

			var actual = Object(
				("parent", Object(
					(key, Constant(value)),
					(reference, Reference(s => s["parent"][key]))
				)
			))();

			Assert.That(actual["parent"][reference], Is.EqualTo(value));
		}

		[Test]
		public void ReferenceGeneratorCanReferenceNephew()
		{
			var key = "a";
			var reference = "b";
			var value = 1;

			var actual = Object(
				("sibling", Object(
					(key, Constant(value))
				)),
				(reference, Reference(s => s["sibling"][key]))
			)();

			Assert.That(actual[reference], Is.EqualTo(value));
		}

		[Test]
		public void ReferenceGeneratorCanReferenceUncle()
		{
			var key = "a";
			var reference = "b";
			var value = 1;

			var actual = Object(
				(key, Constant(value)),
				("parent", Object(
					(reference, Reference(s => s[key]))
				))
			)();

			Assert.That(actual["parent"][reference], Is.EqualTo(value));
		}

		[Test]
		public void LambdaGenerateWithNoParamsReturnsResults()
		{
			var value = 1;
			var actual = Lambda(() => value)();
			Assert.That(actual, Is.EqualTo(value));
		}

		[Test]
		public void LambdaGenerateWithOneParamReturnsResults()
		{
			var value = 2;
			var actual = Lambda((int x) => x * x, Constant(value))();
			Assert.That(actual, Is.EqualTo(value * value));
		}

		[Test]
		public void LambdaGenerateWithTwoParamsReturnsResults()
		{
			var value1 = 1;
			var value2 = 2;
			var actual = Lambda((int x, int y) => x + y, Constant(value1), Constant(value2))();
			Assert.That(actual, Is.EqualTo(value1 + value2));
		}

		private static SqliteConnection _conn = new SqliteConnection("Data Source=:memory:");

		private static PacBillContext NewContext()
		{
			var ctx = new PacBillContext(new DbContextOptionsBuilder<PacBillContext>().
				UseSqlite(_conn).Options);
			ctx.Database.Migrate();

			return ctx;
		}

		[Test]
		public void SqlGeneratorReturnsQueryWithoutArgs()
		{
			using (_conn)
			{
				_conn.Open();
				var refund = new Refund
				{
					Amount = 10m,
				};
				using (var ctx = NewContext())
					ctx.SaveChanges(() => ctx.Add(refund));

				var query = @"
					select *
					from Refunds
				";

				var actual = Sql(NewContext().Database.GetDbConnection(), query)();
				Assert.That(actual[0].Amount, Is.EqualTo(refund.Amount.ToString("0.0")));
			}
		}

		[Test]
		public void SqlGeneratorReturnsQueryWithArgs()
		{
			using (_conn)
			{
				_conn.Open();
				var refund = new Refund
				{
					Id = 3,
					Amount = 10m,
				};
				using (var ctx = NewContext())
					ctx.SaveChanges(() => ctx.Add(refund));

				var query = @"
					select *
					from Refunds
					where Id = @id
				";
				var actual = Sql(NewContext().Database.GetDbConnection(), query, ("id", Constant(3)))();
				Assert.That(actual[0].Amount, Is.EqualTo(refund.Amount.ToString("0.0")));
			}
		}
	}
}
