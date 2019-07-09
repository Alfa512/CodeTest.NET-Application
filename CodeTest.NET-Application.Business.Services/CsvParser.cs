﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace CodeTest.NET_Application.Business.Services
{
    public class CsvParser
    {
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