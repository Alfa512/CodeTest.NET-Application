using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeTest.NET_Application.Business.Services;
using CodeTest.NET_Application.Common.Contracts.Data;
using CodeTest.NET_Application.Common.Contracts.Repositories;
using CodeTest.NET_Application.Common.Contracts.Services;
using CodeTest.NET_Application.Data.Models;
using CodeTest.NET_Application.Data.Repositories;

namespace CodeTest.NET_Application.Data
{
    public class CsvContext : IDataContext
    {
        private CsvService _csvService;
        private IConfigurationService _configurationService;
        private char _delimeter;

        private static bool _usersDirty;
        private static FileStream _userStorage;
        private static string _userStorageFilePath;

        public CsvContext(IConfigurationService configurationService)
        {
            _csvService = new CsvService();
            _configurationService = configurationService;
            _delimeter = ',';
            _usersDirty = true;
            _userStorageFilePath = "C:/Temp/Users.csv"; //configurationService.UserStoragePath;
        }

        IUserRepository IDataContext.Users => new UserRepository(this);

        public TEntity Add<TEntity>(TEntity entity) where TEntity : class, IEntity, new()
        {
            entity.ID = GetLastId<TEntity>() + 1;
            string csv = Write(entity, true);

            GetStream<TEntity>().Seek(0, SeekOrigin.End);
            GetStream<TEntity>().Write(Encoding.Default.GetBytes(csv));
            _usersDirty = true;

            return entity;
        }
        public IEnumerable<TEntity> AddRange<TEntity>(List<TEntity> list) where TEntity : class, IEntity, new()
        {
            var lastId = GetLastId<TEntity>() + 1;
            for (var i = 0; i < list.Count; i++, lastId++)
            {
                list[i].ID = lastId;
            }
            string csv = Write(list, true);
            GetStream<TEntity>().Seek(0, SeekOrigin.End);
            GetStream<TEntity>().Write(Encoding.Default.GetBytes(csv));
            _usersDirty = true;

            return list;
        }

        public TEntity Update<TEntity>(TEntity entity) where TEntity : class, IEntity, new()
        {
            if (entity.ID <= 0)
                return entity;
            var entities = All<TEntity>().ToList();
            var item = entities.FirstOrDefault(r => r.ID == entity.ID);
            if (item == null)
                return entity;
            entities.Remove(item);
            entities.Add(entity);

            RewriteEntities(entities);
            return entity;
        }

        public TEntity Delete<TEntity>(TEntity entity) where TEntity : class, IEntity, new()
        {
            if (entity.ID <= 0)
                return entity;
            var entities = All<TEntity>().ToList();
            var item = entities.FirstOrDefault(r => r.ID == entity.ID);
            if (item == null)
                return entity;
            entities.Remove(item);

            RewriteEntities(entities);
            return entity;
        }

        private int GetLastId<TEntity>() where TEntity : class, IEntity, new()
        {
            var entities = All<TEntity>();
            return entities.Max(r => r.ID);
        }

        private void RewriteEntities<TEntity>(List<TEntity> entities) where TEntity : class, IEntity, new()
        {
            string csv = Write(entities, true);
            GetStream<TEntity>().Seek(0, SeekOrigin.Begin);
            GetStream<TEntity>().Write(Encoding.Default.GetBytes(csv));
            _usersDirty = true;
        }

        public IEnumerable<TEntity> All<TEntity>() where TEntity : class, IEntity, new()
        {
            return _csvService.ReadFromStream<TEntity>(GetStream<TEntity>());
        }

        private Stream GetStream<TEntity>()
        {
            switch (typeof(TEntity).Name.ToString())
            {
                case "User":
                    return UserStream();
                default:
                    return new MemoryStream();
            }
        }

        private static Stream UserStream()
        {
            if (_userStorage == null)
            {
                _userStorage = File.Open(_userStorageFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);
                return _userStorage;
            }
            else
            {
                lock (_userStorage)
                {
                    if (_usersDirty)
                    {
                        _userStorage.Close();
                        _userStorage = File.Open(_userStorageFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

                        _usersDirty = false;
                        return _userStorage;
                    }

                    if (_userStorage.CanSeek)
                        _userStorage.Seek(0, SeekOrigin.Begin);
                    else
                    {
                        _userStorage.Close();
                        _userStorage = File.Open(_userStorageFilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite);

                        _usersDirty = false;
                    }
                }
                return _userStorage;
            }
        }

        private string Write<T>(IList<T> list, bool includeHeader = true)
        {
            StringBuilder sb = new StringBuilder();

            Type type = typeof(T);

            PropertyInfo[] properties = type.GetProperties();

            if (includeHeader)
            {
                sb.AppendLine(CreateCsvHeaderLine(properties));
            }

            foreach (var item in list)
            {
                sb.AppendLine(CreateCsvLine(item, properties));
            }

            return sb.ToString();
        }
        private string Write<T>(T item, bool includeHeader = true)
        {
            StringBuilder sb = new StringBuilder();

            Type type = typeof(T);

            PropertyInfo[] properties = type.GetProperties();

            if (includeHeader)
            {
                sb.AppendLine(CreateCsvHeaderLine(properties));
            }

            sb.AppendLine(CreateCsvLine(item, properties));

            return sb.ToString();
        }

        private string CreateCsvHeaderLine(PropertyInfo[] properties)
        {
            List<string> propertyValues = new List<string>();

            foreach (var prop in properties)
            {
                string stringformatString = string.Empty;
                string value = prop.Name;

                var attribute = prop.GetCustomAttribute(typeof(DisplayAttribute));
                if (attribute != null)
                {
                    value = (attribute as DisplayAttribute).Name;
                }

                CreateCsvStringItem(propertyValues, value);
            }

            return CreateCsvLine(propertyValues);
        }

        private string CreateCsvLine<T>(T item, PropertyInfo[] properties)
        {
            List<string> propertyValues = new List<string>();

            try
            {
                foreach (var prop in properties)
                {
                    string stringformatString = string.Empty;
                    object value = prop.GetValue(item, null);

                    if (prop.PropertyType == typeof(string))
                    {
                        CreateCsvStringItem(propertyValues, value);
                    }
                    else if (prop.PropertyType == typeof(string[]))
                    {
                        CreateCsvStringArrayItem(propertyValues, value);
                    }
                    else if (prop.PropertyType == typeof(List<string>))
                    {
                        CreateCsvStringListItem(propertyValues, value);
                    }
                    else
                    {
                        CreateCsvItem(propertyValues, value);
                    }
                }
            }
            catch (Exception e)
            {
                // Ignore
            }
            
            return CreateCsvLine(propertyValues);
        }

        private string CreateCsvLine(IList<string> list)
        {
            return string.Join(_delimeter, list);
        }

        private void CreateCsvItem(List<string> propertyValues, object value)
        {
            if (value != null)
            {
                propertyValues.Add(value.ToString());
            }
            else
            {
                propertyValues.Add(string.Empty);
            }
        }

        private void CreateCsvStringListItem(List<string> propertyValues, object value)
        {
            string formatString = "\"{0}\"";
            if (value != null)
            {
                value = CreateCsvLine((List<string>)value);
                propertyValues.Add(string.Format(formatString, ProcessStringEscapeSequence(value)));
            }
            else
            {
                propertyValues.Add(string.Empty);
            }
        }

        private void CreateCsvStringArrayItem(List<string> propertyValues, object value)
        {
            string formatString = "\"{0}\"";
            if (value != null)
            {
                value = CreateCsvLine(((string[])value).ToList());
                propertyValues.Add(string.Format(formatString, ProcessStringEscapeSequence(value)));
            }
            else
            {
                propertyValues.Add(string.Empty);
            }
        }

        private void CreateCsvStringItem(List<string> propertyValues, object value)
        {
            string formatString = "\"{0}\"";
            if (value != null)
            {
                propertyValues.Add(string.Format(formatString, ProcessStringEscapeSequence(value)));
            }
            else
            {
                propertyValues.Add(string.Empty);
            }
        }

        private string ProcessStringEscapeSequence(object value)
        {
            return value.ToString().Replace("\"", "\"\"");
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
