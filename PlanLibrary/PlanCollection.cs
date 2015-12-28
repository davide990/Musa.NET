using System;
using System.Collections.Generic;
using MusaCommon;

namespace PlanLibrary
{
    public class PlanCollection
    {
        //Type -> Il tipo di piano 
        //object -> L'istanza di piano (IPlanInstance<PlanModel>)
        readonly Dictionary<Type, IPlanInstance> internalDict = new Dictionary<Type,IPlanInstance>();

        public String Name { get; set; }

        public void Add(Type key, IPlanInstance value) 
        {
            Type plan_type = typeof(PlanInstance<>).MakeGenericType(key);

            //TODO TESTAMI
            if (!value.GetType().IsEquivalentTo(plan_type))
                throw new Exception("Error: provisioned value must be a IPlanInstance with generic IPlanModel type");
            
            internalDict.Add(key, value);
        }

        public bool ContainsKey(Type key)
        {
            return internalDict.ContainsKey(key);
        }

        public ICollection<Type> Keys
        {
            get { return internalDict.Keys; }
        }

        public bool Remove(Type key)
        {
            return internalDict.Remove(key);
        }

        public bool TryGetValue(Type key, out IPlanInstance value)
        {
            return internalDict.TryGetValue(key, out value);
        }

        public ICollection<IPlanInstance> Values
        {
            get { return internalDict.Values; }
        }

        public IPlanInstance this [Type key]
        {
            get
            {
                return internalDict[key];
            }
            set
            {
                internalDict[key] = value;
            }
        }


        #region ICollection<KeyValuePair<string,object>> Members

        public void Add(KeyValuePair<Type, IPlanInstance> item)
        {
            Type plan_type = typeof(PlanInstance<>).MakeGenericType(item.Key);
            var value = item.Value;
            if (!value.GetType().IsEquivalentTo(plan_type))
            {
                //TODO RISCRIVIMI
                throw new Exception("Error: provisioned value must be a " +
                    "IPlanInstance with generic IPlanModel type");
            }

            internalDict.Add(item.Key, item.Value);
        }

        public void Clear()
        {
            internalDict.Clear();
        }

        public bool Contains(KeyValuePair<Type, IPlanInstance> item)
        {
            return (internalDict.ContainsKey(item.Key) &&
            internalDict.ContainsValue(item.Value));
        }

        public int Count
        {
            get { return internalDict.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(KeyValuePair<string, IPlanInstance> item)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IEnumerable<KeyValuePair<string,object>> Members

        public IEnumerator<KeyValuePair<Type, IPlanInstance>> GetEnumerator()
        {    
            return internalDict.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        #endregion

        public void CopyTo(KeyValuePair<Type, IPlanInstance>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }




    }
}

