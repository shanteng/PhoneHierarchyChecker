using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

public class ComponentRenderData
{
    public Component component;
    public string name;
    public bool isBehaviour;
    public bool isEnabled = false;
    public List<PropertyRenderData> propertyDatas = new List<PropertyRenderData>();


    public void CopyFromComponent(Component component)
    {
        this.component = component;
        name = component == null ? "null reference" : component.GetType().Name;
        isBehaviour = component == null ? false : component is Behaviour;
        if (isBehaviour)
        {
            isEnabled = (component as Behaviour).enabled;
        }

        propertyDatas.Clear();
        if (component != null)
        {
            Type type = component.GetType();
            MemberInfo[] infos = type == null ? null : type.GetAllVariables();
            if (infos != null)
            {
                for (int i = 0; i < infos.Length; i++)
                {
                    var data = new PropertyRenderData();
                    data.CopyFromMemberInfo(infos[i], component);
                    propertyDatas.Add(data);
                }
            }
        }
    }//end func
}
