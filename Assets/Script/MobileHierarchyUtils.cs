using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public static class MobileHierarchyUtils
{
    private static Dictionary<Type, MemberInfo[]> typeToVariables = new Dictionary<Type, MemberInfo[]>(89);
    public static HashSet<Type> serializableUnityTypes = new HashSet<Type>() { typeof( Vector2 ), typeof( Vector3 ), typeof( Vector4), typeof( Rect ), typeof( Quaternion ),
                typeof( Matrix4x4 ), typeof( Color ), typeof( Color32 ), typeof( LayerMask ), typeof( Bounds ),
                typeof( AnimationCurve ), typeof( Gradient ), typeof( RectOffset ), typeof( GUIStyle )};

    public static string ToTitleCase(this string str)
    {
        if (str == null || str.Length == 0)
            return string.Empty;

        StringBuilder titleCaser = new StringBuilder(str.Length + 5);
        byte lastCharType = 1; // 0 -> lowercase, 1 -> _ (underscore), 2 -> number, 3 -> uppercase
        int i = 0;
        char ch = str[0];
        if ((ch == 'm' || ch == 'M') && str.Length > 1 && str[1] == '_')
            i = 2;

        for (; i < str.Length; i++)
        {
            ch = str[i];
            if (char.IsUpper(ch))
            {
                if ((lastCharType < 2 || (str.Length > i + 1 && char.IsLower(str[i + 1]))) && titleCaser.Length > 0)
                    titleCaser.Append(' ');

                titleCaser.Append(ch);
                lastCharType = 3;
            }
            else if (ch == '_')
            {
                lastCharType = 1;
            }
            else if (char.IsNumber(ch))
            {
                if (lastCharType != 2 && titleCaser.Length > 0)
                    titleCaser.Append(' ');

                titleCaser.Append(ch);
                lastCharType = 2;
            }
            else
            {
                if (lastCharType == 1 || lastCharType == 2)
                {
                    if (titleCaser.Length > 0)
                        titleCaser.Append(' ');

                    titleCaser.Append(char.ToUpper(ch));
                }
                else
                    titleCaser.Append(ch);

                lastCharType = 0;
            }
        }

        if (titleCaser.Length == 0)
            return str;

        return titleCaser.ToString();
    }

    public static MemberInfo[] GetAllVariables(this Type type)
    {
        MemberInfo[] result;
        if (!typeToVariables.TryGetValue(type, out result))
        {
            BindingFlags flag = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly | BindingFlags.Instance;
            FieldInfo[] fields = type.GetFields(flag);
            PropertyInfo[] properties = type.GetProperties(flag);

            int validFieldCount = 0;
            int validPropertyCount = 0;

            for (int i = 0; i < fields.Length; i++)
            {
                FieldInfo field = fields[i];
                if (!field.IsLiteral && !field.IsInitOnly && (field.FieldType.IsSerializable()))
                    validFieldCount++;
            }

            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo property = properties[i];
                if (property.GetIndexParameters().Length == 0 && property.CanRead && property.CanWrite && property.PropertyType.IsSerializable())
                    validPropertyCount++;
            }

            int validVariableCount = validFieldCount + validPropertyCount;
            if (validVariableCount == 0)
                result = null;
            else
            {
                result = new MemberInfo[validVariableCount];

                int j = 0;
                for (int i = 0; i < fields.Length; i++)
                {
                    FieldInfo field = fields[i];
                    if (!field.IsLiteral && !field.IsInitOnly && field.FieldType.IsSerializable())
                        result[j++] = field;
                }

                for (int i = 0; i < properties.Length; i++)
                {
                    PropertyInfo property = properties[i];
                    if (property.GetIndexParameters().Length == 0 && property.CanRead && property.CanWrite && property.PropertyType.IsSerializable())
                        result[j++] = property;
                }
            }

            typeToVariables[type] = result;
        }

        return result;
    }

    public static bool IsSerializable(this Type type)
    {
#if UNITY_EDITOR || !NETFX_CORE
        if (type.IsPrimitive || type == typeof(string) || type.IsEnum)
#else
            if( type.GetTypeInfo().IsPrimitive || type == typeof( string ) || type.GetTypeInfo().IsEnum )
#endif
            return true;

        if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            return true;

        if (serializableUnityTypes.Contains(type))
            return true;

        if (type.IsArray)
        {
            if (type.GetArrayRank() != 1)
                return false;

            return type.GetElementType().IsSerializable();
        }
#if UNITY_EDITOR || !NETFX_CORE
        else if (type.IsGenericType)
#else
            else if( type.GetTypeInfo().IsGenericType )
#endif
        {
            if (type.GetGenericTypeDefinition() != typeof(List<>))
                return false;

            return type.GetGenericArguments()[0].IsSerializable();
        }

#if UNITY_EDITOR || !NETFX_CORE
        if (Attribute.IsDefined(type, typeof(SerializableAttribute), false))
#else
            if( type.GetTypeInfo().IsDefined( typeof( SerializableAttribute ), false ) )
#endif
            return true;

        return false;
    }//end func

    private static object m_builderLock = new object();
    private static StringBuilder m_builder = new StringBuilder();
    public static string combine(string main, params object[] texts)
    {
        lock (m_builderLock)
        {
            m_builder.Remove(0, m_builder.Length);
            m_builder.Append(main);
            var len = texts.Length;
            for (int i = 0; i < len; i++)
            {
                m_builder.Append(texts[i]);
            }
            return m_builder.ToString(0, m_builder.Length);
        }
    }

    public static string GenerateUId()
    {
        byte[] buffer = Guid.NewGuid().ToByteArray();
        return BitConverter.ToInt64(buffer, 0).ToString();
    }

    public static void GetChildRecursive(Transform parentTF, string childName,ref List<Transform> objs)
    {
        //在子物体中查找名为包含childName 的子物体，如果有就返回，如果没有就开始递归 ,并且屏蔽PhoneHierachyPanel
        if (PhoneHierachyPanel.PrefabName.Equals(parentTF.name) == false)
        {
            if (parentTF.name.ToLower().Contains(childName.ToLower()))
                objs.Add(parentTF);
            int count = parentTF.childCount;
            for (int i = 0; i < count; i++)
            {
                Transform child = parentTF.GetChild(i);
                GetChildRecursive(child, childName, ref objs);
            }
        }
    }


}
