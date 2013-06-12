// _________________________________________________________________________
//
//  © Hi-Integrity Systems 2010. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// 
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//
//	    http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
// _________________________________________________________________________
//

using System.Collections;
using System;
using System.Data;

namespace DatabaseObjects
{
	/// --------------------------------------------------------------------------------
	/// <summary>
	/// The IGlobalDatabaseObjects interface aids in maintaining a class library's
	/// abstraction layer when external objects (objects outside of the class library)
	/// need to be used. For example, if we had two DLL class libraries, the first an
	/// Inventory class library that exposes, amongst other classes a collection of
	/// Product objects and the second library needs to use the Product objects provided
	/// by the Inventory library. In particular, it needs to be able to store and load a
	/// Product using it's distinct value (ProductID). So, rather than exposing a public
	/// function in the Inventory library to load a Product using it's distinct value
	/// (ProductID) the Products class can implement the IGlobalDatabaseObjects interface.
	/// Implementing this interface allows a product to be loaded from it's distinct value
	/// without exposing a public function - thereby maintaining a degree of abstraction
	/// between the two dlls. Conversely, the product's distinct value (ProductID) can
	/// be extracted by calling DirectCast(objProduct, IDatabaseObject).DistinctValue.
	/// <example>
	/// <code>
	/// An example of using an external DLL that implements IGlobalDatabaseObjects:
	///
	/// Dim objProduct As Product = DirectCast(objProducts, IGlobalDatabaseObjects).Object(1234)
	/// </code>
	/// </example>
	/// </summary>
	/// --------------------------------------------------------------------------------
	public interface IGlobalDatabaseObjects
	{
		/// --------------------------------------------------------------------------------
		/// <summary>
		/// Should return the object in the collection for the distinct value
		/// argument.
		/// </summary>
		/// --------------------------------------------------------------------------------
		IDatabaseObject Object(object objDistinctValue);
	}
}
