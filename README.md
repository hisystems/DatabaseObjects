DatabaseObjects
===============

Overview
--------
The DatabaseObjects library is .NET object relational mapping tool that contains a large set of powerful classes that interface to the underlying database system. Classes and properties in the business layer / model are marked with attributes (or functions overridden) in order to indicate the table and field mappings to the database. The library is simple, powerful and easy to learn. There are no external files to setup - everything is setup using standard object-oriented techniques. It is light-weight, flexible and provides facilities for ensuring maximum performance. The library supports access to Microsoft Access, SQL Server, MySQL, SQLite, HyperSQL and Pervasive database systems. It is also under active development and requests for changes are always welcome. 

License
-------
Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. You may obtain a copy of the License at http://www.apache.org/licenses/LICENSE-2.0
Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the License for the specific language governing permissions and limitations under the License.

SQL Support 
-----------
The library also supports other database commands for creating / altering tables, indexes, views and specifying table joins, arithmetic expressions, aggregates, grouping, ordering, unions and support for all common data types and more. All commands are database agnostic.

Documentation
-------------
All documentation and other information is available on the website [www.hisystems.com.au/databaseobjects](http://www.hisystems.com.au/databaseobjects)

Example
--------
Below is a really simple example of what the library can do. This is really just a taste, see the [demo project](https://github.com/hisystems/DatabaseObjects-Demo) or the [unit test project](https://github.com/hisystems/DatabaseObjects-UnitTests) for more advanced examples.

```c#
namespace Northwind.Model
{
	public class Northwind
	{
		public Suppliers Suppliers { get; private set; }

		public Northwind()
		{
			var database = new DatabaseObjects.MicrosoftSQLServerDatabase("localhost", "Northwind");
			this.Suppliers = new Suppliers(database);
		}
	}

	[DatabaseObjects.Table("Suppliers")]
	[DatabaseObjects.DistinctField("SupplierID", eAutomaticAssignment: DatabaseObjects.SQL.FieldValueAutoAssignmentType.AutoIncrement)]
	[DatabaseObjects.OrderByField("CompanyName")]
	public class Suppliers : DatabaseObjects.Generic.DatabaseObjectsEnumerable<Supplier>
	{
		internal Suppliers(DatabaseObjects.Database database)
			: base(database)
		{
		}
	}

	public class Supplier : DatabaseObjects.DatabaseObject
	{
		[DatabaseObjects.FieldMapping("CompanyName")]
		public string Name { get; set; }

		[DatabaseObjects.FieldMapping("ContactName")]
		public string ContactName { get; set; }

		internal Supplier(Suppliers suppliers)
			: base(suppliers)
		{
		}
	}
}

namespace Northwind.Executable
{
	using Model;

	public class Program
	{
		public static void Main(string[] args)
		{
			var model = new Northwind();

			foreach (Supplier supplier in model.Suppliers)
				System.Console.WriteLine(supplier.Name);
		}
	}
}
```

Demo Project
------------
The demonstration project is available in a separate repository on [Github here](https://github.com/hisystems/DatabaseObjects-Demo). It includes a number of examples on how to utilise the library. To run the demo project in conjunction with the library it must be located in same directory as the library.

For example:

/DatabaseObjects
/DatabaseObjects.Demo

Unit Test Project
-----------------
The unit test project is available in a separate repository on [Github here](https://github.com/hisystems/DatabaseObjects-UnitTests). It is also a good resource for examples on how to utilise the library. To run the unit tests project in conjunction with the library it must be located in the same directory as the library.

For example:

/DatabaseObjects
/DatabaseObjects.UnitTests

