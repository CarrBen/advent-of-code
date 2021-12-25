#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <stdbool.h>

#define HEIGHT (137)
#define WIDTH  (139)

void printGrid(char grid[HEIGHT][WIDTH]) {
    for(int y=0; y<HEIGHT; y++) {
        for(int x=0; x<WIDTH; x++) {
            printf("%c", grid[y][x]);
        }
        printf("\n");
    }
    printf("\n");
}

bool step(char (*grid)[HEIGHT][WIDTH]) {
    char b[HEIGHT][WIDTH];
    memset(&b, '.', WIDTH * HEIGHT * sizeof(char));
    bool moved = false;
    
    for(int y=0; y<HEIGHT; y++) {
        for(int x=0; x<WIDTH; x++) {
            if((*grid)[y][x] == '>') {
                int nx = (x+1) % WIDTH;
                if((*grid)[y][nx] == '.') {
                    b[y][nx] = '>';
                    moved = true;
                }else{
                    b[y][x] = '>';
                }
                continue;
            }
        }
    }
    
    for(int y=0; y<HEIGHT; y++) {
        for(int x=0; x<WIDTH; x++) {           
            if((*grid)[y][x] == 'v') {
                int ny = (y+1) % HEIGHT;
                if((*grid)[ny][x] != 'v' && b[ny][x] == '.') {
                    b[ny][x] = 'v';
                    moved = true;
                }else{
                    b[y][x] = 'v';
                }
                continue;
            }
        }
    }
    
    memcpy(grid, &b, WIDTH * HEIGHT * sizeof(char));
    
    return moved;
}

int main(int argc, char* argv[])
{
	FILE* file;
	int maxLineLength = 192;
	char buffer[maxLineLength];
	char* filename = argv[1];

	file = fopen(filename, "r");
	int lineCount = 0;
	
	char grid[HEIGHT][WIDTH] = { 0 };

	while(fgets(buffer, maxLineLength, file))
	{
		memcpy(&grid[lineCount], buffer, WIDTH);
		lineCount++;
	}

	printf("Found %d lines in %s.\n", lineCount, filename);
	fclose(file);

    //printGrid(grid);
    int steps = 0;
    
    while(step(&grid)) {
        steps++;
        printf("After %d steps\n", steps);
        //printGrid(grid);
    }
    
    steps++;
    printf("After %d steps\n", steps);
    //printGrid(grid);

	return 0;
}
