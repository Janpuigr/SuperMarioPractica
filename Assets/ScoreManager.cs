using UnityEngine;

public interface IScoreManager
{
	void AddPoints(int Points);
	int GetPoints();
	event ScoreChanged scoreChangedDelegate;
}
public delegate void ScoreChanged(IScoreManager scoreManager);

public class ScoreManager : MonoBehaviour, IScoreManager
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
	public int GetPoints() { return m_Points; }
}