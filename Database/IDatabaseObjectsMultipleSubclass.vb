
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
''' Extends the functionality of IDatabaseObjects so that if a collection contains
''' multiple subclasses of a particular class in the collection then the 
''' ItemInstanceForSubclass is used rather than ItemInstance to return the particular
''' subclass to be created.
''' </summary>
''' --------------------------------------------------------------------------------
''' 
Public Interface IDatabaseObjectsMultipleSubclass
    Inherits IDatabaseObjects

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Should return an instance of the class that is associated with this collection of objects. 
    ''' The associated class must implement the IDatabaseObjectMultipleSubclass interface.
    ''' </summary>
    ''' <param name="objFieldValues">
    ''' The database record field values that can be used to determine the type of subclass to be loaded.
    ''' Do NOT call ObjectFromFieldValues() or ObjectLoad(). These functions are called after
    ''' ItemInstanceForSubclass returns.
    ''' from this function.
    ''' </param>
    ''' 
    ''' <example> 
    ''' <code>
    ''' Protected Function ItemInstanceForSubclass(ByVal objFieldValues As SQL.SQLFieldValues) As IDatabaseObject Implements IDatabaseObjects.ItemInstanceForSubclass
    ''' 
    '''     If objSubclassRecord("Type") = "Special" Then
    '''         Return New SpecialisedProduct
    '''     Else
    '''         Return New Product
    '''     End If
    ''' 
    ''' End Function
    ''' </code>
    ''' </example>    
    ''' --------------------------------------------------------------------------------
    Function ItemInstanceForSubclass(ByVal objFieldValues As SQL.SQLFieldValues) As IDatabaseObject

End Interface

