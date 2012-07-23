// ___________________________________________________
//
//  © Hi-Integrity Systems 2012. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;

namespace DatabaseObjects
{
	public abstract class RootContainer
	{
		private Database database;
		
		protected RootContainer(Database database)
		{
			if (database == null)
				throw new ArgumentNullException();
			
			this.database = database;
		}
		
		protected internal Database Database
		{
			get
			{
				return this.database;
			}
		}
	}
}
