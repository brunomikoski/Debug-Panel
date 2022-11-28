using System.Reflection;
using BrunoMikoski.DebugTools;

namespace BrunoMikoski.DebugTools
{
    internal class DebuggableField : DebuggableItemBase
    {
        private readonly FieldInfo fieldInfo;
        public FieldInfo FieldInfo => fieldInfo;

        private object owner;
        public object Owner => owner;

        private DebuggableClassAttribute classAttribute;
        public DebuggableClassAttribute ClassAttribute => classAttribute;

        private DebuggableFieldAttribute fieldAttribute;

        public DebuggableFieldAttribute FieldAttribute => fieldAttribute;


        public DebuggableField(string path, FieldInfo fieldInfo, object owner, DebuggableClassAttribute classAttribute,
            DebuggableFieldAttribute fieldAttribute) : base(path)
        {
            this.fieldInfo = fieldInfo;
            this.owner = owner;
            this.classAttribute = classAttribute;
            this.fieldAttribute = fieldAttribute;
        }

        public DebuggableField(string path, string subTitle, FieldInfo fieldInfo, object owner,
            DebuggableClassAttribute classAttribute, DebuggableFieldAttribute fieldAttribute) : base(path, subTitle)
        {
            this.fieldInfo = fieldInfo;
            this.owner = owner;
            this.classAttribute = classAttribute;
            this.fieldAttribute = fieldAttribute;
        }

        public DebuggableField(string path, string subTitle, string spriteName, FieldInfo fieldInfo, object owner,
            DebuggableClassAttribute classAttribute, DebuggableFieldAttribute fieldAttribute) : base(path, subTitle)
        {
            this.fieldInfo = fieldInfo;
            this.owner = owner;
            this.classAttribute = classAttribute;
            this.fieldAttribute = fieldAttribute;
        }
        
        
        // public DebuggableField(FieldInfo fieldInfo,  object targetOwner, DebuggableClassAttribute targetClass, DebuggableFieldAttribute fieldAttribute)
        // {
        //     this.fieldInfo = fieldInfo;
        //     owner = targetOwner;
        //     classAttribute = targetClass;
        //     
        //     title = fieldInfo.Name;
        //     if (!string.IsNullOrEmpty(fieldAttribute.Title))
        //         title = fieldAttribute.Title;
        //
        //     this.fieldAttribute = fieldAttribute;
        // }
    }
}
