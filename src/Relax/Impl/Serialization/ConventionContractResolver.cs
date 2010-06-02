﻿using Newtonsoft.Json.Serialization;

namespace Relax.Impl
{
    public class ConventionContractResolver : DefaultContractResolver
    {
        protected DocumentConventions conventions { get; set; }

        protected override string ResolvePropertyName(string propertyName)
        {
            if (propertyName == "Id")
                return "_id";
            else if (propertyName == "Rev")
                return "_rev";
            else
                return base.ResolvePropertyName(propertyName);
        }

        public ConventionContractResolver(DocumentConventions conventions)
        {
            this.conventions = conventions;
        }
    }
}