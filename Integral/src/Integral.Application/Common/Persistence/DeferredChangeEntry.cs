using Integral.Application.ApplicationSettings.Data;
using Integral.Application.Common.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Integral.Application.Common.Persistence
{
    public class DeferredChangeEntry
    {
        private static readonly Type[] SUPPORTED_TYPES = new [] {
            typeof(bool),
            //typeof(byte),
            //typeof(sbyte),
            //typeof(char),
            typeof(decimal),
            typeof(double),
            //typeof(float),
            typeof(int),
            //typeof(uint),
            typeof(long),
            //typeof(ulong),
            //typeof(short),
            //typeof(ushort),
            typeof(string),
            typeof(DateTime),
        };

        private static readonly string[] EXCLUDED_PROPERTIES = new[] {
            nameof(AuditableEntity.Created),
            nameof(AuditableEntity.CreatedBy),
            nameof(AuditableEntity.LastModified),
            nameof(AuditableEntity.LastModifiedBy),
            nameof(Product.Id),
            nameof(Product.ImagePath),
            nameof(ProductAttachment.FilePath),
            nameof(LandingCarouselSettingsDTO.Item.Path)
        };

        public DeferredChangeEntry(EntityEntry entry)
        {
            Entity = entry.Entity;
            State = entry.State;

            if (State == EntityState.Added)
            {
                AddAdded();
            }

            if (State == EntityState.Modified)
            {
                AddModified(entry);
            }

            if (State == EntityState.Deleted)
            {
                AddDeleted();
            }
        }

        public object Entity { get; }

        public EntityState State { get; }

        public ICollection<(string, string)> ModifiedProperties { get; } 
            = new List<(string, string)>();

        public void AddAdded()
        {
            foreach (var property in GetProperties(Entity))
            {
                ModifiedProperties.Add((null, property.Value.ToString()));
            }
        }

        public void AddModified(EntityEntry entry)
        {
            var original = GetProperties(entry.OriginalValues.ToObject()).ToList();
            var current = GetProperties(Entity).ToList();

            var leftJoin = from o in original
                           join c in current on o.Name equals c.Name into tc
                           from c in tc.DefaultIfEmpty()
                           select new { Original = o, Current = c };

            var rightJoin = from c in current
                            join o in original on c.Name equals o.Name into to
                            from o in to.DefaultIfEmpty()
                            select new { Original = o, Current = c };

            var join = leftJoin.Union(rightJoin).ToList();

            foreach (var item in join)
            {
                var originalValue = item.Original?.Value;
                var currentValue = item.Current?.Value;

                if (originalValue == null && currentValue == null)
                {
                    //just skip
                }
                else if (originalValue == null)
                {
                    ModifiedProperties.Add((string.Empty, currentValue.ToString()));
                }
                else if (currentValue == null)
                {
                    ModifiedProperties.Add((originalValue.ToString(), string.Empty));
                }
                else if (!originalValue.Equals(currentValue))
                {
                    ModifiedProperties.Add((originalValue.ToString(), currentValue.ToString()));
                }
            }
        }

        public void AddDeleted()
        {
            foreach (var property in GetProperties(Entity))
            {
                ModifiedProperties.Add((property.Value.ToString(), null));
            }
        }

        public IEnumerable<ModifiedProperty> GetProperties(object entity)
        {
            IEnumerable<ModifiedProperty> properties;

            if (entity is ApplicationSetting setting)
            {
                var settingValue = JsonConvert.DeserializeObject<JObject>(setting.Value);

                properties = settingValue.Values().SelectMany(GetProperties).ToList();
            }
            else
            {
                properties = entity.GetType().GetProperties()
                    .Where(p => p.CanWrite && SUPPORTED_TYPES.Contains(p.PropertyType))
                    .Select(p => new ModifiedProperty(p.Name, p.GetValue(entity)))
                    .ToList();
            }

            return properties.Where(p => !EXCLUDED_PROPERTIES.Contains(p.Name));
        }

        private IEnumerable<ModifiedProperty> GetProperties(JToken token)
        {
            if (!token.HasValues)
            {
                return new List<ModifiedProperty> 
                { 
                    new ModifiedProperty(token.Path, token.Value<object>()) 
                };
            }

            return token.Values().SelectMany(GetProperties);
        }
    }
}
