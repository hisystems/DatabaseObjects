
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On

Namespace SQL

    Public Class SQLCastExpression
        Inherits SQLExpression

        Private pstrFieldName As String
        Private peCastAsType As DataType
        Private pintPrecision As Integer
        Private pintScale As Integer
        Private pintSize As Integer

        Public Sub New(ByVal strFieldName As String, ByVal eCastAsType As DataType)

            If strFieldName = String.Empty Then
                Throw New ArgumentNullException
            End If

            pstrFieldName = strFieldName
            peCastAsType = eCastAsType

        End Sub

        Public Sub New(ByVal strFieldName As String, ByVal eCastAsType As DataType, ByVal intSize As Integer)

            Me.New(strFieldName, eCastAsType)
            Me.Size = intSize

        End Sub

        ''' <summary>
        ''' The size of the character data type.
        ''' </summary>
        Public Property Size() As Integer
            Set(ByVal Value As Integer)

                Misc.DataTypeEnsureIsCharacter(peCastAsType)

                If Value <= 1 Then
                    Throw New ArgumentException
                End If

                pintSize = Value

            End Set

            Get

                Misc.DataTypeEnsureIsCharacter(peCastAsType)
                Return pintSize

            End Get
        End Property

        ''' <summary>
        ''' Sets or returns the scale of the decimal number.
        ''' This is the location within the number where the decimal is placed.
        ''' The default is 0. 
        ''' Throws an exception if the data type is not SQL.DataType.Decimal.
        ''' </summary>
        Public Property ScaleLength() As Integer
            Get

                DataTypeEnsureIsDecimal(peCastAsType)
                Return pintScale

            End Get

            Set(ByVal Value As Integer)

                DataTypeEnsureIsDecimal(peCastAsType)

                If Value <= 0 Then
                    Throw New ArgumentException
                End If

                pintScale = Value

            End Set
        End Property

        ''' <summary>
        ''' Sets or returns the precision of the decimal number.
        ''' This is the number of number characters that are stored.
        ''' The default is 18 precision and 0 scale.
        ''' Throws an exception if the data type is not SQL.DataType.Decimal.
        ''' </summary>
        Public Property Precision() As Integer
            Get

                DataTypeEnsureIsDecimal(peCastAsType)
                Return pintPrecision

            End Get

            Set(ByVal Value As Integer)

                DataTypeEnsureIsDecimal(peCastAsType)

                If Value <= 0 Then
                    Throw New ArgumentException
                End If

                pintPrecision = Value

            End Set
        End Property

        Friend Overrides Function SQL(ByVal eConnectionType As Database.ConnectionType) As String

            Return "CAST(" & Misc.SQLConvertIdentifierName(pstrFieldName, eConnectionType) & " AS " & Misc.SQLConvertDataTypeString(eConnectionType, peCastAsType, pintSize, pintPrecision, pintScale) & ")"

        End Function

    End Class

End Namespace
