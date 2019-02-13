using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace GraphQl.NetStandard.Client
{
    /// <summary>
    /// Handles deserialization of a GraphQL JSON object that has a child collection of nodes
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class GraphQlEdgesParentConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);

            // Get all the non Node information populated
            var graphQlEdgesParent = JsonConvert.DeserializeObject<GraphQLEdgesParent<T>>(jObject.ToString());
            graphQlEdgesParent.Edges = new List<T>();

            // Now populate the Nodes collection
            if (jObject["edges"] != null)
            {
                foreach (var token in jObject["edges"].Children())
                {
                    var typeJToken = token[typeof(T).Name.ToLower()];

                    // If the children of the node array have an object wrapper with the same name as the generic type
                    if (typeJToken != null)
                    {
                        var genericTypeObject = JsonConvert.DeserializeObject<T>(typeJToken.ToString());
                        graphQlEdgesParent.Edges.Add(genericTypeObject);
                    }
                    // If the chidren of the node array are the generic type objects
                    else
                    {
                        var genericTypeObject = JsonConvert.DeserializeObject<T>(token.ToString());
                        graphQlEdgesParent.Edges.Add(genericTypeObject);
                    }
                }
            }

            return graphQlEdgesParent;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
