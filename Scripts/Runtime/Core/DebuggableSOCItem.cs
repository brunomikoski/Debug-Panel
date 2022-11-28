#if SOC_ENABLED
using System.Reflection;
using BrunoMikoski.ScriptableObjectCollections;

namespace BrunoMikoski.DebugTools
{
    internal class DebuggableSOCItem : DebuggableField
    {
        private ScriptableObjectCollectionItem collectionItem;

        public ScriptableObjectCollectionItem CollectionItem => collectionItem;

        public DebuggableSOCItem(string path, FieldInfo fieldInfo, object owner, DebuggableClassAttribute classAttribute, DebuggableFieldAttribute fieldAttribute) : base(path, fieldInfo, owner, classAttribute, fieldAttribute)
        {
        }

        public DebuggableSOCItem(string path, string subTitle, FieldInfo fieldInfo, object owner, DebuggableClassAttribute classAttribute, DebuggableFieldAttribute fieldAttribute) : base(path, subTitle, fieldInfo, owner, classAttribute, fieldAttribute)
        {
        }

        public DebuggableSOCItem(string path, string subTitle, string spriteName, FieldInfo fieldInfo, object owner, DebuggableClassAttribute classAttribute, DebuggableFieldAttribute fieldAttribute) : base(path, subTitle, spriteName, fieldInfo, owner, classAttribute, fieldAttribute)
        {
        }
        
        public DebuggableSOCItem(string path, FieldInfo fieldInfo, object owner, DebuggableClassAttribute classAttribute, DebuggableFieldAttribute fieldAttribute, ScriptableObjectCollectionItem targetSOCItem) : base(path, fieldInfo, owner, classAttribute, fieldAttribute)
        {
            collectionItem = targetSOCItem;
        }
    }
}
#endif