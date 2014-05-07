﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mono.Debugging.Client;
using Mono.Debugging.Backend;

namespace MonoDevelop.D.Debugging
{
	public class DLocalExamBacktrace : IObjectValueSource
	{
		public readonly IDBacktraceHelpers BacktraceHelper;

		public DLocalExamBacktrace(IDBacktraceHelpers helper)
		{
			this.BacktraceHelper = helper;
		}



		public virtual ObjectValue CreateObjectValue(IDBacktraceSymbol s, EvaluationOptions evalOptions = null)
		{
			if (evalOptions == null)
				evalOptions = EvaluationOptions.DefaultOptions;
			
			

			return ObjectValue.CreatePrimitive(this, new ObjectPath(s.Name), s.TypeName, new EvaluationResult(s.Value), ObjectValueFlags.Field);
		}

		public ObjectValue[] GetParameters(int frameIndex, EvaluationOptions evalOptions)
		{
			BacktraceHelper.SelectStackFrame(frameIndex);
			var l = new List<ObjectValue>();
			foreach (var p in BacktraceHelper.Parameters)
			{
				l.Add(CreateObjectValue(p, evalOptions));
			}
			return l.ToArray();
		}

		public ObjectValue[] GetLocals(int frameIndex, EvaluationOptions evalOptions)
		{
			BacktraceHelper.SelectStackFrame(frameIndex);
			var l = new List<ObjectValue>();
			foreach (var p in BacktraceHelper.Locals)
			{
				l.Add(CreateObjectValue(p, evalOptions));
			}
			return l.ToArray();
		}

		public ObjectValue[] GetChildren(ObjectPath path, int index, int count, EvaluationOptions options)
		{
			return null;
		}

		public object GetRawValue(ObjectPath path, EvaluationOptions options)
		{
			return null;
		}

		public ObjectValue GetValue(ObjectPath path, EvaluationOptions options)
		{
			return null;
		}

		public void SetRawValue(ObjectPath path, object value, EvaluationOptions options)
		{
			
		}

		public EvaluationResult SetValue(ObjectPath path, string value, EvaluationOptions options)
		{
			return null;
		}
	}
}