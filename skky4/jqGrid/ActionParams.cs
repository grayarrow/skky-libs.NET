﻿using skky.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;

namespace skky.jqGrid
{
	[DataContract]
	public class ActionParams
	{
		public const string CONST_ActionAdd = "add";
		public const string CONST_ActionDelete = "del";
		public const string CONST_ActionEdit = "edit";
		public const string CONST_ActionUpdate = "update";

		public const string CONST_sordAsc = "asc";
		public const string CONST_sordDesc = "desc";

		public ActionParams()
		{
			translateIdsForSearching = true;
			searchTranslateTableColumnName = "name";
		}
		public ActionParams(string sortField, bool? sortAscending = null)
			: this()
		{
			if (!string.IsNullOrEmpty(sortField))
				sidx = sortField;

			if (null != sortAscending)
				sord = (sortAscending.Value ? CONST_sordAsc : CONST_sordDesc);
		}
		public ActionParams(string sortField, string sortOrder, int maxrows = 0)
			: this()
		{
			if (!string.IsNullOrWhiteSpace(sortField))
				sidx = sortField;

			if (!string.IsNullOrWhiteSpace(sortOrder))
				sord = sortOrder;

			rows = maxrows;
		}

		[DataContract]
		public class Rule
		{
			[DataMember]
			public string field { get; set; }
			[DataMember]
			public string op { get; set; }
			[DataMember]
			public string data { get; set; }
		};

		[DataContract]
		public class Filter
		{
			[DataMember]
			public string groupOp { get; set; }
			[DataMember]
			public List<Rule> rules { get; set; }
			//public Rule[] rules { get; set; }

			public static Filter Create(string jsonData)
			{
				try
				{
					var serializer =
					  new DataContractJsonSerializer(typeof(Filter));
					System.IO.StringReader reader =
					  new System.IO.StringReader(jsonData);
					System.IO.MemoryStream ms =
					  new System.IO.MemoryStream(
					  Encoding.Default.GetBytes(jsonData));
					return serializer.ReadObject(ms) as Filter;
				}
				catch
				{
					return null;
				}
			}
		};

		[DataMember]
		public bool _search { get; set; }
	
		[DataMember]
		public long nd { get; set; }
		
		[DataMember]
		public int page { get; set; }
		
		[DataMember]
		public int rows { get; set; }
		
		[DataMember]
		public string searchField { get; set; }
		
		[DataMember]
		public string searchOper { get; set; }
		
		[DataMember]
		public string searchString { get; set; }
	
		[DataMember]
		public string sidx { get; set; }

		[DataMember]
		public string sord { get; set; }

		/// <summary>
		/// Timezone offset in minutes.
		/// JavaScript it will be positive west of GMT and negative east of GMT.
		/// This is because in the west, I must add the minutes to be UTC.
		/// </summary>
		[DataMember]
		public string tz { get; set; }

		/// <summary>
		/// Used to transform idTable into Table.translateTableColumn (usually the name field).
		/// </summary>
		[DataMember]
		public bool translateIdsForSearching { get; set; }

		[DataMember]
		public string searchTranslateTableColumnName { get; set; }

		public string GetTranslatedSortIndex()
		{
			if (!string.IsNullOrEmpty(sidx)
				&& translateIdsForSearching
				&& !string.IsNullOrEmpty(searchTranslateTableColumnName)
				&& sidx.StartsWith("id")
				&& sidx.Length > 2)
				return sidx.Mid(2) + "." + searchTranslateTableColumnName;

			return sidx;
		}

		private Filter _Filter = null;
		public Filter theFilter
		{
			get
			{
				if (null == _Filter)
					_Filter = new Filter();

				return _Filter;
			}
			set
			{
				_Filter = value;
			}
		}

		private string _filters;
		[DataMember]
		public string filters
		{
			get
			{
				return _filters;
			}
			set
			{
				_filters = value;
				_Filter = Filter.Create(_filters);
			}
		}

		//[DataMember]
		//public Filter filters2
		//{
		//    get
		//    {
		//        if (null == _filters)
		//            _filters = new Filter();

		//        return _filters;
		//    }
		//    set
		//    {
		//        _filters = value;
		//    }
		//}
		//public string filters { get; set; }

		public bool Fixup()
		{
			bool neededFixup = false;
			if ("\"null\"" == sidx || "null" == sidx || "'null'" == sidx)
			{
				sidx = null;
				neededFixup = true;
			}

			return neededFixup;
		}

		public Rule FindAndRemoveRuleRaw(string ruleName)
		{
			Rule ruleToRemove = null;

			if (null != theFilter
				&& null != theFilter.rules
				&& theFilter.rules.Count() > 0
				&& !string.IsNullOrEmpty(ruleName))
			{
				foreach (var rule in theFilter.rules)
				{
					if (ruleName == rule.field)
					{
						ruleToRemove = rule;
						theFilter.rules.Remove(rule);
						break;
					}
				}
			}

			return ruleToRemove;
		}
		public string FindAndRemoveRule(string ruleName)
		{
			Rule rule = FindAndRemoveRuleRaw(ruleName);
			if(null != rule)
				return rule.data;

			return null;
		}
	}
}
