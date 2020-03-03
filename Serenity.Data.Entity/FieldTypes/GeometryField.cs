using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

namespace Serenity.Data
{
    public class Geometry : MemoryStream
    {

        const int GEOMETRY_LENGTH = 25;
        public Geometry(byte[] val)
            : base(val)
        {
            if (val == null)
                throw new ArgumentNullException("val");

            byte[] buffValue = new byte[val.Length];

            for (int i = 0; i < val.Length; i++)
                buffValue[i] = val[i];

            var xIndex = val.Length == GEOMETRY_LENGTH ? 9 : 5;
            var yIndex = val.Length == GEOMETRY_LENGTH ? 17 : 13;

            var _valBinary = buffValue;
            longitude = BitConverter.ToDouble(val, xIndex);
            latitude = BitConverter.ToDouble(val, yIndex);
            this.srid = val.Length == GEOMETRY_LENGTH ? BitConverter.ToInt32(val, 0) : 0;
        }


        public Geometry(string data)
        {
            var p = data.Split(';');
            if (p != null && p.Length == 2)
            {
                longitude = Convert.ToDouble(p[0]);
                latitude = Convert.ToDouble(p[1]);
            }
            var buffer = new byte[GEOMETRY_LENGTH];

            byte[] sridBinary = BitConverter.GetBytes(srid);

            for (int i = 0; i < sridBinary.Length; i++)
                buffer[i] = sridBinary[i];

            long xVal = BitConverter.DoubleToInt64Bits(longitude);
            long yVal = BitConverter.DoubleToInt64Bits(latitude);

            buffer[4] = 1;
            buffer[5] = 1;

            for (int i = 0; i < 8; i++)
            {
                buffer[i + 9] = (byte)(xVal & 0xff);
                xVal >>= 8;
            }

            for (int i = 0; i < 8; i++)
            {
                buffer[i + 17] = (byte)(yVal & 0xff);
                yVal >>= 8;
            }
            this.Position = 0;
            this.Write(buffer, 0, 25);
        }


        
 

        public int srid { get; }
        public double latitude { get; } = 0;
        public double longitude { get; } = 0;

        public string ToJson()
        {
            return $"{longitude};{latitude}";
        }
    }

    public class GeometryField : GenericClassField<Geometry>
    {
        public GeometryField(ICollection<Field> collection, string name, LocalText caption = null, int size = 0, FieldFlags flags = FieldFlags.Default,
            Func<Row, Geometry> getValue = null, Action<Row, Geometry> setValue = null)
            : base(collection, FieldType.String, name, caption, size, flags, getValue, setValue)
        {
        }

        public static GeometryField Factory(ICollection<Field> collection, string name, LocalText caption, int size, FieldFlags flags,
            Func<Row, Geometry> getValue, Action<Row, Geometry> setValue)
        {
            return new GeometryField(collection, name, caption, size, flags, getValue, setValue);
        }

        public override void GetFromReader(IDataReader reader, int index, Row row)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            if (reader.IsDBNull(index))
                _setValue(row, null);
            else
            {
                byte[] a;
#if COREFX
                if (reader.GetType().Name == "SqliteDataReader")
                {
                    a = (byte[])reader.GetValue(index);
                }
                else if (reader.GetType().Name == "MySqlDataReader")
                {
                    a = (byte[])reader.GetValue(index);
                }
                else
                {
#endif
                    long available = reader.GetBytes(index, (long)0, null, 0, 0);
                    a = new byte[available];
                    if (a.Length > 0)
                        reader.GetBytes(index, (long)0, a, 0, a.Length);
#if COREFX
                }
#endif


                if (a.Length == 25)
                {
                    _setValue(row, new Geometry(a));
                }
                else
                {
                    var ms = new MemoryStream(a);
                    StreamReader sr = new StreamReader(ms);
                    var geoStr = sr.ReadToEnd();
                    _setValue(row, new Geometry(geoStr));
                }
            }

            if (row.tracking)
                row.FieldAssignedValue(this);

        }



        public override int IndexCompare(Row row1, Row row2)
        {
            var value1 = _getValue(row1);
            var value2 = _getValue(row2);

            bool null1 = value1 == null;
            bool null2 = value2 == null;
            if (null1 || null2)
            {
                if (null1 && null2)
                    return 0;
                else if (null1)
                    return -1;
                else
                    return 1;
            }
            else
                return value1.GetHashCode().CompareTo(value2.GetHashCode());
        }

        public override void ValueToJson(JsonWriter writer, Row row, JsonSerializer serializer)
        {
            var value = _getValue(row);
            if (value == null)
                writer.WriteNull();
            else
            {
                writer.WriteValue(value.ToJson());
            }
        }

        public override void ValueFromJson(JsonReader reader, Row row, JsonSerializer serializer)
        {
            if (reader == null)
                throw new ArgumentNullException("reader");

            switch (reader.TokenType)
            {
                case JsonToken.Null:
                case JsonToken.Undefined:
                    _setValue(row, null);
                    break;
                case JsonToken.String:
                case JsonToken.Integer:
                case JsonToken.Float:
                case JsonToken.Bytes:
                    _setValue(row, new Geometry((string)reader.Value));
                    break;
                default:
                    throw JsonUnexpectedToken(reader);
            }

            if (row.tracking)
                row.FieldAssignedValue(this);
        }
    }
}
