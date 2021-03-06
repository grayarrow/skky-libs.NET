﻿using System.Collections.Generic;
using System.Runtime.Serialization;
using skky.Types;
using skky.util;
using System;
using System.Linq;

namespace skky.jqGrid
{
	public class gridModelObjectArray : GridModelBase
	{
		[DataContract]
		public class Row
		{
			[DataMember]
			public object id;

			[DataMember]
			public object[] cell;

			public Row(object oid, int numCells)
			{
				id = oid;
				cell = new object[numCells];
			}
			public Row(object oid, object[] cells)
			{
				id = oid;
				cell = cells;
			}

			public Row(int numCells)
				: this(null, numCells)
			{ }
		}

		public gridModelObjectArray()
		{
			rows = new List<Row>();
		}
		public Row getNewRow(int numCells)
		{
			return new Row(numCells);
		}
		public Row AddRow(object rowId, object[] objects)
		{
			Row row = new Row(rowId, objects);
			rows.Add(row);

			return row;
		}

		[DataMember]
		public List<Row> rows;

		//public static gridModelObjectArray GetGridModel<T>(IEnumerable<T> query, ActionParams ap, string sidxDefault = null, string sordDefault = ActionParams.CONST_sordAsc, int maxRows = 20, Func<T, object[]> conversionFunc = null) where T : IEntityIntid
		//{
		//	var gm = new gridModelObjectArray();
		//	query = query.SortedAndPagedList(gm, ap, sidxDefault, sordDefault, maxRows);

		//	foreach (var item in query)
		//		gm.AddRow(item.id, null == conversionFunc ? item.GetObjectArray(null == ap ? 0 : ap.tzom) : conversionFunc(item));

		//	return gm;
		//}

		public static gridModelObjectArray GetGridModel<T>(IEnumerable<T> equery, ActionParams ap = null, string sidxDefault = null, string sordDefault = ActionParams.CONST_sordAsc, int maxRows = 20, Func<T, object[]> conversionFunc = null) where T : IEntityIntid
		{
			var query = equery.AsQueryable();
			query = query.SortedList(ap, sidxDefault, sordDefault);

			var gm = new gridModelObjectArray();
			query = query.PagedList(gm, ap, maxRows);

			foreach (var item in query)
				gm.AddRow(item.id, null == conversionFunc ? item.GetObjectArray(null == ap ? string.Empty : ap.tz) : conversionFunc(item));

			return gm;
		}
	}
}
