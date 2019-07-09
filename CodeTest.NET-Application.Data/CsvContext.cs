﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using CodeTest.NET_Application.Common.Contracts.Data;
using CodeTest.NET_Application.Common.Contracts.Repositories;
using CodeTest.NET_Application.Common.Contracts.Services;
using CodeTest.NET_Application.Data.Models;
using CodeTest.NET_Application.Data.Repositories;

namespace CodeTest.NET_Application.Data
{
    public class CsvContext : IDataContext
    {
        private IConfigurationService _configurationService;
        private Dictionary<string, string> _map;
        private int _startRowOffset;
        private int _startColumnOffset;
        private int _endRowOffset;
        private char _delimeter;

        private static bool _usersDirty;
        private static FileStream _userStorage;
        private static string _userStorageFilePath;

        public CsvContext(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
            _map = null;
            _startRowOffset = 0;
            _startColumnOffset = 0;
            _endRowOffset = 0;
            _delimeter = ',';
            _usersDirty = true;
            _userStorageFilePath = configurationService.UserStoragePath;
        }

        IUserRepository IDataContext.Users => new UserRepository(this);

        public TEntity Add<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public TEntity Update<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public TEntity Delete<TEntity>(TEntity entity) where TEntity : class
        {
            throw new NotImplementedException();
        }

        public IEnumerable<TEntity> All<TEntity>() where TEntity : class
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

            using (var sr = new StreamReader(GetStream<TEntity>()))
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
                    //var item = new TEntity();
                    //var item = new TEntity();
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
                                retList.Add((TEntity)parsedValue);
                                //prop.PropertyInfo.SetValue(item, parsedValue);
                            }
                            catch (Exception ex)
                            {
                                // Indicate parsing error on row?
                            }
                        }
                    });
                    //retList.Add(item);
                }

                return retList;
            }
        }

        private Stream GetStream<TEntity>()
        {
            switch (typeof(TEntity).ToString())
            {
                case "User":
                    return UserStream();
                default:
                    return new MemoryStream();
            }
        }

        private static Stream UserStream()
        {
            lock (_userStorage)
            {
                if (_usersDirty || _userStorage == null)
                {
                    if (_userStorage == null)
                    {
                        _userStorage = File.Open(_userStorageFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    }
                    else
                    {
                        _userStorage.Close();
                        _userStorage = File.Open(_userStorageFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                    }

                    _usersDirty = false;
                    return _userStorage;
                }

                _userStorage.Seek(0, SeekOrigin.Begin);
                return _userStorage;
            }
        }

        public int SaveChanges()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
