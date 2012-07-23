// ___________________________________________________
//
//  (c) Hi-Integrity Systems 2012. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;
using System.Text.RegularExpressions;

namespace DatabaseObjects.Constraints
{
	/// <summary>
	/// Ensures that the email address is valid.
	/// Credit to: http://regexlib.com/REDetails.aspx?regexp_id=711
	/// </summary>
	public class EmailAddressConstraint : RegExConstraint
	{
		public EmailAddressConstraint() 
            : base("^((?>[a-zA-Z\\d!#$%&\'*+\\-/=?^_`{|}~]+\\x20*|\"((?=[\\x01-\\x7f])[^\"\\\\]|\\\\[\\x01-\\x7f])*\"\\x20*)*(?<angle><))?((?!\\.)(?>\\.?[a-zA-Z\\d!#$%&\'*+\\-/=?^_`{|}~]+)+|\"((?=[\\x01-\\x7f])[^\"\\\\]|\\\\[\\x01-\\x7f])*\")@(((?!-)[a-zA-Z\\d\\-]+(?<!-)\\.)+[a-zA-Z]{2,}|\\[(((?(?<!\\[)\\.)(25[0-5]|2[0-4]\\d|[01]?\\d?\\d)){4}|[a-zA-Z\\d\\-]*[a-zA-Z\\d]:((?=[\\x01-\\x7f])[^\\\\\\[\\]]|\\\\[\\x01-\\x7f])+)\\])(?(angle)>)$")
		{
		}
			
		public override string ToString()
		{
			return "email address is valid";
		}
	}
}