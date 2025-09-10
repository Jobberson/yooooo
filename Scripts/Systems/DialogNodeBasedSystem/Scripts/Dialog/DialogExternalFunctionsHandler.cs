using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace cherrydev
{
    public class DialogExternalFunctionsHandler
    {
        public delegate object ExternalFunction();

        private readonly Dictionary<string, ExternalFunction> _externals = new();

        public object CallExternalFunction(string funcName)
        {
            if (_externals.TryGetValue(funcName, out ExternalFunction external))
            {
                try
                {
                    // Invoke and return the result (may be null or an IEnumerator)
                    return external?.Invoke();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Exception when calling external function '{funcName}': {ex}");
                    return null;
                }
            }
            else
            {
                Debug.LogWarning($"There is no function with name '{funcName}'");
                return null;
            }
        }

        public void BindExternalFunction(string funcName, Action function)
        {
            BindExternalFunctionBase(funcName, () =>
            {
                function();
                return null;
            });
        }

        public void BindExternalFunction(string funcName, Func<IEnumerator> function)
        {
            BindExternalFunctionBase(funcName, () =>
            {
                return (object)function();
            });
        }

        public void UnbindExternalFunction(string funcName)
        {
            if (_externals.ContainsKey(funcName))
                _externals.Remove(funcName);
        }

        private void BindExternalFunctionBase(string funcName, ExternalFunction externalFunction)
        {
            if (_externals.ContainsKey(funcName))
                _externals.Remove(funcName);

            _externals[funcName] = externalFunction;
        }
    }
}