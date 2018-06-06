using System;
using System.Linq;
using System.IO;

using Newtonsoft.Json;
using NPOI.XSSF.UserModel;
using NUnit.Framework;

using models.Reporters.Exporters;

namespace models.Tests.Reporters.Exporters
{
	[TestFixture]
	public class XlsxExporterTests
	{
		private XlsxExporter _uut;

		[SetUp]
		public void SetUp()
		{
			_uut = new XlsxExporter();
		}

		[Test]
		public void ExportExportsJSON()
		{
			var template = new XSSFWorkbook(File.OpenRead("../../../TestData/test-template.xlsx"));
			var data = new
			{
				String = "bob", // JTokenType.String
				Object = new
				{
					Float = 1m, // JTokenType.Float
					Deeper = new
					{
						Boolean = true, // JTokenType.Boolean
					},
				},
				Array = new dynamic[] {
					DateTime.Now.Date, // JTokenType.Date
					new
					{
						Integer = 3, // JTokenType.Integer
					},
				},
			};

			var actual = _uut.Export(template, JsonConvert.DeserializeObject(JsonConvert.SerializeObject(data)));
			var sheet = actual.GetSheetAt(0);
			Assert.That(sheet.GetRow(0).GetCell(0).StringCellValue,
				Is.EqualTo(data.String));
			Assert.That(sheet.GetRow(1).GetCell(0).NumericCellValue,
				Is.EqualTo(data.Object.Float));
			Assert.That(sheet.GetRow(2).GetCell(0).DateCellValue,
				Is.EqualTo(data.Array[0]));
			Assert.That(sheet.GetRow(3).GetCell(0).NumericCellValue,
				Is.EqualTo(data.Array[1].Integer));
			Assert.That(sheet.GetRow(5).GetCell(0).BooleanCellValue,
				Is.EqualTo(data.Object.Deeper.Boolean));

			XSSFFormulaEvaluator.EvaluateAllFormulaCells(actual);
			Assert.That(sheet.GetRow(4).GetCell(0).StringCellValue,
				Is.EqualTo($"{data.String}, {data.Object.Float}"));
		}
	}
}
