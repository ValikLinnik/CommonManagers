using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Injection
{
    public class Context : IDisposable
    {
        #region SINGLETONE SECTION

        private Context()
        {
            
        }

        private static Context _instance;

        public static Context Instance
        {
            get
            {
                if (_instance == null) _instance = new Context();
                return _instance;
            }
        }

        #endregion

        #region PRIVATE FIELDS

        private Dictionary<Type,Component> _componentDictionary = new Dictionary<Type, Component>();
        private List<Component> _missedComponents = new List<Component>();

        #endregion

        #region PUBLIC METHODS

        public void Inject<T>(T module) where T : Component
        {
            Type type = module.GetType();

            if (!_componentDictionary.ContainsKey(type))
            {
                _componentDictionary.Add(type, module);
            }

            var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            foreach (var item in fields)
            {
                if (item.IsDefined(typeof(Inject), false))
                {
                    if (!_componentDictionary.ContainsKey(item.FieldType))
                    {
                        if (!_missedComponents.Contains(module)) _missedComponents.Add(module);
                    }
                    else
                    {
                        item.SetValue(module, _componentDictionary[item.FieldType]);
                    }
                }
            }

            AddMissedModules();
        }

        #endregion

        #region PRIVATE METHODS

        private void AddMissedModules()
        {
            if (_missedComponents.Count == 0 || _componentDictionary.Count == 0) return;

            for (int i = 0; i < _missedComponents.Count; i++)
            {
                var item = _missedComponents[i];
                Type type = item.GetType();
                var fields = type.GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

                bool isAllAdded = true;

                foreach (var field in fields)
                {
                    if (field.IsDefined(typeof(Inject), false))
                    {
                        if (!_componentDictionary.ContainsKey(field.FieldType)) isAllAdded = false;
                        else field.SetValue(item, _componentDictionary[field.FieldType]);
                    }
                }

                if (isAllAdded)
                {
                    _missedComponents.Remove(item);
                    i--;
                }
            }
        }

        #endregion

        #region IDisposable implementation

        public void Dispose()
        {
            _componentDictionary.Clear();
            _missedComponents.Clear();
        }

        #endregion
    }
}
