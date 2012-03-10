' ___________________________________________________
'
'  (c) Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace Constraints

    ''' <summary>
    ''' Ensures that the key field (KeyFieldName for parent collection class) value is unique within the collection.
    ''' The value passed to the ConstraintBinding must by the key field value.
    ''' The constraint ensure s that if the object is new then the key field value must be unique within the entire collection (the collection's SubSet applies).
    ''' Otherwise, if the new object is already saved and the key field value has changed then it is ensured to be unique within the collection (the collection's SubSet applies).
    ''' </summary>
    ''' <remarks></remarks>
    ''' <typeparam name="T">The key field data type (typically string)</typeparam>
    Public Class KeyFieldIsUniqueInCollectionConstraint(Of T)
        Implements IConstraint(Of T)

        Private pobjDatabaseObject As DatabaseObject

        ''' <summary>
        ''' Indicates the object that will be ensured to be unique within the collection based on the key field value.
        ''' </summary>
        ''' <param name="objDatabaseObject">
        ''' The object that must be unique based on the key field value.
        ''' </param>
        Public Sub New(ByVal objDatabaseObject As DatabaseObject)

            If objDatabaseObject Is Nothing Then
                Throw New ArgumentNullException
            End If

            pobjDatabaseObject = objDatabaseObject

        End Sub

        Private Function ValueSatisfiesConstraint(value As T) As Boolean Implements IConstraint(Of T).ValueSatisfiesConstraint

            Dim objParentCollection As IDatabaseObjects = pobjDatabaseObject.ParentCollection

            Dim objSelect As New SQL.SQLSelect
            objSelect.Fields.Add(objParentCollection.DistinctFieldName)
            objSelect.Tables.Add(objParentCollection.TableName)

            Dim objSubset As SQL.SQLConditions = objParentCollection.Subset
            If Not objSubset Is Nothing AndAlso Not objSubset.IsEmpty Then
                objSelect.Where.Add(objSubset)
            End If
            objSelect.Where.Add(objParentCollection.KeyFieldName, SQL.ComparisonOperator.EqualTo, value)

            'objFoundItem is nothing if the unique item is not found
            Dim objExistingObjectDistinctValue As Object = pobjDatabaseObject.ParentDatabase.Connection.ExecuteScalarWithConnect(objSelect)

            'If a value was not found in the database then the value is unique
            If objExistingObjectDistinctValue Is Nothing Then
                Return True
            Else
                'If a value was found in the database then it should only be for this object (i.e. this field was not changed)
                If DirectCast(pobjDatabaseObject, IDatabaseObject).IsSaved Then
                    Return objExistingObjectDistinctValue.Equals(DirectCast(pobjDatabaseObject, IDatabaseObject).DistinctValue)
                Else
                    'If value was found in the database but the object has not been saved then the item found cannot be for this object
                    'and therefore it is being used by another object
                    Return False
                End If
            End If

        End Function

    End Class

End Namespace
