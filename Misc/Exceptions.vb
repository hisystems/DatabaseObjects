
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace Exceptions


    ''' <summary>
    ''' Represents a general DatabaseObjects library exception
    ''' </summary>
    <Serializable()> _
    Public Class DatabaseObjectsException
        Inherits ApplicationException

        Public Sub New(ByVal info As System.Runtime.Serialization.SerializationInfo, ByVal context As System.Runtime.Serialization.StreamingContext)

            MyBase.New(info, context)

        End Sub

        Public Sub New(ByVal strMessage As String)

            MyBase.New(strMessage)

        End Sub

        Public Sub New(ByVal strMessage As String, ByVal objInnerException As Exception)

            MyBase.New(strMessage, objInnerException)

        End Sub

    End Class


    Public Class ObjectAlreadyExistsException
        Inherits ApplicationException

        Private pobjItem As IDatabaseObject
        Private pobjKey As Object

        Public Sub New( _
            ByVal objItem As IDatabaseObject, _
            ByVal objKey As Object)

            MyBase.New(TypeName(objItem) & ": '" & CStr(objKey) & "'")

            pobjItem = objItem
            pobjKey = objKey

        End Sub

        Public ReadOnly Property Item() As IDatabaseObject
            Get

                Return pobjItem

            End Get
        End Property

        Public ReadOnly Property Key() As Object
            Get

                Return pobjKey

            End Get
        End Property

    End Class


    Public Class ObjectDoesNotExistException
        Inherits ApplicationException

        Private pobjDistinctOrKeyValue As Object

        Public Sub New( _
            ByVal objCollection As IDatabaseObjects, _
            ByVal objDistinctOrKeyValue As Object)

            MyBase.New(TypeName(objCollection) & ": '" & CType(objDistinctOrKeyValue, String) & "'")

            pobjDistinctOrKeyValue = objDistinctOrKeyValue

        End Sub

        Public Sub New( _
            ByVal objItem As IDatabaseObject)

            MyBase.New(TypeName(objItem) & ": '" & CStr(objItem.DistinctValue) & "'")

            pobjDistinctOrKeyValue = objItem.DistinctValue

        End Sub

        Public ReadOnly Property DistinctOrKeyValue() As Object
            Get

                Return pobjDistinctOrKeyValue

            End Get
        End Property

    End Class


    Public Class MethodLockedException
        Inherits ApplicationException

        Public Sub New()

        End Sub

        Public Sub New(ByVal strMessage As String)

            MyBase.New(strMessage)

        End Sub

    End Class


    ''' <summary>
    ''' Thrown when an object tries to be locked - but it is already locked.
    ''' </summary>
    ''' <remarks>
    ''' Originally the DatabaseObjectsLockController.Lock would throw a DatabaseObjectsException but now it throws a ObjectAlreadyLockedException.
    ''' </remarks>
    Public Class ObjectAlreadyLockedException
        Inherits DatabaseObjectsException

        Public Sub New(ByVal objCollection As IDatabaseObjects, ByVal objObject As IDatabaseObject)

            MyBase.New(Misc.TypeName(objObject) & "." & objCollection.DistinctFieldName & " " & objObject.DistinctValue.ToString & " is already locked")

        End Sub

    End Class

End Namespace



Module Misc

    Friend Function TypeName(ByVal objItem As Object) As String

        Return objItem.GetType.Name

    End Function

End Module

