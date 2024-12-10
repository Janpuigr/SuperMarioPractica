using UnityEngine;
public class Item : MonoBehaviour, IRestartGameElement
{
	private void Start()
	{
		GameManager.GetGameManager().AddRestartGameElement(this);
	}
	private void OnTriggerEnter(Collider other)
	{
		if (other.CompareTag("Player"))
		{
			DependencyInjector.GetDependency<IScoreManager>().AddPoints(1);
			gameObject.SetActive(false);
		}
	}

	public void RestartGame()
	{
		DependencyInjector.GetDependency<IScoreManager>().RestartPoints();
		gameObject.SetActive(true);
	}
}
