
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
''' The IGlobalDatabaseObjects interface aids in maintaining a class library's 
''' abstraction layer when external objects (objects outside of the class library) 
''' need to be used. For example, if we had two DLL class libraries, the first an 
''' Inventory class library that exposes, amongst other classes a collection of 
''' Product objects and the second library needs to use the Product objects provided 
''' by the Inventory library. In particular, it needs to be able to store and load a 
''' Product using it's distinct value (ProductID). So, rather than exposing a public 
''' function in the Inventory library to load a Product using it's distinct value 
''' (ProductID) the Products class can implement the IGlobalDatabaseObjects interface. 
''' Implementing this interface allows a product to be loaded from it's distinct value 
''' without exposing a public function - thereby maintaining a degree of abstraction 
''' between the two dlls. Conversely, the product's distinct value (ProductID) can 
''' be extracted by calling DirectCast(objProduct, IDatabaseObject).DistinctValue.
''' <example>
''' <code>
''' An example of using an external DLL that implements IGlobalDatabaseObjects: 
''' 
''' Dim objProduct As Product = DirectCast(objProducts, IGlobalDatabaseObjects).Object(1234)
''' </code>
''' </example>
''' </summary>
''' --------------------------------------------------------------------------------
''' 
Public Interface IGlobalDatabaseObjects

    ''' --------------------------------------------------------------------------------
    ''' <summary>
    ''' Should return the object in the collection for the distinct value
    ''' argument. 
    ''' </summary>
    ''' --------------------------------------------------------------------------------
    Function [Object](ByVal objDistinctValue As Object) As IDatabaseObject

End Interface
