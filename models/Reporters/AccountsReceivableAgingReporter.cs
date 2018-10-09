using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace models.Reporters
{
	public class AccountsReceivableAgingTransaction
	{
		public string Identifier { get; set; }
		public string Type { get; set; }
		public DateTime Date { get; set; }
		public decimal? Amount { get; set; }
		public decimal? WriteOff { get; set; }
		public IList<decimal?> Buckets { get; set; }
	}

	public class AccountsReceivableAgingSchoolDistrict
	{
		public int Aun { get; set; }
		public string Name { get; set; }
		public IList<AccountsReceivableAgingTransaction> Transactions { get; set; }
		public IList<decimal> Totals { get; set; }
		public decimal Balance { get; set; }
	}

	public class AccountsReceivableAging
	{
		public DateTime? From { get; set; }
		public IList<AccountsReceivableAgingSchoolDistrict> SchoolDistricts { get; set; }
		public IList<decimal> GrandTotals { get; set; }
		public decimal GrandBalance { get; set; }
	}

	public class AccountsReceivableAgingReporter : IReporter<AccountsReceivableAging, AccountsReceivableAgingReporter.Config>
	{
		const string DATE_ID_FORMAT = "yyyyMMdd";
		const string INVOICE_TYPE = "INV";
		const string REFUND_TYPE = "REF";
		const string PAYMENT_TYPE = "PYMT";

		private readonly PacBillContext _context;
		public AccountsReceivableAgingReporter(PacBillContext context) => _context = context;

		public class Config
		{
			public DateTime? From { get; set; }
			public DateTime AsOf { get; set; }
			public IList<int> Auns { get; set; }
		}

		private struct AunNames
		{
			public int Aun { get; set; }
			public string Name { get; set; }
		}

		private IList<AunNames> GetAunNames(IList<int> auns = null)
		{
			var dd = _context.SchoolDistricts.AsQueryable();
			if (auns != null)
				dd = dd.Where(d => auns.Contains(d.Aun));

			return dd.Select(d => new AunNames { Aun = d.Aun, Name = d.Name }).ToList();
		}

		private IList<BulkInvoice> GetInvoices(DateTime? from)
		{
			var invoices = _context.Reports.Where(r => r.Type == ReportType.BulkInvoice || r.Type == ReportType.Invoice);
			if (from.HasValue)
				invoices = invoices.Where(r => r.Created >= from.Value);

			return invoices.
				Select(r => JsonConvert.DeserializeObject<BulkInvoice>(r.Data)).
				OrderByDescending(i => i.Scope).
				ThenByDescending(i => i.Prepared).
				ToList();
		}

		private IDictionary<int, Queue<Payment>> GetPayments(DateTime? from, DateTime now, IList<int> auns)
		{
			var payments = _context.Payments.
				Where(p => auns.Contains(p.SchoolDistrict.Aun)).
				Where(p => p.Date <= now.AddDays(1).Date);
			if (from.HasValue)
				payments = payments.Where(p => p.Date >= from.Value);

			return payments.
				OrderBy(p => p.Date).
				GroupBy(p => p.SchoolDistrict.Aun).
				ToDictionary(g => g.Key, g => new Queue<Payment>(g.ToList()));
		}

		private IDictionary<int, Queue<Refund>> GetRefunds(DateTime? from, DateTime now, IList<int> auns)
		{
			var refunds = _context.Refunds.
				Where(r => auns.Contains(r.SchoolDistrict.Aun)).
				Where(r => r.Date <= now.AddDays(1).Date);
			if (from.HasValue)
				refunds = refunds.Where(r => r.Date >= from.Value && r.Date <= now.AddDays(1).Date);

			return refunds.
				OrderBy(r => r.Date).
				GroupBy(r => r.SchoolDistrict.Aun).
				ToDictionary(g => g.Key, g => new Queue<Refund>(g.ToList()));
		}

		private class InvoiceDistrict
		{
			public string Scope { get; set; }
			public DateTime Prepared { get; set; }
			public BulkInvoiceSchoolDistrict Bisd { get; set; }

			public InvoiceDistrict(string s, DateTime p, BulkInvoiceSchoolDistrict d)
			{
				Scope = s;
				Prepared = p;
				Bisd = d;
			}
		}

		private IList<AccountsReceivableAgingSchoolDistrict> GetSchoolDistricts(DateTime? from, IList<int> auns = null)
		{
			var aunNames = GetAunNames(auns);
			auns = aunNames.Select(an => an.Aun).ToList();

			var invoices = GetInvoices(from).OrderBy(i => i.Scope).ThenBy(i => i.Prepared);
			var districts = new Dictionary<int, Queue<InvoiceDistrict>>();
			foreach (var i in invoices)
			{
				var dd = i.Districts.Where(d => auns.Contains(d.SchoolDistrict.Aun));
				foreach (var d in dd)
				{
					if (!districts.ContainsKey(d.SchoolDistrict.Aun))
					{
						var q = new Queue<InvoiceDistrict>();
						q.Enqueue(new InvoiceDistrict(i.Scope, i.Prepared, d));
						districts.Add(d.SchoolDistrict.Aun, q);
					}
					else
						districts[d.SchoolDistrict.Aun].Enqueue(new InvoiceDistrict(i.Scope, i.Prepared, d));
				}
			}

			var now = DateTime.Now.Date;
			var payments = GetPayments(from, now, auns);
			var refunds = GetRefunds(from, now, auns);

			var results = new List<AccountsReceivableAgingSchoolDistrict>();
			foreach (var an in aunNames.OrderBy(an => an.Name))
			{
				Queue<Refund> rr = null;
				if (refunds.ContainsKey(an.Aun))
					rr = refunds[an.Aun];

				Queue<InvoiceDistrict> dd = null;
				if (districts.ContainsKey(an.Aun))
					dd = districts[an.Aun];

				Queue<Payment> pp = null;
				if (payments.ContainsKey(an.Aun))
					pp = payments[an.Aun];

				var transactions = new List<AccountsReceivableAgingTransaction>();
				InvoiceDistrict previous = null;
				while ((rr != null && rr.Count > 0) || (dd != null && dd.Count > 0))
				{
					Refund r = null;
					if (rr != null && rr.Count > 0)
						r = rr.Peek();

					InvoiceDistrict d = null;
					if (dd != null && dd.Count > 0)
						d = dd.Peek();

					AccountsReceivableAgingTransaction transaction = null;
					if ((r != null && d != null && r.Date < d.Prepared) || (r != null && d == null))
					{
						r = rr.Dequeue();
						var amount = r.Amount;

						transaction = new AccountsReceivableAgingTransaction
						{
							Identifier = $"{REFUND_TYPE}{r.Date.ToString(DATE_ID_FORMAT)}",
							Type = REFUND_TYPE,
							Date = r.Date,
							Amount = amount,
						};
					}
					else if (d != null)
					{
						d = dd.Dequeue();
						var amount = GetTotal(d.Bisd);
						if (previous != null)
							amount -= GetTotal(previous.Bisd);

						transaction = new AccountsReceivableAgingTransaction
						{
							Identifier = d.Scope,
							Type = INVOICE_TYPE,
							Date = d.Prepared,
							Amount = amount,
						};

						previous = d;
					}

					var buckets = new decimal?[4];
					var bi = 3;
					if (transaction.Date >= now.LessDays(30))
						bi = 0;
					else if (transaction.Date >= now.LessDays(60) && transaction.Date <= now.LessDays(31))
						bi = 1;
					else if (transaction.Date >= now.LessDays(90) && transaction.Date <= now.LessDays(61))
						bi = 2;
					else
						bi = 3;
					buckets[bi] = transaction.Amount;
					transaction.Buckets = buckets;

					transactions.Add(transaction);

					var rem = buckets[bi];
					while (rem > 0 && (pp != null && pp.Count > 0))
					{
						var p = pp.Dequeue();
						var pt = new AccountsReceivableAgingTransaction
						{
							Identifier = $"{PAYMENT_TYPE}{p.Date.ToString(DATE_ID_FORMAT)}",
							Type = PAYMENT_TYPE,
							Date = p.Date,
							Amount = -p.Amount,
						};
						var pbb = new decimal?[4];
						pbb[bi] = -p.Amount;
						pt.Buckets = pbb;

						transactions.Add(pt);

						rem -= p.Amount;
					}
				}

				while (pp != null && pp.Count > 0)
				{
					var p = pp.Dequeue();
					transactions.Add(new AccountsReceivableAgingTransaction
					{
						Identifier = $"{PAYMENT_TYPE}{p.Date.ToString(DATE_ID_FORMAT)}",
						Type = PAYMENT_TYPE,
						Date = p.Date,
						Amount = -p.Amount,
						Buckets = new decimal?[4] {
							-p.Amount, null, null, null,
						}
					});
				}

				var sd = new AccountsReceivableAgingSchoolDistrict
				{
					Aun = an.Aun,
					Name = an.Name,
					Transactions = transactions,
				};
				sd.Totals = new[] {
					sd.Transactions.Sum(t => t.Buckets[0] ?? 0).Round(),
					sd.Transactions.Sum(t => t.Buckets[1] ?? 0).Round(),
					sd.Transactions.Sum(t => t.Buckets[2] ?? 0).Round(),
					sd.Transactions.Sum(t => t.Buckets[3] ?? 0).Round(),
				};
				sd.Balance = sd.Totals.Sum().Round();

				results.Add(sd);
			}

			return results;

			decimal GetTotal(BulkInvoiceSchoolDistrict d)
			{
				var regular = (d.SchoolDistrict.RegularRate * ((decimal)d.RegularEnrollments.Values.Sum() / 12)).Round();
				var special = (d.SchoolDistrict.SpecialRate * ((decimal)d.SpecialEnrollments.Values.Sum() / 12)).Round();
				return (regular + special).Round();
			}
		}

		public AccountsReceivableAging GenerateReport(Config config)
		{
			var report = new AccountsReceivableAging
			{
				From = config.From,
				SchoolDistricts = GetSchoolDistricts(config.From, config.Auns),
			};

			report.GrandTotals = new[] {
				report.SchoolDistricts.Sum(d => d.Totals[0]).Round(),
				report.SchoolDistricts.Sum(d => d.Totals[1]).Round(),
				report.SchoolDistricts.Sum(d => d.Totals[2]).Round(),
				report.SchoolDistricts.Sum(d => d.Totals[3]).Round(),
			};
			report.GrandBalance = report.GrandTotals.Sum().Round();

			return report;
		}
	}
}