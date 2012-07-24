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

            if (DataTypeExtensions.IsCharacter(eDataType) && intSize == 0)
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

				var tableField = GetTableFieldOrDefault(strFieldName);

				if (tableField == null)
                {
                    tableField = new SQLTableField();
                    tableField.Name = strFieldName;
                    pobjFields.Add(tableField);
                }

                return tableField;
            }
        }

        public void Drop(string strFieldName)
        {
			EnsureAlterModeValid(AlterModeType.Drop);

            SQLTableField objField = new SQLTableField();

            objField.Name = strFieldName;

			if (GetTableFieldOrDefault(strFieldName) != null)
				throw new ArgumentException("Field '" + strFieldName + "' already exists");
			
            pobjFields.Add(objField);
        }
		
		private SQLTableField GetTableFieldOrDefault(string strFieldName)
		{
			return pobjFields.Where(field => field is SQLTableField).Cast<SQLTableField>().SingleOrDefault(field => Equals(field, strFieldName));
		}

        private bool Equals(SQLTableField tableField, string strFieldName)
        {
			return tableField.Name.Equals(strFieldName, StringComparison.InvariantCultureIgnoreCase);
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
		internal abstract string SQL(Serializers.Serializer serializer, SQLTableFields.AlterModeType alterMode);

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

        internal SQLExpression NameAsExpression
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

		public SQLExpression Computation
		{
			get
			{
				return pobjComputation;
			}
		}

		internal override string SQL(Serializers.Serializer serializer, SQLTableFields.AlterModeType alterMode)
        {
			return serializer.SerializeTableFieldComputed(this, alterMode);
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
                if (pbAutoIncrements && !DataTypeExtensions.IsInteger(value))
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
				DataTypeExtensions.EnsureIsCharacter(peType);
						
				if (value <= 1)
					throw new ArgumentException();
						
				pintSize = value;
			}

            get
            {
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
                return pintScale;
            }

            set
			{
				DataTypeExtensions.EnsureIsDecimal(peType);
						
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
                return pintPrecision;
            }

            set
			{
				DataTypeExtensions.EnsureIsDecimal(peType);
						
				if (value <= 0)
					throw new ArgumentException();
						
				pintPrecision = value;
			}
        }

        public bool AutoIncrements
        {
            get
            {
                return pbAutoIncrements;
            }

            set
            {
                DataTypeExtensions.EnsureIsInteger(peType);
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

		internal override string SQL(Serializers.Serializer serializer, SQLTableFields.AlterModeType alterMode)
		{
			return serializer.SerializeTableField(this, alterMode);
		}
    }
}
