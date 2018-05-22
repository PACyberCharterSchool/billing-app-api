using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;

using Dapper;

namespace models.Reporters.Generators
{
	public interface IGenerator
	{
		dynamic Generate(Dictionary<string, dynamic> input, dynamic state = null);
	}

	public sealed class PropertiesGenerator : IGenerator
	{
		private readonly IDictionary<string, IGenerator> _properties;

		internal PropertiesGenerator(IDictionary<string, IGenerator> properties) => _properties = properties;

		public dynamic Generate(Dictionary<string, dynamic> input, dynamic state = null)
		{
			var next = new Dictionary<string, dynamic>();
			foreach (var property in _properties)
				next.Add(property.Key, property.Value.Generate(input, state));

			return next;
		}
	}

	public sealed class ConstantGenerator<T> : IGenerator
	{
		private readonly T _constant;

		internal ConstantGenerator(T constant) => _constant = constant;

		public dynamic Generate(Dictionary<string, dynamic> input, dynamic state = null) => _constant;
	}

	// TODO(Erik): use selector expression
	public sealed class InputGenerator : IGenerator
	{
		private readonly string _path;

		internal InputGenerator(string path) => _path = path;

		public dynamic Generate(Dictionary<string, dynamic> input, dynamic state = null)
		{
			if (input == null || input.Count == 0)
				return null;

			dynamic value = null;
			foreach (var part in _path.Split("."))
				value = input[part];

			return value;
		}
	}

	// TODO(Erik): use selector expression
	public sealed class ReferenceGenerator : IGenerator
	{
		private readonly string _path;

		internal ReferenceGenerator(string path) => _path = path;

		public dynamic Generate(Dictionary<string, dynamic> input, dynamic state = null)
		{
			if (state == null)
				return null;

			dynamic value = state;
			foreach (var part in _path.Split("."))
				value = value[part];

			return value;
		}
	}

	public sealed class LambdaGenerator : IGenerator
	{
		private readonly LambdaExpression _lambda;
		private readonly IList<IGenerator> _values;

		internal LambdaGenerator(LambdaExpression lambda, IList<IGenerator> values = null)
		{
			_lambda = lambda; ;
			_values = values;
		}

		private static Func<Delegate, dynamic[], dynamic>[] _actions = new Func<Delegate, dynamic[], dynamic>[] {
			(del, _) => del.DynamicInvoke(),
			(del, values) => del.DynamicInvoke(values[0]),
			(del, values) => del.DynamicInvoke(values[0], values[1]),
		};

		public dynamic Generate(Dictionary<string, dynamic> input, dynamic state = null)
		{
			var count = _lambda.Parameters.Count;
			var del = _lambda.Compile();
			dynamic[] values = null;
			if (_values != null)
				values = _values.Select(v => v.Generate(input, state)).ToArray();

			return _actions[count](del, values);
		}
	}

	public sealed class SqlGenerator : IGenerator
	{
		private readonly IDbConnection _db;
		private readonly string _query;
		private readonly PropertiesGenerator _args;

		internal SqlGenerator(IDbConnection db, string query, PropertiesGenerator args = null)
		{
			_db = db;
			_query = query;
			_args = args;
		}

		public dynamic Generate(Dictionary<string, dynamic> input, dynamic state = null)
		{
			if (_args == null)
				return _db.Query<dynamic>(_query);

			return _db.Query<dynamic>(_query, _args.Generate(input, state) as Dictionary<string, dynamic>);
		}
	}
}
