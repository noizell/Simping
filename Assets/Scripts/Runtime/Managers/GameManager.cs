using NPP.TaskTimers;
using UnityEngine;
using UnityEngine.Playables;
using DG.Tweening;
using MoreMountains.Feedbacks;

namespace IGJ.SIMP.Runtime.Managers
{
    public class GameManager : MonoBehaviour
    {
        [Header("Game Settings")]
        [SerializeField] float gameDuration = 80f;
        [SerializeField] PlayableDirector gamePlayable;
        [SerializeField] TimelineManager timelineManager;

        [Header("UI Setup")]
        [SerializeField] CanvasGroup titleCanvasGroup;
        [SerializeField] CanvasGroup hudGameGroup;

        [Header("UI Feedbacks")]
        [SerializeField] MMF_Player timerFeedbacks;

        bool startGame = false;

        TaskDelay gameTimer;

        private void Start()
        {
            GameRuntimeManager.CurrentState = GameState.Game;
            gamePlayable.playableAsset = timelineManager.GetPlayableByName("Main");
            gameTimer = TaskTimer.CreateTask(.0001f);
            GameRuntimeManager.EnablePlayer(false);
            GameRuntimeManager.UpdateTime((int)gameDuration);

            titleCanvasGroup.DOFade(1, .01f);
            hudGameGroup.DOFade(0, .01f);
        }

        public void StartGame()
        {
            if (startGame) return;
            startGame = true;

            titleCanvasGroup.DOFade(0, .55f).OnComplete(() =>
            {
                timerFeedbacks.GetFeedbackOfType<MMF_TMPText>().NewText = GameRuntimeManager.CurrentTime.ToString();
                timerFeedbacks.PlayFeedbacks();
                hudGameGroup.DOFade(1, .45f);
            });

            gamePlayable.Play();


            TaskTimer.CreateTask(float.Parse(gamePlayable.duration.ToString()) - .5f, () =>
              {
                  GameRuntimeManager.EnablePlayer(true);
                  GameRuntimeManager.SetGameCameraPriority(10);
                  GameRuntimeManager.SetTitleCameraPriority(9);

                  gameTimer = TaskTimer.CreateTaskLoop(1f,
                      () =>
                      {
                          gamePlayable.playableAsset = timelineManager.GetPlayableByName("Ending");
                          GameRuntimeManager.CurrentState = GameState.Cutscene;
                          GameRuntimeManager.EnablePlayer(false);
                      },
                      (int i) =>
                      {
                          if (GameRuntimeManager.CurrentTime > 0)
                          {
                              GameRuntimeManager.UpdateTime(GameRuntimeManager.CurrentTime - 1);
                              timerFeedbacks.GetFeedbackOfType<MMF_TMPText>().NewText = GameRuntimeManager.CurrentTime.ToString();
                              timerFeedbacks.PlayFeedbacks();
                          }
                          else
                              gameTimer.Stop();
                      });
              });
        }
    }
}
