using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using CodeTest.NET_Application.Data.Models;

namespace CodeTest.NET_Application.Business.Services
{
    public class CsvParser
    {
        private Dictionary<string, string> _map;
        private int _startRowOffset;
        private int _startColumnOffset;
        private int _endRowOffset;
        private char _delimeter;

        public CsvParser()
        {
            _map = null;
            _startRowOffset = 0;
            _startColumnOffset = 0;
            _endRowOffset = 0;
            _delimeter = ',';
        }
        public CsvParser(Dictionary<string, string> map = null, int startRowOffset = 0, int startColumnOffset = 0, int endRowOffset = 0, char delimeter = ',')
        {
            _map = map;
            _startRowOffset = startRowOffset;
            _startColumnOffset = startColumnOffset;
            _endRowOffset = endRowOffset;
            _delimeter = delimeter;
        }

        public IEnumerable<TEntity> ReadFromStream<TEntity>(Stream stream) where TEntity : class, IEntity, new()
        {
            var convertDateTime = new Func<double, DateTime>(csvDate =>
            {
                if (csvDate < 1)
                    throw new ArgumentException("CSV dates cannot be smaller than 0.");
                var dateOfReference = new DateTime(1900, 1, 1);
                if (csvDate > 60d)
                    csvDate = csvDate - 2;
                else
                    csvDate = csvDate - 1;
                return dateOfReference.AddDays(csvDate);
            });

            using (var sr = new StreamReader(stream))
            {
                var data = sr.ReadToEnd();
                var lines = data.Split(new char[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries)
                    .Skip(_startRowOffset);
                var props = typeof(TEntity).GetProperties()
                    .Select(prop =>
                    {
                        var displayAttribute =
                            (DisplayAttribute)prop.GetCustomAttributes(typeof(DisplayAttribute), false)
                                .FirstOrDefault();
                        return new
                        {
                            Name = prop.Name,
                            DisplayName = displayAttribute?.Name ?? prop.Name,
                            Order = displayAttribute == null || !displayAttribute.GetOrder().HasValue
                                ? 999
                                : displayAttribute.Order,
                            PropertyInfo = prop,
                            PropertyType = prop.PropertyType,
                            HasDisplayName = displayAttribute != null
                        };
                    })
                    .Where(prop => !string.IsNullOrWhiteSpace(prop.DisplayName))
                    .ToList();
                var retList = new List<TEntity>();
                var columns = new List<CsvMap>();
                var startCol = _startColumnOffset;
                var startRow = _startRowOffset;
                var headerRow = lines.ElementAt(startRow).Split(_delimeter);
                var endCol = headerRow.Length;
                var endRow = lines.Count();
                // Assume first row has column names
                for (int col = startCol; col < endCol; col++)
                {
                    var cellValue = (lines.ElementAt(startRow).Split(_delimeter)[col] ?? string.Empty).ToString().Trim();
                    if (!string.IsNullOrWhiteSpace(cellValue))
                    {
                        columns.Add(new CsvMap()
                        {
                            Name = cellValue,
                            MappedTo = _map == null || _map.Count == 0 ? cellValue :
                                _map.ContainsKey(cellValue) ? _map[cellValue] : string.Empty,
                            Index = col
                        });
                    }
                }

                // Now iterate over all the rows
                for (int rowIndex = startRow + 1; rowIndex < endRow; rowIndex++)
                {
                    //TEntity item;
                    var item = new TEntity();
                    columns.ForEach(column =>
                    {
                        var value = lines.ElementAt(rowIndex).Split(_delimeter)[column.Index];
                        var valueStr = value == null ? string.Empty : value.ToString().Trim();
                        var prop = string.IsNullOrWhiteSpace(column.MappedTo)
                            ? null
                            : props.FirstOrDefault(p => p.Name.Trim().Contains(column.MappedTo));
                        // Handle mapping by DisplayName
                        if (prop == null && !string.IsNullOrWhiteSpace(column.MappedTo))
                        {
                            prop = props.FirstOrDefault(p =>
                                p.HasDisplayName && p.DisplayName.Trim().Contains(column.MappedTo));
                        }

                        // Excel stores all numbers as doubles, but we're relying on the object's property types
                        if (prop != null)
                        {
                            var propertyType = prop.PropertyType;
                            object parsedValue = null;
                            if (propertyType == typeof(int?) || propertyType == typeof(int))
                            {
                                int val;
                                if (!int.TryParse(valueStr, out val))
                                {
                                    val = default(int);
                                }

                                parsedValue = val;
                            }
                            else if (propertyType == typeof(short?) || propertyType == typeof(short))
                            {
                                short val;
                                if (!short.TryParse(valueStr, out val))
                                    val = default(short);
                                parsedValue = val;
                            }
                            else if (propertyType == typeof(long?) || propertyType == typeof(long))
                            {
                                long val;
                                if (!long.TryParse(valueStr, out val))
                                    val = default(long);
                                parsedValue = val;
                            }
                            else if (propertyType == typeof(decimal?) || propertyType == typeof(decimal))
                            {
                                decimal val;
                                if (!decimal.TryParse(valueStr, out val))
                                    val = default(decimal);
                                parsedValue = val;
                            }
                            else if (propertyType == typeof(double?) || propertyType == typeof(double))
                            {
                                double val;
                                if (!double.TryParse(valueStr, out val))
                                    val = default(double);
                                parsedValue = val;
                            }
                            else if (propertyType == typeof(DateTime?) || propertyType == typeof(DateTime))
                            {
                                if (value is DateTime)
                                {
                                    parsedValue = value;
                                }
                                else
                                {
                                    try
                                    {
                                        DateTime output;
                                        if (DateTime.TryParse(value, out output))
                                        {
                                            parsedValue = output;
                                        }
                                        else
                                        {
                                            parsedValue = convertDateTime(Double.Parse(value));
                                        }
                                    }
                                    catch
                                    {
                                        if (propertyType == typeof(DateTime))
                                        {
                                            parsedValue = DateTime.MinValue;
                                        }
                                    }
                                }
                            }
                            else if (propertyType.IsEnum)
                            {
                                try
                                {
                                    parsedValue = Enum.ToObject(propertyType, int.Parse(valueStr));
                                }
                                catch
                                {
                                    parsedValue = Enum.ToObject(propertyType, 0);
                                }
                            }
                            else if (propertyType == typeof(string))
                            {
                                parsedValue = valueStr;
                            }
                            else
                            {
                                try
                                {
                                    parsedValue = Convert.ChangeType(value, propertyType);
                                }
                                catch
                                {
                                    parsedValue = valueStr;
                                }
                            }

                            try
                            {
                                //retList.Add((TEntity)parsedValue);
                                prop.PropertyInfo.SetValue(item, parsedValue);
                            }
                            catch (Exception ex)
                            {
                                // Indicate parsing error on row?
                            }
                        }
                    });
                    retList.Add(item);
                }

                return retList;
            }
        }

        private Tuple<T, IEnumerable<T>> HeadAndTail<T>(IEnumerable<T> source)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            var en = source.GetEnumerator();
            en.MoveNext();
            return Tuple.Create(en.Current, EnumerateTail(en));
        }

        private IEnumerable<T> EnumerateTail<T>(IEnumerator<T> en)
        {
            while (en.MoveNext()) yield return en.Current;
        }

        public IEnumerable<IList<string>> Parse(string content, char delimiter, char qualifier)
        {
            var reader = new StringReader(content);
            return Parse(reader, delimiter, qualifier);

        }

        public Tuple<IList<string>, IEnumerable<IList<string>>> ParseHeadAndTail(TextReader reader, char delimiter, char qualifier)
        {
            return HeadAndTail(Parse(reader, delimiter, qualifier));
        }

        public IEnumerable<IList<string>> Parse(TextReader reader, char delimiter, char qualifier)
        {
            var inQuote = false;
            var record = new List<string>();
            var sb = new StringBuilder();

            //reader.
            while (reader.Peek() != -1)
            {
                var readChar = (char)reader.Read();

                if (readChar == '\n' || (readChar == '\r' && (char)reader.Peek() == '\n'))
                {
                    // If it's a \r\n combo consume the \n part and throw it away.
                    if (readChar == '\r')
                        reader.Read();

                    if (inQuote)
                    {
                        if (readChar == '\r')
                            sb.Append('\r');
                        sb.Append('\n');
                    }
                    else
                    {
                        if (record.Count > 0 || sb.Length > 0)
                        {
                            record.Add(sb.ToString());
                            sb.Clear();
                        }

                        if (record.Count > 0)
                            yield return record;

                        record = new List<string>(record.Count);
                    }
                }
                else if (sb.Length == 0 && !inQuote)
                {
                    if (readChar == qualifier)
                        inQuote = true;
                    else if (readChar == delimiter)
                    {
                        record.Add(sb.ToString());
                        sb.Clear();
                    }
                    else if (char.IsWhiteSpace(readChar))
                    {
                        // Ignore leading whitespace
                    }
                    else
                        sb.Append(readChar);
                }
                else if (readChar == delimiter)
                {
                    if (inQuote)
                        sb.Append(delimiter);
                    else
                    {
                        record.Add(sb.ToString());
                        sb.Clear();
                    }
                }
                else if (readChar == qualifier)
                {
                    if (inQuote)
                    {
                        if ((char)reader.Peek() == qualifier)
                        {
                            reader.Read();
                            sb.Append(qualifier);
                        }
                        else
                            inQuote = false;
                    }
                    else
                        sb.Append(readChar);
                }
                else
                    sb.Append(readChar);
            }

            if (record.Count > 0 || sb.Length > 0)
                record.Add(sb.ToString());

            if (record.Count > 0)
                yield return record;
        }

        public Encoding GetEncoding(string filename)
        {

            var textDetect = new TextEncodingDetect();
            var bytes = File.ReadAllBytes(filename);
            var encoding = textDetect.DetectEncoding(bytes, bytes.Length);

            Console.Write("Encoding: ");
            if (encoding == TextEncodingDetect.Encoding.None)
            {
                Console.WriteLine("Binary");
                return Encoding.Default;
            }
            if (encoding == TextEncodingDetect.Encoding.Ascii)
            {
                Console.WriteLine("ASCII (chars in the 0-127 range)");
                return Encoding.ASCII;
            }
            if (encoding == TextEncodingDetect.Encoding.Ansi)
            {
                Console.WriteLine("ANSI (chars in the range 0-255 range)");
            }
            else if (encoding == TextEncodingDetect.Encoding.Utf8Bom || encoding == TextEncodingDetect.Encoding.Utf8Nobom)
            {
                Console.WriteLine("UTF-8");
                return Encoding.UTF8;
            }
            else if (encoding == TextEncodingDetect.Encoding.Utf16LeBom || encoding == TextEncodingDetect.Encoding.Utf16LeNoBom)
            {
                Console.WriteLine("UTF-16 Little Endian");
                return Encoding.UTF8;
            }
            else if (encoding == TextEncodingDetect.Encoding.Utf16BeBom || encoding == TextEncodingDetect.Encoding.Utf16BeNoBom)
            {
                Console.WriteLine("UTF-16 Big Endian");
                return Encoding.UTF32;
            }

            return Encoding.ASCII;
        }

        public Encoding GetEncoding(byte[] bytes)
        {

            var textDetect = new TextEncodingDetect();
            var encoding = textDetect.DetectEncoding(bytes, bytes.Length);

            Console.Write("Encoding: ");
            if (encoding == TextEncodingDetect.Encoding.None)
            {
                Console.WriteLine("Binary");
                return Encoding.Default;
            }
            if (encoding == TextEncodingDetect.Encoding.Ascii)
            {
                Console.WriteLine("ASCII (chars in the 0-127 range)");
                return Encoding.ASCII;
            }
            if (encoding == TextEncodingDetect.Encoding.Ansi)
            {
                Console.WriteLine("ANSI (chars in the range 0-255 range)");
            }
            else if (encoding == TextEncodingDetect.Encoding.Utf8Bom || encoding == TextEncodingDetect.Encoding.Utf8Nobom)
            {
                Console.WriteLine("UTF-8");
                return Encoding.UTF8;
            }
            else if (encoding == TextEncodingDetect.Encoding.Utf16LeBom || encoding == TextEncodingDetect.Encoding.Utf16LeNoBom)
            {
                Console.WriteLine("UTF-16 Little Endian");
                return Encoding.UTF8;
            }
            else if (encoding == TextEncodingDetect.Encoding.Utf16BeBom || encoding == TextEncodingDetect.Encoding.Utf16BeNoBom)
            {
                Console.WriteLine("UTF-16 Big Endian");
                return Encoding.UTF32;
            }

            return Encoding.ASCII;
        }
    }
}