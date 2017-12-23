using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class ChasePlayer : MonoBehaviour
{
	public MazeSolver Solver;
	public GameObject Player;
	public float Speed = 1f;

	private Vector2 _nextSquare = Vector2.zero;

	void Start()
	{
		InvokeRepeating("Recalculate", 0f, 0.3f);
	}

	void Update()
	{
		if(_nextSquare == Vector2.zero)
			return;

		var changePos = _nextSquare - new Vector2(transform.position.x, transform.position.y);
		transform.position = Vector3.MoveTowards(transform.position, new Vector3(_nextSquare.x, transform.position.y, _nextSquare.y), Speed * Time.deltaTime);
	}
	
	private void Recalculate()
	{
		var path = Solver.FindPath((int)transform.position.y + 3, (int)transform.position.x + 3, (int)Player.transform.position.y + 3, (int)Player.transform.position.x + 3);
		if(path == null)
		{
			Debug.Log("no path");
			return;
		}
		var x = (int)transform.position.x + 3;
		var y = (int)transform.position.y + 3;

		var squares = new List<Vector2>();
		for(var i = 0; i < path.GetLength(0); i++)
		{
			for(var j = 0; j < path.GetLength(1); j++)
			{
				if(path[i, j] == 100)
				{
					squares.Add(new Vector2(i, j));
				}
			}
		}

		var posV = new Vector2(transform.position.x, transform.position.z);
		var squaresAvailable = squares.Where(v => Vector2.Distance(v, posV) >= 0.9).OrderBy(v => Vector2.Distance(v, posV)).ToList();
		Debug.Log(string.Join(", ", squaresAvailable.Select(s => s.ToString()).ToArray()));
		_nextSquare = squaresAvailable.First();
		Debug.Log(_nextSquare);
	}
}