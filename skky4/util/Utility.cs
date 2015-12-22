﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace skky.util
{
	public static class Utility
	{
		public const string CONST_CopyFromDefaultExcludeColumns = "id,actionedby,actionedon,createdby,createdon,updatedBy,updatedOn";
		/// <summary>
		/// Copies from one object, the source, to the destination object using Reflection.
		/// The only member objects copied to the the destination,
		///  are the ones that contain the same exact named fields in both the source and destination objects.
		/// </summary>
		/// <typeparam name="T1">Any class object.</typeparam>
		/// <typeparam name="T2">Any class object</typeparam>
		/// <param name="src">The T1 source object to copy from.</param>
		/// <param name="dest">The T2 destination object to copy to.</param>
		/// <param name="skipIDCreateActioned">Skip the ID, actionedBy, actionedOn, createdBy and createdOn columns from the copy..</param>
		/// <returns>The T1 used to call this method.</returns>
		public static T1 CopyFrom<T1, T2>(this T1 dest, T2 src, bool skipIDCreateActioned = false)
			where T1 : class
			where T2 : class
		{
			List<string> changedFields = null;

			return dest.CopyFrom(src, skipIDCreateActioned ? CONST_CopyFromDefaultExcludeColumns : null, ref changedFields);
		}

		public static T1 CopyFrom<T1, T2>(this T1 dest, T2 src, bool skipIDCreateActioned, ref List<string> changedFields)
			where T1 : class
			where T2 : class
		{
			return dest.CopyFrom(src, skipIDCreateActioned ? CONST_CopyFromDefaultExcludeColumns : null, ref changedFields);
		}

		public static T1 CopyFrom<T1, T2>(this T1 dest, T2 src, string ignoreList)
			where T1 : class
			where T2 : class
		{
			List<string> changedFields = null;

			return CopyFrom(dest, src, ignoreList, ref changedFields);
		}

		/// <summary>
		/// Copies from one object, the source, to the destination object using Reflection.
		/// The only member objects copied to the the destination,
		///  are the ones that contain the same exact named fields in both the source and destination objects.
		/// </summary>
		/// <typeparam name="T1">Any class object.</typeparam>
		/// <typeparam name="T2">Any class object</typeparam>
		/// <param name="src">The T1 source object to copy from.</param>
		/// <param name="dest">The T2 destination object to copy to.</param>
		/// <returns>The T1 used to call this method.</returns>
		public static T1 CopyFrom<T1, T2>(this T1 dest, T2 src, string ignoreList, ref List<string> changedFields)
			where T1 : class
			where T2 : class
		{
			if (null != src)
			{
				List<string> lstIgnore = skky.util.Parser.SplitAndTrimString(ignoreList).ToList();

				PropertyInfo[] srcFields = src.GetType().GetProperties(
					BindingFlags.Instance | BindingFlags.Public | BindingFlags.GetProperty);

				PropertyInfo[] destFields = dest.GetType().GetProperties(
					BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty);

				foreach (var property in srcFields)
				{
					if (lstIgnore.Contains(property.Name) || lstIgnore.Contains(property.Name.ToLower()))
						continue;

					var destField = destFields.FirstOrDefault(x => x.Name == property.Name);
					if (null != destField && destField.CanWrite)
					{
						var srcValue = property.GetValue(src, null);

						if (null != changedFields)
						{
							bool changed = false;

							var destValue = destField.GetValue(dest, null);
							//var srcField = srcFields.FirstOrDefault(x => x.Name == property.Name);
							//if (null != srcField)
							//{
								//var srcValue = srcField.GetValue(src, null);

								if ((null != destValue && !destValue.Equals(srcValue))
								|| (null != srcValue && !srcValue.Equals(destValue)))
									changed = true;
							//}

							if(changed)
								changedFields.Add(destField.Name);
						}

						destField.SetValue(dest, srcValue, null);
					}
				}
			}

			return dest;
		}
	}
}
