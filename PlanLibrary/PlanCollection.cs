using System;
using System.Collections.Generic;
using MusaCommon;
using System.Collections;

namespace PlanLibrary
{
    [Register(typeof(IPlanCollection))]
    public class PlanCollection : MusaModule, IPlanCollection
    {
        //Type -> Il tipo di piano 
        //object -> L'istanza di piano (IPlanInstance<PlanModel>)
        readonly Dictionary<Type, IPlanInstance> internalDict = new Dictionary<Type,IPlanInstance>();

        #region Properties

        public String Name { get; set; }

        public ICollection<Type> Keys
        {
            get { return internalDict.Keys; }
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

        public int Count
        {
            get { return internalDict.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        #endregion Properties

        #region Methods

        public void Add(Type key, IPlanInstance value) 
        {
            Type plan_type = typeof(PlanInstance<>).MakeGenericType(key);

            //TODO TESTAMI
            if (!value.GetType().IsEquivalentTo(plan_type))
                throw new Exception("Error: provisioned value must be a IPlanInstance with generic IPlanModel type");
            
            internalDict.Add(key, value);
        }

        public void Add(KeyValuePair<Type, IPlanInstance> item)
        {
            Type plan_type = typeof(PlanInstance<>).MakeGenericType(item.Key);
            var value = item.Value;
            if (!value.GetType().IsEquivalentTo(plan_type))
            {
                //TODO RISCRIVIMI
                throw new Exception("Error: provisioned value must be a IPlanInstance with generic IPlanModel type");
            }

            internalDict.Add(item.Key, item.Value);
        }

        public bool ContainsKey(Type key)
        {
            return internalDict.ContainsKey(key);
        }

        public bool Remove(Type key)
        {
            return internalDict.Remove(key);
        }

        public bool Remove(KeyValuePair<Type, IPlanInstance> item)
        {
            throw new NotImplementedException();
        }

        public bool TryGetValue(Type key, out IPlanInstance value)
        {
            return internalDict.TryGetValue(key, out value);
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

        public IEnumerator<KeyValuePair<Type, IPlanInstance>> GetEnumerator()
        {    
            return internalDict.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            // call the generic version of the method
            return GetEnumerator();
        }

        public void CopyTo(KeyValuePair<Type, IPlanInstance>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        #endregion Methods



    }
}

