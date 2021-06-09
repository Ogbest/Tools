using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
    [ProtoContract]
    [Config]
    public partial class ActiveCaseConfCategory : ProtoObject
    {
        public static ActiveCaseConfCategory Instance;
		
        [ProtoIgnore]
        [BsonIgnore]
        private Dictionary<Type, Dictionary<int, IConfig>> dicts = new Dictionary<Type, Dictionary<int, IConfig>>();
		
		[BsonElement]
		[ProtoMember(1)]
		private List<DailyTask> DailyTaskList = new List<DailyTask>();

		[BsonElement]
		[ProtoMember(2)]
		private List<BoxReward> BoxRewardList = new List<BoxReward>();


		
        public ActiveCaseConfCategory()
        {
            Instance = this;
        }
		
		[ProtoAfterDeserialization]
        public void AfterDeserialization()
        {
			dicts.Clear();
			
			Type type_DailyTask = typeof(DailyTask);
			dicts.Add(type_DailyTask, new Dictionary<int, IConfig>());
			foreach (DailyTask config in DailyTaskList)
			{
				this.dicts[type_DailyTask].Add(config.Id, config);
			}
			DailyTaskList.Clear();

			Type type_BoxReward = typeof(BoxReward);
			dicts.Add(type_BoxReward, new Dictionary<int, IConfig>());
			foreach (BoxReward config in BoxRewardList)
			{
				this.dicts[type_BoxReward].Add(config.Id, config);
			}
			BoxRewardList.Clear();


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
		public partial class DailyTask: ProtoObject, IConfig
		{
			[ProtoMember(1, IsRequired  = true)]
			public int Id { get; set; }
			[ProtoMember(2, IsRequired  = true)]
			public int TargetValue { get; set; }
			[ProtoMember(3, IsRequired  = true)]
			public int TaskIndex { get; set; }
			[ProtoMember(4, IsRequired  = true)]
			public string Award { get; set; }
			[ProtoMember(5, IsRequired  = true)]
			public int AwardExp { get; set; }
			[ProtoMember(6, IsRequired  = true)]
			public double TimeLimit { get; set; }
			[ProtoMember(7, IsRequired  = true)]
			public string Position { get; set; }
			[ProtoMember(8, IsRequired  = true)]
			public string TaskIcon { get; set; }
			[ProtoMember(9, IsRequired  = true)]
			public string TaskDir { get; set; }
			[ProtoMember(10, IsRequired  = true)]
			public int TaskProcess { get; set; }

			[ProtoAfterDeserialization]
			public void AfterDeserialization()
			{
				this.EndInit();
			}
		}


		[ProtoContract]
		public partial class BoxReward: ProtoObject, IConfig
		{
			[ProtoMember(1, IsRequired  = true)]
			public int Id { get; set; }
			[ProtoMember(2, IsRequired  = true)]
			public int Typr { get; set; }
			[ProtoMember(3, IsRequired  = true)]
			public int TaskCondition { get; set; }
			[ProtoMember(4, IsRequired  = true)]
			public string ConditionSource { get; set; }
			[ProtoMember(5, IsRequired  = true)]
			public string GoldcoinAward { get; set; }
			[ProtoMember(6, IsRequired  = true)]
			public string DiamondsAward { get; set; }
			[ProtoMember(7, IsRequired  = true)]
			public string PropAward { get; set; }
			[ProtoMember(8, IsRequired  = true)]
			public int TaskProcess { get; set; }
			[ProtoMember(9, IsRequired  = true)]
			public int TriggerCondition { get; set; }
			[ProtoMember(10, IsRequired  = true)]
			public string TriggerLconF { get; set; }
			[ProtoMember(11, IsRequired  = true)]
			public string TriggerLconA { get; set; }

			[ProtoAfterDeserialization]
			public void AfterDeserialization()
			{
				this.EndInit();
			}
		}



    }
}
