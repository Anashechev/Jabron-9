using System;
using System.Collections.Generic;
using System.Threading;

enum Border
{
    MaxRight = 79,
    MaxBottom = 24
}

class SnakeGame
{
    private List<(int, int)> snake;
    private int directionX;
    private int directionY;
    private bool isGameOver;
    private bool isFoodEaten;
    private (int, int) foodPosition;
    private Random random;

    public SnakeGame()
    {
        snake = new List<(int, int)>();
        snake.Add((40, 12)); 
        directionX = 0; 
        directionY = 1; 
        isGameOver = false;
        isFoodEaten = false;
        random = new Random();

        Console.CursorVisible = false;
        Console.SetWindowSize((int)Border.MaxRight + 1, (int)Border.MaxBottom + 1);
        Console.SetBufferSize((int)Border.MaxRight + 1, (int)Border.MaxBottom + 1);


        foodPosition = GenerateFoodPosition();
    }

    public void Run()
    {
        Thread inputThread = new Thread(ReadInput);
        Thread drawThread = new Thread(Draw);

        inputThread.Start();
        drawThread.Start();

        while (!isGameOver)
        {
            if (isFoodEaten)
            {
                snake.Add((0, 0)); 
                isFoodEaten = false;
                foodPosition = GenerateFoodPosition();
            }

            MoveSnake();
            CheckCollision();
            Thread.Sleep(100); 
        }

        inputThread.Join();
        drawThread.Join();
    }

    private void ReadInput()
    {
        while (!isGameOver)
        {
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow when directionY != 1:
                    directionX = 0;
                    directionY = -1;
                    break;
                case ConsoleKey.DownArrow when directionY != -1:
                    directionX = 0;
                    directionY = 1;
                    break;
                case ConsoleKey.LeftArrow when directionX != 1:
                    directionX = -1;
                    directionY = 0;
                    break;
                case ConsoleKey.RightArrow when directionX != -1:
                    directionX = 1;
                    directionY = 0;
                    break;
            }
        }
    }

    private void MoveSnake()
    {
        
        for (int i = snake.Count - 1; i > 0; i--)
        {
            snake[i] = snake[i - 1];
        }

        
        int newX = snake[0].Item1 + directionX;
        int newY = snake[0].Item2 + directionY;
        snake[0] = (newX, newY);
    }

    private void CheckCollision()
    {
        
        if (snake[0].Item1 < 0 || snake[0].Item1 >= (int)Border.MaxRight || snake[0].Item2 < 0 || snake[0].Item2 >= (int)Border.MaxBottom)
        {
            isGameOver = true;
            return;
        }

        
        for (int i = 1; i < snake.Count; i++)
        {
            if (snake[i].Item1 == snake[0].Item1 && snake[i].Item2 == snake[0].Item2)
            {
                isGameOver = true;
                return;
            }
        }

       
        if (snake[0].Item1 == foodPosition.Item1 && snake[0].Item2 == foodPosition.Item2)
        {
            isFoodEaten = true;
        }
    }

    private void Draw()
    {
        while (!isGameOver)
        {
            Console.Clear();

           
            foreach ((int, int) segment in snake)
            {
                Console.SetCursorPosition(segment.Item1, segment.Item2);
                Console.Write("■");
            }

        
            Console.SetCursorPosition(foodPosition.Item1, foodPosition.Item2);
            Console.Write("★");

            Thread.Sleep(50); 
        }
    }

    private (int, int) GenerateFoodPosition()
    {
        int x, y;
        do
        {
            x = random.Next((int)Border.MaxRight);
            y = random.Next((int)Border.MaxBottom);
        }
        while (snake.Exists(segment => segment.Item1 == x && segment.Item2 == y));

        return (x, y);
    }
}

class Program
{
    static void Main()
    {
        SnakeGame game = new SnakeGame();
        game.Run();
    }
}