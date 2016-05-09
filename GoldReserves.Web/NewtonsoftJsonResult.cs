using GoldReserves.Data;
using Newtonsoft.Json;
using System;
using System.Web.Mvc;

namespace GoldReserves.Web
{
    public class TonsPerPoliticalEntityJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TonsPerPoliticalEntity);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {

            var obj = (TonsPerPoliticalEntity)value;
            if (obj != null)
            {
                writer.WriteStartArray();
                writer.WriteValue(obj.PoliticalEntityId);
                writer.WriteValue(obj.Tons != null ? (double)(decimal)obj.Tons : 0.0);
                writer.WriteEndArray();
                return;
            }
            writer.WriteNull();
        }
    }

    public class NewtonsoftJsonResult : ActionResult
    {
        private JsonSerializer m_jsonSerializer;
        private object m_value;

        public NewtonsoftJsonResult(JsonSerializer jsonSerializer, object value)
        {
            if (jsonSerializer == null) throw new ArgumentNullException();
            m_jsonSerializer = jsonSerializer;
            m_value = value;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var r = context.HttpContext.Response;
            r.ContentType = "application/json";
            m_jsonSerializer.Serialize(r.Output, m_value);
        }
    }
}