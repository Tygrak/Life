﻿using System;
using System.Threading;

namespace Life{
    public class Program{
        public static void Main(string[] args){
            try{
                Console.Clear();
                Console.SetWindowSize(160, 80);
            } catch{

            }
            life c = new life(79);
            c.randomiseGrid(65);
            c.runGenerations(100000, 100);
        }
    }

    public class life{
        public int[,] grid;
        public int gridSize;
        public life(int gridSize){
            this.grid = new int[gridSize, gridSize];
            this.gridSize = gridSize;
        }

        public int getCell(int x, int y){
            if(x<0){
                x += gridSize;
            }
            if(y<0){
                y += gridSize;
            }
            x = x % gridSize;
            y = y % gridSize;
            return grid[x, y];
        }

        public int liveNeighbors(int x, int y){
            int count = 0;
            count += getCell(x+1, y+1);
            count += getCell(x+1, y);
            count += getCell(x+1, y-1);
            count += getCell(x, y-1);
            count += getCell(x-1, y-1);
            count += getCell(x-1, y);
            count += getCell(x-1, y+1);
            count += getCell(x, y+1);
            return count;
        }

        public int cellResult(int x, int y){
            int cell = getCell(x, y);
            int neighbors = liveNeighbors(x, y);
            if(cell == 1 && (neighbors < 2 || neighbors > 3)){
                cell = 0;
            } else if(cell == 0 && neighbors == 3){
                cell = 1;
            }
            return cell;
        }

        public void nextGeneration(){
            int[,] nextGrid = new int[gridSize, gridSize];
            for (int x = 0; x < gridSize; x++){
                for (int y = 0; y < gridSize; y++){
                    nextGrid[x, y] = cellResult(x, y);
                }
            }
            grid = nextGrid;
        }

        public void randomiseGrid(int deadPercent, int seed = -1){
            Random r;
            if(seed == -1){
                r = new Random();
                seed = r.Next();
            }
            r = new Random(seed);
            for (int x = 0; x < gridSize; x++){
                for (int y = 0; y < gridSize; y++){
                    if(r.Next(0, 101)>deadPercent){
                        grid[x, y] = 1;
                    } else{
                        grid[x, y] = 0;
                    }
                }
            }
        }

        public void printGrid(){
            try{
                Console.SetCursorPosition(0, 0);
            } catch{

            }
            string gr = "";
            for (int y = 0; y < gridSize; y++){
                gr += "\n";
                for (int x = 0; x < gridSize; x++){
                    if(grid[x, y] == 0){
                        gr += "  ";
                    } else{
                        gr += "O ";
                    }
                }
            }
            Console.Write(gr);
        }

        public void runGenerations(int num, int delay){
            for (int i = 0; i < num; i++){
                printGrid();
                nextGeneration();
                Thread.Sleep(delay);
            }
        }
    }
}