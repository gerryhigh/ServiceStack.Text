using System;
using System.Collections.Generic;
using NUnit.Framework;
using ServiceStack.Common.Tests.Models;

namespace ServiceStack.Text.Tests.JsonTests
{
	[TestFixture]
	public class BasicJsonTests
		: TestBase
	{
		public class JsonPrimitives
		{
			public int Int { get; set; }
			public long Long { get; set; }
			public float Float { get; set; }
			public double Double { get; set; }
			public bool Boolean { get; set; }
			public DateTime DateTime { get; set; }
			public string NullString { get; set; }

			public static JsonPrimitives Create(int i)
			{
				return new JsonPrimitives
				{
					Int = i,
					Long = i,
					Float = i,
					Double = i,
					Boolean = i % 2 == 0,
					DateTime = new DateTime(DateTimeExtensions.UnixEpoch + (i * DateTimeExtensions.TicksPerMs), DateTimeKind.Utc),
				};
			}
		}

		[Test]
		public void Can_handle_json_primitives()
		{
			var json = JsonSerializer.SerializeToString(JsonPrimitives.Create(1));
			Log(json);

			Assert.That(json, Is.EqualTo(
				"{\"Int\":1,\"Long\":1,\"Float\":1,\"Double\":1,\"Boolean\":false,\"DateTime\":\"\\/Date(1+0000)\\/\"}"));
		}

		[Test]
		public void Can_parse_json_with_nulls()
		{
			const string json = "{\"Int\":1,\"NullString\":null}";
			var value = JsonSerializer.DeserializeFromString<JsonPrimitives>(json);

			Assert.That(value.Int, Is.EqualTo(1));
			Assert.That(value.NullString, Is.Null);
		}

        [Test]
        public void Can_serialize_dictionary_of_int_int()
        {
            var json = JsonSerializer.SerializeToString<IntIntDictionary>(new IntIntDictionary() {Dictionary = {{10,100},{20,200}}});
            const string expected = "{\"Dictionary\":{\"10\":100,\"20\":200}}";
            Assert.That(json,Is.EqualTo(expected));
        }

        private class IntIntDictionary
        {
            public IntIntDictionary()
            {
                Dictionary = new Dictionary<int, int>();
            }
            public IDictionary<int,int> Dictionary { get; set; }
        }

        [Test]
        public void Serialize_skips_null_values_by_default()
        {
            var o = new {
                Name = "Brandon",
                Type = "Programmer",
                SampleKey = 12,
                Nothing = (string)null
            };
            
            var s = JsonSerializer.SerializeToString(o);
            Assert.That(s, Is.EqualTo("{\"Name\":\"Brandon\",\"Type\":\"Programmer\",\"SampleKey\":12}"));
        }

        [Test]
        public void Serialize_can_include_null_values()
        {
            var o = new {
                Name = "Brandon",
                Type = "Programmer",
                SampleKey = 12,
                Nothing = (string)null
            };
            
            JsConfig.WriteNullValues = true;
            var s = JsonSerializer.SerializeToString(o);
            JsConfig.WriteNullValues = false;
            Assert.That(s, Is.EqualTo("{\"Name\":\"Brandon\",\"Type\":\"Programmer\",\"SampleKey\":12,\"Nothing\":null}"));
        }
	}
}