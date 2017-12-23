using UnityEngine;
using System.Collections;

public class MazeGenerator : MonoBehaviour
{
	public int MazeSize = 64;
	public GameObject WallTemplate;
	public GameObject WallContainer;
	public GameObject Floor;
	public GameObject Player;
	public GameObject EnemyTemplate;
	public GameObject BleachObject;

	private Maze _maze;
	private GameObject[,] _mazeCubes;
	private int[,] _pathfindingMaze;
	private MazeSolver _mazeSolver;

	void Start()
	{
		_maze = new Maze(MazeSize, MazeSize);
		_mazeCubes = new GameObject[MazeSize * 2 + 6, MazeSize * 2 + 6];
		_pathfindingMaze = new int[MazeSize * 2 + 6, MazeSize * 2 + 6];
		_maze.Generate();

		Floor.transform.position = new Vector3(MazeSize, -0.5f, MazeSize);
		Floor.transform.localScale = new Vector3(MazeSize / 4, 0, MazeSize / 4);

		for(var i = 0; i < MazeSize * 2 + 6; i++)
		{
			MakeCell(i - 3, 0 - 3);
			MakeCell(i - 3, MazeSize * 2 + 5 - 3);

			MakeCell(0 - 3, i - 3);
			MakeCell(MazeSize * 2 + 5 - 3, i - 3);

			for(var j = 0; j < MazeSize * 2 + 6; j++)
			{
				_pathfindingMaze[i, j] = 0;
			}
		}

		for(var i = 0; i < MazeSize; i++) // x
		{
			for(var j = 0; j < MazeSize; j++) // y
			{
				var cell = _maze.Board[i, j];
				
				if(cell.NorthWall)
				{
					MakeCell(i * 2, j * 2 - 1);
					MakeCell(i * 2 - 1, j * 2 - 1);
					MakeCell(i * 2 + 1, j * 2 - 1);
				}
				else if(cell.SouthWall)
				{
					MakeCell(i * 2, j * 2 + 1);
					MakeCell(i * 2 - 1, j * 2 + 1);
					MakeCell(i * 2 + 1, j * 2 + 1);
				}
				else if(cell.EastWall)
				{
					MakeCell(i * 2 - 1, j * 2);
					MakeCell(i * 2 - 1, j * 2 - 1);
					MakeCell(i * 2 - 1, j * 2 + 1);
				}
				else if(cell.WestWall)
				{
					MakeCell(i * 2 + 1, j * 2);
					MakeCell(i * 2 + 1, j * 2 - 1);
					MakeCell(i * 2 + 1, j * 2 + 1);
				}
			}
		}

		_mazeSolver = new MazeSolver(_pathfindingMaze);

		// place player
		var playerPos = FindRandomAccessibleSpot(Vector2.zero, 0);
		Player.transform.position = new Vector3(playerPos.x, 0.5f, playerPos.y);

		// place enemy
		var enemyPos = FindRandomAccessibleSpot(playerPos, 10);
		var enemy = Instantiate(EnemyTemplate);
		enemy.GetComponent<ChasePlayer>().Solver = _mazeSolver;
		enemy.transform.position = new Vector3(enemyPos.x, 0f, enemyPos.y);
		enemy.SetActive(true);

		// place bleach
		var bleachPos = FindRandomAccessibleSpot(playerPos, 10);
		BleachObject.transform.position = new Vector3(bleachPos.x, 0f, bleachPos.y);
	}

	private void MakeCell(int i, int j)
	{
		if(_mazeCubes[i + 3, j + 3] != null) return;
		var cube = Instantiate(WallTemplate);
		cube.transform.position = new Vector3(i, 0, j);
		cube.transform.parent = WallContainer.transform;
		cube.transform.name = "Cube (" + i + ", " + j + ")";
		cube.SetActive(true);
		_mazeCubes[i + 3, j + 3] = cube;
		_pathfindingMaze[i + 3, j + 3] = 1;
	}

	private Vector2 FindRandomAccessibleSpot(Vector2 pos, float distFrom)
	{
		Vector2 origin = new Vector2(0, 0);
		Vector2 randomSpot = new Vector2(0, 0);
		do
		{
			randomSpot = new Vector2(UnityEngine.Random.Range(3, MazeSize * 2 + 3), UnityEngine.Random.Range(3, MazeSize * 2 + 3));
		} while(SpotOpen(randomSpot.x, randomSpot.y) && !HasPathTo(origin, randomSpot) && Vector2.Distance(pos, randomSpot) >= distFrom);

		return randomSpot;
	}

	private bool HasPathTo(Vector2 from, Vector2 to)
	{
		var path = _mazeSolver.FindPath((int)from.y, (int)from.x, (int)to.y, (int)to.x);
		return path == null;
	}

	private bool SpotOpen(float x, float y)
	{
		return _mazeCubes[(int)x, (int)y] == null;
	}
}
