﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace skky.Types
{
	public interface IGetObjectArray
	{
		object[] GetObjectArray(string timezone);
	}
	public interface IEntityid<TEntityId> : IGetObjectArray
	{
		TEntityId id { get; }
	}

	public interface IEntityIntid : IEntityid<int>
	{ }

	public interface IEntityLongid : IEntityid<long>
	{ }

	public interface IEntityGuidid : IEntityid<Guid>
	{ }

	public interface IEntityStringid : IEntityid<string>
	{ }

	public interface IEntityName
	{
		string name { get; set; }
	}
}
