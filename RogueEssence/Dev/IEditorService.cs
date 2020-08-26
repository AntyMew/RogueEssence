using System;

namespace RogueEssence.Dev
{
    public interface IEditorService<TView>
    {
        bool Supports(Type modelType);
        void Load(object model, ref TView view);
        void Save(TView view, ref object model);
    }
}