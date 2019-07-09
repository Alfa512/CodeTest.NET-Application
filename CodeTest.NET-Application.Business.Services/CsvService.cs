using System;
using System.Collections.Generic;
using System.Linq;
using CodeTest.NET_Application.Common.Models.ViewModel;
using System.Text;

namespace CodeTest.NET_Application.Business.Services
{
    public class CsvService
    {
        public IEnumerable<UserVm> ParseUsers(byte[] bytes)
        {
            var users = new List<UserVm>();

            var csvParser = new CsvParser();

            var delimiter = ';';
            var qualifier = '\r';
            var content = Encoding.Default.GetString(bytes);
            var encoding = csvParser.GetEncoding(bytes);
            var utf8 = Encoding.UTF8;


            var convertedBytes = Encoding.Convert(encoding, utf8, bytes);
            if (!Equals(encoding, utf8))
                content = utf8.GetString(convertedBytes);


            var parser = csvParser.Parse(content, delimiter, qualifier).ToList();

            foreach (var strings in parser)
            {
                try
                {
                    if (strings == null) continue;

                    var fields = strings.ToList();

                    users.Add(new UserVm
                    {
                        Id = fields[0] != null ? Convert.ToInt32(fields[0]) : 0,
                        FirstName = fields[1] ?? "",
                        LastName = fields[2] ?? "",
                        Age = fields[3] != null ? Convert.ToByte(fields[3]) : byte.MinValue
                    });
                }
                catch (Exception)
                {
                    // Ignore
                }
            }

            return users;
        }
        public IEnumerable<UserVm> ParseUsers(string text)
        {
            var users = new List<UserVm>();

            var csvParser = new CsvParser();

            var delimiter = ';';
            var qualifier = '\r';
            var bytes = Encoding.Default.GetBytes(text);
            var content = text;
            var encoding = csvParser.GetEncoding(text);
            var utf8 = Encoding.UTF8;


            var convertedBytes = Encoding.Convert(encoding, utf8, bytes);
            if (!Equals(encoding, utf8))
                content = utf8.GetString(convertedBytes);


            var parser = csvParser.Parse(content, delimiter, qualifier).ToList();

            foreach (var strings in parser)
            {
                try
                {
                    if (strings == null) continue;

                    var fields = strings.ToList();

                    users.Add(new UserVm
                    {
                        Id = fields[0] != null ? Convert.ToInt32(fields[0]) : 0,
                        FirstName = fields[1] ?? "",
                        LastName = fields[2] ?? "",
                        Age = fields[3] != null ? Convert.ToByte(fields[3]) : byte.MinValue
                    });
                }
                catch (Exception)
                {
                    // Ignore
                }
            }

            return users;
        }
    }
}