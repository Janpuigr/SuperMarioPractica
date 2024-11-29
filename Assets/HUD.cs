using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
	public Text m_Score;
	public Animation m_Animation;
	public AnimationClip m_IdleAnimationClip;
	public AnimationClip m_ShowAnimationClip;

	private void Start()
	{
		DependencyInjector.GetDependency<IScoreManager>().scoreChangedDelegate += UpdateScore;
	}
	private void OnDestroy()
	{
		DependencyInjector.GetDependency<IScoreManager>().scoreChangedDelegate -= UpdateScore;
	}
	public void UpdateScore(IScoreManager scoreManager)
	{
		m_Score.text = "" + scoreManager.GetPoints();
		ShowAnimation();
	}
	void ShowAnimation()
	{
		m_Animation.Play(m_ShowAnimationClip.name);
		m_Animation.PlayQueued(m_IdleAnimationClip.name);
	}
}