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
        
        public static void GetChdFieldTypesAndValues(object inputObject)
        {
            if (inputObject == null) return;
            var chdType = inputObject.GetType();

        }
        
        public static bool PublicInstancePropertiesEqual<T>(T self, T to, params string[] ignore) where T : class
        {
            if (self != null && to != null)
            {
                Type type = typeof(T);
                List<string> ignoreList = new List<string>(ignore);
                foreach (System.Reflection.PropertyInfo pi in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                {
                    if (!ignoreList.Contains(pi.Name))
                    {
                        object selfValue = type.GetProperty(pi.Name).GetValue(self, null);
                        object toValue = type.GetProperty(pi.Name).GetValue(to, null);

                        if (selfValue != toValue && (selfValue == null || !selfValue.Equals(toValue)))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            return self == to;
        }
        public static Type GetUnderlyingType(this MemberInfo member)
            {
                switch (member.MemberType)
                {
                    case MemberTypes.Event:
                        return ((EventInfo)member).EventHandlerType;
                    case MemberTypes.Field:
                        return ((FieldInfo)member).FieldType;
                    case MemberTypes.Method:
                        return ((MethodInfo)member).ReturnType;
                    case MemberTypes.Property:
                        return ((PropertyInfo)member).PropertyType;
                    default:
                        throw new ArgumentException
                        (
                           "Input MemberInfo must be if type EventInfo, FieldInfo, MethodInfo, or PropertyInfo"
                        );
                }
            }
        }
}
