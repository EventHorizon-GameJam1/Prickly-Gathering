using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    //Singleton
    public static InputManager Instance;

    #region EVENTS
    //---------------------------GAMEPLAY---------------------------
    //Movement
    public delegate void DirectionChanged(Vector3 direction);
    public static event DirectionChanged OnDirectionChanged = (Vector3 direction) => { };
    //Sprint
    public delegate void SprintCalled();
    public static event SprintCalled OnSprint = () => { };
    public static event SprintCalled OnSprintCancelled = () => { };
    //Jump
    public delegate void JumpCalled();
    public static event JumpCalled OnJump = () => { };
    //Parry
    public delegate void ParryCalled();
    public static event ParryCalled OnParry = () => { };
    //-----------------------------GAME-----------------------------
    //Menu
    public delegate void GameMenuCalled();
    public static event GameMenuCalled OnMenuCalled = () => { };
    //Interact
    public delegate void IntercatCalled();
    public static event IntercatCalled OnSelect = () => { };
    //------------------------------UI------------------------------
    //UI Movement
    //public delegate void UI_DirectionChanged(Vector3 direction);
    //public static event UI_DirectionChanged OnUIDirectionChanged = (Vector3 direction) => { };
    //UI Select
    //public delegate void UI_Select();
    //public static event UI_Select OnUISelect = () => { };
    //--------------------------------------------------------------
    #endregion

    //Serialized Var
    [SerializeField] private bool StartOnMenu = false;

    private InputMap InputActions;

    private void Awake()
    {
        //Set Up Singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogWarning("WARNING: MULTIPLE INPUT MANAGER FOUND!");
            Destroy(this.gameObject);
            return;
        }

        //setup InputMap
        InputActions = new InputMap();
        
        //set up enabled action
        if (StartOnMenu)
        {
            InputActions.Gameplay.Disable();
            InputActions.Menu.Enable();
        }
        else
        {
            InputActions.Gameplay.Enable();
            InputActions.Menu.Disable();
        }
    }

    private void Start()
    {
        if (StartOnMenu)
            OnMenuCalled();
    }

    private void Update()
    {
        Vector3 dir = Vector3.zero;
        if (InputActions.Gameplay.Movement.inProgress)
            dir  = InputActions.Gameplay.Movement.ReadValue<Vector3>();
        
        OnDirectionChanged(dir);
    }

    private void MenuInput()
    {
        InputActions.Gameplay.Disable();
        InputActions.Menu.Enable();
    }

    private void GameInput()
    {
        InputActions.Gameplay.Enable();
        InputActions.Menu.Disable();
    }

    #region EVENTS FUNCTIONS
    //---------------------------GAMEPLAY---------------------------
    //Movement
    private void CallMovement(InputAction.CallbackContext context) => OnDirectionChanged(context.ReadValue<Vector3>());
    //Sprint
    private void CallSprint(InputAction.CallbackContext context) => OnSprint();
    private void CancelSprint(InputAction.CallbackContext context) => OnSprintCancelled();
    //Jump
    //private void CallJump(InputAction.CallbackContext context) => OnJump();
    //Parry
    private void CallParry(InputAction.CallbackContext context) => OnParry();
    //-----------------------------GAME-----------------------------
    //Menu
    private void CallMenu(InputAction.CallbackContext context)
    {
        OnMenuCalled();

        if (GameManager.OnPause)
            MenuInput();
        else
            GameInput();
    }
    //Select
    private void CallSelect(InputAction.CallbackContext context) => OnSelect();
    //------------------------------UI------------------------------
    //UI Movement
    //private void CallUIMovement(InputAction.CallbackContext context) => OnUIDirectionChanged(context.ReadValue<Vector2>());
    //UI Select
    //private void CallUISelect(InputAction.CallbackContext context) => OnUISelect();
    //--------------------------------------------------------------
    #endregion

    private void OnEnable()
    {
        #region Events Setup
        //Setup events
        //---------------------------GAMEPLAY---------------------------
        //Movement
        InputActions.Gameplay.Movement.performed += CallMovement;
        //Sprint
        InputActions.Gameplay.Sprint.started += CallSprint;
        InputActions.Gameplay.Sprint.canceled += CancelSprint;
        //Jump
        //InputActions.Gameplay.Jump.performed += CallJump;
        //Parry
        InputActions.Gameplay.Parry.performed += CallParry;
        //-----------------------------GAME-----------------------------
        //Menu
        InputActions.Gameplay.EnterMenu.performed += CallMenu;
        InputActions.Menu.ExitMenu.performed += CallMenu;
        //Interact
        InputActions.Gameplay.Select.performed += CallSelect;
        //------------------------------UI------------------------------
        //UI Movement
        //InputActions.Menu.MenuNavigation.performed += CallUIMovement;
        //UI Select
        //InputActions.Menu.Select.performed += CallUISelect;
        //--------------------------------------------------------------
        #endregion

        GameManager.OnNewDay += GameInput;
        GameManager.OnEndDay += MenuInput;
    }

    private void OnDisable()
    {
        //Disconnect events
        //---------------------------GAMEPLAY---------------------------
        //Movement
        OnDirectionChanged -= OnDirectionChanged;
        //Sprint
        OnSprint -= OnSprint;
        OnSprintCancelled -= OnSprintCancelled;
        //Jump
        OnJump -= OnJump;
        //Parry
        OnParry -= OnParry;
        //-----------------------------GAME-----------------------------
        //Menu
        OnMenuCalled -= OnMenuCalled;
        //Select
        OnSelect -= OnSelect;
        //------------------------------UI------------------------------
        //UI Movement
        //OnUIDirectionChanged -= OnUIDirectionChanged;
        //UI Select
        //OnUISelect -= OnUISelect;
        //--------------------------------------------------------------

        GameManager.OnNewDay -= GameInput;
        GameManager.OnEndDay -= MenuInput;

        InputActions.Disable();
    }
}
