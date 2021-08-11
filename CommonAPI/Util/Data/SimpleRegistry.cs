﻿using System;
using System.Collections.Generic;
using System.IO;

namespace CommonAPI
{
    public class SimpleRegistry : ISerializeState
    {
        public Dictionary<string, int> idMap = new Dictionary<string, int>();
        
        public Dictionary<int, int> migrationMap = new Dictionary<int, int>();
        public List<string> removedIds = new List<string>();

        protected int lastId;
        
        public virtual int Register(string key, Type item = null)
        {
            if (!idMap.ContainsKey(key))
            {
                idMap.Add(key, ++lastId);
                return lastId;
            }

            return GetUniqueId(key);
        }

        public int GetUniqueId(string typeId)
        {
            if (idMap.ContainsKey(typeId))
                return idMap[typeId];
            return 0;
        }

        public int MigrateId(int oldId)
        {
            if (migrationMap.ContainsKey(oldId))
            {
                return migrationMap[oldId];
            }

            return 0;
        }

        public void Free()
        {
        }

        public void Export(BinaryWriter w)
        {
            w.Write((byte)0);
            w.Write(idMap.Count);
            foreach (var kv in idMap)
            {
                w.Write(kv.Key);
                w.Write(kv.Value);
            }
        }

        public void Import(BinaryReader r)
        {
            r.ReadByte();
            int count = r.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                string key = r.ReadString();
                int oldId = r.ReadInt32();
                int newId = GetUniqueId(key);
                if (newId == 0)
                {
                    removedIds.Add(key);
                }
                else
                {
                    migrationMap.Add(oldId, newId);
                }
            }
        }
    }
}