using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
    [ProtoContract]
    [Config]
    public partial class VipPrivilegeConfCategory : ProtoObject
    {
        public static VipPrivilegeConfCategory Instance;
		
        [ProtoIgnore]
        [BsonIgnore]
        private Dictionary<Type, Dictionary<int, IConfig>> dicts = new Dictionary<Type, Dictionary<int, IConfig>>();
		
		[BsonElement]
		[ProtoMember(1)]
		private List<Privilege> PrivilegeList = new List<Privilege>();

		[BsonElement]
		[ProtoMember(2)]
		private List<Special> SpecialList = new List<Special>();


		
        public VipPrivilegeConfCategory()
        {
            Instance = this;
        }
		
		[ProtoAfterDeserialization]
        public void AfterDeserialization()
        {
			dicts.Clear();
			
			Type type_Privilege = typeof(Privilege);
			dicts.Add(type_Privilege, new Dictionary<int, IConfig>());
			foreach (Privilege config in PrivilegeList)
			{
				this.dicts[type_Privilege].Add(config.Id, config);
			}
			PrivilegeList.Clear();

			Type type_Special = typeof(Special);
			dicts.Add(type_Special, new Dictionary<int, IConfig>());
			foreach (Special config in SpecialList)
			{
				this.dicts[type_Special].Add(config.Id, config);
			}
			SpecialList.Clear();


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
		public partial class Privilege: ProtoObject, IConfig
		{
			[ProtoMember(1, IsRequired  = true)]
			public int Id { get; set; }
			[ProtoMember(2, IsRequired  = true)]
			public int Money { get; set; }
			[ProtoMember(3, IsRequired  = true)]
			public double FreeMoneyDaily { get; set; }
			[ProtoMember(4, IsRequired  = true)]
			public string Desc { get; set; }
			[ProtoMember(5, IsRequired  = true)]
			public double Probability { get; set; }
			[ProtoMember(6, IsRequired  = true)]
			public int InfullMaxLimit { get; set; }
			[ProtoMember(7, IsRequired  = true)]
			public string AwardOnce { get; set; }
			[ProtoMember(8, IsRequired  = true)]
			public int ShoppingCrazyMode { get; set; }
			[ProtoMember(9, IsRequired  = true)]
			public string SpecialAward { get; set; }
			[ProtoMember(10, IsRequired  = true)]
			public double SaleWeaponMoneyLimit { get; set; }
			[ProtoMember(11, IsRequired  = true)]
			public int FreeCannonNum { get; set; }
			[ProtoMember(12, IsRequired  = true)]
			public string AwardWeaponId { get; set; }
			[ProtoMember(13, IsRequired  = true)]
			public string BatterySkin { get; set; }
			[ProtoMember(14, IsRequired  = true)]
			public string SkinEffects { get; set; }

			[ProtoAfterDeserialization]
			public void AfterDeserialization()
			{
				this.EndInit();
			}
		}


		[ProtoContract]
		public partial class Special: ProtoObject, IConfig
		{
			[ProtoMember(1, IsRequired  = true)]
			public int Id { get; set; }
			[ProtoMember(2, IsRequired  = true)]
			public int Vip { get; set; }
			[ProtoMember(3, IsRequired  = true)]
			public int SpecialOffer { get; set; }
			[ProtoMember(4, IsRequired  = true)]
			public string GoldCoins { get; set; }
			[ProtoMember(5, IsRequired  = true)]
			public string Propr1 { get; set; }
			[ProtoMember(6, IsRequired  = true)]
			public string Propr2 { get; set; }
			[ProtoMember(7, IsRequired  = true)]
			public string Propr3 { get; set; }

			[ProtoAfterDeserialization]
			public void AfterDeserialization()
			{
				this.EndInit();
			}
		}



    }
}
