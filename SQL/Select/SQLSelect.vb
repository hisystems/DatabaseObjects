
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLSelect
        Inherits SQLStatement

        Private pobjFields As SQLSelectFields = New SQLSelectFields
        Private pobjTables As SQLSelectTables = New SQLSelectTables
        Private pobjConditions As SQLConditions = New SQLConditions
        Private pobjHavingConditions As SQLSelectHavingConditions = New SQLSelectHavingConditions
        Private pobjOrderByFields As SQLSelectOrderByFields = New SQLSelectOrderByFields
        Private pobjGroupByFields As SQLSelectGroupByFields = New SQLSelectGroupByFields
        Private pbDistinct As Boolean = False
        Private pintTop As Integer
        Private pbPerformLocking As Boolean = False

        Public Sub New()

        End Sub

        Public Sub New(ByVal strTableName As String)

            pobjTables.Add(strTableName)

        End Sub

        Public Sub New(ByVal strTableName As String, ByVal objWhereCondition As SQLCondition)

            Me.Tables.Add(strTableName)
            Me.Where.Add(objWhereCondition)

        End Sub

        Public Property Distinct() As Boolean
            Get

                Return pbDistinct

            End Get

            Set(ByVal Value As Boolean)

                pbDistinct = Value

            End Set
        End Property

        Public Property Top() As Integer
            Get

                Return pintTop

            End Get

            Set(ByVal Value As Integer)

                If Value < 0 Then
                    Throw New ArgumentException
                End If

                pintTop = Value

            End Set
        End Property

        ''' <summary>
        ''' Indicates whether the rows that are selected are locked for reading and updating.
        ''' Equivalent to Serialiazable isolation level.
        ''' These rows cannot be read or updated until the lock is released.
        ''' Locks are released when the transaction has been committed or rolled back.
        ''' </summary>
        Public Property PerformLocking() As Boolean
            Get

                Return pbPerformLocking

            End Get

            Set(ByVal value As Boolean)

                pbPerformLocking = value

            End Set
        End Property

        Public Property Tables() As SQLSelectTables
            Get

                Return pobjTables

            End Get

            Set(ByVal Value As SQLSelectTables)

                If Value Is Nothing Then
                    Throw New ArgumentNullException
                End If

                pobjTables = Value

            End Set
        End Property

        Public Property Fields() As SQLSelectFields
            Get

                Return pobjFields

            End Get

            Set(ByVal Value As SQLSelectFields)

                If Value Is Nothing Then
                    Throw New ArgumentNullException
                End If

                pobjFields = Value

            End Set
        End Property

        Public Property Where() As SQLConditions
            Get

                Return pobjConditions

            End Get

            Set(ByVal Value As SQLConditions)

                pobjConditions = Value

            End Set
        End Property

        Public Property Having() As SQLSelectHavingConditions
            Get

                Return pobjHavingConditions

            End Get

            Set(ByVal Value As SQLSelectHavingConditions)

                pobjHavingConditions = Value

            End Set
        End Property

        Public Property OrderBy() As SQLSelectOrderByFields
            Get

                Return pobjOrderByFields

            End Get

            Set(ByVal Value As SQLSelectOrderByFields)

                pobjOrderByFields = Value

            End Set
        End Property

        Public Property GroupBy() As SQLSelectGroupByFields
            Get

                Return pobjGroupByFields

            End Get

            Set(ByVal Value As SQLSelectGroupByFields)

                pobjGroupByFields = Value

            End Set
        End Property

        Public Overrides ReadOnly Property SQL() As String
            Get

                Dim strSQL As String

                If pobjTables.Count = 0 Then
                    Throw New Exceptions.DatabaseObjectsException("The table has not been set.")
                End If

                strSQL = _
                    "SELECT " & DistinctClause() & TopClause() & pobjFields.SQL(Me.ConnectionType) & _
                    " FROM " & pobjTables.SQL(Me.ConnectionType)

                If pbPerformLocking Then
                    Select Case Me.ConnectionType
                        Case Database.ConnectionType.SQLServer, _
                             Database.ConnectionType.SQLServerCompactEdition
                            strSQL &= " WITH (HOLDLOCK, ROWLOCK)"
                        Case Database.ConnectionType.MySQL, _
                             Database.ConnectionType.Pervasive
                            'Done below
                        Case Database.ConnectionType.HyperSQL, _
                             Database.ConnectionType.MicrosoftAccess
                            Throw New NotSupportedException("Locking is not supported for " & Me.ConnectionType.ToString)
                        Case Else
                            Throw New NotImplementedException(Me.ConnectionType.ToString)
                    End Select
                End If

                If Not pobjConditions Is Nothing AndAlso Not pobjConditions.IsEmpty Then
                    strSQL &= " WHERE " & pobjConditions.SQL(Me.ConnectionType)
                End If

                If Not pobjGroupByFields Is Nothing AndAlso Not pobjGroupByFields.IsEmpty Then
                    strSQL &= " GROUP BY " & pobjGroupByFields.SQL(Me.ConnectionType)
                End If

                If Not pobjOrderByFields Is Nothing AndAlso Not pobjOrderByFields.IsEmpty Then
                    strSQL &= " ORDER BY " & pobjOrderByFields.SQL(Me.ConnectionType)
                End If

                If Not pobjHavingConditions Is Nothing AndAlso Not pobjHavingConditions.IsEmpty Then
                    strSQL &= " HAVING " & pobjHavingConditions.SQL(Me.ConnectionType)
                End If

                If pbPerformLocking Then
                    Select Case Me.ConnectionType
                        Case Database.ConnectionType.MySQL, _
                             Database.ConnectionType.Pervasive
                            strSQL &= " FOR UPDATE"
                    End Select
                End If

                Return strSQL

            End Get
        End Property

        Private Function TopClause() As String

            If pintTop > 0 Then
                If MyBase.ConnectionType = Database.ConnectionType.SQLServerCompactEdition Then
                    Return "TOP(" & pintTop & ") "
                Else
                    Return "TOP " & pintTop & " "
                End If
            Else
                Return String.Empty
            End If

        End Function

        Private Function DistinctClause() As String

            If pbDistinct Then
                Return "DISTINCT "
            Else
                Return String.Empty
            End If

        End Function

    End Class

End Namespace
