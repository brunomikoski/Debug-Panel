﻿using System.Reflection;
using BrunoMikoski.DebugTools;

namespace BrunoMikoski.DebugTools
{
    internal class DebuggableEnumItem : DebuggableField
    {
        public DebuggableEnumItem(string path, FieldInfo fieldInfo, object owner, DebuggableClassAttribute classAttribute, DebuggableFieldAttribute fieldAttribute) : base(path, fieldInfo, owner, classAttribute, fieldAttribute)
        {
        }

        public DebuggableEnumItem(string path, string subTitle, FieldInfo fieldInfo, object owner, DebuggableClassAttribute classAttribute, DebuggableFieldAttribute fieldAttribute) : base(path, subTitle, fieldInfo, owner, classAttribute, fieldAttribute)
        {
        }

        public DebuggableEnumItem(string path, string subTitle, string spriteName, FieldInfo fieldInfo, object owner, DebuggableClassAttribute classAttribute, DebuggableFieldAttribute fieldAttribute) : base(path, subTitle, spriteName, fieldInfo, owner, classAttribute, fieldAttribute)
        {
        }
    }
}