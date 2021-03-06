﻿//-----------------------------------------------------------------------
// <copyright file="SwaggerGeneratorSettings.cs" company="NSwag">
//     Copyright (c) Rico Suter. All rights reserved.
// </copyright>
// <license>https://github.com/NSwag/NSwag/blob/master/LICENSE.md</license>
// <author>Rico Suter, mail@rsuter.com</author>
//-----------------------------------------------------------------------

using Namotion.Reflection;
using Newtonsoft.Json;
using NJsonSchema;
using NJsonSchema.Generation;
using NSwag.Generation.Processors;
using NSwag.Generation.Processors.Collections;
using NSwag.Generation.Processors.Contexts;
using System;
using System.Threading.Tasks;

namespace NSwag.Generation
{
    /// <summary>Settings for the Swagger generator.</summary>
    public class OpenApiDocumentGeneratorSettings : JsonSchemaGeneratorSettings
    {
        /// <summary>Initializes a new instance of the <see cref="OpenApiDocumentGeneratorSettings"/> class.</summary>
        public OpenApiDocumentGeneratorSettings()
        {
            SchemaGenerator = new OpenApiSchemaGenerator(this);
            DefaultResponseReferenceTypeNullHandling = ReferenceTypeNullHandling.Null;
            SchemaType = SchemaType.Swagger2;
        }

        /// <summary>Gets or sets the JSON Schema generator.</summary>
        public OpenApiSchemaGenerator SchemaGenerator { get; set; }

        /// <summary>Gets or sets the Swagger specification title.</summary>
        public string Title { get; set; } = "My Title";

        /// <summary>Gets or sets the Swagger specification description.</summary>
        public string Description { get; set; }

        /// <summary>Gets or sets the Swagger specification version.</summary>
        public string Version { get; set; } = "1.0.0";

        /// <summary>Gets or sets a value indicating whether nullable body parameters are allowed (ignored when MvcOptions.AllowEmptyInputInBodyModelBinding is available (ASP.NET Core 2.0+), default: true).</summary>
        public bool AllowNullableBodyParameters { get; set; } = true;

        /// <summary>Gets or sets the default response reference type null handling when no nullability information is available (if NotNullAttribute and CanBeNullAttribute are missing, default: Null).</summary>
        public ReferenceTypeNullHandling DefaultResponseReferenceTypeNullHandling { get; set; }

        /// <summary>Gets the operation processors.</summary>
        [JsonIgnore]
        public OperationProcessorCollection OperationProcessors { get; } = new OperationProcessorCollection
        {
            new OperationSummaryAndDescriptionProcessor(),
            new OperationTagsProcessor(),
            new OperationExtensionDataProcessor(),
        };

        /// <summary>Gets the document processors.</summary>
        [JsonIgnore]
        public DocumentProcessorCollection DocumentProcessors { get; } = new DocumentProcessorCollection
        {
            new DocumentTagsProcessor(),
            new DocumentExtensionDataProcessor(),
        };

        /// <summary>Gets or sets the document template representing the initial Swagger specification (JSON data).</summary>
        public string DocumentTemplate { get; set; }

        /// <summary>Inserts a function based operation processor at the beginning of the pipeline to be used to filter operations.</summary>
        /// <param name="filter">The processor filter.</param>
        public void AddOperationFilter(Func<OperationProcessorContext, bool> filter)
        {
            OperationProcessors.Insert(0, new OperationProcessor(filter));
        }

        /// <summary>Applies the given settings to this settings object.</summary>
        /// <param name="serializerSettings">The serializer settings.</param>
        /// <param name="mvcOptions">The MVC options.</param>
        public void ApplySettings(JsonSerializerSettings serializerSettings, object mvcOptions)
        {
            if (serializerSettings != null)
            {
                var areSerializerSettingsSpecified =
                    DefaultPropertyNameHandling != PropertyNameHandling.Default ||
                    DefaultEnumHandling != EnumHandling.Integer ||
                    ContractResolver != null ||
                    SerializerSettings != null;

                if (!areSerializerSettingsSpecified)
                {
                    SerializerSettings = serializerSettings;
                }
            }

            if (mvcOptions != null && mvcOptions.HasProperty("AllowEmptyInputInBodyModelBinding"))
            {
                AllowNullableBodyParameters = mvcOptions.TryGetPropertyValue("AllowEmptyInputInBodyModelBinding", false);
            }
        }
    }
}
