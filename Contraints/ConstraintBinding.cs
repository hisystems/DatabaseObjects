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

namespace DatabaseObjects.Constraints
{
	/// <summary>
	/// Used for binding a value (typically a field) to a particular constraint.
	/// This allows the user interface to call ConstraintBinding.ConstraintSatisfied() at any point to determine
	/// whether the constraint has been satisfied. 
	/// If not, the ConstraintBinding.ErrorMessage() can be displayed to the user to aid in the notification.
	/// </summary>
	public class ConstraintBinding<T>
	{
		private Func<T> getValue;
		private IConstraint<T> pconstraint;
		private string perrorMessage;
			
		/// <summary>
		///
		/// </summary>
		/// <param name="getValue"></param>
		/// <param name="constraint"></param>
		/// <param name="errorMessage">
		/// An error message that is raised as part of an exception, and/or from the user interface.
		/// Parameter {0} represents the value from the callback.</param>
		/// <remarks></remarks>
		public ConstraintBinding(Func<T> getValue, IConstraint<T> constraint, string errorMessage)
		{
			this.pconstraint = constraint;
			this.getValue = getValue;
			this.perrorMessage = errorMessage;
		}
			
		/// <summary>
		///
		/// </summary>
		/// <param name="getValue"></param>
		/// <param name="constraint"></param>
		/// <remarks></remarks>
		public ConstraintBinding(Func<T> getValue, IConstraint<T> constraint) 
            : this(getValue, constraint, "Value '{0}' did not satisfy constraint; " + constraint.ToString())
		{
		}
			
		/// <summary>
		/// Uses the current value from the object to determine whether the constraint passes.
		/// True indicates that the constraint passes.
		/// </summary>
		/// <remarks></remarks>
		public bool ConstraintSatisfied()
		{
			return pconstraint.ValueSatisfiesConstraint(this.getValue());
		}
			
		/// <summary>
		/// Uses the current value from the callback to determine whether the constraint passes.
		/// If not, then an ArgumentException is thrown.
		/// </summary>
		public void EnsureConstraintSatisfied()
		{
			T value = getValue();
				
			if (!pconstraint.ValueSatisfiesConstraint(value))
				throw new ArgumentException(ErrorMessage(value));
		}
			
		/// <summary>
		/// Returns the error message using the current value associated with this constraint.
		/// </summary>
		public string ErrorMessage()
		{
			return ErrorMessage(this.getValue());
		}
			
		private string ErrorMessage(T value)
		{
			return string.Format(perrorMessage, value);
		}
			
		public IConstraint<T> Constraint
		{
			get
			{
				return pconstraint;
			}
		}
			
		/// <summary>
		/// Copies the current constraint binding, but binds to the variable specified.
		/// </summary>
		/// <example>
		/// Public Property Name As String
		///     Set(ByVal value As String)
		///
		///        nameIsSetBinding.Clone(value).EnsureConstraintSatisfied()
		///        Me._name = value
		///
		///     End Set
		/// End Property
		/// </example>
		public ConstraintBinding<T> Clone(T valueToBind)
		{
			return new ConstraintBinding<T>(() => valueToBind, this.pconstraint, this.perrorMessage);
		}
			
		/// <summary>
		/// Copies the current constraint binding, but binds to a new 'getValue' callback.
		/// </summary>
		public ConstraintBinding<T> Clone(Func<T> getValue)
		{
			return new ConstraintBinding<T>(getValue, this.pconstraint, this.perrorMessage);
		}
	}
}
