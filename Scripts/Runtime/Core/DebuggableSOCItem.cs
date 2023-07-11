#if SOC_ENABLED
using System.Reflection;
using BrunoMikoski.ScriptableObjectCollections;
using UnityEngine;

namespace BrunoMikoski.DebugTools
{
    internal class DebuggableSOCItem : DebuggableField
    {
        private ScriptableObject collectionItem;

        public ScriptableObject CollectionItem => collectionItem;

        public DebuggableSOCItem(string path, FieldInfo fieldInfo, object owner, DebuggableClassAttribute classAttribute, DebuggableFieldAttribute fieldAttribute) : base(path, fieldInfo, owner, classAttribute, fieldAttribute)
        {
        }

        public DebuggableSOCItem(string path, string subTitle, FieldInfo fieldInfo, object owner, DebuggableClassAttribute classAttribute, DebuggableFieldAttribute fieldAttribute) : base(path, subTitle, fieldInfo, owner, classAttribute, fieldAttribute)
        {
        }

        public DebuggableSOCItem(string path, string subTitle, string spriteName, FieldInfo fieldInfo, object owner, DebuggableClassAttribute classAttribute, DebuggableFieldAttribute fieldAttribute) : base(path, subTitle, spriteName, fieldInfo, owner, classAttribute, fieldAttribute)
        {
        }
        
        public DebuggableSOCItem(string path, FieldInfo fieldInfo, object owner, DebuggableClassAttribute classAttribute, DebuggableFieldAttribute fieldAttribute, ScriptableObject targetSOCItem) : base(path, fieldInfo, owner, classAttribute, fieldAttribute)
        {
            collectionItem = targetSOCItem;
        }
    }
}
#endif