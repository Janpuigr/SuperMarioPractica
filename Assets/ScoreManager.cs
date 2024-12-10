using UnityEngine;

public interface IScoreManager
{
	void AddPoints(int Points);
	void RestartPoints();
	int GetPoints();
	event ScoreChanged scoreChangedDelegate;
}
public delegate void ScoreChanged(IScoreManager scoreManager);

public class ScoreManager : MonoBehaviour, IScoreManager, IRestartGameElement
{
	[SerializeField] int m_Points;
	public event ScoreChanged scoreChangedDelegate;


	void Awake()
	{
		DependencyInjector.AddDependency<IScoreManager>(this);
	}
	public void AddPoints(int points)
	{
		m_Points += points;
		scoreChangedDelegate?.Invoke(this);
	}
	public void RestartPoints()
	{
		m_Points = 0;
		scoreChangedDelegate?.Invoke(this);
	}
	public int GetPoints() { return m_Points; }

	public void RestartGame()
	{

		m_Points = 0;
	}
}