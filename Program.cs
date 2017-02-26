using System;
using System.Threading;

namespace Life{
    public class Program{
        public static void Main(string[] args){
            Canvas canv = new Canvas(900, 900);
            Life c = new Life(450, canv);
            c.randomiseGrid(90);
            canv.lif = c;
            canv.initialize();
            /*
            try{
                Console.Clear();
                Console.SetWindowSize(160, 80);
            } catch{
            }
            life c = new life(79);
            c.randomiseGrid(65);
            c.runGenerations(100000, 0);
            */
        }
    }

    public class Life{
        public int[,] grid;
        public int[] ruleStay = {2, 3};
        public int[] ruleSpawn = {3};
        public int generation = 0;
        public int gridSize;
        public int seed = -1;
        public float squareSize = 1;
        public Canvas canv;
        public Life(int gridSize, Canvas canv){
            this.grid = new int[gridSize, gridSize];
            this.gridSize = gridSize;
            this.squareSize = 100f/gridSize;
            this.canv = canv;
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
            if(cell == 1 && Array.IndexOf(ruleStay, neighbors) == -1){
                cell = 0;
            } else if(cell == 0 && Array.IndexOf(ruleSpawn, neighbors) != -1){
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
            generation += 1;
        }

        public void randomiseGrid(int deadPercent, int seed = -1){
            generation = 0;
            Random r;
            if(seed == -1){
                r = new Random();
                seed = r.Next();
            }
            this.seed = seed;
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

        public void clearGrid(){
            generation = 0;
            for (int x = 0; x < gridSize; x++){
                for (int y = 0; y < gridSize; y++){
                    grid[x, y] = 0;
                }
            }
        }

        public void setGridSize(int gridSize){
            generation = 0;
            this.grid = new int[gridSize, gridSize];
            this.gridSize = gridSize;
            this.squareSize = 100f/gridSize;
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

        public void drawGrid(){
            for (int y = 0; y < gridSize; y++){
                for (int x = 0; x < gridSize; x++){
                    if(grid[x, y] == 1){
                        canv.drawSquare(x*squareSize, y*squareSize, x*squareSize+squareSize, y*squareSize+squareSize);
                    }
                }
            }
            //Thread.Sleep(10);
        }

        public void runGenerations(int num, int delay){
            for (int i = 0; i < num; i++){
                printGrid();
                nextGeneration();
                Thread.Sleep(delay);
            }
        }

        public void silentRunGenerations(int num){
            for (int i = 0; i < num; i++){
                nextGeneration();
            }
        }
    }
}
