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
    public class GraphQlNodesParentConverter<T> : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);

            // Get all the non Node information populated
            var graphQlNodesParent = JsonConvert.DeserializeObject<GraphQlNodesParent<T>>(jObject.ToString());
            graphQlNodesParent.Nodes = new List<T>();

            // Now populate the Nodes collection
            if (jObject["nodes"] != null)
            {
                foreach (var token in jObject["nodes"].Children())
                {
                    var typeJToken = token[typeof(T).Name.ToLower()];

                    // If the children of the node array have an object wrapper with the same name as the generic type
                    if (typeJToken != null)
                    {
                        var genericTypeObject = JsonConvert.DeserializeObject<T>(typeJToken.ToString());
                        graphQlNodesParent.Nodes.Add(genericTypeObject);
                    }
                    // If the chidren of the node array are the generic type objects
                    else
                    {
                        var genericTypeObject = JsonConvert.DeserializeObject<T>(token.ToString());
                        graphQlNodesParent.Nodes.Add(genericTypeObject);
                    }
                }
            }

            return graphQlNodesParent;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
