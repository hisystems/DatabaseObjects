
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
''' Specifies the type of class instance that will represent each database record / object.
''' The type must have a constructor with argument of type DatabaseObjects.DatabaseObjects. 
''' This is the same arguments that are required for a class that inherits from DatabaseObject. 
''' Alternatively, an empty constructor if available will be used.
''' The type must implement IDatabaseObject or inherit from DatabaseObject.
''' Using this attribute is logically equivalent to overridding the ItemInstance function.
''' </summary>
''' --------------------------------------------------------------------------------
<AttributeUsage(AttributeTargets.Class, AllowMultiple:=False, Inherited:=True)> _
Public Class ItemInstanceAttribute
    Inherits Attribute

    Private pobjType As Type

    ''' <summary>
    ''' Indicates the 
    ''' </summary>
    Public Sub New(ByVal objType As Type)

        pobjType = objType

    End Sub

    Public ReadOnly Property Type() As Type
        Get

            Return pobjType

        End Get
    End Property

End Class
