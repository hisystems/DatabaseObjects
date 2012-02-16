
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'


''' ------------------------------------------------------------------------------------
''' <summary>
''' Applies to child objects of a DatabaseObjectsVolatile collection. 
''' Save is called for each child that implements IDatabaseObjectVolatile when
''' DatabaseObjectsVolatile.VolatileObjectsSave() is called.
''' This allows the object to determine when it is being saved and perform
''' additional checking and validation if necessary.
''' </summary>
''' ------------------------------------------------------------------------------------
Public Interface IDatabaseObjectVolatile
    Inherits IDatabaseObject

    Sub Save()

End Interface
