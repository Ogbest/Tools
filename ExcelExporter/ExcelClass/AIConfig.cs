using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
    [ProtoContract]
    [Config]
    public partial class AIConfigCategory : ProtoObject
    {
        public static AIConfigCategory Instance;
		
        [ProtoIgnore]
        [BsonIgnore]
        private Dictionary<Type, Dictionary<int, IConfig>> dicts = new Dictionary<Type, Dictionary<int, IConfig>>();
		
		[BsonElement]
		[ProtoMember(1)]
		private List<AIConfig_1> AIConfig_1List = new List<AIConfig_1>();

		[BsonElement]
		[ProtoMember(2)]
		private List<AIConfig_2> AIConfig_2List = new List<AIConfig_2>();


		
        public AIConfigCategory()
        {
            Instance = this;
        }
		
		[ProtoAfterDeserialization]
        public void AfterDeserialization()
        {
			dicts.Clear();
			
			Type type_AIConfig_1 = typeof(AIConfig_1);
			dicts.Add(type_AIConfig_1, new Dictionary<int, IConfig>());
			foreach (AIConfig_1 config in AIConfig_1List)
			{
				this.dicts[type_AIConfig_1].Add(config.Id, config);
			}
			AIConfig_1List.Clear();

			Type type_AIConfig_2 = typeof(AIConfig_2);
			dicts.Add(type_AIConfig_2, new Dictionary<int, IConfig>());
			foreach (AIConfig_2 config in AIConfig_2List)
			{
				this.dicts[type_AIConfig_2].Add(config.Id, config);
			}
			AIConfig_2List.Clear();


			this.EndInit();
        }

        public bool ContainType(Type type)
        {
            return this.dicts.ContainsKey(type);
        }

        public bool Contain<T>(int id) where T : class, IConfig
        {
            Type type = typeof(T);
            return Contain(type, id);
        }

        public bool Contain(Type type, int id)
        {
            if (this.dicts.TryGetValue(type, out Dictionary<int, IConfig> values))
            {
                return values.ContainsKey(id);
            }
            return false;
        }

        public Dictionary<int, T> GetAll<T>() where T : class, IConfig
        {
            Type type = typeof(T);
            Dictionary<int, IConfig> configs = GetAll(type);

            if (configs == null) return null;

            Dictionary<int, T> values = new Dictionary<int, T>(configs.Count);
            foreach (var item in configs)
            {
                values.Add(item.Key, item.Value as T);
            }
            return values;
        }

        public Dictionary<int, IConfig> GetAll(Type type)
        {
            if (this.dicts.TryGetValue(type, out Dictionary<int, IConfig> value))
            {
                return value;
            }
            return null;
        }

        public T GetOne<T>(int id) where T : class, IConfig
        {
            Type type = typeof(T);
            try
            {
                IConfig config = GetOne(type, id);
                if (config != null)
                {
                    return config as T;
                }
            }
            catch (Exception)
            {

            }
            return null;
        }

        public IConfig GetOne(Type type, int id)
        {
            if (dicts.TryGetValue(type, out Dictionary<int, IConfig> dict))
            {
                if (dict.TryGetValue(id, out IConfig config))
                {
                    return config;
                }
            }
            return null;
        }
		[ProtoContract]
		public partial class AIConfig_1: ProtoObject, IConfig
		{
			[ProtoMember(1, IsRequired  = true)]
			public int Id { get; set; }
			[ProtoMember(2, IsRequired  = true)]
			public int AIConfigId { get; set; }
			[ProtoMember(3, IsRequired  = true)]
			public int Order { get; set; }
			[ProtoMember(4, IsRequired  = true)]
			public string Name { get; set; }
			[ProtoMember(5, IsRequired  = true)]
			public int[] NodeParams { get; set; }

			[ProtoAfterDeserialization]
			public void AfterDeserialization()
			{
				this.EndInit();
			}
		}


		[ProtoContract]
		public partial class AIConfig_2: ProtoObject, IConfig
		{
			[ProtoMember(1, IsRequired  = true)]
			public int Id { get; set; }
			[ProtoMember(2, IsRequired  = true)]
			public int AIConfigId { get; set; }
			[ProtoMember(3, IsRequired  = true)]
			public int Order { get; set; }
			[ProtoMember(4, IsRequired  = true)]
			public string Name { get; set; }
			[ProtoMember(5, IsRequired  = true)]
			public int[] NodeParams { get; set; }

			[ProtoAfterDeserialization]
			public void AfterDeserialization()
			{
				this.EndInit();
			}
		}



    }
}
