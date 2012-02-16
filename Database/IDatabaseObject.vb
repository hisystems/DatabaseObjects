
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

''' --------------------------------------------------------------------------------
''' <summary>
''' An IDatabaseObject object represents a single record in a database table. The 
''' Database class provides the facility for classes implementing IDatabaseObject to 
''' copy data to and from the associated database record. Rather than directly 
''' implementing IDatabaseObject inherit from DatabaseObject as the DatabaseObject 
''' class provides the basic "plumbing" code required by IDatabaseObjects.
''' </summary>
''' --------------------------------------------------------------------------------
''' 
Public Interface IDatabaseObject

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Should return whether the object has been saved to the database. Generally, 
    ''' this is implemented using a private variable that stores the object's saved state.
    ''' If a new object is saved or an existing object is loaded then this property 
    ''' is automatically set to true by the library.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    ''' 
    Property IsSaved() As Boolean

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Should return the distinct value that uniquely identifies this 
    ''' object in the database. If a new object is saved (which uses an auto increment 
    ''' field) or an existing object is loaded then this property is automatically 
    ''' set by the library.
    ''' Typically, this is the value of an identity or auto increment database field.
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    ''' 
    Property DistinctValue() As Object

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' This subroutine should copy the database fields from objFields to the object's 
    ''' variables. objFields is populated with all of the fields from the associated record.
    ''' </summary>
    ''' 
    ''' <example> 
    ''' <code>
    ''' Protected Sub LoadFields(ByVal objFields As SQL.SQLFieldValues) Implements IDatabaseObject.LoadFields
    ''' 
    '''     pstrCode = objFields("ProductCode").Value
    '''     pstrDescription = objFields("ProductDescription").Value
    ''' 
    ''' End Sub
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    ''' 
    Sub LoadFields(ByVal objFields As SQL.SQLFieldValues)

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Should return an SQLFieldValues object populated with the  
    ''' fields to be written to the database. The first argument of the SQLFieldValues.Add 
    ''' function is the database field name, the second is the field's value.
    ''' </summary>
    ''' 
    ''' <example> 
    ''' <code>
    ''' Protected Function SaveFields() As SQL.SQLFieldValues Implements IDatabaseObject.SaveFields
    ''' 
    '''     SaveFields = New SQL.SQLFieldValues
    '''     SaveFields.Add("ProductCode", pstrCode)
    '''     SaveFields.Add("ProductDescription", pstrDescription)
    ''' 
    ''' End Function
    ''' </code>
    ''' </example>
    ''' --------------------------------------------------------------------------------
    ''' 
    Function SaveFields() As SQL.SQLFieldValues

End Interface

