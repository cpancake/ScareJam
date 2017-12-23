using System;
using System.Collections.Generic;
using UnityEngine;

public struct Point
{
	public int x;
	public int y;

	public Point(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
}

/// <summary>
/// This class contains a maze that is
/// generated using the recursive backtracker algorithm. 
/// </summary>
public class Maze
{
	public Cell[,] Board;
	public readonly int Height;
	public readonly int Width;
	public readonly System.Random rng = new System.Random();
	public Point Start = new Point(0, 0);
	public Point End = new Point(0, 0);
	public List<Tuple<Cell, Direction>> Points = new List<Tuple<Cell, Direction>>();
	public int iterationcount = 0;

	public Maze(int width, int height)
	{
		this.Height = height;
		this.Width = width;
		Board = new Cell[height, width];
		Initialise();
	}

	public void Generate()
	{
		Generate(rng.Next(Width), rng.Next(Height));
	}

	public void Generate(int startX, int startY)
	{
		this.Start = new Point(startX, startY);
		Points = new List<Tuple<Cell, Direction>>();
		CarvePassage(startX, startY);
	}

	/// <summary>
	/// Populates board with default Cells.
	/// </summary>
	private void Initialise()
	{
		for(int row = 0; row < Height; row++)
		{
			for(int col = 0; col < Width; col++)
			{
				this.Board[row, col] = new Cell();
			}
		}
	}

	/// <summary>
	/// Recursive backtracking maze generation algorithm.
	/// </summary>
	/// <param name="currentPos"></param>
	private void CarvePassage(Point currentPos)
	{

		this.Board[currentPos.y, currentPos.x].Point = new Point(currentPos.x, currentPos.y);
		this.Board[currentPos.y, currentPos.x].Visited = true;
		this.Board[currentPos.y, currentPos.x].position_in_iteration = ++iterationcount;

		List<Direction> validDirections = GetAllDirections();
		ValidateDirections(currentPos, validDirections);

		//If there is no valid direction we have found a dead end.
		if(validDirections.Count == 0)
		{
			this.Board[currentPos.y, currentPos.x].isdeadend = true;
			Points.Add(new Tuple<Cell, Direction>(this.Board[currentPos.y, currentPos.x], Direction.Invalid));
		}

		while(validDirections.Count > 0)
		{
			Direction rndDirection = Direction.Invalid;

			if(validDirections.Count > 1)
				rndDirection = validDirections[rng.Next(validDirections.Count)];
			else if(validDirections.Count == 1)
				rndDirection = validDirections[0];

			this.Board[currentPos.y, currentPos.x].visited_count = ++this.Board[currentPos.y, currentPos.x].visited_count;

			RemoveWall(currentPos, rndDirection);
			validDirections.Remove(rndDirection);
			Point newPos = GetAdjPos(currentPos, rndDirection);
			Points.Add(new Tuple<Cell, Direction>(this.Board[currentPos.y, currentPos.x], rndDirection));

			CarvePassage(newPos);

			ValidateDirections(currentPos, validDirections);
		}
	}

	private void CarvePassage(int currentX, int currentY)
	{
		CarvePassage(new Point(currentX, currentY));
	}

	private List<Direction> GetAllDirections()
	{
		return new List<Direction>() {
			Direction.North,
			Direction.East,
			Direction.South,
			Direction.West
		};
	}

	private void ValidateDirections(Point cellPos, List<Direction> directions)
	{
		List<Direction> invalidDirections = new List<Direction>();

		// Check for invalid moves
		for(int i = 0; i < directions.Count; i++)
		{
			switch(directions[i])
			{
				case Direction.North:
					if(cellPos.y == 0 || CellVisited(cellPos.x, cellPos.y - 1))
						invalidDirections.Add(Direction.North);
					break;
				case Direction.East:
					if(cellPos.x == Width - 1 || CellVisited(cellPos.x + 1, cellPos.y))
						invalidDirections.Add(Direction.East);
					break;
				case Direction.South:
					if(cellPos.y == Height - 1 || CellVisited(cellPos.x, cellPos.y + 1))
						invalidDirections.Add(Direction.South);
					break;
				case Direction.West:
					if(cellPos.x == 0 || CellVisited(cellPos.x - 1, cellPos.y))
						invalidDirections.Add(Direction.West);
					break;
			}
		}

		// Eliminating invalid moves
		foreach(var item in invalidDirections)
			directions.Remove(item);
	}

	private void RemoveWall(Point pos, Direction direction)
	{
		switch(direction)
		{
			case Direction.North:
				this.Board[pos.y, pos.x].NorthWall = false;
				this.Board[pos.y - 1, pos.x].SouthWall = false;
				break;
			case Direction.East:
				this.Board[pos.y, pos.x].EastWall = false;
				this.Board[pos.y, pos.x + 1].WestWall = false;
				break;
			case Direction.South:
				this.Board[pos.y, pos.x].SouthWall = false;
				this.Board[pos.y + 1, pos.x].NorthWall = false;
				break;
			case Direction.West:
				this.Board[pos.y, pos.x].WestWall = false;
				this.Board[pos.y, pos.x - 1].EastWall = false;
				break;
		}
	}

	private bool CellVisited(int x, int y)
	{
		return this.Board[y, x].Visited;
	}

	private Point GetAdjPos(Point position, Direction direction)
	{
		Point adjPosition = position;

		switch(direction)
		{
			case Direction.North:
				adjPosition.y = adjPosition.y - 1;
				break;
			case Direction.East:
				adjPosition.x = adjPosition.x + 1;
				break;
			case Direction.South:
				adjPosition.y = adjPosition.y + 1;
				break;
			case Direction.West:
				adjPosition.x = adjPosition.x - 1;
				break;
		}

		return adjPosition;
	}
}

public enum Direction
{
	Invalid,
	North,
	East,
	South,
	West,
}

public class Tuple<T1, T2>
{
	public T1 First { get; private set; }
	public T2 Second { get; private set; }
	internal Tuple(T1 first, T2 second)
	{
		First = first;
		Second = second;
	}
}

public static class Tuple
{
	public static Tuple<T1, T2> New<T1, T2>(T1 first, T2 second)
	{
		var tuple = new Tuple<T1, T2>(first, second);
		return tuple;
	}
}