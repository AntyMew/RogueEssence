using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RogueEssence.Dev
{
    public sealed class EditorServiceProvider<TView>
    {
        private readonly Dictionary<Type, IEditorService<TView>> services = new Dictionary<Type, IEditorService<TView>>();

        public IEditorService<TView> Get(Type modelType)
        {
            Type currentType = modelType ?? throw new ArgumentNullException(nameof(modelType));
            IEditorService<TView> ret = null;

            while (ret == null)
            {
                if (services.TryGetValue(currentType, out ret))
                    continue;

                if (currentType.IsConstructedGenericType)
                    if (services.TryGetValue(currentType.GetGenericTypeDefinition(), out ret))
                        continue;

                if (currentType.BaseType == null)
                    throw new KeyNotFoundException($"No service found for type {modelType}");

                currentType = currentType.BaseType;
            }

            Debug.Assert(ret.Supports(modelType));
            return ret;
        }

        public void Register(Type modelType, IEditorService<TView> service)
        {
            if (!service.Supports(modelType))
                throw new ArgumentException($"Does not support {modelType}", nameof(service));

            if (!services.TryAdd(modelType, service))
                services[modelType] = service;
        }
    }
}