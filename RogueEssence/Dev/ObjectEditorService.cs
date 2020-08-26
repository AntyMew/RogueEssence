using System;
using System.Reflection;

namespace RogueEssence.Dev
{
    public abstract class ObjectEditorService<TView> : IEditorService<TView>
    {
        protected EditorServiceProvider<TView> Provider { get; }

        protected abstract bool ShowNonPublic { get; }

        protected bool ShowAsMember { get; private set; }

        protected ObjectEditorService(EditorServiceProvider<TView> provider)
        {
            Provider = provider;
        }

        public abstract bool Supports(Type modelType);

        public void Load(object model, ref TView view)
        {
            foreach (MemberInfo member in GetStatefulMembers(model.GetType()))
            {
                if (!LoadMember(member, model, ref view))
                {
                    var service = GetDefaultMemberEditor(member.GetMemberInfoType());
                    service.Load(model, ref view);
                }
            }
        }

        public void Save(TView view, ref object model)
        {
            foreach (MemberInfo member in GetStatefulMembers(model.GetType()))
            {
                if (!SaveMember(member, view, ref model))
                {
                    var service = GetDefaultMemberEditor(member.GetMemberInfoType());
                    service.Save(view, ref model);
                }
            }
        }

        protected abstract bool LoadMember(MemberInfo info, object model, ref TView view);

        protected abstract bool SaveMember(MemberInfo info, TView view, ref object model);

        private MemberInfo[] GetStatefulMembers(Type type)
        {
            var types = MemberTypes.Field | MemberTypes.Property;
            var flags = BindingFlags.Public | BindingFlags.Instance;
            if (ShowNonPublic)
                flags |= BindingFlags.NonPublic;
            return type.FindMembers(types, flags, null, null);
        }

        private IEditorService<TView> GetDefaultMemberEditor(Type type)
        {
            var service = Provider.Get(type);
            if (service is ObjectEditorService<TView> oes)
                oes.ShowAsMember = true;
            return service;
        }
    }
}