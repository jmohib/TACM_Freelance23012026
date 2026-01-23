using System.Collections.Immutable;
using System.ComponentModel;
using System.Reflection;
using TACM.Core.Extensions;
using TACM.UI.Utils;

namespace TACM.UI.Models;

public class ModelMetada
{
    private ImmutableArray<PropertyInfo>? ModelProperties;
    private readonly IDictionary<int, ImmutableArray<PropertyInfo>> CommonPropertiesWithEntity = new Dictionary<int, ImmutableArray<PropertyInfo>>();

    internal ref ImmutableArray<PropertyInfo>? GetModelProperties(in Type typeOfModel)
    {
        ModelProperties ??= PropertiesMemoryCache.GetProperties(in typeOfModel);
        return ref ModelProperties;
    }

    internal (ImmutableArray<PropertyInfo> modelProps, ImmutableArray<PropertyInfo> entityProps) GetCommonPropertiesWithEntity(in Type typeOfEntity, in Type typeOfModel)
    {
        var typeOfModelHashCode = typeOfModel.GetHashCode();
        var typeOfEntityHashCode = typeOfEntity.GetHashCode();
        var succesModel = CommonPropertiesWithEntity.TryGetValue(typeOfModelHashCode, out var propertiesModel);
        var successEntity = CommonPropertiesWithEntity.TryGetValue(typeOfEntityHashCode, out var propertiesEntity);

        if (successEntity && succesModel)
            return (propertiesModel, propertiesEntity);

        var entityProperties = PropertiesMemoryCache.GetProperties(typeOfEntity);
        var thisProperties = PropertiesMemoryCache.GetProperties(typeOfModel);

        var entityPropertiesNames = entityProperties!.Select(p => p.Name).ToImmutableHashSet();
        var modelPropertiesNames = thisProperties!.Select(p => p.Name).ToImmutableHashSet();

        var commonModelProperties = thisProperties!.Where(_ => entityPropertiesNames.Contains(_.Name)).OrderBy(_ => _.Name).ToImmutableArray();
        var commonEntityProperties = entityProperties.Where(_ => modelPropertiesNames.Contains(_.Name)).OrderBy(_ => _.Name).ToImmutableArray();

        CommonPropertiesWithEntity.TryAdd(typeOfModelHashCode, commonModelProperties);
        CommonPropertiesWithEntity.TryAdd(typeOfEntityHashCode, commonEntityProperties);

        return (commonModelProperties, commonEntityProperties);
    }
}

public abstract class Model : INotifyPropertyChanged
{
    protected ModelMetada Metadata;
    public event PropertyChangedEventHandler? PropertyChanged;

    protected Model()
    {
        Metadata = new ModelMetada();
    }

    internal virtual bool AllPropertiesHaveValues()
    {
        var typeOfModel = this.GetType();
        ref var properties = ref Metadata.GetModelProperties(in typeOfModel);

        if (properties is null or { Length: 0 })
            return false;

        foreach( var property in properties)
        {
            if (!property.HasValue(this))
                return false;
        }

        return true;
    }

    internal virtual TEntity? ToEntity<TEntity>() where TEntity: class
    {
        var typeOfData = typeof(TEntity);
        var typeOfThis = this.GetType();
        var typeOfChar = typeof(char);
        var typeOfNullableChar = typeof(char?);

        (var commonModelProperties, var commonEntityProperties) = Metadata.GetCommonPropertiesWithEntity(in typeOfData, in typeOfThis);
        var instance = Activator.CreateInstance(typeOfData);

        for (var i = 0; i < commonModelProperties.Length; i++)
        {
            var prop = commonEntityProperties[i];

            if(prop.PropertyType.Equals(typeOfChar) || prop.PropertyType.Equals(typeOfNullableChar))
            {
                var value = Convert.ToChar(commonModelProperties[i].GetValue(this));
                commonEntityProperties[i].SetValue(instance, value);
            }
            else
                commonEntityProperties[i].SetValue(instance, commonModelProperties[i].GetValue(this));
        }

        return instance as TEntity;
    }

    internal virtual void CopyFromEntity(in object entity)
    {
        var typeOfData = entity.GetType();
        var typeOfThis = this.GetType();
        var typeOfChar = typeof(char);
        var typeOfNullableChar = typeof(char?);

        (var commonModelProperties, var commonEntityProperties) = Metadata.GetCommonPropertiesWithEntity(in typeOfData, in typeOfThis);

        for (var i = 0; i < commonModelProperties.Length; i++)
        {
            var prop = commonModelProperties[i];

            if (prop.PropertyType.Equals(typeOfChar) || prop.PropertyType.Equals(typeOfNullableChar))
            {
                var value = Convert.ToChar(commonEntityProperties[i].GetValue(entity));
                commonModelProperties[i].SetValue(this, value);
            }
            else
                commonModelProperties[i].SetValue(this, commonEntityProperties[i].GetValue(entity));
        }
    }

    internal virtual void CopyFromModel(in Model model)
    {
        var typeOfThis = this.GetType();
        var typeOfChar = typeof(char);
        var typeOfNullableChar = typeof(char?);
        var modelProperties = Metadata.GetModelProperties(in typeOfThis).GetValueOrDefault();

        for (var i = 0; i < modelProperties.Length; i++)
        {
            var prop = modelProperties[i];

            if (prop.PropertyType.Equals(typeOfChar) || prop.PropertyType.Equals(typeOfNullableChar))
            {
                var value = Convert.ToChar(modelProperties[i].GetValue(model));
                modelProperties[i].SetValue(this, value);
            }
            else
                modelProperties[i].SetValue(this, modelProperties[i].GetValue(model));
        }
    }

    public void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
}
