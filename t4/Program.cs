
using System.Collections.Generic;
using System;

Console.WriteLine("Introduceti grila:");
List<string> input = new List<string>();
string line = Console.ReadLine();
while (!string.IsNullOrEmpty(line))
{
    input.Add(line);
    line = Console.ReadLine();
}

int m = input.Count;
int n = input[0].Length;

char[,] grid = new char[m, n];

int keysCount = 0;
int startX = -1;
int startY = -1;
Dictionary<char, bool> keysFound = new Dictionary<char, bool>();
for (char c = 'a'; c <= 'f'; c++)
{
    keysFound[c] = false;
}

for (int i = 0; i < m; i++)
{
    if (input[i].Length != n)
    {
        Console.WriteLine("Eroare: Linia " + (i + 1) + " nu are acelasi număr de caractere ca celelalte linii.");
        return;
    }

    for (int j = 0; j < n; j++)
    {
        grid[i, j] = input[i][j];

        if (grid[i, j] == '@')
        {
            if (startX != -1 || startY != -1)
            {
                Console.WriteLine("Eroare: Există mai mult de un simbol '@' în grilă.");
                return;
            }
            startX = i;
            startY = j;
        }
        else if (Char.IsLower(grid[i, j]))
        {
            keysCount++;
            if (keysFound.ContainsKey(grid[i, j]) && keysFound[grid[i, j]])
            {
                Console.WriteLine("Eroare: Există o cheie duplicată în grilă.");
                return;
            }
            keysFound[grid[i, j]] = true;
        }
    }
}

if (startX == -1 || startY == -1)
{
    Console.WriteLine("Eroare: Nu există simbol '@' în grilă.");
    return;
}

/*Console.WriteLine("Grila introdusă:");
for (int i = 0; i < m; i++)
{
    for (int j = 0; j < n; j++)
    {
        Console.Write(grid[i, j]);
    }
    Console.WriteLine();
}

Console.WriteLine();*/

int result = FindKeys(grid, startX, startY, keysCount, keysFound);
Console.WriteLine("Numărul minim de miscari pentru a obtine toate cheile: " + result);


static int FindKeys(char[,] grid, int x, int y, int keysCount, Dictionary<char, bool> keysFound)
{
    int rowCount = grid.GetLength(0);
    int colCount = grid.GetLength(1);

    Queue<State> queue = new Queue<State>();
    HashSet<State> visited = new HashSet<State>();

    var initialState = new State(x, y, 0, new Dictionary<char, bool>());
    queue.Enqueue(initialState);
    visited.Add(initialState);

    while (queue.Count > 0)
    {
        var current = queue.Dequeue();
        int currentX = current.X;
        int currentY = current.Y;
        int moves = current.Moves;
        var currentKeys = current.Keys;

        if (currentKeys.Count == keysCount)
        {
            return moves;
        }

        foreach (var direction in new[] { new Tuple<int, int>(-1, 0), new Tuple<int, int>(1, 0), new Tuple<int, int>(0, -1), new Tuple<int, int>(0, 1) })
        {
            int newX = currentX + direction.Item1;
            int newY = currentY + direction.Item2;

            if (newX >= 0 && newX < rowCount && newY >= 0 && newY < colCount && grid[newX, newY] != '#')
            {
                if (Char.IsLower(grid[newX, newY]))
                {
                    var newKeys = new Dictionary<char, bool>(currentKeys);
                    newKeys[grid[newX, newY]] = true;

                    var newState = new State(newX, newY, moves + 1, newKeys);
                    if (!visited.Contains(newState))
                    {
                        queue.Enqueue(newState);
                        visited.Add(newState);
                    }
                }
                else if (Char.IsUpper(grid[newX, newY]))
                {
                    if (currentKeys.ContainsKey(Char.ToLower(grid[newX, newY])) && currentKeys[Char.ToLower(grid[newX, newY])])
                    {
                        var newState = new State(newX, newY, moves + 1, currentKeys);
                        if (!visited.Contains(newState))
                        {
                            queue.Enqueue(newState);
                            visited.Add(newState);
                        }
                    }
                }
                else
                {
                    var newState = new State(newX, newY, moves + 1, currentKeys);
                    if (!visited.Contains(newState))
                    {
                        queue.Enqueue(newState);
                        visited.Add(newState);
                    }
                }
            }
        }
    }

    return -1;
}
struct State
{
    public int X;
    public int Y;
    public int Moves;
    public Dictionary<char, bool> Keys;

    public State(int x, int y, int moves, Dictionary<char, bool> keys)
    {
        X = x;
        Y = y;
        Moves = moves;
        Keys = keys;
    }
}
