using System;
using DatabaseObjects.SQL;
using DatabaseObjects.SQL.Serializers;

namespace DatabaseObjects
{
    public class SQLLiteralExpression : SQLExpression
    {
        private readonly string literalValue;

        public SQLLiteralExpression(string literalValue)
        {
            this.literalValue = literalValue;
        }

        internal override string SQL(Serializer serializer)
        {
            return literalValue;
        }
    }
}