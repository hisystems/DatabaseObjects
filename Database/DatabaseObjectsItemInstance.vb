
' ___________________________________________________
'
'  © Hi-Integrity Systems 2012. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On 

Imports System.Reflection

Friend Module DatabaseObjectsItemInstance

    ''' <summary>
    ''' </summary>
    ''' <param name="itemInstanceTypeToCreate">The type of DatabaseObjects.DatabaseObject to create.</param>
    ''' <param name="databaseObjects">Parameter that is passed to the constructor of the DatabaseObject to create. If there is a default constructor then this argument is not used.</param>
    Public Function CreateItemInstance(itemInstanceTypeToCreate As Type, databaseObjects As Global.DatabaseObjects.DatabaseObjects) As IDatabaseObject

        Dim objObjectInstance As Object = Nothing

        For Each objConstructor As ConstructorInfo In itemInstanceTypeToCreate.GetConstructors(BindingFlags.Instance Or BindingFlags.Public Or BindingFlags.NonPublic)
            Dim objConstructorParameters As ParameterInfo() = objConstructor.GetParameters()
            If objConstructorParameters.Length = 0 Then
                objObjectInstance = objConstructor.Invoke(Nothing)
                Exit For
            ElseIf objConstructorParameters.Length = 1 AndAlso (objConstructorParameters(0).ParameterType.IsSubclassOf(GetType(DatabaseObjects)) OrElse objConstructorParameters(0).ParameterType.Equals(GetType(DatabaseObjects))) Then
                objObjectInstance = objConstructor.Invoke({databaseObjects})
                Exit For
            End If
        Next

        If objObjectInstance Is Nothing Then
            Throw New Exceptions.DatabaseObjectsException("An empty constructor or constructor with argument DatabaseObjects.DatabaseObjects (or subclass) could not be found for type '" & itemInstanceTypeToCreate.FullName & "'. This type has been specified by the ItemInstanceAttribute for the type '" & databaseObjects.GetType.FullName & "' or as the T argument.")
        ElseIf Not TypeOf objObjectInstance Is IDatabaseObject Then
            Throw New Exceptions.DatabaseObjectsException("'" & itemInstanceTypeToCreate.FullName & "' does not implement IDatabaseObject or inherit from DatabaseObject. Type was specified for use by the ItemInstanceAttribute on the type '" & databaseObjects.GetType.FullName & "'")
        Else
            Return DirectCast(objObjectInstance, IDatabaseObject)
        End If

    End Function

    ''' <summary>
    ''' Returns the first generic argument that is passed to the DatabaseObjects.Generic.DatabaseObjects class (or super class).
    ''' This is used to determine the type of item that the collection represents and the type of class that should be returned from
    ''' the IDatabaseObjects.ItemInstance function.
    ''' </summary>
    Public Function GetGenericCollectionTArgument(collectionType As Type) As Type

        Dim currentCollectionType As Type = collectionType

        While currentCollectionType IsNot Nothing AndAlso Not (currentCollectionType.IsGenericType AndAlso currentCollectionType.GetGenericArguments(0).IsSubclassOf(GetType(DatabaseObject)))
            currentCollectionType = currentCollectionType.BaseType
        End While

        If currentCollectionType IsNot Nothing Then
            Return currentCollectionType.GetGenericArguments(0)
        Else
            Throw New Exceptions.DatabaseObjectsException("The ItemInstance type could not be found for '" & collectionType.FullName & "' because it does not inherit from DatabaseObjects.Generic.DatabaseObjects")
        End If

    End Function

End Module
