
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
''' The OrderByFieldAttribute should specify the field that is sorted and whether it
''' is ascending or descending. If the TableJoinAttribute has been specified then
''' the sort order can be specified on the joined table field.
''' To further customer the ordering (i.e. sort by more than one field)
''' simply override the OrderBy function and do not specify an OrderByAttribute.
''' Specifying this attribute is optional and if not specified the table is not
''' sorted.
''' </summary>
''' <example>
''' <code>
'''    &lt;OrderBy("CustomerName", SQL.OrderBy.Ascending)&gt;
'''    Public Class Customers
'''        ...
''' </code>
''' </example>
''' --------------------------------------------------------------------------------
<AttributeUsage(AttributeTargets.Class, AllowMultiple:=True, Inherited:=True)> _
Public Class OrderByFieldAttribute
    Inherits Attribute

    Private pstrOrderByFieldName As String
    Private peOrdering As SQL.OrderBy
    Private pintPrecendence As Integer

    ''' <summary>
    ''' Specifies the field to sort the table/collection by in ascending order.
    ''' If the TableJoinAttribute has been specified then the sort order can be 
    ''' specified on the joined table field.
    ''' </summary>
    Public Sub New(ByVal strOrderByFieldName As String)

        Me.New(strOrderByFieldName, SQL.OrderBy.Ascending)

    End Sub

    ''' <summary>
    ''' Specifies the field to sort the table/collection by in ascending or descending order.
    ''' If the TableJoinAttribute has been specified then the sort order can be 
    ''' specified on the joined table field.
    ''' </summary>
    Public Sub New(ByVal strOrderByFieldName As String, ByVal eOrder As SQL.OrderBy)

        Me.New(strOrderByFieldName, eOrder, 1)

    End Sub

    ''' <summary>
    ''' Specifies the field to sort the table/collection by in ascending or descending order.
    ''' If the TableJoinAttribute has been specified then the sort order can be 
    ''' specified on the joined table field.
    ''' </summary>
    ''' <param name="intPrecendence">
    ''' Indicates the order precendence level when multiple OrderByAttributes are specified.
    ''' OrderByAttributes are sorted for items with the lowest to the highest integer value.
    ''' </param>
    Public Sub New(ByVal strOrderByFieldName As String, ByVal eOrder As SQL.OrderBy, ByVal intPrecendence As Integer)

        If strOrderByFieldName = String.Empty Then
            Throw New ArgumentNullException
        End If

        pstrOrderByFieldName = strOrderByFieldName
        peOrdering = eOrder
        pintPrecendence = intPrecendence

    End Sub

    Public ReadOnly Property Name() As String
        Get

            Return pstrOrderByFieldName

        End Get
    End Property

    Public ReadOnly Property Ordering() As SQL.OrderBy
        Get

            Return peOrdering

        End Get
    End Property

    Public ReadOnly Property Precendence As Integer
        Get

            Return pintPrecendence

        End Get
    End Property

End Class
