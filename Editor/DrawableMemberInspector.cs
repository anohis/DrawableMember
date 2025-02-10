namespace DrawableMember.Editor
{
    using UnityEditor;

    [CustomEditor(typeof(UnityEngine.Object), true)]
    [CanEditMultipleObjects]
    public class DrawableMemberInspector : Editor
    {
        private IMemberDrawer _drawer;

        private void OnEnable()
        {
            _drawer = new DrawableMemberDrawer(
                new MemberDrawerFactory().Create(target.GetType()));
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            if (targets.Length == 1)
            {
                _drawer.Draw(target);
            }
        }
    }
}