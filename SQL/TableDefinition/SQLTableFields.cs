// ___________________________________________________
//
//  © Hi-Integrity Systems 2010. All rights reserved.
//  www.hisystems.com.au - Toby Wicks
// ___________________________________________________
//

using System.Collections;
using System;
using System.Data;
using System.Linq;
using System.Collections.Generic;
	
namespace DatabaseObjects.SQL
{
    public class SQLTableFields : IEnumerable<SQLTableFieldBase>
    {
        internal enum AlterModeType
        {
            /// <summary>
            /// Columns are being added or created
            /// </summary>
            /// <remarks></remarks>
            Add,

            /// <summary>
            /// Columns are being modified.
            /// </summary>
            /// <remarks></remarks>
            Modify,

            /// <summary>
            /// Columns are being dropped
            /// </summary>
            /// <remarks></remarks>
            Drop
        }

        private const int pcintAlterModeUninitialized = -1;

        internal AlterModeType AlterMode = (AlterModeType)pcintAlterModeUninitialized;
        private List<SQLTableFieldBase> pobjFields = new List<SQLTableFieldBase>();

        public SQLTableFields()
        {
        }

        public SQLTableField Add()
        {
            return Add("", DataType.VariableCharacter, 255);
        }

        public SQLTableField Add(string strFieldName, SQL.DataType eDataType)
        {
            return this.Add(strFieldName, eDataType, 0);
        }

        public SQLTableField Add(string strFieldName, SQL.DataType eDataType, int intSize)
        {
            EnsureAlterModeValid(AlterModeType.Add);

            SQLTableField objField = new SQLTableField();

            objField.Name = strFieldName;
            objField.DataType = eDataType;
            if (intSize > 0)
                objField.Size = intSize;

            if (Misc.DataTypeIsCharacter(eDataType) && intSize == 0)
                throw new ArgumentException("Size not specified for character based field " + strFieldName);

            pobjFields.Add(objField);

            return objField;
        }

        public SQLTableFieldComputed AddComputed(string strFieldName, SQLExpression objComputation)
        {
            EnsureAlterModeValid(AlterModeType.Add);

            SQLTableFieldComputed objField = new SQLTableFieldComputed(strFieldName, objComputation);

            pobjFields.Add(objField);

            return objField;
        }

        public void Add(SQLTableField objField)
		{
			if (objField == null)
				throw new ArgumentNullException();
				
			EnsureAlterModeValid(AlterModeType.Add);
				
			pobjFields.Add(objField);
		}

        public SQLTableField this[string strFieldName]
        {
            get
            {
                EnsureAlterModeValid(AlterModeType.Modify);

                SQLTableField tableField;
                var intIndex = FieldNameIndex(strFieldName);

                if (intIndex == -1)
                {
                    tableField = new SQLTableField();
                    tableField.Name = strFieldName;
                    pobjFields.Add(tableField);
                }
                else
                    tableField = (SQLTableField)(pobjFields[intIndex]);

                return tableField;
            }
        }

        public void Drop(string strFieldName)
        {
            EnsureAlterModeValid(AlterModeType.Drop);

            SQLTableField objField = new SQLTableField();

            objField.Name = strFieldName;

            if (FieldNameIndex(strFieldName) == -1)
                pobjFields.Add(objField);
            else
                throw new ArgumentException("Field '" + strFieldName + "' already exists");
        }

        private int FieldNameIndex(string strFieldName)
        {
            for (int intIndex = 0; intIndex < pobjFields.Count; intIndex++)
            {
                if (string.Compare(((SQLTableField)(pobjFields[intIndex])).Name, strFieldName, true) == 0)
                    return intIndex;
            }

            return -1;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="eConnectionType"></param>
        /// <param name="bIncludeColumnModifier">
        /// Indicates whether the ADD, MODIFY or DROP modifiers are required for each column.
        /// When utilised from SQLCreateTable this will always be false. However, for SQLAlterTable this will be true.</param>
        /// <returns></returns>
        /// <remarks></remarks>
        internal string SQL(Database.ConnectionType eConnectionType, bool bIncludeColumnModifier)
        {
            const string cstrSeperator = ", ";

            string strSQL = string.Empty;
            string strColumnModifier = string.Empty;

            //Include mode when altering a table, otherwise when creating a table the mode is not required.
            if (bIncludeColumnModifier)
            {
                strColumnModifier = GetAlterModeColumnModifier(AlterMode, eConnectionType);

                //This case statement is related to the if statement below with the mode space char being added for MySQL
                if (eConnectionType != Database.ConnectionType.MySQL)
                    strSQL = strColumnModifier + " ";
            }

            foreach (SQLTableFieldBase objField in pobjFields)
            {
                if (bIncludeColumnModifier)
                {
                    if (eConnectionType == Database.ConnectionType.MySQL)
                        strSQL += strColumnModifier + " ";
                }

                strSQL += objField.SQL(eConnectionType, (AlterMode == AlterModeType.Drop)) + cstrSeperator;
            }

            return strSQL.Substring(0, strSQL.Length - cstrSeperator.Length); //remove the last comma and space
        }

        private static string GetAlterModeColumnModifier(AlterModeType eAlterMode, Database.ConnectionType eConnectionType)
		{
			switch (eAlterMode)
			{
				case AlterModeType.Add:
					return "ADD";
				case AlterModeType.Modify:
					switch (eConnectionType)
					{
						case Database.ConnectionType.MySQL:
						case Database.ConnectionType.Pervasive:
							return "MODIFY COLUMN";
						case Database.ConnectionType.MicrosoftAccess:
						case Database.ConnectionType.SQLServer:
						case Database.ConnectionType.SQLServerCompactEdition:
						case Database.ConnectionType.HyperSQL:
							return "ALTER COLUMN";
						default:
							throw new NotImplementedException(eConnectionType.ToString());
							break;
					}
					break;
				case AlterModeType.Drop:
					return "DROP COLUMN";
				default:
					throw new NotImplementedException();
					break;
			}
		}

        private void EnsureAlterModeValid(SQLTableFields.AlterModeType eAlterMode)
        {
            //if the alter mode hasn't been set then any of the modes are valid
            if ((int)AlterMode == pcintAlterModeUninitialized)
                AlterMode = eAlterMode;
            else if (eAlterMode != AlterMode)
                throw new Exceptions.DatabaseObjectsException("Cannot mix " + AlterModeDescription(AlterMode) + " fields and " + AlterModeDescription(eAlterMode) + " fields into one SQL statement");
        }

        private string AlterModeDescription(SQLTableFields.AlterModeType eAlterMode)
		{
			switch (eAlterMode)
			{
				case AlterModeType.Add:
					return "adding";
				case AlterModeType.Modify:
					return "altering";
				case AlterModeType.Drop:
					return "dropping";
				default:
					throw new NotSupportedException();
			}
		}

        IEnumerator<SQLTableFieldBase> IEnumerable<SQLTableFieldBase>.GetEnumerator()
        {
            return pobjFields.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return pobjFields.GetEnumerator();
        }
    }

    public abstract class SQLTableFieldBase
    {
        protected internal abstract string SQL(Database.ConnectionType eConnectionType, bool bOnlyFieldName);

        private SQLFieldExpression pobjNameAsExpression = new SQLFieldExpression();

        public string Name
        {
            get
            {
                return pobjNameAsExpression.Name;
            }

            set
			{
				if (string.IsNullOrEmpty(value))
					throw new ArgumentNullException();
						
				pobjNameAsExpression.Name = value;
			}
        }

        protected SQLExpression NameAsExpression
        {
            get
            {
                return pobjNameAsExpression;
            }
        }
    }

    public class SQLTableFieldComputed : SQLTableFieldBase
    {
        private SQLExpression pobjComputation;

        public SQLTableFieldComputed(string strFieldName, SQLExpression objComputation)
		{
			if (objComputation == null)
				throw new ArgumentNullException();

            base.Name = strFieldName;
					
			pobjComputation = objComputation;
		}

        protected internal override string SQL(Database.ConnectionType eConnectionType, bool bOnlyFieldName)
        {
            if (bOnlyFieldName)
                throw new InvalidOperationException("Computed columns cannot be used for dropping fields");

            return base.NameAsExpression.SQL(eConnectionType) + " AS (" + pobjComputation.SQL(eConnectionType) + ")";
        }
    }

    /// <summary>
    /// Represents a new database field to be created or added to a database table.
    /// </summary>
    /// <remarks></remarks>
    public class SQLTableField : SQLTableFieldBase
    {
        private SQL.DataType peType = DataType.VariableCharacter;
        private int pintSize = 1;
        private int pintScale = 0;
        private int pintPrecision = 18;
        private bool pbAutoIncrements;
        private bool pbAcceptsNull = true;
        private object pobjDefault;
        private SQL.KeyType peKeyType = KeyType.None;

        public SQLTableField()
        {
        }

        public SQL.DataType DataType
        {
            get
            {
                return peType;
            }

            set
            {
                if (pbAutoIncrements && !Misc.DataTypeIsInteger(value))
                    throw new InvalidOperationException("Data type " + value.ToString() + " cannot be used as an autoincrement field, as it is not an integer field");

                peType = value;
            }
        }

        public SQL.KeyType KeyType
        {
            get
            {
                return peKeyType;
            }

            set
            {
                peKeyType = value;

                if (value == KeyType.Primary)
                    pbAcceptsNull = false;
            }
        }

        public int Size
        {
            set
			{
				Misc.DataTypeEnsureIsCharacter(peType);
						
				if (value <= 1)
					throw new ArgumentException();
						
				pintSize = value;
			}

            get
            {
                Misc.DataTypeEnsureIsCharacter(peType);

                return pintSize;
            }
        }

        /// <summary>
        /// Sets or returns the scale of the decimal number.
        /// This is the location within the number where the decimal is placed.
        /// The default is 0.
        /// Throws an exception if the data type is not SQl.DataType.Decimal.
        /// </summary>
        public int ScaleLength
        {
            get
            {
                Misc.DataTypeEnsureIsDecimal(peType);

                return pintScale;
            }

            set
			{
				Misc.DataTypeEnsureIsDecimal(peType);
						
				if (value <= 0)
					throw new ArgumentException();
						
				pintScale = value;
			}
        }

        /// <summary>
        /// Sets or returns the precision of the decimal number.
        /// This is the number of number characters that are stored.
        /// The default is 18 precision and 0 scale.
        /// Throws an exception if the data type is not SQl.DataType.Decimal.
        /// </summary>
        public int Precision
        {
            get
            {
                Misc.DataTypeEnsureIsDecimal(peType);

                return pintPrecision;
            }

            set
			{
				Misc.DataTypeEnsureIsDecimal(peType);
						
				if (value <= 0)
					throw new ArgumentException();
						
				pintPrecision = value;
			}
        }

        public bool AutoIncrements
        {
            get
            {
                Misc.DataTypeEnsureIsInteger(peType);

                return pbAutoIncrements;
            }

            set
            {
                Misc.DataTypeEnsureIsInteger(peType);
                pbAutoIncrements = value;
                pbAcceptsNull = System.Convert.ToBoolean(!value);
            }
        }

        public bool AcceptsNull
        {
            get
            {
                return pbAcceptsNull;
            }

            set
            {
                pbAcceptsNull = value;
            }
        }

        public object Default
        {
            get
            {
                return pobjDefault;
            }

            set
            {
                if (peType == DataType.UniqueIdentifier)
                    throw new InvalidOperationException("Cannot set the default value when the data type is of type DatabaseObjects.SQL.DataType.UniqueIdentifier");

                pobjDefault = value;
            }
        }

        protected internal override string SQL(Database.ConnectionType eConnectionType, bool bOnlyFieldName)
        {
            if (bOnlyFieldName)
                return base.NameAsExpression.SQL(eConnectionType);
            else
                return SQLForColumnAddOrModify(eConnectionType);
        }

        private string SQLForColumnAddOrModify(Database.ConnectionType eConnectionType)
        {
            string strDataType = string.Empty;
            string strColumnOptions;
            string strSQL = string.Empty;

            //For Pervasive do not specify NULL/NOT NULL and the data type for an IDENTITY field
            bool bSpecifyNullStatus = !(pbAutoIncrements && (eConnectionType == Database.ConnectionType.Pervasive));
            bool bSpecifyDataType = !(pbAutoIncrements && (eConnectionType == Database.ConnectionType.Pervasive));

            if (bSpecifyDataType)
                strDataType = Misc.SQLConvertDataTypeString(eConnectionType, peType, pintSize, pintPrecision, pintScale);

            strColumnOptions = ColumnOptions(eConnectionType, bSpecifyNullStatus);
            strSQL = base.NameAsExpression.SQL(eConnectionType) + " " + strDataType + strColumnOptions;

            return strSQL;
        }

        private string ColumnOptions(Database.ConnectionType eConnection, bool bSpecifyNullStatus)
        {
            ArrayList objOptions = new ArrayList();
            string strOptions = string.Empty;

            //In version 2.13+ the IDENTITY constraint has been reordered
            //to before the contraints (i.e. DEFAULT, NULL etc.)
            if (Misc.DataTypeIsInteger(peType) && pbAutoIncrements)
            {
                switch (eConnection)
                {
                    case Database.ConnectionType.MicrosoftAccess:
                        objOptions.Add("IDENTITY");
                        break;
                    case Database.ConnectionType.MySQL:
                        objOptions.Add("AUTO_INCREMENT");
                        //must be set to a key
                        if (peKeyType == KeyType.None)
                            peKeyType = KeyType.Unique;
                        break;
                    case Database.ConnectionType.SQLServer:
                    case Database.ConnectionType.SQLServerCompactEdition:
                        objOptions.Add("IDENTITY");
                        break;
                    case Database.ConnectionType.Pervasive:
                        objOptions.Add("IDENTITY");
                        break;
                    case Database.ConnectionType.HyperSQL:
                        objOptions.Add("GENERATED BY DEFAULT AS IDENTITY(START WITH 1 INCREMENT BY 1)");
                        break;
                    default:
                        throw new NotImplementedException(eConnection.ToString() + "; Auto Increment");
                        break;
                }
            }

            if (pobjDefault != null)
                objOptions.Add("DEFAULT " + Misc.SQLConvertValue(pobjDefault, eConnection));

            //NULL status MUST be after the default value for Pervasive
            //In version 2.13+ the NULL/NOT NULL constraint has been reordered
            //to after the DEFAULT constraint. Hopefully, this won't cause any problems.
            //According to the T-SQL docs it should not be a problem.
            if (bSpecifyNullStatus)
            {
                if (pbAcceptsNull)
                    objOptions.Add("NULL");
                else
                    objOptions.Add("NOT NULL");
            }

            if (peKeyType == KeyType.Primary)
                objOptions.Add("PRIMARY KEY");
            else if (peKeyType == KeyType.Unique)
                objOptions.Add("UNIQUE");

            foreach (object objOption in objOptions)
                strOptions += " " + ((string)objOption);

            return strOptions;
        }
    }
}
