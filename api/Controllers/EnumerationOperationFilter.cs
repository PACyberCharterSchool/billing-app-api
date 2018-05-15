﻿using System;
using System.Linq;

using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace api.Controllers
{
	public class EnumerationOperationFilter : IOperationFilter
	{
		public void Apply(Operation operation, OperationFilterContext context)
		{
			if (operation.Parameters == null)
				return;

			foreach (var param in context.ApiDescription.ParameterDescriptions)
			{
				var type = param.ParameterDescriptor.ParameterType;
				if (type.Name == param.Type.Name)
					continue;

				var prop = type.GetProperty(param.Name);
				if (prop == null)
					continue;

				var att = prop.GetCustomAttributes(typeof(EnumerationValidationAttribute), true).FirstOrDefault();
				if (att == null)
					continue;

				var values = ((EnumerationValidationAttribute)att).Values;
				var def = operation.Parameters.SingleOrDefault(p => p.Name == param.Name);
				if (def == null)
					continue;

				def.Extensions.Add("enum", values);
			}
		}
	}
}