using R3;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Gast.Lib.AI.Editor.Debugging
{
    public class AIDebuggerWindow : EditorWindow
    {
        [SerializeField] VisualTreeAsset visualTreeAsset;

        AIDebuggerViewModel viewModel;

        ActorListView actorListView;
        DomainToolbarView domainToolbarView;
        WorldStateView worldStateView;
        CurrentPlanView currentPlanView;
        LogListView logListView;

        protected virtual void OnEnable()
        {
            viewModel = new AIDebuggerViewModel();
        }

        protected virtual void OnDisable()
        {
            viewModel?.Dispose();
        }

        void OnDestroy()
        {
            actorListView?.Dispose();
            domainToolbarView?.Dispose();
            worldStateView?.Dispose();
            currentPlanView?.Dispose();
            logListView?.Dispose();
        }

        public virtual void CreateGUI()
        {
            var root = rootVisualElement;
            root.Clear();

            if (visualTreeAsset == null)
            {
                root.Add(new Label("VisualTreeAsset is not assigned"));
                return;
            }

            visualTreeAsset.CloneTree(root);

            actorListView = root.Q<ActorListView>();
            domainToolbarView = root.Q<DomainToolbarView>();
            worldStateView = root.Q<WorldStateView>();
            currentPlanView = root.Q<CurrentPlanView>();
            logListView = root.Q<LogListView>();

            actorListView.Bind(viewModel);
            domainToolbarView.Bind(viewModel);
            worldStateView.Bind(viewModel);
            currentPlanView.Bind(viewModel);
            logListView.Bind(viewModel);
        }

        [MenuItem("Gast/Tools/AI Debugger")]
        public static void ShowWindow()
        {
            GetWindow<AIDebuggerWindow>("AI Debugger");
        }
    }
}
