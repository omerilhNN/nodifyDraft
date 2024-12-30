using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Nodify.Calculator.Helpers
{
    public static class ReflectionTools
    {
        public static List<Type> GetSubClasses<T>()
        { 
            Assembly msgAgentAssembly = typeof(DriverBase_Platform.FuncMsg.discreteIn.discreteIn_init).Assembly;

            var types =
                from type in msgAgentAssembly.GetTypes()
                where type.IsClass && !type.IsAbstract && type.IsSubclassOf(typeof(T)) &&
                type.Namespace != null && type.Namespace.StartsWith("DriverBase_Platform.FuncMsg")
                select type;
            var subclasses = types.ToList();
            return subclasses;
        }
        /// <summary>
        /// Get chd fields with TYPE subClass parameter
        /// </summary>
        /// <param name="subClass"></param>
        /// <returns></returns>
        public static List<FieldInfo> GetCHDFields(Type subClass)
        {
            List<FieldInfo> chdFieldsList= new List<FieldInfo>();
            object classInstance = null;

            if (subClass.GetConstructor(Type.EmptyTypes) != null)
            {
                classInstance = Activator.CreateInstance(subClass); //Runtime'da instance oluştur - field.GetValue için instance oluşturulması gerekir
            }
            var fields = subClass.GetFields(BindingFlags.Public | BindingFlags.NonPublic |BindingFlags.Instance); // Instance: bir sınıfın örnek üyelerine erişmek için kullanılır
            foreach(var field in fields)
            {
                if(classInstance != null)
                {
                    var fieldValue = field.GetValue(classInstance);
                    if(fieldValue != null && field.Name == "chd")
                    {
                        var chdFields = field.FieldType.GetFields(BindingFlags.Public | BindingFlags.NonPublic |
                            BindingFlags.Instance | BindingFlags.Static);
                        foreach(var chdField in chdFields)
                        {
                            chdFieldsList.Add(chdField);
                        }
                    }
                }
            }
            return chdFieldsList;
        }
        public static bool IsSubclassOf(Type type, Type baseType)
        {
            if (type == null || baseType == null) return false;

            return type.IsClass && type.IsSubclassOf(baseType);
        }
        public static bool ArePropertiesEqual<T>(T obj1, T obj2)
        {
            if (obj1 == null || obj2 == null) return false;

            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var value1 = property.GetValue(obj1);
                var value2 = property.GetValue(obj2);

                if (value1 is IEnumerable<object> enumerable1 && value2 is IEnumerable<object> enumerable2)
                {
                    if (!enumerable1.SequenceEqual(enumerable2)) return false;
                }
                else if (!Equals(value1, value2))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
