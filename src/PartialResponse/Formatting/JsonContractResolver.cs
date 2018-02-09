// Copyright (c) Arjen Post. See License.txt and Notice.txt in the project root for license information.

using System.Net.Http.Formatting;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace PartialResponse.Net.Http.Formatting
{
    // Default Contract resolver for JsonMediaTypeFormatter
    // Uses the IRequiredMemberSelector to choose required members
    internal class JsonContractResolver : DefaultContractResolver
    {
        private readonly MediaTypeFormatter formatter;

        public JsonContractResolver(MediaTypeFormatter formatter)
        {
            this.formatter = formatter;

            // Need this setting to have [Serializable] types serialized correctly
            this.IgnoreSerializableAttribute = false;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);
            this.ConfigureProperty(member, property);
            return property;
        }

        // Determines whether a member is required or not and sets the appropriate JsonProperty settings
        private void ConfigureProperty(MemberInfo member, JsonProperty property)
        {
            if (this.formatter.RequiredMemberSelector != null && this.formatter.RequiredMemberSelector.IsRequiredMember(member))
            {
                property.Required = Required.AllowNull;
                property.DefaultValueHandling = DefaultValueHandling.Include;
                property.NullValueHandling = NullValueHandling.Include;
            }
            else
            {
                property.Required = Required.Default;
            }
        }
    }
}