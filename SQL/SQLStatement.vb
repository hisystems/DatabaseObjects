
' ___________________________________________________
'
'  © Hi-Integrity Systems 2010. All rights reserved.
'  www.hisystems.com.au - Toby Wicks
' ___________________________________________________
'

Option Strict On
Option Explicit On 

Namespace SQL

    Public MustInherit Class SQLStatement
        Implements ISQLStatement

        Private Shared peDefaultConnectionType As Database.ConnectionType

        ''' --------------------------------------------------------------------------------
        ''' <summary>
        ''' This is used as the default connection type when manually instantiating an 
        ''' SQLSelect, SQLDelete, SQLUpdate or SQLInsert command (which all inherit from 
        ''' SQLStatement) and is set by the last Database.Connect function's ConnectionType 
        ''' argument. However, the Database class does not rely on this property as the 
        ''' ConnectionType property is set before any SQL statements are executed. This 
        ''' allows different Database instances to use different databases.
        ''' </summary>
        ''' --------------------------------------------------------------------------------
        Public Shared Property DefaultConnectionType() As Database.ConnectionType
            Get

                Return peDefaultConnectionType

            End Get

            Set(ByVal Value As Database.ConnectionType)

                peDefaultConnectionType = Value

            End Set
        End Property

        Private peConnectionType As Database.ConnectionType = DefaultConnectionType

        Public Property ConnectionType() As Database.ConnectionType Implements ISQLStatement.ConnectionType
            Get

                Return peConnectionType

            End Get

            Set(ByVal Value As Database.ConnectionType)

                peConnectionType = Value

            End Set
        End Property

        Public MustOverride ReadOnly Property SQL() As String Implements ISQLStatement.SQL

        Public Overrides Function ToString() As String

            Return Me.SQL

        End Function

    End Class

End Namespace
