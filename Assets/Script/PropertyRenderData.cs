using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

public class PropertyRenderData
{
    public const string INDENT_SPACE = "    ";
    public string name;
    public string value;

    public void CopyFromMemberInfo(MemberInfo memInfo, UnityEngine.Object targetComponent)
    {
        value = "null";
        object propertyValue = null;
        if (memInfo is PropertyInfo)
        {
            PropertyInfo proInfo = memInfo as PropertyInfo;
            name = proInfo.Name.ToTitleCase();
            propertyValue = proInfo.GetValue(targetComponent, null);
        }
        else if (memInfo is FieldInfo)
        {
            FieldInfo fInfo = memInfo as FieldInfo;
            name = fInfo.Name.ToTitleCase();
            propertyValue = fInfo.GetValue(targetComponent);
        }

        if (propertyValue != null)
        {
            if (propertyValue.GetType().IsValueType)
            {
                value = propertyValue.ToString();
            }
            else
            {
                //针对不同的类型进行特殊处理
                 if (propertyValue is List<int>)
                {
                    value = this.GetListIntFormat(propertyValue as List<int>);
                }
                else if (propertyValue is List<string>)
                {
                    value = this.GetListStringFormat(propertyValue as List<string>);
                }
              
                else
                {
                    value = propertyValue + "(" + propertyValue.ToString() + ")";
                }
            }//end else
        }
    }//end func

    private string GetListIntFormat(List<int> datalist)
    {
        StringBuilder allCodeBuilder = new StringBuilder();
        int count = datalist.Count;
        for (int i = 0; i < count; ++i)
        {
            allCodeBuilder.Append(MobileHierarchyUtils.combine("Element", i, ":", datalist[i].ToString(), "\n"));
        }
        return allCodeBuilder.ToString();
    }

    private string GetListStringFormat(List<string> datalist)
    {
        StringBuilder allCodeBuilder = new StringBuilder();
        int count = datalist.Count;
        for (int i = 0; i < count; ++i)
        {
            allCodeBuilder.Append(MobileHierarchyUtils.combine("Element", i, ":", datalist[i].ToString(), "\n"));
        }
        return allCodeBuilder.ToString();
    }

}//end class
