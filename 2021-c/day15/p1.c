#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <limits.h>
#include <stdbool.h>

#define WIDTH (100)
#define HEIGHT (100)

bool inRange(char x, char y) {
	return x>=0 && x<WIDTH&& y>=0 && y<HEIGHT;
}

void printGridChar(char grid[HEIGHT][WIDTH]) {
	if(WIDTH > 40) {
		return;
	}
	
	for(int y=0; y<HEIGHT; y++) {
		for(int x=0; x<WIDTH; x++) {
			printf("%d,", grid[y][x]);
		}
		printf("\n");
	}
}

void printGridInt(int grid[HEIGHT][WIDTH]) {
	if(WIDTH > 40) {
		return;
	}
	
	for(int y=0; y<HEIGHT; y++) {
		for(int x=0; x<WIDTH; x++) {
			printf("%5d,", grid[y][x]);
		}
		printf("\n");
	}
}

int main(int argc, char* argv[])
{
	FILE* file;
	int maxLineLength = 128;
	char buffer[maxLineLength];
	char* filename = argv[1];

	file = fopen(filename, "r");
	int lineCount = 0;
	
	char grid[HEIGHT][WIDTH] = { { 0 } };
	
	while(fgets(buffer, maxLineLength, file))
	{
		for(int i=0; i<WIDTH; i++) {
			grid[lineCount][i] = buffer[i] - '0';
		}
		lineCount++;
	}

	printf("Found %d lines in %s.\n", lineCount, filename);
	//printGridChar(grid);

	int riskGrid[HEIGHT][WIDTH] = { { 0 } };
	
	for(int i=0; i<10; i++) {
		for(int y=0; y<HEIGHT; y++) {
			for(int x=0; x<WIDTH; x++) {
				int risk = 0;
				if(inRange(x-1, y)) {
					risk = grid[y][x] + riskGrid[y][x-1];
				}
				if(inRange(x, y-1)) {
					int r = grid[y][x] + riskGrid[y-1][x];
					if(r < risk || risk == 0) {
						risk = r;
					}
				}
				if(inRange(x+1, y) && riskGrid[y][x+1] > 0) {
					int r = grid[y][x] + riskGrid[y][x+1];
					if(r < risk || risk == 0) {
						risk = r;
					}
				}
				if(inRange(x, y+1) && riskGrid[y+1][x] > 0) {
					int r = grid[y][x] + riskGrid[y+1][x];
					if(r < risk || risk == 0) {
						risk = r;
					}
				}
				if(risk < riskGrid[y][x] || riskGrid[y][x] == 0) {
					riskGrid[y][x] = risk;
				}
			}
		}
	}
	
	printGridInt(riskGrid);
	printf("%d\n", riskGrid[HEIGHT-1][WIDTH-1]);

	fclose(file);

	return 0;
}
