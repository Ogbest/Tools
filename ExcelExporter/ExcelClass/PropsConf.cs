using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using ProtoBuf;

namespace ET
{
    [ProtoContract]
    [Config]
    public partial class PropsConfCategory : ProtoObject
    {
        public static PropsConfCategory Instance;
		
        [ProtoIgnore]
        [BsonIgnore]
        private Dictionary<int, PropsConf> dict = new Dictionary<int, PropsConf>();
		
		[BsonElement]
		[ProtoMember(1)]
		private List<PropsConf> PropsConfList = new List<PropsConf>();


		
        public PropsConfCategory()
        {
            Instance = this;
        }
		
		[ProtoAfterDeserialization]
        public void AfterDeserialization()
        {
			foreach (PropsConf config in PropsConfList)
			{
				this.dict.Add(config.Id, config);
			}
			PropsConfList.Clear();

            this.EndInit();
        }
		
        public PropsConf Get(int id)
        {
            this.dict.TryGetValue(id, out PropsConf item);

            if (item == null)
            {
                throw new Exception($"配置找不到，配置表名: {nameof (PropsConf)}，配置id: {id}");
            }

            return item;
        }
		
        public bool Contain(int id)
        {
            return this.dict.ContainsKey(id);
        }

        public Dictionary<int, PropsConf> GetAll()
        {
            return this.dict;
        }

        public PropsConf GetOne()
        {
            if (this.dict == null || this.dict.Count <= 0)
            {
                return null;
            }
            return this.dict.Values.GetEnumerator().Current;
        }
    }

    [ProtoContract]
	public partial class PropsConf: ProtoObject, IConfig
	{
		[ProtoMember(1, IsRequired  = true)]
		public int Id { get; set; }
		[ProtoMember(2, IsRequired  = true)]
		public string Name { get; set; }
		[ProtoMember(3, IsRequired  = true)]
		public int UseType { get; set; }
		[ProtoMember(4, IsRequired  = true)]
		public int MaxNum { get; set; }
		[ProtoMember(5, IsRequired  = true)]
		public int Priority { get; set; }
		[ProtoMember(6, IsRequired  = true)]
		public int ShowInBag { get; set; }
		[ProtoMember(7, IsRequired  = true)]
		public int ShowInGameBag { get; set; }
		[ProtoMember(8, IsRequired  = true)]
		public int ShowIfZero { get; set; }
		[ProtoMember(9, IsRequired  = true)]
		public int BuyPrice { get; set; }
		[ProtoMember(10, IsRequired  = true)]
		public string BuyCondition { get; set; }
		[ProtoMember(11, IsRequired  = true)]
		public string BtnTypeWithProp { get; set; }
		[ProtoMember(12, IsRequired  = true)]
		public string BtnTypeWithoutProp { get; set; }
		[ProtoMember(13, IsRequired  = true)]
		public string LevelToUse { get; set; }
		[ProtoMember(14, IsRequired  = true)]
		public string Desc { get; set; }
		[ProtoMember(15, IsRequired  = true)]
		public string DescFirstDrop { get; set; }
		[ProtoMember(16, IsRequired  = true)]
		public string Icon { get; set; }
		[ProtoMember(17, IsRequired  = true)]
		public string DropIcon { get; set; }
		[ProtoMember(18, IsRequired  = true)]
		public int IsBinding { get; set; }
		[ProtoMember(19, IsRequired  = true)]
		public int VipLevelToGift { get; set; }
		[ProtoMember(20, IsRequired  = true)]
		public int BaseNumForGift { get; set; }
		[ProtoMember(21, IsRequired  = true)]
		public int LndividualWarehouse { get; set; }
		[ProtoMember(22, IsRequired  = true)]
		public int BuyNumInLobby { get; set; }

		[ProtoAfterDeserialization]
        public void AfterDeserialization()
        {
            this.EndInit();
        }
	}
}
