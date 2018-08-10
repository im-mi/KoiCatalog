using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;

namespace KoiCatalog.Data
{
    public abstract class ComponentTypeCode
    {
        private const string NameOfInstanceProperty = "Instance";

        public static ComponentTypeCode Get(Type componentType)
        {
            Component.CheckComponentType(componentType);
            var componentTypeCodeType = typeof(ComponentTypeCode<>).MakeGenericType(componentType);
            var property = componentTypeCodeType.GetProperty(NameOfInstanceProperty, BindingFlags.Static | BindingFlags.NonPublic);
            Trace.Assert(property != null);
            return (ComponentTypeCode)property.GetValue(null);
        }

        public static ComponentTypeCode<T> Get<T>() where T : Component, new() =>
            ComponentTypeCode<T>.Instance;

        public int TypeCode { get; }
        public abstract Type ComponentType { get; }

        internal ComponentTypeCode(int typeCode)
        {
            TypeCode = typeCode;
        }

        internal static int CreateTypeCode() =>
            Interlocked.Increment(ref NextComponentTypeCode) - 1;
        private static int NextComponentTypeCode = 1;
    }

    public sealed class ComponentTypeCode<T> : ComponentTypeCode where T : Component, new()
    {
        internal static ComponentTypeCode<T> Instance { get; } =
            new ComponentTypeCode<T>(CreateTypeCode());

        public override Type ComponentType => typeof(T);

        private ComponentTypeCode(int typeCode) : base(typeCode)
        {
        }
    }
}
