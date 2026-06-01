using AkaitoAi.Advertisement;
using AkaitoAi.Extensions;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using AkaitoAi.GameBase;

[RequireComponent(typeof(Button))]
public class MenuButtonController : MonoBehaviour
{
    public ButtonType buttonType;

    [SerializeField] private ButtonParam _buttonParam;

    private MenuManager menuManager;
    private MenuScreenManager canvasManager;
    private Button button;
    
    private Animator animator;
    internal string _currentState;

    private static bool showDailyReward = true;

    [System.Serializable]
    public struct ButtonParam
    {
        public ScreenType screen;
        public string closeAnimation;
        public string openAnimation;
    }

    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonClicked);
        canvasManager = MenuScreenManager.GetInstance();
        menuManager = MenuManager.GetInstance();
        animator = menuManager.transitionAnimator;
    }

    private void OnButtonClicked()
    {
        //TODO Sound Calling
        AkaitoAi.SoundManager.Instance?.PlayOnButtonSound();

        switch (buttonType)
        {
            case ButtonType.Play_Game:

                break;

            case ButtonType.Quit_Game:

                break;
            
            case ButtonType.Quit_Game_No:

                //TODO Ads Calling
                AdsWrapper.GetInstance()?.HideMediumBannerBottomLeft();
                AdsWrapper.GetInstance()?.ShowSmallBannerTopRight();

                break;

            case ButtonType.Quit_Game_Yes:
                Application.Quit();

                break;

            case ButtonType.Settings:

                break;

            case ButtonType.Settings_Close:

                break;

            case ButtonType.Mode_Select:

                break;

            case ButtonType.Back_From_Mode:

                break;

            case ButtonType.Level_Select:

                break;

            case ButtonType.Back_From_level:

                break;
            
            case ButtonType.Skip_Level_Select:

                break;
            
            case ButtonType.Player_Profile:

                break;
            
            case ButtonType.Back_From_Player_Profile:

                break;
            
            case ButtonType.Player_Profile_Country:

                break;
            
            case ButtonType.Back_From_Player_Profile_Country:


            break;
            
            case ButtonType.Back_From_Profile:

                bool showDailyRewardScreen = showDailyReward && DailyReward7.CanGetRewardNow;

                _buttonParam.screen = showDailyRewardScreen? ScreenType.DailyReward : ScreenType.MainMenu;

                if (showDailyRewardScreen) showDailyReward = false;
                
                break;

            default: break;
        }

        Transition(_buttonParam);
    }

    private void Transition(ButtonParam _param)
    {
        if (menuManager.transitionAnimator)
            StartCoroutine(ChangeAnimation(_param.closeAnimation, _param.openAnimation, animator.GetCurrentAnimatorStateInfo(0).length, _param.screen));
        else  ChangePanel();

        void ChangePanel() => canvasManager.SwitchCanvas(_param.screen);

        IEnumerator ChangeAnimation(string _anim_1, string _anim_2, float _duration,ScreenType _screen)
        {
            ChangeAnimationState(_anim_1);

            yield return AkaitoAiExtensions.Seconds(_duration);

            canvasManager.SwitchCanvas(_screen);
            
            ChangeAnimationState(_anim_2);

            void ChangeAnimationState(string _newState)
            {
                if(animator == null) return;

                if (_currentState == _newState) return;

                animator.Play(_newState);

                _currentState = _newState;
            }
        }
    }

    private void SwitchCanvas(OnSwitchPanel screen) => canvasManager.SwitchCanvas(screen.type);

    EventBinding<OnSwitchPanel> panelSwitchEventBinding;
    private void OnEnable()
    {
        panelSwitchEventBinding = new EventBinding<OnSwitchPanel>(SwitchCanvas);
        
        EventBus<OnSwitchPanel>.Register(panelSwitchEventBinding);
    }

    private void OnDisable()
    {
        EventBus<OnSwitchPanel>.Deregister(panelSwitchEventBinding);
    }
}
