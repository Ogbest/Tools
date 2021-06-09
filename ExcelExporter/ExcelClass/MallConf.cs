using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
    [ProtoContract]
    [Config]
    public partial class MallConfCategory : ProtoObject
    {
        public static MallConfCategory Instance;
		
        [ProtoIgnore]
        [BsonIgnore]
        private Dictionary<Type, Dictionary<int, IConfig>> dicts = new Dictionary<Type, Dictionary<int, IConfig>>();
		
		[BsonElement]
		[ProtoMember(1)]
		private List<Preferential> PreferentialList = new List<Preferential>();

		[BsonElement]
		[ProtoMember(2)]
		private List<Money> MoneyList = new List<Money>();

		[BsonElement]
		[ProtoMember(3)]
		private List<Diamond> DiamondList = new List<Diamond>();

		[BsonElement]
		[ProtoMember(4)]
		private List<BatterySkin> BatterySkinList = new List<BatterySkin>();


		
        public MallConfCategory()
        {
            Instance = this;
        }
		
		[ProtoAfterDeserialization]
        public void AfterDeserialization()
        {
			dicts.Clear();
			
			Type type_Preferential = typeof(Preferential);
			dicts.Add(type_Preferential, new Dictionary<int, IConfig>());
			foreach (Preferential config in PreferentialList)
			{
				this.dicts[type_Preferential].Add(config.Id, config);
			}
			PreferentialList.Clear();

			Type type_Money = typeof(Money);
			dicts.Add(type_Money, new Dictionary<int, IConfig>());
			foreach (Money config in MoneyList)
			{
				this.dicts[type_Money].Add(config.Id, config);
			}
			MoneyList.Clear();

			Type type_Diamond = typeof(Diamond);
			dicts.Add(type_Diamond, new Dictionary<int, IConfig>());
			foreach (Diamond config in DiamondList)
			{
				this.dicts[type_Diamond].Add(config.Id, config);
			}
			DiamondList.Clear();

			Type type_BatterySkin = typeof(BatterySkin);
			dicts.Add(type_BatterySkin, new Dictionary<int, IConfig>());
			foreach (BatterySkin config in BatterySkinList)
			{
				this.dicts[type_BatterySkin].Add(config.Id, config);
			}
			BatterySkinList.Clear();


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
		public partial class Preferential: ProtoObject, IConfig
		{
			[ProtoMember(1, IsRequired  = true)]
			public int Id { get; set; }
			[ProtoMember(2, IsRequired  = true)]
			public int Preferential1 { get; set; }
			[ProtoMember(3, IsRequired  = true)]
			public int Index { get; set; }
			[ProtoMember(4, IsRequired  = true)]
			public int Multiple { get; set; }
			[ProtoMember(5, IsRequired  = true)]
			public string GoldCoins { get; set; }
			[ProtoMember(6, IsRequired  = true)]
			public string Propr { get; set; }

			[ProtoAfterDeserialization]
			public void AfterDeserialization()
			{
				this.EndInit();
			}
		}


		[ProtoContract]
		public partial class Money: ProtoObject, IConfig
		{
			[ProtoMember(1, IsRequired  = true)]
			public int Id { get; set; }
			[ProtoMember(2, IsRequired  = true)]
			public int Preferential { get; set; }
			[ProtoMember(3, IsRequired  = true)]
			public string GoldCoins { get; set; }
			[ProtoMember(4, IsRequired  = true)]
			public string Additional { get; set; }

			[ProtoAfterDeserialization]
			public void AfterDeserialization()
			{
				this.EndInit();
			}
		}


		[ProtoContract]
		public partial class Diamond: ProtoObject, IConfig
		{
			[ProtoMember(1, IsRequired  = true)]
			public int Id { get; set; }
			[ProtoMember(2, IsRequired  = true)]
			public int Preferential { get; set; }
			[ProtoMember(3, IsRequired  = true)]
			public string GoldCoins { get; set; }
			[ProtoMember(4, IsRequired  = true)]
			public string Additional { get; set; }

			[ProtoAfterDeserialization]
			public void AfterDeserialization()
			{
				this.EndInit();
			}
		}


		[ProtoContract]
		public partial class BatterySkin: ProtoObject, IConfig
		{
			[ProtoMember(1, IsRequired  = true)]
			public int Id { get; set; }
			[ProtoMember(2, IsRequired  = true)]
			public int Price { get; set; }
			[ProtoMember(3, IsRequired  = true)]
			public int SpecialUi { get; set; }
			[ProtoMember(4, IsRequired  = true)]
			public string BatteryUi { get; set; }
			[ProtoMember(5, IsRequired  = true)]
			public string BatteryAction { get; set; }
			[ProtoMember(6, IsRequired  = true)]
			public string Bullet { get; set; }
			[ProtoMember(7, IsRequired  = true)]
			public string Hit { get; set; }
			[ProtoMember(8, IsRequired  = true)]
			public int Time { get; set; }
			[ProtoMember(9, IsRequired  = true)]
			public string BackgTound { get; set; }
			[ProtoMember(10, IsRequired  = true)]
			public string Animation { get; set; }
			[ProtoMember(11, IsRequired  = true)]
			public string TextSpecial { get; set; }

			[ProtoAfterDeserialization]
			public void AfterDeserialization()
			{
				this.EndInit();
			}
		}



    }
}
