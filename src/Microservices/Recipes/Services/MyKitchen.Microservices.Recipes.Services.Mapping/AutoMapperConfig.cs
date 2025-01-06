// |-----------------------------------------------------------------------------------------------------|
// <copyright file="AutoMapperConfig.cs" company="MyKitchen">
// Copyright (c) MyKitchen. All Rights Reserved.
// Licensed under the GPLv3 license. See LICENSE file in the project root for full license information.
// </copyright>
// |-----------------------------------------------------------------------------------------------------|

namespace MyKitchen.Microservices.Recipes.Services.Mapping
{
    using System.Reflection;

    using AutoMapper;

    /// <summary>
    /// This class registers the automapper mappings by getting all classes implementing the <see cref="IMapFrom{TClass}"/>,
    /// <see cref="IMapTo{TClass}"/> and <see cref="IHaveCustomMappings"/> from the provided assemblies and initializes the
    /// <see cref="IMapper"/> instance.
    /// </summary>
    public static class AutoMapperConfig
    {
        private static bool initialized;

        /// <summary>
        /// Gets or sets the <see cref="IMapper"/> instance.
        /// </summary>
        public static IMapper MapperInstance { get; set; } = null!;

        /// <summary>
        /// This method creates and register all automapper mappings by
        /// getting all classes implementing the <see cref="IMapFrom{TClass}"/>,
        /// <see cref="IMapTo{TClass}"/>, <see cref="IHaveCustomMappings"/> interfaces
        /// from the specified <paramref name="assemblies"/>.
        /// </summary>
        /// <param name="assemblies">
        /// The assemblies containing the classes implementing <see cref="IMapFrom{TClass}"/>
        /// <see cref="IMapTo{TClass}"/> and <see cref="IHaveCustomMappings"/>.
        /// </param>
        public static void RegisterMappings(params Assembly[] assemblies)
        {
            if (initialized)
            {
                return;
            }

            initialized = true;

            var types = assemblies.SelectMany(a => a.GetExportedTypes()).ToList();

            var config = new MapperConfigurationExpression();

            config.CreateProfile(
                "ReflectionProfile",
                configuration =>
                {
                    // IMapFrom<>
                    foreach (var map in GetFromMaps(types))
                    {
                        configuration.CreateMap(map.Source, map.Destination);
                    }

                    // IMapTo<>
                    foreach (var map in GetToMaps(types))
                    {
                        configuration.CreateMap(map.Source, map.Destination);
                    }

                    // IHaveCustomMappings
                    foreach (var map in GetCustomMappings(types))
                    {
                        map.CreateMappings(configuration);
                    }
                });

            MapperInstance = new Mapper(new MapperConfiguration(config));
        }

        /// <summary>
        /// This method creates a automapper instance that can maps objects of the same <typeparamref name="TType"/>.
        /// </summary>
        /// <typeparam name="TType">The type of the objects that can mapped between themselves.</typeparam>
        /// <returns>Returns a configured <see cref="IMapper"/> instance.</returns>
        public static IMapper CreateDuplicateTypeMapper<TType>()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<TType, TType>();
            });
            IMapper mapper = new Mapper(mapperConfiguration);

            return mapper;
        }

        /// <summary>
        /// This method creates a automapper instance that can maps objects of the same type.
        /// </summary>
        /// <param name="types">The types of the objects that can mapped between themselves.</param>
        /// <returns>Returns a configured <see cref="IMapper"/> instance.</returns>
        public static IMapper CreateDuplicateTypeMapper(params Type[] types)
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                foreach (var type in types)
                {
                    cfg.CreateMap(type, type);
                }
            });
            IMapper mapper = new Mapper(mapperConfiguration);

            return mapper;
        }

        private static IEnumerable<TypesMap> GetFromMaps(IEnumerable<Type> types)
        {
            var fromMaps = from t in types
                           from i in t.GetTypeInfo().GetInterfaces()
                           where i.GetTypeInfo().IsGenericType &&
                                 i.GetGenericTypeDefinition() == typeof(IMapFrom<>) &&
                                 !t.GetTypeInfo().IsAbstract &&
                                 !t.GetTypeInfo().IsInterface
                           select new TypesMap
                           {
                               Source = i.GetTypeInfo().GetGenericArguments()[0],
                               Destination = t,
                           };

            return fromMaps;
        }

        private static IEnumerable<TypesMap> GetToMaps(IEnumerable<Type> types)
        {
            var toMaps = from t in types
                         from i in t.GetTypeInfo().GetInterfaces()
                         where i.GetTypeInfo().IsGenericType &&
                               i.GetTypeInfo().GetGenericTypeDefinition() == typeof(IMapTo<>) &&
                               !t.GetTypeInfo().IsAbstract &&
                               !t.GetTypeInfo().IsInterface
                         select new TypesMap
                         {
                             Source = t,
                             Destination = i.GetTypeInfo().GetGenericArguments()[0],
                         };

            return toMaps;
        }

        private static IEnumerable<IHaveCustomMappings> GetCustomMappings(IEnumerable<Type> types)
        {
            var customMaps = from t in types
                             from i in t.GetTypeInfo().GetInterfaces()
                             where typeof(IHaveCustomMappings).GetTypeInfo().IsAssignableFrom(t) &&
                                   !t.GetTypeInfo().IsAbstract &&
                                   !t.GetTypeInfo().IsInterface
                             select (IHaveCustomMappings)Activator.CreateInstance(t)!;

            return customMaps;
        }

        private class TypesMap
        {
            /// <summary>
            /// Gets or sets the type of the source class.
            /// </summary>
            public Type Source { get; set; } = null!;

            /// <summary>
            /// Gets or sets the type of the destination class.
            /// </summary>
            public Type Destination { get; set; } = null!;
        }
    }
}
